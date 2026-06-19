using BLL.Services.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<User> userManager;

        public TokenService(IConfiguration configuration, UserManager<User> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
        }
        public async Task<string> GenerateToken(User user)
        {

            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

            var Creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var Roles = await userManager.GetRolesAsync(user);
            var Claims = new List<Claim>(){
                new Claim(ClaimTypes.NameIdentifier, user.Id ),
                new Claim(ClaimTypes.Email , user.Email!),
                new Claim(ClaimTypes.Name , user.UserName!)
                };
            if (Roles != null)
            {
                foreach (var role in Roles)
                {
                    Claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            var JwtToken = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
               audience: configuration["Jwt:Audience"],
               claims: Claims,
               expires: DateTime.Now.AddHours(Convert.ToDouble(configuration["Jwt:Expires"])),
               signingCredentials: Creds
                );
            var Token = new JwtSecurityTokenHandler().WriteToken(JwtToken);
            return Token;
        }
    }
}
