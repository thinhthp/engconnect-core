using EngConnect.Services.DTOs.Payments;
using EngConnect.Services.Services.Payments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Net.payOS.Types;
using System.Text;

namespace EngConnect.Api.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost("link/payos")]
        public async Task<IActionResult> CreatePaymentLink([FromBody] CreatePaymentLinkRequest request, CancellationToken ct)
        {
            var result = await _service.CreatePaymentLinkAsync(request, ct);
            return Ok(result);
        }

        [HttpPost("webhook/payos")]
        public async Task<IActionResult> Webhook([FromBody] WebhookType body, CancellationToken ct)
        {
            var ok = await _service.HandlePayOSWebhookAsync(body, ct);
            return ok ? Ok(new { code = 0, message = "Ok" }) : Ok(new { code = -1, message = "fail" });
        }

        // for be callback/redirect after payment (isnt necessary)
        [HttpGet("return")]
        public IActionResult Return([FromQuery] string? code)
        {
            // fe handle anyway
            return Ok(new { message = "Payment returned", transactionCode = code });
        }

        // for be callback/redirect after cancel payment (isnt necessary)
        [HttpGet("cancel")]
        public IActionResult Cancel([FromQuery] string? code)
        {
            return Ok(new { message = "Payment cancelled", transactionCode = code });
        }
    }
}