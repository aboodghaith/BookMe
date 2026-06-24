using BLL.Common;
using BLL.DTOs.BookingDTOs;
using BLL.DTOs.ServicesDTOs;
using BLL.Services.Interfaces;
using DAL.Enums;
using DAL.Models;
using DAL.UnitOfWork;


namespace BLL.Services.Implementations
{
    public class BookingService : IBookingService
    {  
        private readonly IUnitOfWork _unitOfWork;

        private const int CancelWindowHours = 2;
        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<BookingDTOForRead>> CreateBookingAsync(BookingDTOForCreate booking , string customerId)
        {

            var HasService = await _unitOfWork.ServiceRepo.AnyAsync(s => s.Id == booking.ServiceId);
            if(!HasService)
                return ApiResponseHelper.Fail<BookingDTOForRead>("The service is not available", 400);

            var Service = await _unitOfWork.ServiceRepo.GetAsync(s => s.Id == booking.ServiceId);

            if (booking.StartDateTime <= DateTime.Now)
            {
                return ApiResponseHelper.Fail<BookingDTOForRead>("Cannot book a slot in the past", 400);
            }
            var newStart = booking.StartDateTime;
            var newEnd = booking.StartDateTime.AddMinutes(Service.EstimatedDuration);
            var HasConflictBooking =await _unitOfWork.BookingRepo.AnyAsync(
                                     b => booking.ServiceId == b.ServiceId &&
                                     newStart < b.EndDateTime &&
                                     newEnd > b.StartDateTime &&
                                     b.Status == BookingStatus.Accepted);
            if (HasConflictBooking)
              return ApiResponseHelper.Fail<BookingDTOForRead>("There are overlapping reservations", 400);

            var NewBooking = MapToEntity(booking, Service, customerId);

            _unitOfWork.BookingRepo.Add(NewBooking);

            if(await _unitOfWork.SaveChanges() > 0)
            {
                return ApiResponseHelper.Success<BookingDTOForRead>(MapToReadDTO(NewBooking), "The Booking is Created Successfully" , 201);

            }


            return ApiResponseHelper.Fail<BookingDTOForRead>("Failed To Create Booking !", 500);

        }


        public async Task<ApiResponse<object>> AcceptBookingAsync(int bookingId)
        {

            
            var booking = await _unitOfWork.BookingRepo.GetAsync(
                 b => b.Id == bookingId && b.Status == BookingStatus.Pending
    );

            if (booking == null)
            {
                return ApiResponseHelper.Fail<object>(
                    "Booking not found or already processed",
                    404
                );
            }

            booking.Status = BookingStatus.Accepted;

            _unitOfWork.BookingRepo.Update(booking);

            if (await _unitOfWork.SaveChanges() > 0)
            {
                return ApiResponseHelper.Success<object>(
                    null,
                    "The Booking is Accepted",
                    200
                );
            }

            return ApiResponseHelper.Fail<object>(
                "Error",
                500
            );
        }



        public async Task<ApiResponse<object>> RejectBookingAsync(int bookingId)
        {
            var booking = await _unitOfWork.BookingRepo.GetAsync(
                 b => b.Id == bookingId && b.Status == BookingStatus.Pending
    );

            if (booking == null)
            {
                return ApiResponseHelper.Fail<object>(
                    "Booking not found or already processed",
                    404
                );
            }
            booking.Status = BookingStatus.Rejected;

            _unitOfWork.BookingRepo.Update(booking);

            if (await _unitOfWork.SaveChanges() > 0)
                return ApiResponseHelper.Success<object>(null, "Booking rejected", 200);

            return ApiResponseHelper.Fail<object>("Error", 500);
        }




        // pendeing & Accept(with Condation) 
        // can`t Cancelled when status is Completed 
        // can`t Cancelled one more time 

        public async Task<ApiResponse<object>> CancelledBookingAsync(int bookingId , string customerId)
        {
            var booking = await _unitOfWork.BookingRepo.GetAsync(b => b.Id == bookingId && b.CustomerId == customerId);

            if (booking == null)
                return ApiResponseHelper.Fail<object>("Booking not found", 404);


            if (booking.Status == BookingStatus.Cancelled)
                return ApiResponseHelper.Fail<object>("Already cancelled", 400);

            if (booking.Status == BookingStatus.Rejected)
                return ApiResponseHelper.Fail<object>("Cannot cancel rejected booking", 400);

            if (booking.Status == BookingStatus.Completed)
                return ApiResponseHelper.Fail<object>("Cannot cancel completed booking", 400);


            // 5 
            // the time to cancel is 3 
            // 5 - 3 = 2 
            // 2 <= 2 -> so stop canceld 

            var difference = booking.StartDateTime - DateTime.Now;
            if (difference.TotalHours <= CancelWindowHours)
            {
                return ApiResponseHelper.Fail<object>($"You cannot cancel within {CancelWindowHours} hours of the booking", 400);
            }

            booking.Status = BookingStatus.Cancelled;

            _unitOfWork.BookingRepo.Update(booking);

            if (await _unitOfWork.SaveChanges() > 0)
                return ApiResponseHelper.Success<object>(null, "Booking cancelled", 200);

            return ApiResponseHelper.Fail<object>("Error", 500);
        }







        public async Task<ApiResponse<List<BookingDTOForRead>>> GetCustomerBookingHistoryAsync(string customerId)
        {
          
            var bookings = await _unitOfWork.BookingRepo.FindAllAsync(
                b => b.CustomerId == customerId,
                b => b.Service,
                b => b.Service.ServiceProvider,
                b => b.Customer
            );

  
            var result = bookings.OrderByDescending(b => b.StartDateTime)
                                 .Select(b => MapToReadDTO(b))
                                 .ToList();

            return ApiResponseHelper.Success<List<BookingDTOForRead>>(
                result,
                "Customer booking history retrieved successfully.",
                200
            );
        }


        public async Task<ApiResponse<List<BookingDTOForRead>>> GetProviderIncomingBookingsAsync(string providerId)
        {

            var bookings = await _unitOfWork.BookingRepo.FindAllAsync(
                b => b.Service.ServiceProviderId == providerId,
                b => b.Service,
                b => b.Service.ServiceProvider,
                b => b.Customer
            );


            var result = bookings.OrderBy(b => b.Status == BookingStatus.Pending ? 0 : 1)
                                 .ThenByDescending(b => b.StartDateTime)
                                 .Select(b => MapToReadDTO(b))
                                 .ToList();

            return ApiResponseHelper.Success<List<BookingDTOForRead>>(
                result,
                "Provider incoming bookings retrieved successfully.",
                200
            );
        }





        private Booking MapToEntity(BookingDTOForCreate dto, Service service , string customerId)
        {
            return new Booking
            {
                StartDateTime = dto.StartDateTime,
                EndDateTime = dto.StartDateTime.AddMinutes(service.EstimatedDuration),

                CustomerId = customerId,
                ServiceId = dto.ServiceId,

                Status = BookingStatus.Pending,

                // Snapshot from Service (important)
                ServiceName = service.Name,
                ServiceDescription = service.Description,
                ServicePrice = service.Price,
                EstimatedDuration = service.EstimatedDuration
            };
        }



        private BookingDTOForRead MapToReadDTO(Booking booking)
        {
            return new BookingDTOForRead
            {
                BookingId = booking.Id,

                ServiceProviderId = booking.Service?.ServiceProviderId,
                ProviderName = booking.Service?.ServiceProvider?.UserName,
                ProviderPhoneNumber = booking.Service?.ServiceProvider?.PhoneNumber,

                StartDateTime = booking.StartDateTime,
                EndDateTime = booking.EndDateTime,

                ServiceId = booking.ServiceId ,
                ServiceName = booking.ServiceName,
                ServiceDescription = booking.ServiceDescription,
                ServicePrice = booking.ServicePrice,

                EstimatedServiceTime = TimeSpan.FromMinutes(booking.EstimatedDuration),

                CustomerId = booking.CustomerId,
                CustomerName = booking.Customer?.UserName,

                Status = booking.Status
            };
        }

       
    }
}
