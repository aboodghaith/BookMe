using DAL.Configruations;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<User> Users {  get; set; }
        public DbSet<Service> Services { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Booking> Bookings { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) 
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserConfig());
            builder.ApplyConfiguration(new ServiceConfig());
            builder.ApplyConfiguration(new BookingConfig());
            builder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
            builder.Entity<City>().HasQueryFilter(c => !c.IsDeleted);


            base.OnModelCreating(builder);
        }
    }
}
