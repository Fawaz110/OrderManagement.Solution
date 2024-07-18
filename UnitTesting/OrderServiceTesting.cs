using Core.Entities;
using Core.Entities.Order.Aggregate;
using Core.Entities.OrderAggregate;
using Core.Repositories.Contract;
using Core.Services.Contract;
using Core.UnitsOfWork;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrderManagement.Entities;
using Repository;
using Service.OrderServices;

namespace UnitTesting
{
    public class OrderServiceTesting
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly OrderService _orderService;

        public OrderServiceTesting()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserManager = MockUserManager<AppUser>();
            _mockConfiguration = new Mock<IConfiguration>();
            _orderService = new OrderService(
                _mockUserManager.Object,
                _mockConfiguration.Object,
                _mockUnitOfWork.Object
            );
        }

        private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task CreateOrderAsync_ReturnsNull_WhenUserNotFound()
        {
            // Arrange
            string buyerEmail = "xyz@as.com";
            List<OrderItem> orderItems = new List<OrderItem>();
            PaymentMethod paymentMethod = new PaymentMethod();
            OrderStatus status = new OrderStatus();

            _mockUserManager.Setup(u => u.FindByEmailAsync(buyerEmail))
                .ReturnsAsync((AppUser)null);

            // Act
            var result = await _orderService.CreateOrderAsync(buyerEmail, orderItems, paymentMethod, status);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateOrderAsync_ReturnsOrder_WhenValidInput()
        {
            // Arrange
            string buyerEmail = "mustafa.customer@store.com";
            var user = new AppUser { Id = "1", Email = buyerEmail };
            List<OrderItem> orderItems = new List<OrderItem>()
            {
                    new OrderItem
                    {
                        Product = new ProductOrderItem
                        {
                            ProductId = 1,
                            ProductName = "asda"
                        },
                    }
            };
            PaymentMethod paymentMethod = PaymentMethod.Card;
            OrderStatus status = OrderStatus.Pending;

            var product = new Product { Id = 1, Name = "Product1", Price = 50, Stock = 10 };
            _mockUserManager.Setup(u => u.FindByEmailAsync(buyerEmail)).ReturnsAsync(user);
            _mockUnitOfWork.Setup(p => p.Repository<Product>().GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
            _mockUnitOfWork.Setup(o => o.Repository<Order>().AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(o => o.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _orderService.CreateOrderAsync(buyerEmail, orderItems, paymentMethod, status);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateOrderAsync_UpdatesProductStock_WhenOrderIsCreated()
        {
            // Arrange
            string buyerEmail = "mustafa.customer@store.com";
            var user = new AppUser { Id = "1", Email = buyerEmail };
                List<OrderItem> orderItems = new List<OrderItem>
            {
                new OrderItem { Product = new ProductOrderItem { ProductId = 1 }, Quantity = 2 },
                new OrderItem { Product = new ProductOrderItem { ProductId = 2 }, Quantity = 1 }
            };
            PaymentMethod paymentMethod = new PaymentMethod();
            OrderStatus status = new OrderStatus();

            var product1 = new Product { Id = 1, Name = "Product1", Price = 50, Stock = 10 };
            var product2 = new Product { Id = 2, Name = "Product2", Price = 30, Stock = 5 };

            _mockUserManager.Setup(u => u.FindByEmailAsync(buyerEmail)).ReturnsAsync(user);
            _mockUnitOfWork.Setup(p => p.Repository<Product>().GetByIdAsync(1)).ReturnsAsync(product1);
            _mockUnitOfWork.Setup(p => p.Repository<Product>().GetByIdAsync(2)).ReturnsAsync(product2);
            _mockUnitOfWork.Setup(o => o.Repository<Order>().AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(o => o.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _orderService.CreateOrderAsync(buyerEmail, orderItems, paymentMethod, status);

            // Assert
            Assert.NotNull(result);
            // Verify that the stock has been updated
            _mockUnitOfWork.Verify(p => p.Repository<Product>().GetByIdAsync(1), Times.Once);
            _mockUnitOfWork.Verify(p => p.Repository<Product>().GetByIdAsync(2), Times.Once);
            product1.Stock.Should().Be(8); // 10 - 2
            product2.Stock.Should().Be(4); // 5 - 1
        }
    }
}