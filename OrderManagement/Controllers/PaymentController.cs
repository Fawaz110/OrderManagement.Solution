using Core.Entities.Order.Aggregate;
using Core.Repositories.Contract;
using Core.Services.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Errors;
using Stripe;

namespace OrderManagement.Controllers
{
    public class PaymentController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        private const string _whSecret = "whsec_6f90552d7e57bd2b37c4a24f189239b8faf5d48d72cbd2f2dfa40db84a8bbbc7";

        public PaymentController(
            IPaymentService paymentService,
            ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{orderId}")]
        public async Task<ActionResult<Order>> CreateOrUpdatePaymentIntent(int orderId)
        {
            var order = await _paymentService.CreateOrUpdatePaymentIntent(orderId);

            if (order is null)
                return BadRequest(new ApiResponse(400, "an error with order"));

            return Ok(order);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> WebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], _whSecret);

                var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;

                Order order;

                switch (stripeEvent.Type)
                {
                    case Events.PaymentIntentSucceeded:
                        order = await _paymentService.UpdatePaymentIntentStatus(paymentIntent.Id, true);
                        _logger.LogWarning("Status SUcceeded: ", order.PaymentIntentId);
                        break;
                    case Events.PaymentIntentPaymentFailed: 
                        order = await _paymentService.UpdatePaymentIntentStatus(paymentIntent.Id, false);
                        _logger.LogWarning("Status Failed: ", order.PaymentIntentId);
                    break;
                    default:
                        Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                        break;
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
