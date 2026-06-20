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
    public class SubscriptionTypeConfig : IEntityTypeConfiguration<SubscriptionType>
    {
        public void Configure(EntityTypeBuilder<SubscriptionType> builder)
        {
            
            builder.Property(st => st.Name)
                   .IsRequired(true)
                   .HasMaxLength(100);

            
            builder.Property(st => st.Price)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired(true);

           
            builder.Property(st => st.DurationDays)
                   .IsRequired(true);

            
            builder.HasQueryFilter(st => !st.IsDeleted);
        }
    }
}
