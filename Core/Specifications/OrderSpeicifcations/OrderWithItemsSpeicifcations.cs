using Core.Entities.Order.Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications.OrderSpeicifcations
{
    public class OrderWithItemsSpeicifcations : BaseSpecifications<Order>
    {
        public OrderWithItemsSpeicifcations() 
            : base()
        {
            AddIncludes();
        }

        public OrderWithItemsSpeicifcations(int id)
            : base(O => O.Id == id)
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            Includes.Add(O => O.Items);
            Includes.Add(O => O.Customer);
        }
    }
}
