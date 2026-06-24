using BLL.Common;
using BLL.DTOs.ServicesDTOs;
using BLL.DTOs.TimeSlotDTOs;
using BLL.Services.Interfaces;
using DAL.Enums;
using DAL.Models;
using DAL.UnitOfWork;
namespace BLL.Services.Implementations
{
    public class ServiceService : IServiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public ServiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<ServiceDTOForRead>> CreateServiceAsync(ServiceDTOForCreate service , string providerId)
        {
            if (service.StartWork >= service.EndWork)
            {
                return ApiResponseHelper.Fail<ServiceDTOForRead>(
                    "StartWork must be less than EndWork",
                    400
                );
            }

            var serviceEntity = MapToEntity(service , providerId);
            var TotalWorkMinutes = (serviceEntity.EndWork - serviceEntity.StartWork).TotalMinutes;

            if (TotalWorkMinutes < serviceEntity.EstimatedDuration)
            {

                return ApiResponseHelper.Fail<ServiceDTOForRead>("Estimated Duration Exceeds Working Hours !", 400);

            }
            _unitOfWork.ServiceRepo.Add(serviceEntity);
            if(await _unitOfWork.SaveChanges() > 0)
            {

                return ApiResponseHelper.Success<ServiceDTOForRead>(MapToReadDTO(serviceEntity), "Service Created Successfully", 201); 


            }
             
            return ApiResponseHelper.Fail<ServiceDTOForRead>("Failed To Create Service !", 500);
        }

        public async Task<ApiResponse<object>> DeleteServiceAsync(int Id)
        {
           var Service = await _unitOfWork.ServiceRepo.GetAsync(s  => s.Id == Id);


            if (Service == null)
            {

                return ApiResponseHelper.Fail<object>("Service Not Found !" , 404);

            }

            var hasActiveBookings = await _unitOfWork.BookingRepo.AnyAsync(
                b => b.ServiceId == Service.Id && b.Status == BookingStatus.Accepted
                );

            if (hasActiveBookings)
            {
                return ApiResponseHelper.Fail<object>("A service with reservations cannot be deleted" , 400);
            }

                Service.IsDeleted = true;

            _unitOfWork.ServiceRepo.Update(Service);
            if (await _unitOfWork.SaveChanges() > 0)
            {

                return ApiResponseHelper.Success<object>(MapToReadDTO(Service) , "Service Deleted successfully" , 200);
               
            }
            return ApiResponseHelper.Fail<object>("Failed to Delete Serivce !" , 500);
         
            
        }

        private async Task<ApiResponse<IEnumerable<ServiceDTOForRead>>> GetServices(bool ignoreSoftDelete)
        {
            var services = await _unitOfWork.ServiceRepo.GetAllAsync(
                              ignoreSoftDelete,
                              null, null,
                              s => s.Category,
                              s => s.ServiceProvider,
                              s => s.City
                              );


            var result = services.Select(s => MapToReadDTO(s)).ToList();

            return ApiResponseHelper.Success<IEnumerable<ServiceDTOForRead>>(
                result,
                "Services Retrieved Successfully",
                200
            );
        }
        public async Task<ApiResponse<IEnumerable<ServiceDTOForRead>>> GetAllActiveServicesAsync()
        {

            var Services = await GetServices(false);

            return Services;

        }


        public async Task<ApiResponse<IEnumerable<ServiceDTOForRead>>> GetAllServicesAsync()
        {
            var Services = await GetServices(true);

            return Services;
        }
        public async Task<ApiResponse<IEnumerable<ServiceDTOForRead>>> GetAllServicesByPagnationAsync( int pageNumber = 1 , int pageSize = 10,  bool ignoreSoftDelete = false)
        {
            var services = await _unitOfWork.ServiceRepo.GetAllAsync(
                                ignoreSoftDelete,   
                                (pageNumber - 1) * pageSize,
                                pageSize,
                               s => s.Category,
                               s => s.ServiceProvider,
                               s => s.City
                               );


            var result = services.Select(s => MapToReadDTO(s)).ToList();

            return ApiResponseHelper.Success<IEnumerable<ServiceDTOForRead>>(
                result,
                "Services Retrieved Successfully",
                200
            );
        }
        public async Task<ApiResponse<ServiceDTOForRead>> GetServiceByIdAsync(int Id)
        {
            var service = await _unitOfWork.ServiceRepo.GetAsync(
                                             s => s.Id == Id,
                                             s => s.Category,
                                             s => s.ServiceProvider,
                                             s => s.City
   );

            if (service == null)
            {
                return ApiResponseHelper.Fail<ServiceDTOForRead>(
                    "Service Not Found !",
                    404
                );
            }

            return ApiResponseHelper.Success(
                MapToReadDTO(service),
                "Service Retrieved Successfully",
                200
            );
        }

        public async Task<ApiResponse<ServiceDTOForRead>> GetServiceByNameAsync(string Name)
        {
            var service = await _unitOfWork.ServiceRepo.GetAsync(
                                          s => s.Name == Name,
                                          s => s.Category,
                                          s => s.ServiceProvider,
                                          s => s.City
    );

            if (service == null)
            {
                return ApiResponseHelper.Fail<ServiceDTOForRead>(
                    "Service Not Found !",
                    404
                );
            }

            return ApiResponseHelper.Success(
                MapToReadDTO(service),
                "Service Retrieved Successfully",
                200
            );
        }


        public async Task<ApiResponse<ServiceDTOForRead>> UpdateServiceAsync(int Id, ServiceDTOForCreate service)
        {
            var ServiceFromDb = await _unitOfWork.ServiceRepo.GetAsync(s => s.Id == Id,
                                             s => s.Category,
                                             s => s.ServiceProvider,
                                             s => s.City);

            if(ServiceFromDb == null)
            {
                return ApiResponseHelper.Fail<ServiceDTOForRead>(
                    "Service Not Found !",
                    404
                );
            }

            if (service.StartWork >= service.EndWork)
            {
                return ApiResponseHelper.Fail<ServiceDTOForRead>(
                    "StartWork must be less than EndWork",
                    400
                );
            }

            // old service : 21 -> 7 
            // new service : 22 -> 2 
            // estimations service 1 hour 
            // bookings : 23 -> 1 ,, 21 -> 22


            var hasActiveBookings = await _unitOfWork.BookingRepo.FindAllAsync(
                b => b.ServiceId == ServiceFromDb.Id && b.Status == BookingStatus.Accepted
              );

            if (hasActiveBookings.Count > 0) 
            {
                var HasInvalidBookings = hasActiveBookings.Any( b => 
                  b.StartDateTime.TimeOfDay < service.StartWork || b.EndDateTime.TimeOfDay > service.EndWork
                    );
            
                // 4 <= 3 false  ,,, 6 >= 4 true  :: false 
                // 4 <= 5 true ,,, 6 >= 6 true :: true 

                // any booking outside the range of the new service -> stop the updateing

                if (HasInvalidBookings) {

                    return ApiResponseHelper.Fail<ServiceDTOForRead>(
                "There are Reserved Services !",
                400
            );
                } 
            }


            ServiceFromDb.Name = service.Name;
            ServiceFromDb.Description = service.Description;
            ServiceFromDb.Price = service.Price;
            ServiceFromDb.ImagePath = service.ImagePath;
            ServiceFromDb.StartWork = service.StartWork;
            ServiceFromDb.EndWork = service.EndWork;
            ServiceFromDb.EstimatedDuration = service.EstimatedDuration;
            ServiceFromDb.CategoryId = service.CategoryId;
            ServiceFromDb.CityId = service.CityId;

          _unitOfWork.ServiceRepo.Update(ServiceFromDb);

           if(await _unitOfWork.SaveChanges() > 0)
            {
                return ApiResponseHelper.Success<ServiceDTOForRead>(
                     MapToReadDTO(ServiceFromDb) , "The Serivce is Updated Successfully" , 200 );

            }

            return new ApiResponse<ServiceDTOForRead>
            {
                IsSuccess = false,
                Message = "Failed to Update Serivce !",
                StatusCode = 500
            }; ;


        }

        public async Task<ApiResponse<IEnumerable<ServiceDTOForRead>>> GetServicesByCityId(int cityId)
        {
            var Services = await _unitOfWork.ServiceRepo.FindAllAsync(
                s => s.CityId == cityId && !s.IsDeleted,
                s => s.Category,
                s => s.ServiceProvider,
                s => s.City
            );

            var result = Services.Select(s => MapToReadDTO(s)).ToList();

            return ApiResponseHelper.Success<IEnumerable<ServiceDTOForRead>>(
                result,
                "Services retrieved successfully by city",
                200
            );
        }

        public async Task<ApiResponse<IEnumerable<ServiceDTOForRead>>> GetServicesByCategoryId(int categoryId)
        {
            var Services = await _unitOfWork.ServiceRepo.FindAllAsync(
                s => s.CategoryId == categoryId && !s.IsDeleted,
                s => s.Category,
                s => s.ServiceProvider,
                s => s.City
            );

            var result = Services.Select(s => MapToReadDTO(s)).ToList();

            return ApiResponseHelper.Success<IEnumerable<ServiceDTOForRead>>(
                result,
                "Services retrieved successfully by category",
                200
            );
        }




        // Time Slot 

        public async Task<ApiResponse<List<TimeSlotDTOForRead>>> GetAvailableSlots(int serviceId, DateTime date)
        {
            // 1 : Get All The Booking Based On The ServiceId and DateTime

            var Bookings = await  _unitOfWork.BookingRepo.FindAllAsync(b =>
            b.Status == BookingStatus.Accepted &&
            b.ServiceId == serviceId &&
            b.StartDateTime.Date == date.Date);


            // 2 : comparison and get all availablety bookings based on Serviece Work and Estimation duration 

            var ServiceFromDb = await _unitOfWork.ServiceRepo.GetAsync(s => s.Id  == serviceId);
            if (ServiceFromDb.CreateAt.Date > date)
                return ApiResponseHelper.Fail<List<TimeSlotDTOForRead>>("Available time slots cannot be retrieved for past dates");
            var TimeSlots = new List<TimeSlotDTOForRead>(); 
            var StartWork = date.Date.Add(ServiceFromDb.StartWork);
            var EndWork = date.Date.Add(ServiceFromDb.EndWork);
            var EstimatedDuration = TimeSpan.FromMinutes(ServiceFromDb.EstimatedDuration);

            for (var SlotStart = StartWork; SlotStart < EndWork; SlotStart += EstimatedDuration)
            {
                var SlotEnd = SlotStart + EstimatedDuration;

                var IsBooking = Bookings.Any(b =>
             SlotStart < b.EndDateTime &&
             SlotEnd > b.StartDateTime);



                if (IsBooking) {
                    continue;
                
                }

                TimeSlots.Add(new TimeSlotDTOForRead { StartDateTime = SlotStart , EndDateTime = SlotEnd }); 
            }



            /*
             
             slotStart < booking.End &&
             slotEnd > booking.Start


             // if the start of new book less than end of any end slot ( 1 , 2 , 3 , 3.5 ....) and the end of new greater than start of any slot so -> conflict   


            1 -> 7 


            2.5 -> 3.5 

            1 -> 2 :: 1 < 3.5 && 2 > 2.5 : false 
            2 -> 3 :: 2 < 3.5 && 3 > 2.5 : true -> stop 
             */


            return ApiResponseHelper.Success(TimeSlots, "Available slots retrieved", 200);


        }


        // Mapper 
        private Service MapToEntity(ServiceDTOForCreate dto , string providerId)
        {
            return new Service
            {
                Name = dto.Name,
                Description = dto.Description,
                StartWork = dto.StartWork,
                EndWork = dto.EndWork,
                EstimatedDuration = dto.EstimatedDuration,
                Price = dto.Price,
                ImagePath = dto.ImagePath,
                ServiceProviderId = providerId,
                CategoryId = dto.CategoryId,
                CityId = dto.CityId
            };
        }


        private ServiceDTOForRead MapToReadDTO(Service service)
        {
            return new ServiceDTOForRead
            {
                ServiceId = service.Id,
                ServiceProviderId = service.ServiceProviderId,
                ServiceName = service.Name,
                Description = service.Description,
                Price = service.Price,
                EstimatedDuration = service.EstimatedDuration,
                StartWork = service.StartWork,
                EndWork = service.EndWork,
                ImagePath = service.ImagePath,
                ProviderName = service.ServiceProvider?.UserName,
                CategoryName = service.Category?.Name,
                CityName = service.City?.Name
            };
        }

    
    }
}
