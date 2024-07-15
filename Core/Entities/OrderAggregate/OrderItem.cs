using Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Order.Aggregate
{
    public class OrderItem : BaseEntity
    {
        public OrderItem()
        {
            
        }

        public OrderItem(ProductOrderItem product, int orderId, int quantity, decimal unitPrice)
        {
            Product = product;
            OrderId = orderId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        // OrderItemId, OrderId, ProductId, Quantity, UnitPrice,

        // Discount => canceled becouse discount depends on total in order (business logic)
        public ProductOrderItem Product { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

    }
}
