using BLL.Common;
using BLL.DTOs.UserDTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_Layer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }



        /// <summary>
        ///Get All Active User 
        /// </summary>
        [HttpGet("GetAllActiveUser")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<UserReadDTO>>>> GetAllActiveUser()
        {
            var response = await _userService.GetAllUsersAsync(true);
            return StatusCode(response.StatusCode, response);
        }



        /// <summary>
        ///Get All User (Active and Deactivate)
        /// </summary>
        [HttpGet("GetAllUserWithDeleted")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<UserReadDTO>>>> GetAllUserWithDeleted()
        {
            var response = await _userService.GetAllUsersAsync(false);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>
        ///Get All Active User By Pagination (User Active)
        /// </summary>
        [HttpGet("GetAllActiveUserWithPagination")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<UserReadDTO>>>> GetAllActiveUserWithPagination(int pageNumber, int pageSize = 10)
        {
            var response = await _userService.GetUsersWithPaginationAsync(pageNumber, pageSize, true);
            return StatusCode(response.StatusCode, response);
        }



        /// <summary>
        ///Get All User By Pagination (User Active and Deactive)
        /// </summary>
        [HttpGet("GetAllUserWithPagination")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<UserReadDTO>>>> GetAllUserWithPagination(int pageNumber, int pageSize = 10)
        {
            var response = await _userService.GetUsersWithPaginationAsync(pageNumber, pageSize, true);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>
        ///Get All Service Provider by Pagination
        /// </summary>
        [HttpGet("ServiceProviders")]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<ApiResponse<List<UserReadDTO>>>> GetServiceProviders(int pageNumber, int pageSize = 10)
        {
            
            var response = await _userService.GetUsersByRoleWithPaginationAsync("ServiceProvider", pageNumber, pageSize, true);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        ///Get All Customer by Pagination
        /// </summary>
        [HttpGet("Customers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<UserReadDTO>>>> GetCustomers(int pageNumber, int pageSize = 10)
        {
      
            var response = await _userService.GetUsersByRoleWithPaginationAsync("Customer", pageNumber, pageSize, true);
            return StatusCode(response.StatusCode, response);
        }




        /// <summary>
        ///Deactivate User
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> Deactivate(string id)
        {
            var response = await _userService.DeactivateUserAsync(id);
            return StatusCode(response.StatusCode, response);
        }



        [HttpGet("Profile")]
        public async Task<ActionResult<ApiResponse<UserReadDTO>>> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _userService.GetUserByIdAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Profile")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateProfile(UserUpdateDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _userService.UpdateUserProfileAsync(userId, model);
            return StatusCode(response.StatusCode, response);
        }
    }
}