using EngConnect.Services.DTOs.Payments;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Payments
{
    public interface IPaymentService
    {
        Task<CreatePaymentLinkResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request, CancellationToken cancellationToken = default);
        Task<bool> HandlePayOSWebhookAsync(WebhookType body, CancellationToken cancellationToken = default);
    }
}