using System.Runtime.Serialization;

namespace Core.Entities.Order.Aggregate
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,

        [EnumMember(Value = "Payment Succeded")]
        PaymentSucceded,
        
        [EnumMember(Value = "Payment Failed")]
        PaymentFailed
    }
}
