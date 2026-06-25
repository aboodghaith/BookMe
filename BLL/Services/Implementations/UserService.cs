using BLL.Common;
using BLL.DTOs.AuthDTOs;
using BLL.DTOs.UserDTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly UserManager<User> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IImageService _imageService; 
        private readonly IUrlService _urlService;     
        public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IImageService imageService, IUrlService urlService)
        {
            _unitOfWork = unitOfWork;

            _userManager = userManager;
            _roleManager = roleManager;
            _imageService = imageService;
            _urlService = urlService;     
        }

        public async Task<ApiResponse<List<UserReadDTO>>> GetAllUsersAsync(bool ignoreDeleted = true)
        {
            var users = await _unitOfWork.UserRepository.GetAllUserAsync(ignoreDeleted);
            var baseUrl = _urlService.GetBaseUrl();
            var userDtos = users.Select(u => new UserReadDTO
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Address = u.Address,
                ImagePath = string.IsNullOrEmpty(u.ImagePath) ? null : $"{baseUrl}{u.ImagePath}",
                Description = u.Description
            }).ToList();

            return ApiResponseHelper.Success(userDtos, "Users retrieved successfully.");
        }

        public async Task<ApiResponse<List<UserReadDTO>>> GetUsersWithPaginationAsync(int pageNumber, int pageSize, bool ignoreDeleted = true)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return ApiResponseHelper.Fail<List<UserReadDTO>>("Page number and page size must be greater than zero.", 400);
            }

            var users = await _unitOfWork.UserRepository.GetAllUserWithPaginationAsync(pageNumber, pageSize, ignoreDeleted);
            var baseUrl = _urlService.GetBaseUrl();
            var userDtos = users.Select(u => new UserReadDTO
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Address = u.Address,
                ImagePath = string.IsNullOrEmpty(u.ImagePath) ? null : $"{baseUrl}{u.ImagePath}",
                Description = u.Description
            }).ToList();

            return ApiResponseHelper.Success(userDtos, "Paginated users retrieved successfully.");
        }

        public async Task<ApiResponse<UserReadDTO>> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return ApiResponseHelper.Fail<UserReadDTO>("User not found.", 404);
            }
            var baseUrl = _urlService.GetBaseUrl();

            var userDto = new UserReadDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                ImagePath = string.IsNullOrEmpty(user.ImagePath) ? null : $"{baseUrl}{user.ImagePath}",
                Description = user.Description
            };

            return ApiResponseHelper.Success(userDto, "User details retrieved successfully.");
        }

        public async Task<ApiResponse<bool>> DeactivateUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);


            if (user == null)
            {
                return ApiResponseHelper.Fail<bool>("User not found.", 404);
            }

            user.IsDeleted = true;
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChanges();

            return ApiResponseHelper.Success(true, "User deactivated successfully.");
        }


        public async Task<ApiResponse<bool>> UpdateUserProfileAsync(string id, UserUpdateDTO userDto)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return ApiResponseHelper.Fail<bool>("User not found.", 404);
            }
            var ImageFile = await _imageService.UploadImageAsync(userDto.ImagePath, "images");
            
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.PhoneNumber = userDto.PhoneNumber;
            user.Address = userDto.Address;
            user.Description = userDto.Description;
            if (!string.IsNullOrEmpty(ImageFile))
            {
                user.ImagePath = ImageFile; 
            }

            _unitOfWork.UserRepository.Update(user);
            if (await _unitOfWork.SaveChanges() > 0)
            {
                return ApiResponseHelper.Success(true, "Profile updated successfully");


            }

            return ApiResponseHelper.Fail<bool>("User data has not been updated", 500);

        }

        public async Task<ApiResponse<bool>> UpdateUserPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);

            if (user == null)
                return ApiResponseHelper.Fail<bool>("The email address is incorrect or the user does not exist", 404);

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, resetPasswordDTO.NewPassword);

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return ApiResponseHelper.Success<bool>(true, "Password Updated", 201);
            }
            return ApiResponseHelper.Fail<bool>("Failed to update password in database", 500);

        }


        #region Get Service Provider

        public async Task<ApiResponse<List<UserReadDTO>>> GetUsersByRoleWithPaginationAsync(string roleName, int pageNumber, int pageSize, bool ignoreDeleted = true)
        {
           
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return ApiResponseHelper.Fail<List<UserReadDTO>>("Page number and page size must be greater than zero.", 400);
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                return ApiResponseHelper.Fail<List<UserReadDTO>>("Role name cannot be empty.", 400);
            }
         
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
               
                return ApiResponseHelper.Success(new List<UserReadDTO>(), $"No users found for the role '{roleName}'.");
            }

            var allUsersInRole = await _userManager.GetUsersInRoleAsync(roleName);

            var baseUrl = _urlService.GetBaseUrl();
            var paginatedUsers = allUsersInRole
                .Where(u => !ignoreDeleted || !u.IsDeleted)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserReadDTO
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Address = u.Address,
                    ImagePath = string.IsNullOrEmpty(u.ImagePath) ? null : $"{baseUrl}{u.ImagePath}",
                    Description = u.Description
                }).ToList();

            return ApiResponseHelper.Success(paginatedUsers, $"Paginated users for role '{roleName}' retrieved successfully.");
        }


        #endregion Get Service Provider




    }
}
