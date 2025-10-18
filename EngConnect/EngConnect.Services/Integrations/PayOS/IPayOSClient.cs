using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EngConnect.Services.Integrations.PayOS
{
    public record PayOSItem(string Name, int Quantity, long Price);

    public record PayOSCreateLinkRequest(
        int OrderCode,
        long Amount,
        string Description,
        string ReturnUrl,
        string CancelUrl,
        IReadOnlyList<PayOSItem> Items
    );

    public record PayOSCreateLinkResponse(
        string CheckoutUrl,
        string TransactionCode
    );

    public interface IPayOSClient
    {
        Task<PayOSCreateLinkResponse> CreatePaymentLinkAsync(PayOSCreateLinkRequest request, CancellationToken ct = default);
    }
}