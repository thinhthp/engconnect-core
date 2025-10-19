using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using EngConnect.Repositories.Repositories.Orders;
using EngConnect.Repositories.Repositories.Payments;
using EngConnect.Services.DTOs.Payments;
using EngConnect.Services.Integrations.PayOS;
using EngConnect.Services.Services.UserContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _context;
        private readonly IPayOSClient _payOS;
        private readonly IUserContextService _userContext;
        private readonly PayOSOptions _opts;
        private readonly PayOS _payOSSdk;

        public PaymentService(
            IUnitOfWork context,
            IPayOSClient payOS,
            IUserContextService userContext,
            IOptions<PayOSOptions> opts,
            PayOS payOSSdk)
        {
            _context = context;
            _payOS = payOS;
            _userContext = userContext;
            _opts = opts.Value;
            _payOSSdk = payOSSdk;
        }

        public async Task<CreatePaymentLinkResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Items == null || request.Items.Count == 0)
                throw new ArgumentException("At least one item is required.", nameof(request.Items));

            var learnerId = _userContext.GetCurrentUserId() ?? throw new UnauthorizedAccessException();

            // Map to PayOS item shape (unit price integer VND)
            var payItems = request.Items.Select(i => new PayOSItem(
                Name: $"Course {i.CourseId}",
                Quantity: i.Sessions,
                Price: Convert.ToInt64(Math.Round(i.Price, 0, MidpointRounding.AwayFromZero))
            )).ToList();

            // Calculate total amount in VND as long
            long amountVnd = payItems.Sum(pi => pi.Price * (long)pi.Quantity);

            // Create order and order items
            var order = new Orders
            {
                LearnerId = learnerId,
                TotalAmount = request.Items.Sum(i => i.Price * i.Sessions),
                PaymentMethod = "PAYOS",
                Status = "pending",
                Note = request.Note,
                CreatedAt = DateTime.UtcNow,
                CreateBy = learnerId
            };

            foreach (var it in request.Items)
            {
                order.Orderitems.Add(new OrderItem
                {
                    CourseId = it.CourseId,
                    Price = it.Price,
                    Sessions = it.Sessions
                });
            }

            await _context.OrderRepository.AddAsync(order);
            await _context.SaveChangesAsync();

            var payment = new Payment
            {
                OrderId = order.OrderId,
                Amount = order.TotalAmount,
                Status = "pending",
                Note = "PayOS",
                CreatedAt = DateTime.UtcNow,
                CreateBy = learnerId,
                IsActive = true
            };
            await _context.PaymentRepository.AddAsync(payment);
            await _context.SaveChangesAsync();

            // Build an order code
            int orderCode = RandomNumberGenerator.GetInt32(100000, 1000000);
            var returnUrl = string.IsNullOrWhiteSpace(request.ReturnUrlOverride) ? _opts.ReturnUrl : request.ReturnUrlOverride!;
            var cancelUrl = string.IsNullOrWhiteSpace(request.CancelUrlOverride) ? _opts.CancelUrl : request.CancelUrlOverride!;

            // Create payment link via PayOS
            var linkReq = new PayOSCreateLinkRequest(
                                OrderCode: orderCode,
                                Amount: amountVnd,
                                Description: $"Payment for order #{order.OrderId}",
                                ReturnUrl: returnUrl,
                                CancelUrl: cancelUrl,
                                Items: payItems
);

            var linkResp = await _payOS.CreatePaymentLinkAsync(linkReq, cancellationToken);

            // Defensively set transaction code
            payment.TransactionCode = string.IsNullOrWhiteSpace(linkResp.TransactionCode)
                ? orderCode.ToString()
                : linkResp.TransactionCode;

            _context.PaymentRepository.Update(payment);
            await _context.SaveChangesAsync();

            return new CreatePaymentLinkResponse
            {
                OrderId = order.OrderId,
                PaymentId = payment.PaymentId,
                TransactionCode = payment.TransactionCode!,
                CheckoutUrl = linkResp.CheckoutUrl
            };
        }

        // Verify and handle PayOS webhook
        public async Task<bool> HandlePayOSWebhookAsync(WebhookType body, CancellationToken cancellationToken = default)
        {
            WebhookData verified;
            try
            {
                verified = _payOSSdk.verifyPaymentWebhookData(body); // throws if signature invalid
            }
            catch
            {
                return false;
            }

            return await HandlePayOSWebhookCoreAsync(verified, cancellationToken);
        }

        // Business logic using verified data
        private async Task<bool> HandlePayOSWebhookCoreAsync(WebhookData data, CancellationToken ct)
        {
            var transactionCode = data.orderCode.ToString();
            var normalized = NormalizeFromPayOSCode(data.code, data.desc);

            var payment = await _context.PaymentRepository.GetByTransactionCodeAsync(transactionCode);
            if (payment == null) return true; // ignore unknown

            payment.Status = normalized;
            payment.TransactionDate = DateTime.UtcNow;
            _context.PaymentRepository.Update(payment);

            var order = await _context.OrderRepository.GetByIdWithItemsAndCoursesAsync(payment.OrderId);
            if (order != null)
            {
                if (normalized == "success")
                {
                    order.Status = "paid";
                    foreach (var item in order.Orderitems)
                    {
                        var sessions = System.Math.Max(item.Course?.TotalSessions ?? 0, 0);
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

                        await _context.EnrolmentRepository.CreateEnrollment(enrollment, ct);
                    }
                }
                else if (normalized is "canceled" or "failed")
                {
                    order.Status = normalized;
                }
                _context.OrderRepository.Update(order);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private static string NormalizeFromPayOSCode(string code, string desc)
        {
            if (string.Equals(code, "00", StringComparison.OrdinalIgnoreCase)) return "success";
            var d = desc?.ToLowerInvariant() ?? "";
            if (d.Contains("cancel")) return "canceled";
            if (d.Contains("fail") || d.Contains("error")) return "failed";
            return "failed";
        }

        // HMAC SHA256 signature verification (isnt necessary rn)
        private static bool VerifySignature(string body, string signature, string secret)
        {
            try
            {
                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
                var computed = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                return computed == signature.ToLowerInvariant();
            }
            catch
            {
                return false;
            }
        }
    }
}