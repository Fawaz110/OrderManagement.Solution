namespace Core.Entities
{
    public class Invoice : BaseEntity
    {
        //InvoiceId, OrderId, InvoiceDate, TotalAmount
        public int OrderId { get; set; }
        public Order.Aggregate.Order? Order { get; set; }
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
    }
}
