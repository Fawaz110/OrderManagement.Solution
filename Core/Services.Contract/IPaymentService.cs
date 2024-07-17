using Core.Entities.Order.Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Contract
{
    public interface IPaymentService
    {
        Task<Order> CreateOrUpdatePaymentIntent(int orderId);
        Task<Order> UpdatePaymentIntentStatus(string paymentIntentId, bool succeeded);
    }
}
