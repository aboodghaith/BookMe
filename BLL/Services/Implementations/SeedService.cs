using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;


namespace BLL.Services.Implementations
{
    public class SeedService
    {

        private readonly UserManager<User> _user;
        private readonly RoleManager<IdentityRole> _role;

        private readonly IConfiguration _config;

        public SeedService(UserManager<User> user , RoleManager<IdentityRole> role, IConfiguration config)
        {
            
            _user = user; 
            _role = role;

            _config = config;
        }


        public async Task SeedAdmin()
        {

            await SeedRole("Admin");

            string AdminEmail = _config["SeedAdmin:Email"]!;
            string AdminPassword = _config["SeedAdmin:Password"]!;

            if (await _user.FindByEmailAsync(AdminEmail) == null) {

                var User = new User()
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,

                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true,
                   
                };


                var result = await _user.CreateAsync(User  , AdminPassword);

                if (result.Succeeded) {

                    await _user.AddToRoleAsync(User, "Admin");

                } else
                {
                    var Errors = string.Join(", ", result.Errors.Select(e => e.Description));

                    throw new Exception(Errors);
                }
            
            } 


        }


        public async Task SeedRoles()
        {
            await SeedRole("User");
            await SeedRole("ServiceProvider");

        }

        public async Task SeedRole(string roleName)
        {
            if (!await _role.RoleExistsAsync(roleName)) {

                var result = await _role.CreateAsync(new IdentityRole { Name = roleName}); 
            
            } 
        }
    }
}
