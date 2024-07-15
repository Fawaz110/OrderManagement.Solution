using Core.Entities.Order.Aggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DbContexts.Config
{
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.Status).HasConversion(S => S.ToString(), S => (OrderStatus)Enum.Parse(typeof(OrderStatus), S));
            builder.Property(o => o.PaymentMethod).HasConversion(S => S.ToString(), S => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), S));

            builder.Property(O => O.TotalAmount).HasColumnType("decimal(18,2)");
        }
    }
}
