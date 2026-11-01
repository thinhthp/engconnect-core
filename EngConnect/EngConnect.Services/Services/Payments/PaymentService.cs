using EngConnect.Entities.Common;
using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Repositories.Enrollments;
using EngConnect.Repositories.Repositories.Orders;
using EngConnect.Repositories.Repositories.Payments;
using EngConnect.Repositories.Repositories.Payments.Filters;
using EngConnect.Services.DTOs.Payments;
using EngConnect.Services.Integrations.Payment;
using EngConnect.Services.Services.Payments.Filters;
using EngConnect.Services.Services.UserContext;
using Microsoft.Extensions.Logging;

namespace EngConnect.Services.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _context;
        private readonly IPaymentGatewayResolver _gatewayResolver;
        private readonly IUserContextService _userContext;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IEnrolmentRepository _enrollmentRepository;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IUnitOfWork context,
            IPaymentGatewayResolver gatewayResolver,
            IUserContextService userContext,
            IOrderRepository orderRepository,
            IPaymentRepository paymentRepository,
            IEnrolmentRepository enrolmentRepository,
            ILogger<PaymentService> logger)
        {
            _context = context;
            _gatewayResolver = gatewayResolver;
            _userContext = userContext;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _enrollmentRepository = enrolmentRepository;
            _logger = logger;
        }

        public async Task<CreatePaymentLinkResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Items == null || request.Items.Count == 0)
                throw new ArgumentException("At least one item is required.", nameof(request.Items));

            if (request.Items.Any(i => i.CourseId <= 0 || i.Sessions <= 0 || i.Price < 0))
                throw new ArgumentException("Each item must have a valid course, sessions, and price.", nameof(request.Items));

            var learnerId = _userContext.GetCurrentUserId() ?? throw new UnauthorizedAccessException();
            var gateway = _gatewayResolver.Resolve(request.Gateway);

            var lineItems = request.Items.Select(i => new PaymentLineItem(
                Name: $"Course {i.CourseId}",
                Quantity: i.Sessions,
                UnitAmount: ToMinorUnits(i.Price)
            )).ToList();

            var amount = lineItems.Sum(i => i.UnitAmount * i.Quantity);
            if (amount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero.", nameof(request.Items));

            var order = new Orders
            {
                LearnerId = learnerId,
                TotalAmount = request.Items.Sum(i => i.Price * i.Sessions),
                PaymentMethod = gateway.Name,
                Status = PaymentStatuses.Pending,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow,
                CreateBy = learnerId
            };

            foreach (var item in request.Items)
            {
                order.Orderitems.Add(new OrderItem
                {
                    CourseId = item.CourseId,
                    Price = item.Price,
                    Sessions = item.Sessions
                });
            }

            await _orderRepository.AddAsync(order);
            await _context.SaveChangesAsync();

            var payment = new Payment
            {
                OrderId = order.OrderId,
                Amount = order.TotalAmount,
                Status = PaymentStatuses.Pending,
                Note = gateway.Name,
                CreatedAt = DateTime.UtcNow,
                CreateBy = learnerId,
                IsActive = true
            };
            await _paymentRepository.AddAsync(payment);
            await _context.SaveChangesAsync();

            var linkRequest = new PaymentLinkRequest(
                ReferenceId: payment.PaymentId,
                Amount: amount,
                Description: $"Payment for order #{order.OrderId}",
                ReturnUrl: request.ReturnUrlOverride,
                CancelUrl: request.CancelUrlOverride,
                Items: lineItems
            );

            PaymentLinkResult linkResult;
            try
            {
                linkResult = await gateway.CreatePaymentLinkAsync(linkRequest, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Payment link creation failed. Gateway={Gateway} OrderId={OrderId} PaymentId={PaymentId} LearnerId={LearnerId}",
                    gateway.Name,
                    order.OrderId,
                    payment.PaymentId,
                    learnerId);

                payment.Status = PaymentStatuses.Failed;
                order.Status = PaymentStatuses.Failed;
                _paymentRepository.Update(payment);
                _orderRepository.Update(order);
                await _context.SaveChangesAsync();
                throw;
            }

            if (string.IsNullOrWhiteSpace(linkResult.CheckoutUrl) || string.IsNullOrWhiteSpace(linkResult.TransactionCode))
            {
                _logger.LogError(
                    "Gateway returned incomplete payment link. Gateway={Gateway} OrderId={OrderId} PaymentId={PaymentId}",
                    gateway.Name,
                    order.OrderId,
                    payment.PaymentId);

                payment.Status = PaymentStatuses.Failed;
                order.Status = PaymentStatuses.Failed;
                _paymentRepository.Update(payment);
                _orderRepository.Update(order);
                await _context.SaveChangesAsync();
                throw new PaymentGatewayException(gateway.Name, "Payment gateway returned an incomplete checkout result.");
            }

            payment.TransactionCode = linkResult.TransactionCode;
            _paymentRepository.Update(payment);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Payment link created. Gateway={Gateway} OrderId={OrderId} PaymentId={PaymentId} TransactionCode={TransactionCode}",
                gateway.Name,
                order.OrderId,
                payment.PaymentId,
                payment.TransactionCode);

            return new CreatePaymentLinkResponse
            {
                OrderId = order.OrderId,
                PaymentId = payment.PaymentId,
                TransactionCode = payment.TransactionCode,
                CheckoutUrl = linkResult.CheckoutUrl
            };
        }

        public async Task<bool> HandleWebhookAsync(string gatewayName, string payload, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                _logger.LogWarning("Webhook payload was empty. Gateway={Gateway}", gatewayName);
                return false;
            }

            IPaymentGateway gateway;
            try
            {
                gateway = _gatewayResolver.Resolve(gatewayName);
            }
            catch (PaymentGatewayException ex)
            {
                _logger.LogWarning(ex, "Webhook received for unknown gateway {Gateway}", gatewayName);
                return false;
            }

            PaymentWebhookResult? webhook;
            try
            {
                webhook = await gateway.VerifyAndParseWebhookAsync(payload, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook parsing failed. Gateway={Gateway}", gateway.Name);
                return false;
            }

            if (webhook is null || string.IsNullOrWhiteSpace(webhook.TransactionCode))
            {
                _logger.LogWarning("Webhook could not be verified. Gateway={Gateway}", gateway.Name);
                return false;
            }

            return await ApplyWebhookAsync(gateway.Name, webhook, cancellationToken);
        }

        private async Task<bool> ApplyWebhookAsync(string gatewayName, PaymentWebhookResult webhook, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByTransactionCodeAsync(webhook.TransactionCode);
            if (payment == null)
            {
                _logger.LogWarning(
                    "Webhook ignored for unknown transaction. Gateway={Gateway} TransactionCode={TransactionCode}",
                    gatewayName,
                    webhook.TransactionCode);
                return true;
            }

            payment.Status = webhook.Status;
            payment.TransactionDate = DateTime.UtcNow;
            _paymentRepository.Update(payment);

            var order = await _orderRepository.GetByIdWithItemsAndCoursesAsync(payment.OrderId);
            if (order != null)
            {
                if (webhook.Status == PaymentStatuses.Success)
                {
                    order.Status = "paid";
                    foreach (var item in order.Orderitems)
                    {
                        var sessions = Math.Max(item.Course?.TotalSessions ?? 0, 0);
                        var enrollment = new Enrollment
                        {
                            LearnerId = order.LearnerId,
                            CourseId = item.CourseId,
                            SessionsPurchased = sessions,
                            SessionsRemaining = sessions,
                            Status = "active",
                            Note = $"Order #{order.OrderId}",
                            CreatedAt = DateTime.UtcNow,
                            CreateBy = order.LearnerId
                        };

                        await _enrollmentRepository.CreateEnrollment(enrollment, cancellationToken);
                    }
                }
                else if (webhook.Status is PaymentStatuses.Canceled or PaymentStatuses.Failed)
                {
                    order.Status = webhook.Status;
                }

                _orderRepository.Update(order);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Webhook applied. Gateway={Gateway} TransactionCode={TransactionCode} PaymentId={PaymentId} Status={Status}",
                gatewayName,
                webhook.TransactionCode,
                payment.PaymentId,
                webhook.Status);

            return true;
        }

        public async Task<PagedResult<PaymentAdminDto>> GetForAdminAsync(PaymentSearchFilter filter, CancellationToken cancellationToken = default)
        {
            var page = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var size = filter.PageSize <= 0 ? 10 : Math.Min(filter.PageSize, 200);

            var parameters = new PaymentQueryParameters
            {
                Search = filter.Search,
                Status = filter.Status,
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                SortBy = string.IsNullOrWhiteSpace(filter.SortBy) ? "createdAt" : filter.SortBy,
                SortDir = string.IsNullOrWhiteSpace(filter.SortDir) ? "desc" : filter.SortDir,
                PageNumber = page,
                PageSize = size
            };

            var (items, total) = await _paymentRepository.GetForAdminAsync(parameters, cancellationToken);

            var dtos = items.Select(p => new PaymentAdminDto
            {
                PaymentId = p.PaymentId,
                OrderId = p.OrderId,
                Amount = p.Amount,
                TransactionCode = p.TransactionCode,
                Status = p.Status,
                TransactionDate = p.TransactionDate,
                Note = p.Note,
                CreatedAt = p.CreatedAt,
                Order = new PaymentOrderSummaryDto
                {
                    OrderId = p.OrderId,
                    LearnerId = p.Order.LearnerId,
                    TotalAmount = p.Order.TotalAmount,
                    PaymentMethod = p.Order.PaymentMethod,
                    Status = p.Order.Status,
                    CreatedAt = p.Order.CreatedAt
                }
            }).ToList();

            return new PagedResult<PaymentAdminDto>
            {
                Items = dtos,
                TotalCount = total,
                PageNumber = page,
                PageSize = size
            };
        }

        private static long ToMinorUnits(decimal amount)
        {
            return Convert.ToInt64(Math.Round(amount, 0, MidpointRounding.AwayFromZero));
        }
    }
}
