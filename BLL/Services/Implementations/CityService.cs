using BLL.Common;
using BLL.DTOs.CityDTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class CityService : ICityService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CityService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

        public async Task<ApiResponse<bool>> CreateCityAsync(CityCreateDTO dto)
        {
            var city = new City { Name = dto.CityName };

            _unitOfWork.CityRepository.Add(city);
            if (await _unitOfWork.SaveChanges() > 0)
            {
                return ApiResponseHelper.Success(true, "City created successfully", 201);
            }
            return ApiResponseHelper.Fail<bool>("City not created");
        }

        public async Task<ApiResponse<bool>> DeleteCityAsync(int id)
        {
          
            var city = await _unitOfWork.CityRepository.GetAsync(c => c.Id == id); 
            if (city == null) return ApiResponseHelper.Fail<bool>("City not found", 404);

            city.IsDeleted = true;
            _unitOfWork.CityRepository.Update(city);
            if (await _unitOfWork.SaveChanges() > 0)
            {
                return ApiResponseHelper.Success(true, "City deleted successfully.");
            }
            return ApiResponseHelper.Fail<bool>("City not deleted");
        }

        public async Task<ApiResponse<List<CityReadDTO>>> GetAllCitiesAsync()
        {
            var cities = await _unitOfWork.CityRepository.GetAllAsync();
            if (cities != null)
            {
                var dtos = cities.Select(c => new CityReadDTO
                {
                    CityId = c.Id, 
                    CityName = c.Name
                }).ToList();

                return ApiResponseHelper.Success(dtos, "Cities retrieved successfully.");
            }
            return ApiResponseHelper.Fail<List<CityReadDTO>>("Cities not retrieved");
        }

     
        public async Task<ApiResponse<CityReadDTO>> GetCityByIdAsync(int id)
        {
            var city = await _unitOfWork.CityRepository.GetAsync(c => c.Id == id);
            if (city == null) return ApiResponseHelper.Fail<CityReadDTO>("City not found", 404);

            var dto = new CityReadDTO
            {
                CityId = city.Id,
                CityName = city.Name
            };
            return ApiResponseHelper.Success(dto, "City retrieved successfully.");
        }

        public async Task<ApiResponse<bool>> UpdateCityAsync(int id, CityCreateDTO dto)
        {
            var city = await _unitOfWork.CityRepository.GetAsync(c => c.Id == id);
            if (city == null) return ApiResponseHelper.Fail<bool>("City not found", 404);

            city.Name = dto.CityName;

            _unitOfWork.CityRepository.Update(city);
            await _unitOfWork.SaveChanges();
            return ApiResponseHelper.Success(true, "City updated successfully.");
        }
    }
}
