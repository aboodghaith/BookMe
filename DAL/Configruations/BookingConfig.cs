using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DAL.Configruations
{
    public class BookingConfig : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {

            builder.HasOne(b => b.Customer)
                   .WithMany()
                   .HasForeignKey(b => b.CustomerId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(b => b.Service)
                   .WithMany()
                   .HasForeignKey(b => b.ServiceId)
                   .OnDelete(DeleteBehavior.NoAction);

           
            builder.Property(b => b.Status)
                   .HasConversion<string>()
                   .HasColumnType("nvarchar(20)")
                   .HasMaxLength(20);

            builder.HasIndex(b => b.ServiceId);

            builder.HasIndex(b => b.CustomerId);

            builder.HasIndex(b => b.StartDateTime);

            builder.HasIndex(b => new { b.ServiceId, b.StartDateTime });

           


        }
    }
}
