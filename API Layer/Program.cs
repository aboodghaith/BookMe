
using BLL.Services.Implementations;
using DAL.Data;
using DAL.Models;
using DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API_Layer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Edit controller to stop return default status code to model state 
            builder.Services.AddControllers().ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

            // Add Cors Policy 
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Add services to the container.

            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            // Add DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
            });

            // Add Unit Of Work 
            builder.Services.AddScoped<IUnitOfWork , UnitOfWork>();
            // Add Identity 
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>();



            // SeedService 
            builder.Services.AddScoped<SeedService>();


            var app = builder.Build();


            // SeedData
            using (var scope =  app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

  

                if(!context.Cities.Any())
                {

                    context.Cities.AddRange(
                          new City { Name = "Cairo" },
                          new City { Name = "Alexandria" }
                        ); 
                }

                context.SaveChanges();

                var servic = scope.ServiceProvider.GetRequiredService<SeedService>();
                await servic.SeedAdmin();
                await servic.SeedRoles();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}




// 1 : create Seed Service to add role and conceted with User 
// 2 : create crud and business logic of Service Model 
        // 1 : Create Service 
        // 2 : 
// 3 : create crud and business logic of Booking Model 
// 4 : 