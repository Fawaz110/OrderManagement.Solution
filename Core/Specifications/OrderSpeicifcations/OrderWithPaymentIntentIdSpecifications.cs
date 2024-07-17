using Core.Entities.Order.Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications.OrderSpeicifcations
{
    public class OrderWithPaymentIntentIdSpecifications : BaseSpecifications<Order>
    {
        public OrderWithPaymentIntentIdSpecifications(string paymentIntentId)
            :base(O => O.PaymentIntentId == paymentIntentId)
        {
            
        }
    }
}
