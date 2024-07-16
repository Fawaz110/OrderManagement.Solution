using Core.Entities.Order.Aggregate;
using OrderManagement.Entities;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Dtos
{
    public class OrderDto
    {
        [Required]
        [EmailAddress]
        public string buyerEmail { get; set; }
        [Required]
        [Range(0, 1, ErrorMessage = "Available payment methods: 0 for Card, 1 for PayPal")]
        public PaymentMethod PaymentMethod { get; set; }
        [Required]
        [Range(0, 2, ErrorMessage = "Status must be: 0 for Pending, 1 for PaymentSucceded and 2 for PaymentFailed")]
        public OrderStatus OrderStatus { get; set; } 
        [Required]
        public List<OrderItem> Items { get; set; }
    }
}
