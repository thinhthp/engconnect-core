namespace EngConnect.Services.Integrations.Payment
{
    // factory
    public interface IPaymentGatewayResolver
    {
        IPaymentGateway Resolve(string? gatewayName);
    }
}
