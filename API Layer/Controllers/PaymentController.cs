using BLL.Common;
using BLL.Services.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_Layer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = _paymentService;
        }

        /// <summary>
        /// Get all payments in the system (Only for Admin)
        /// </summary>
        [HttpGet("GetAllPayments")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<Payment>>>> GetAllPayments()
        {
            var response = await _paymentService.GetAllPaymentsAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get payment logs for a specific user name (Admin can query anyone, User queries themselves)
        /// </summary>
        [HttpGet("GetPaymentsByBranch/{userName}")]
        public async Task<ActionResult<ApiResponse<List<Payment>>>> GetPaymentsByUserName(string userName)
        {
            if (!User.IsInRole("Admin"))
            {
                var currentUserName = User.FindFirstValue(ClaimTypes.Name);
                if (currentUserName != userName)
                {
                    return StatusCode(403, ApiResponseHelper.Fail<List<Payment>>("Forbidden: You can only view your own payments.", 403));
                }
            }

            var response = await _paymentService.GetPaymentsByUserNameAsync(userName);
            return StatusCode(response.StatusCode, response);
        }
    }
}
