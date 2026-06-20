using BLL.Common;
using BLL.DTOs.CityDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface ICityService
    {
        Task<ApiResponse<List<CityReadDTO>>> GetAllCitiesAsync();
        Task<ApiResponse<CityReadDTO>> GetCityByIdAsync(int id);
        Task<ApiResponse<bool>> CreateCityAsync(CityCreateDTO dto);
        Task<ApiResponse<bool>> UpdateCityAsync(int id, CityCreateDTO dto);
        Task<ApiResponse<bool>> DeleteCityAsync(int id);
    }
}
