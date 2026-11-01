using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace EngConnect.Services.Integrations.Payment.PayOS
{
    public sealed class PayOSAdapter : IPaymentGateway
    {
        private readonly Net.payOS.PayOS _sdk;
        private readonly PayOSOptions _options;
        private readonly ILogger<PayOSAdapter> _logger;

        public PayOSAdapter(
            Net.payOS.PayOS sdk,
            IOptions<PayOSOptions> options,
            ILogger<PayOSAdapter> logger)
        {
            _sdk = sdk;
            _options = options.Value;
            _logger = logger;
        }

        public string Name => PaymentGateways.PayOS;

        public async Task<PaymentLinkResult> CreatePaymentLinkAsync(PaymentLinkRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Amount <= 0)
                throw new PaymentGatewayException(Name, "Payment amount must be greater than zero.");

            if (request.Items is null || request.Items.Count == 0)
                throw new PaymentGatewayException(Name, "At least one payment item is required.");

            var returnUrl = string.IsNullOrWhiteSpace(request.ReturnUrl) ? _options.ReturnUrl : request.ReturnUrl;
            var cancelUrl = string.IsNullOrWhiteSpace(request.CancelUrl) ? _options.CancelUrl : request.CancelUrl;

            if (string.IsNullOrWhiteSpace(returnUrl) || string.IsNullOrWhiteSpace(cancelUrl))
                throw new PaymentGatewayException(Name, "PayOS returnUrl and cancelUrl must be configured.");

            var orderCode = CreateOrderCode(request.ReferenceId);
            var items = request.Items
                .Select(i => new ItemData(i.Name, i.Quantity, ToPayOSAmount(i.UnitAmount)))
                .ToList();

            var paymentData = new PaymentData(
                orderCode,
                ToPayOSAmount(request.Amount),
                TruncateDescription(request.Description),
                items,
                cancelUrl,
                returnUrl
            );

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var response = await _sdk.createPaymentLink(paymentData);

                if (string.IsNullOrWhiteSpace(response.checkoutUrl))
                    throw new PaymentGatewayException(Name, "PayOS returned an empty checkout URL.");

                var transactionCode = response.orderCode == 0
                    ? orderCode.ToString()
                    : response.orderCode.ToString();

                _logger.LogInformation(
                    "PayOS payment link created. ReferenceId={ReferenceId} OrderCode={OrderCode}",
                    request.ReferenceId,
                    transactionCode);

                return new PaymentLinkResult(response.checkoutUrl, transactionCode);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (PaymentGatewayException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "PayOS createPaymentLink failed. ReferenceId={ReferenceId} Amount={Amount}",
                    request.ReferenceId,
                    request.Amount);
                throw new PaymentGatewayException(Name, "Failed to create PayOS payment link.", ex);
            }
        }

        // Verify and return webhook data
        public Task<PaymentWebhookResult?> VerifyAndParseWebhookAsync(string payload, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                _logger.LogWarning("PayOS webhook payload was empty.");
                return Task.FromResult<PaymentWebhookResult?>(null);
            }

            WebhookType? body;
            try
            {
                body = JsonConvert.DeserializeObject<WebhookType>(payload);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "PayOS webhook payload could not be deserialized.");
                return Task.FromResult<PaymentWebhookResult?>(null);
            }

            if (body is null)
            {
                _logger.LogWarning("PayOS webhook payload deserialized to null.");
                return Task.FromResult<PaymentWebhookResult?>(null);
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var verified = _sdk.verifyPaymentWebhookData(body); // Return WebhookData(long orderCode, int amount, string description, string accountNumber, string reference, string transactionDateTime, string currency, string paymentLinkId, string code, string desc, string? counterAccountBankId, string? counterAccountBankName, string? counterAccountName, string? counterAccountNumber, string? virtualAccountName, string virtualAccountNumber);
                var status = NormalizeStatus(verified.code, verified.desc);

                _logger.LogInformation(
                    "PayOS webhook verified. OrderCode={OrderCode} GatewayCode={GatewayCode} Status={Status}",
                    verified.orderCode,
                    verified.code,
                    status);

                // Return needed data from webhook to service
                return Task.FromResult<PaymentWebhookResult?>(new PaymentWebhookResult(
                    TransactionCode: verified.orderCode.ToString(),
                    Status: status,
                    GatewayCode: verified.code,
                    Description: verified.desc,
                    Amount: verified.amount
                ));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "PayOS webhook signature verification failed.");
                return Task.FromResult<PaymentWebhookResult?>(null);
            }
        }

        private static int ToPayOSAmount(long amount)
        {
            if (amount <= 0 || amount > int.MaxValue)
                throw new PaymentGatewayException(PaymentGateways.PayOS, $"PayOS amount is out of range: {amount}.");

            return (int)amount;
        }

        private static int CreateOrderCode(long referenceId)
        {
            var suffix = (int)(Math.Abs(referenceId) % 1000);
            var prefix = RandomNumberGenerator.GetInt32(100000, 999999);
            return prefix * 1000 + suffix;
        }

        private static string TruncateDescription(string description)
        {
            var value = string.IsNullOrWhiteSpace(description) ? "Payment" : description.Trim();
            return value.Length <= 25 ? value : value[..25];
        }

        private static string NormalizeStatus(string? code, string? desc)
        {
            if (string.Equals(code, "00", StringComparison.OrdinalIgnoreCase))
                return PaymentStatuses.Success;

            var d = desc?.ToLowerInvariant() ?? string.Empty;
            if (d.Contains("cancel"))
                return PaymentStatuses.Canceled;

            return PaymentStatuses.Failed;
        }
    }
}
