using Core.Entities;
using Core.Entities.Order.Aggregate;
using Core.Entities.OrderAggregate;
using Core.Repositories.Contract;
using Core.Specifications.OrderSpeicifcations;
using Core.UnitsOfWork;
using Microsoft.Extensions.Configuration;
using Moq;
using Service;
using Stripe;

namespace UnitTesting
{
    public class PaymentServiceTesting
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly PaymentService _paymentService;
        public PaymentServiceTesting()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _paymentService = new PaymentService(
                _mockConfiguration.Object,
                _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task CreateOrUpdatePaymentIntent_ReturnsUpdatedOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = 1;
            var order = new Order
            {
                Id = orderId,
                TotalAmount = 100,
                Items = new List<OrderItem>
            {
                new OrderItem { Product = new ProductOrderItem { ProductId = 1 }, Quantity = 2, UnitPrice = 50 }
            },
                ClienSecret = "sda",
                PaymentIntentId = "pi_3PdVoKDr4hz1mUok33iEmuYH",
            };

            _mockConfiguration.Setup(c => c["StripeSettings:SecretKey"]).Returns("sk_test_51PDClRDr4hz1mUoke50rdh3S6cVIJF0KUKSJEkssIA2qzSdkKDjAKHogH8gAVB4gbI6Mu5nqkWzRTcT8OdGtAYTt00eEoOlb97");
            _mockUnitOfWork.Setup(r => r.Repository<Order>().GetWithSpecAsync(It.IsAny<OrderWithItemsSpeicifcations>())).ReturnsAsync(order);
            _mockUnitOfWork.Setup(r => r.Repository<Core.Entities.Product>().GetByIdAsync(1)).ReturnsAsync(new Core.Entities.Product { Id = 1, Price = 50 });

            // Act
            var result = await _paymentService.CreateOrUpdatePaymentIntent(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.TotalAmount, result.TotalAmount);
            Assert.Equal(order.PaymentIntentId, result.PaymentIntentId);
        }

        [Fact]
        public async Task CreateOrUpdatePaymentIntent_ReturnsNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = 1;
            _mockUnitOfWork.Setup(r => r.Repository<Order>().GetWithSpecAsync(It.IsAny<OrderWithItemsSpeicifcations>())).ReturnsAsync((Order)null);

            // Act
            var result = await _paymentService.CreateOrUpdatePaymentIntent(orderId);

            // Assert
            Assert.Null(result);
            _mockUnitOfWork.Verify(r => r.Repository<Order>().Update(It.IsAny<Order>()), Times.Never);
            _mockUnitOfWork.Verify(r => r.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdatePaymentIntentStatus_UpdatesOrderStatusToFailed_WhenSucceededIsFalse()
        {
            // Arrange
            var paymentIntentId = "pi_123";
            var order = new Order { Id = 1, PaymentIntentId = paymentIntentId, CustomerId = "user_123" };

            _mockUnitOfWork.Setup(r => r.Repository<Order>().GetWithSpecAsync(It.IsAny<OrderWithPaymentIntentIdSpecifications>())).ReturnsAsync(order);

            // Act
            var result = await _paymentService.UpdatePaymentIntentStatus(paymentIntentId, false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(OrderStatus.PaymentFailed, result.Status);
        }
    }
}
