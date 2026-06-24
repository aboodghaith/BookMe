using BLL.Common;
using BLL.DTOs.ServicesDTOs;
using BLL.DTOs.TimeSlotDTOs;
using BLL.Services.Implementations;
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
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly ISubscriptionService _subscriptionService; 
        public ServiceController(IServiceService serviceService , ISubscriptionService subscriptionService)
        {
            _serviceService = serviceService;
            _subscriptionService = subscriptionService;
        }

        /// <summary>
        /// Create a new service (Only for Service Providers with Active Subscription)
        /// </summary>
        [HttpPost("CreateService")]
        [Authorize(Roles = "ServiceProvider")]
        public async Task<ActionResult<ApiResponse<ServiceDTOForRead>>> CreateService(ServiceDTOForCreate model)
        {
            var providerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

           
            var subscriptionCheck = await _subscriptionService.IsTheSubscriptionValid(providerId);
            if (!subscriptionCheck.IsSuccess)
            {
                return StatusCode(subscriptionCheck.StatusCode, subscriptionCheck);
            }

            var response = await _serviceService.CreateServiceAsync(model, providerId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Update an existing service (Only for Service Providers with Active Subscription)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "ServiceProvider")]
        public async Task<ActionResult<ApiResponse<ServiceDTOForRead>>> UpdateService(int id, ServiceDTOForCreate model)
        {
            var providerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

         
            var subscriptionCheck = await _subscriptionService.IsTheSubscriptionValid(providerId);
            if (!subscriptionCheck.IsSuccess)
            {
                return StatusCode(subscriptionCheck.StatusCode, subscriptionCheck);
            }

            var response = await _serviceService.UpdateServiceAsync(id, model);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Soft Delete a service (Only for Active Service Providers or Admins)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ServiceProvider,Admin")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteService(int id)
        {
           
            if (!User.IsInRole("Admin"))
            {
                var providerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

              
                var subscriptionCheck = await _subscriptionService.IsTheSubscriptionValid(providerId);
                if (!subscriptionCheck.IsSuccess)
                {
                    return StatusCode(subscriptionCheck.StatusCode, subscriptionCheck);
                }
            }

            var response = await _serviceService.DeleteServiceAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get Active Services (Available for anyone, even without login)
        /// </summary>
        [HttpGet("GetAllActiveServices")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<ServiceDTOForRead>>>> GetAllActiveServices()
        {
            var response = await _serviceService.GetAllActiveServicesAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get All Services including soft-deleted ones (Only for Admin)
        /// </summary>
        [HttpGet("GetAllServicesWithDeleted")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ServiceDTOForRead>>>> GetAllServicesWithDeleted()
        {
            var response = await _serviceService.GetAllServicesAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get Services with Pagination
        /// </summary>
        [HttpGet("GetServicesWithPagination")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<ServiceDTOForRead>>>> GetServicesWithPagination(int pageNumber = 1, int pageSize = 10, bool ignoreSoftDelete = false)
        {
            if (ignoreSoftDelete && !User.IsInRole("Admin"))
            {
                return StatusCode(403, ApiResponseHelper.Fail<IEnumerable<ServiceDTOForRead>>("Forbidden: Only Admins can see deleted services.", 403));
            }

            var response = await _serviceService.GetAllServicesByPagnationAsync(pageNumber, pageSize, ignoreSoftDelete);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get Service by its ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ServiceDTOForRead>>> GetServiceById(int id)
        {
            var response = await _serviceService.GetServiceByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get Service by Name
        /// </summary>
        [HttpGet("GetByName/{name}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ServiceDTOForRead>>> GetServiceByName(string name)
        {
            var response = await _serviceService.GetServiceByNameAsync(name);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get Services by City ID
        /// </summary>
        [HttpGet("GetByCity/{cityId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<ServiceDTOForRead>>>> GetServicesByCity(int cityId)
        {
            var response = await _serviceService.GetServicesByCityId(cityId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get Services by Category ID
        /// </summary>
        [HttpGet("GetByCategory/{categoryId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<ServiceDTOForRead>>>> GetServicesByCategory(int categoryId)
        {
            var response = await _serviceService.GetServicesByCategoryId(categoryId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get Available Time Slots for a specific service and date
        /// </summary>
        [HttpGet("GetAvailableSlots")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<TimeSlotDTOForRead>>>> GetAvailableSlots(int serviceId, DateTime date)
        {
            var response = await _serviceService.GetAvailableSlots(serviceId, date);
            return StatusCode(response.StatusCode, response);
        }
    }
}
