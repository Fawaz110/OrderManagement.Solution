using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications.InvoiceSpecifications
{
    public class InvoiceWithOrderSpecifications : BaseSpecifications<Invoice>
    {
        public InvoiceWithOrderSpecifications()
            : base()
        {
            AddIncludes();
        }

        public InvoiceWithOrderSpecifications(int id)
                    : base(inv => inv.Id == id)
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            Includes.Add(inv => inv.Order);
            Includes.Add(inv => inv.Order.Customer);
            Includes.Add(inv => inv.Order.Items);
        }
    }
}
