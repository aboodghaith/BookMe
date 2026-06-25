using BLL.Common;
using BLL.DTOs.AuthDTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IImageService _imageService;
        private readonly IUrlService _urlService;
        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService,
            IImageService imageService , IUrlService urlService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _imageService = imageService;
            _urlService = urlService;
        }






        public async Task<ApiResponse<object>> LoginUser(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null)
            {
                return ApiResponseHelper.Fail<object>("User not found or account is inactive.", 404);
            }


            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!isPasswordValid)
            {
                return ApiResponseHelper.Fail<object>("The password is incorrect", 401);
            }

            var token = await _tokenService.GenerateToken(user);

            return ApiResponseHelper.Success<object>(new { Token = token }, "Login Successful", 200);
        }









        public async Task<ApiResponse<object>> RegisterUser(RegisterDTO registerDTO)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);

            if (existingUser != null)
            {
                return ApiResponseHelper.Fail<object>("Email already exists", 400);
            }

            var ImageFile = await _imageService.UploadImageAsync(registerDTO.ImagePath, "images");

            var user = new User
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Address = registerDTO.Address,
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                Description = registerDTO?.Description ?? string.Empty,
                ImagePath = string.IsNullOrEmpty(ImageFile) ? null : ImageFile
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();

                return new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "User creation failed",
                    Errors = errors,
                    StatusCode = 400
                };
            }

            bool roleExists = await _roleManager.RoleExistsAsync("User");
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            await _userManager.AddToRoleAsync(user, "User");

            return ApiResponseHelper.Success<object>(null, "User Created Successfully", 201);
        }
    }
}
