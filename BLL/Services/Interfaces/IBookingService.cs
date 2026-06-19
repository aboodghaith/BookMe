using BLL.Common;
using BLL.DTOs.BookingDTOs;


namespace BLL.Services.Interfaces
{
    public interface IBookingService
    {

        public Task<ApiResponse<BookingDTOForRead>> CreateBookingAsync(BookingDTOForCreate booking , string customerId);

        public Task<ApiResponse<object>> AcceptBookingAsync(int bookingId);

        public Task<ApiResponse<object>> RejectBookingAsync(int bookingId);


        public Task<ApiResponse<object>> CancelledBookingAsync(int bookingId , string customerId);


    }
}
