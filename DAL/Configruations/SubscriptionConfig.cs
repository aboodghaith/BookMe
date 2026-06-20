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
    public class SubscriptionConfig : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
         
            builder.Property(s => s.StartDate)
                   .IsRequired(true);

            builder.Property(s => s.EndDate)
                   .IsRequired(true);

            
            builder.Property(s => s.IsActive)
                   .HasDefaultValue(true);

            
            builder.HasOne(s => s.ServiceProvider)
                   .WithMany() 
                   .HasForeignKey(s => s.ServiceProviderID) 
                   .IsRequired(true)
                   .OnDelete(DeleteBehavior.Cascade);

            
            builder.HasOne(s => s.SubscriptionType)
                   .WithMany() 
                   .HasForeignKey(s => s.SubscriptionTypeId)
                   .IsRequired(true)
                   .OnDelete(DeleteBehavior.NoAction); 

          
            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}
