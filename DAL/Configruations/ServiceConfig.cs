using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DAL.Configruations
{
    public class ServiceConfig : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasOne(s => s.City).WithMany(s => s.Services).HasForeignKey(s => s.CityId);
            builder.HasOne(s => s.Category).WithMany(s => s.Services).HasForeignKey(s => s.CategoryId);
            builder.HasOne(s => s.ServiceProvider).WithMany(s => s.Services).HasForeignKey(s => s.ServiceProviderId);
            builder.Property(s => s.Name).HasMaxLength(100);
            builder.Property(s => s.Description).HasMaxLength(500);
            builder.HasIndex(s => new { s.ServiceProviderId , s.IsDeleted });
            builder.HasIndex(s => s.CityId);
            builder.HasIndex(s => s.CategoryId);
            builder.HasQueryFilter(s => !s.IsDeleted);
           
        }
    }
}
