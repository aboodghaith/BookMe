using BLL.Common;
using BLL.DTOs.AuthDTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API_Layer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<User> _userManager;
        //private readonly IUserService _userService;

        public AccountController(IAuthService authService, UserManager<User> userManager)
        {
            this._authService = authService;
            this._userManager = userManager;
           
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<object>>> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var result = await _authService.RegisterUser(registerDTO);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<object>>> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginUser(loginDTO);

            if (!result.IsSuccess)
                return Unauthorized(result.Message);

            return Ok(result);
        }



        [HttpPost("ForgetPassword")]

        public async Task<ActionResult<ApiResponse<object>>> ForgetPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);

            if (user == null)
                return NotFound();

            return Ok(ApiResponseHelper.Success<ForgotPasswordDTO>(new ForgotPasswordDTO { Email = user.Email! }));
        }


        //[HttpPut("ResetPassword")]

        //public async Task<ActionResult<ApiResponse<object>>> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        //{

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);


        //    var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);

        //    if (user == null)
        //        return NotFound();


        //    var result = await _userService.UpdateUserPasswordAsync(resetPasswordDTO);

        //    if (result.IsSuccess)
        //    {
        //        return Ok(result);
        //    }

        //    return BadRequest(ModelState);
        //}
    }
}
