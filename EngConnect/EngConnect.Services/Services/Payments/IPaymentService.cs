using EngConnect.Entities.Common;
using EngConnect.Services.DTOs.Payments;
using EngConnect.Services.Services.Payments.Filters;

namespace EngConnect.Services.Services.Payments
{
    public interface IPaymentService
    {
        Task<CreatePaymentLinkResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request, CancellationToken cancellationToken = default);
        Task<bool> HandleWebhookAsync(string gatewayName, string payload, CancellationToken cancellationToken = default);
        Task<PagedResult<PaymentAdminDto>> GetForAdminAsync(PaymentSearchFilter filter, CancellationToken cancellationToken = default);
    }
}
