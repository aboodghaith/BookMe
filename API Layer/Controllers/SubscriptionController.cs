using BLL.Common;
using BLL.DTOs.SubscriptionDTOs;
using BLL.Services.Interfaces;
using DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Buffers.Text;
using System.Security.Claims;

namespace API_Layer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        
        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }



        #region Subscription Operations (Provider & Admin)

        /// <summary>
        /// Create or Upgrade a subscription for the current logged-in user
        /// </summary>
        [HttpPost("CreateOrUpgrade")]
        public async Task<ActionResult<ApiResponse<bool>>> CreateOrUpgrade(int subscriptionTypeId, PaymentMethod method = PaymentMethod.DemoPayment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _subscriptionService.CreateOrUpgradeSubscription(subscriptionTypeId, userId, method);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Check if the current logged-in user's subscription is still valid
        /// </summary>
        [HttpGet("CheckValidity")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckValidity()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _subscriptionService.IsTheSubscriptionValid(userId);
            return StatusCode(response.StatusCode, response);
        }


        /// <summary>
        /// Get Subscription (This End Point return the subscription for any user (but if found))
        /// </summary>
        [HttpGet("GetSubscriptionById")]
        public async Task<ActionResult<ApiResponse<SubscriptionReadDTO>>> GetSubscription()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _subscriptionService.GetSubscriptionById(userId);
            return StatusCode(response.StatusCode, response);


        }

        /// <summary>
        /// Get all subscriptions without pagination (Only for Admin)
        /// </summary>
        [HttpGet("GetAllSubscriptions")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<SubscriptionReadDTO>>>> GetAllSubscriptions()
        {
            var response = await _subscriptionService.GetAllSubscriptions();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get subscriptions with pagination (Only for Admin)
        /// </summary>
        [HttpGet("GetSubscriptionsWithPagination")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<SubscriptionReadDTO>>>> GetSubscriptionsWithPagination(int pageNumber = 1, int pageSize = 10)
        {
            var response = await _subscriptionService.GetAllSubscriptions(pageNumber, pageSize);
            return StatusCode(response.StatusCode, response);
        }

        #endregion

        #region Subscription Type CRUD Operations (Admin & Public)

        /// <summary>
        /// Get all available subscription plans (Public for anyone)
        /// </summary>
        [HttpGet("Plans")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<SubscriptionTypeReadDTO>>>> GetAllPlans()
        {
            
            var response = await _subscriptionService.GetAllSubscriptionTypes();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Create a new subscription plan (Only for Admin)
        /// </summary>
        [HttpPost("Plans")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<SubscriptionTypeReadDTO>>> CreatePlan(SubscriptionTypeCreateDTO model)
        {
          
            var response = await _subscriptionService.CreateSubscriptionType(model);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Update an existing subscription plan (Only for Admin)
        /// </summary>
        [HttpPut("Plans/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<SubscriptionTypeReadDTO>>> UpdatePlan(int id, SubscriptionTypeCreateDTO model)
        {
 
            var response = await _subscriptionService.UpdateSubscriptionType(id, model);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Soft delete a subscription plan (Only for Admin)
        /// </summary>
        [HttpDelete("Plans/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> DeletePlan(int id)
        {
            var response = await _subscriptionService.DeleteSubscriptionType(id);
            return StatusCode(response.StatusCode, response);
        }

        #endregion
    }
}
