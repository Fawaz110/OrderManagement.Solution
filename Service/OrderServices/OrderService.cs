﻿using Core.Entities;
using Core.Entities.Order.Aggregate;
using Core.Entities.OrderAggregate;
using Core.Repositories.Contract;
using Core.Services.Contract;
using Core.Specifications.OrderSpeicifcations;
using Microsoft.AspNetCore.Identity;
using OrderManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Invoice> _invoiceRepository;
        private readonly UserManager<AppUser> _userManager;

        public OrderService(
            IGenericRepository<Order> orderRepository,
            IGenericRepository<Product> productRepository,
            IGenericRepository<Invoice> invoiceRepository,
            UserManager<AppUser> userManager) 
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _invoiceRepository = invoiceRepository;
            _userManager = userManager;
        }
        public async Task<Order> CreateOrderAsync(string buyerEmail, List<OrderItem> orderItems, PaymentMethod paymentMethod, OrderStatus status)
        {
            // 1. Get User By Email To Send his Id
            var user = await _userManager.FindByEmailAsync(buyerEmail);

            if (user is null)
                return null;

            // 2. Collect Order Items & Calculate Total

            var itemsToAdd = new List<OrderItem>();

            if(orderItems.Count() > 0)
            {
                foreach (var item in orderItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.Product.ProductId);

                    if(product is not null)
                    {
                        var prodItemOrdered = new ProductOrderItem
                        {
                            ProductId = product.Id,
                            ProductName = product.Name,
                        };

                        var productItem = new OrderItem(prodItemOrdered, item.Quantity, product.Price);

                        itemsToAdd.Add(productItem);
                    }
                }
            }

            var total = itemsToAdd.Sum(o => o.UnitPrice * o.Quantity);

            if (total > 200)
                total = total * 0.9m;
            else if (total > 100)
                total = total * 0.95m;


            // 3. Add & SaveChanges

            var order = new Order
            {
                CustomerId = user.Id,
                Items = itemsToAdd,
                PaymentMethod = paymentMethod,
                Status = status,
                TotalAmount = total,
            };

            await _orderRepository.AddAsync(order);

            var result = await _orderRepository.CompleteAsync();

            if (result <= 0)
                return null;

            // [TODO] add invoice here

            var invoice = new Invoice
            {
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
            };

            await _invoiceRepository.AddAsync(invoice);

            result = await _invoiceRepository.CompleteAsync();

            if (result <= 0)
                return null;

            return order;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
            => await _orderRepository.GetAllAsync();

        public async Task<IEnumerable<Order>> GetAllOrdersForUserAsync(string customerId)
        {
            var spec = new OrderWithItemsSpeicifcations();

            var result = await _orderRepository.GetAllWithSpecAsync(spec);

            var orders = result.Where(O => O.CustomerId == customerId).ToList();

            return orders;
        }

        public async Task<IEnumerable<Order>> GetAllWithSpecAsync(OrderWithItemsSpeicifcations spec)
            => await _orderRepository.GetAllWithSpecAsync(spec);

        public async Task<Order> GetByIdAsync(int id)
            => await _orderRepository.GetByIdAsync(id);

        public async Task<Order> GetWithSpecAsync(OrderWithItemsSpeicifcations spec)
            => await _orderRepository.GetWithSpecAsync(spec);

        public async Task<Order> UpdateStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order is null)
                return null;

            if (order.Status != status)
                order.Status = status;

            var result = await _orderRepository.CompleteAsync();

            if (result <= 0)
                return null;

            return order;
        }
    }
}