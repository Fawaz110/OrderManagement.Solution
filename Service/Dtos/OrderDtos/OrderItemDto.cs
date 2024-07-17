using Core.Entities.OrderAggregate;

namespace Service.Dtos.OrderDtos
{
    public class OrderItemDto
    {
        public ProductOrderItem Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
