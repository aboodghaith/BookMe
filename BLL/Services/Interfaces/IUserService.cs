using BLL.Common;
using BLL.DTOs.AuthDTOs;
using BLL.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<List<UserReadDTO>>> GetAllUsersAsync(bool ignoreDeleted = true);
        Task<ApiResponse<List<UserReadDTO>>> GetUsersWithPaginationAsync(int pageNumber, int pageSize, bool ignoreDeleted = true);
        Task<ApiResponse<UserReadDTO>> GetUserByIdAsync(string id);
        Task<ApiResponse<bool>> DeactivateUserAsync(string id);

        Task<ApiResponse<bool>> UpdateUserProfileAsync(string id, UserUpdateDTO model);

        Task<ApiResponse<bool>> UpdateUserPasswordAsync(ResetPasswordDTO resetPasswordDTO);

        Task<ApiResponse<List<UserReadDTO>>> GetUsersByRoleWithPaginationAsync(string roleName, int pageNumber, int pageSize, bool ignoreDeleted = true);
    }
}
