using OrderManagement.Entities;

namespace Core.Entities.Order.Aggregate
{
    public class Order : BaseEntity
    {
        public string CustomerId { get; set; }
        public AppUser? Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentIntentId { get; set; } = string.Empty;
        public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();
    }
}
