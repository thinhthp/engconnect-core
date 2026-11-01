using Microsoft.Extensions.Logging;

namespace EngConnect.Services.Integrations.Payment
{
    // factory
    public sealed class PaymentGatewayResolver : IPaymentGatewayResolver
    {
        private readonly IReadOnlyDictionary<string, IPaymentGateway> _gateways;
        private readonly ILogger<PaymentGatewayResolver> _logger;

        public PaymentGatewayResolver(IEnumerable<IPaymentGateway> gateways, ILogger<PaymentGatewayResolver> logger)
        {
            _gateways = gateways.ToDictionary(g => g.Name, StringComparer.OrdinalIgnoreCase);
            _logger = logger;
        }

        public IPaymentGateway Resolve(string? gatewayName)
        {
            var name = string.IsNullOrWhiteSpace(gatewayName)
                ? PaymentGateways.PayOS
                : gatewayName.Trim();

            if (_gateways.TryGetValue(name, out var gateway))
                return gateway;

            _logger.LogError(
                "No payment gateway registered for {GatewayName}. Registered: {Registered}",
                name,
                string.Join(", ", _gateways.Keys));

            throw new PaymentGatewayException(name, $"Payment gateway '{name}' is not registered.");
        }
    }
}
