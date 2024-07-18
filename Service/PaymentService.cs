using Core.Entities.Order.Aggregate;
using Core.Repositories.Contract;
using Core.Services.Contract;
using Core.Specifications.OrderSpeicifcations;
using Core.UnitsOfWork;
using Microsoft.Extensions.Configuration;
using Stripe;
using Entities = Core.Entities;
namespace Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }
        public async Task<Order> CreateOrUpdatePaymentIntent(int orderId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            var spec = new OrderWithItemsSpeicifcations(orderId);

            var order = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);

            if (order is null) return null;

            if(order?.Items?.Count() > 0)
            {
                foreach (var item in order?.Items)
                {
                    var product = await _unitOfWork.Repository<Entities.Product>().GetByIdAsync(item.Product.ProductId);

                    if(product.Price != item.UnitPrice)
                        item.UnitPrice = product.Price;
                }

                var total = order.Items.Sum(o => o.UnitPrice * o.Quantity);

                if (total > 200)
                    total = total * 0.9m;
                else if (total > 100)
                    total = total * 0.95m;

                if (order.TotalAmount != total)
                    order.TotalAmount = total;
            }

            PaymentIntent paymentIntent;

            PaymentIntentService paymentIntentService = new PaymentIntentService();

            if (string.IsNullOrEmpty(order.PaymentIntentId)) // Create New Payment Intent
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(order.TotalAmount * 100),
                    Currency = "usd",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    }
                };

                paymentIntent = await paymentIntentService.CreateAsync(options); 

                order.PaymentIntentId = paymentIntent.Id;
                order.ClienSecret = order.ClienSecret;
            } // Create New Payment Intent
            else // Update Payment Intent
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)(order.Items.Sum(i => i.UnitPrice * i.Quantity * 100))
                };

                await paymentIntentService.UpdateAsync(order.PaymentIntentId, options);
            }// Update Payment Intent

            _unitOfWork.Repository<Order>().Update(order);

            await _unitOfWork.CompleteAsync();

            return order;
        }

        public async Task<Order> UpdatePaymentIntentStatus(string paymentIntentId, bool succeeded)
        {
            var spec = new OrderWithPaymentIntentIdSpecifications(paymentIntentId);

            var order = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);

            if (succeeded)
            {
                order.Status = OrderStatus.PaymentSucceded;

                var invoice = new Entities.Invoice
                {
                    OrderId = order.Id,
                    TotalAmount = order.TotalAmount
                };
                if(invoice != null)
                    await _unitOfWork.Repository<Entities.Invoice>().AddAsync(invoice);
            }
            else
                order.Status = OrderStatus.PaymentFailed;

            _unitOfWork.Repository<Order>().Update(order);

            await _unitOfWork.CompleteAsync();

            return order;
        }
    }
}
