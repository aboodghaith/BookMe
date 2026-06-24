using BLL.Common;
using BLL.DTOs.BookingDTOs;
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
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Create a new booking (For Customers)
        /// </summary>
        [HttpPost("Create")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<ApiResponse<BookingDTOForRead>>> CreateBooking(BookingDTOForCreate model)
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _bookingService.CreateBookingAsync(model, customerId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Accept a pending booking (For Service Providers)
        /// </summary>
        [HttpPut("Accept/{id}")]
        [Authorize(Roles = "ServiceProvider")]
        public async Task<ActionResult<ApiResponse<object>>> AcceptBooking(int id)
        {
            var response = await _bookingService.AcceptBookingAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Reject a pending booking (For Service Providers)
        /// </summary>
        [HttpPut("Reject/{id}")]
        [Authorize(Roles = "ServiceProvider")]
        public async Task<ActionResult<ApiResponse<object>>> RejectBooking(int id)
        {
            var response = await _bookingService.RejectBookingAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Cancel a booking (For Customers - subject to the cancellation window)
        /// </summary>
        [HttpPut("Cancel/{id}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<ApiResponse<object>>> CancelBooking(int id)
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _bookingService.CancelledBookingAsync(id, customerId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get booking history for the current logged-in customer
        /// </summary>
        [HttpGet("CustomerHistory")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<ApiResponse<List<BookingDTOForRead>>>> GetCustomerHistory()
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _bookingService.GetCustomerBookingHistoryAsync(customerId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get incoming bookings for the current logged-in provider
        /// </summary>
        [HttpGet("ProviderIncoming")]
        [Authorize(Roles = "ServiceProvider")]
        public async Task<ActionResult<ApiResponse<List<BookingDTOForRead>>>> GetProviderIncoming()
        {
            var providerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _bookingService.GetProviderIncomingBookingsAsync(providerId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
