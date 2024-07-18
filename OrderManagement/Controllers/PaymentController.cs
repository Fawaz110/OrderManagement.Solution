using Core.Entities.Order.Aggregate;
using Core.Services.Contract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Entities;
using OrderManagement.Errors;
using OrderManagement.Helper;
using Stripe;

namespace OrderManagement.Controllers
{
    public class PaymentController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private const string _whSecret = "whsec_6f90552d7e57bd2b37c4a24f189239b8faf5d48d72cbd2f2dfa40db84a8bbbc7";

        public PaymentController(
            IPaymentService paymentService,
            ILogger<PaymentController> logger,
            UserManager<AppUser> userManager)
        {
            _paymentService = paymentService;
            _logger = logger;
            _userManager = userManager;
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
                AppUser customer;

                switch (stripeEvent.Type)
                {
                    case Events.PaymentIntentSucceeded:
                        order = await _paymentService.UpdateOrderStatus(paymentIntent.Id, true);
                        _logger.LogWarning("Status SUcceeded: ", order.PaymentIntentId);
                        customer = await _userManager.FindByIdAsync(order.CustomerId);
                        EmailSettings.SendEmail(customer.Email, "Change in order status - Order Mnagament", order);
                        break;
                    case Events.PaymentIntentPaymentFailed: 
                        order = await _paymentService.UpdateOrderStatus(paymentIntent.Id, false);
                        customer = await _userManager.FindByIdAsync(order.CustomerId);
                        EmailSettings.SendEmail(customer.Email, "Change in order status - Order Mnagament", order);
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
