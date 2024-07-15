using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Order.Aggregate
{
    public enum PaymentMethod
    {
        [EnumMember(Value = "Card")]
        Card,

        [EnumMember(Value = "PayPal")]
        PayPal
    }
}
