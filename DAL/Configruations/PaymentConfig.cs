using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Configruations
{
    public class PaymentConfig : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
           builder.HasIndex(p => p.SubscriptionId);

            builder.HasIndex(p => new { p.SubscriptionId, p.PaymentStatus });

            builder.HasOne(p => p.Subscription).WithOne().HasForeignKey<Payment>(p => p.SubscriptionId).IsRequired(true).OnDelete(DeleteBehavior.NoAction);
            builder.HasQueryFilter(p => !p.IsDeleted);

        }
    }
}
