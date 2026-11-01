namespace EngConnect.Services.Integrations.Payment
{
    public class PaymentGatewayException : Exception
    {
        public string GatewayName { get; }

        public PaymentGatewayException(string gatewayName, string message, Exception? innerException = null)
            : base(message, innerException)
        {
            GatewayName = gatewayName;
        }
    }
}
