using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EngConnect.Services.Integrations.PayOS
{
    public class PayOSAdapter : IPayOSClient
    {
        private readonly Net.payOS.PayOS _sdk;

        public PayOSAdapter(Net.payOS.PayOS sdk)
        {
            _sdk = sdk;
        }

        public async Task<PayOSCreateLinkResponse> CreatePaymentLinkAsync(PayOSCreateLinkRequest request, CancellationToken ct = default)
        {
            var items = request.Items.Select(i => new ItemData(i.Name, i.Quantity, (int)i.Price)).ToList();

            // SDK PaymentData signature/formatting handled internally
            var paymentData = new PaymentData(
                request.OrderCode,
                (int)request.Amount,
                request.Description,
                items,
                request.CancelUrl,
                request.ReturnUrl
            );

            var res = await _sdk.createPaymentLink(paymentData);
            var txnCode = res.orderCode.ToString();
            return new PayOSCreateLinkResponse(res.checkoutUrl, txnCode);
        }
    }
}