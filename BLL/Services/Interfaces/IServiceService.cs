using BLL.Common;
using BLL.DTOs.ServicesDTOs;
using BLL.DTOs.TimeSlotDTOs;


namespace BLL.Services.Interfaces
{
    public interface IServiceService
    {
        Task<ApiResponse<ServiceDTOForRead>> CreateServiceAsync(ServiceDTOForCreate service , string providerId);

        Task<ApiResponse<ServiceDTOForRead>> UpdateServiceAsync(int Id , ServiceDTOForCreate service);

        Task<ApiResponse<object>> DeleteServiceAsync(int Id);

        Task<ApiResponse<IEnumerable<ServiceDTOForRead>>> GetAllActiveServicesAsync();

        Task<ApiResponse<IEnumerable<ServiceDTOForRead>>> GetAllServicesAsync();


        Task<ApiResponse<ServiceDTOForRead>> GetServiceByIdAsync(int Id);

        Task<ApiResponse<ServiceDTOForRead>> GetServiceByNameAsync(string Name);

       Task<ApiResponse<IEnumerable<ServiceDTOForRead>>> GetServicesByCityId(int cityId);

        Task<ApiResponse<IEnumerable<ServiceDTOForRead>>> GetServicesByCategoryId(int categoryId);

        Task<ApiResponse<IEnumerable<ServiceDTOForRead>>> GetAllServicesByPagnationAsync(int pageSize , int pageNumber , bool ignoreSoftDelete = false);

        Task<ApiResponse<List<TimeSlotDTOForRead>>> GetAvailableSlots(int serviceId , DateTime date);
    }
}
