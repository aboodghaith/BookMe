using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DAL.Configruations
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FirstName).HasMaxLength(70);
            builder.Property(u => u.LastName).HasMaxLength(70);
            builder.Property(u => u.UserName).IsRequired(true);
            builder.Property(u => u.Email).IsRequired(true);
            builder.Property(u => u.PhoneNumber).HasMaxLength(20);

            builder.HasQueryFilter(u => !u.IsDeleted);

        }
    }
}
