using BLL.Common;
using BLL.DTOs.CityDTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Layer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;
        public CityController(ICityService cityService) => _cityService = cityService;

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CityReadDTO>>>> GetAll()
        {
            var result = await _cityService.GetAllCitiesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CityReadDTO>>> GetById(int id)
        {
            var result = await _cityService.GetCityByIdAsync(id);
            if (!result.IsSuccess) return StatusCode(result.StatusCode, result);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> Create(CityCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _cityService.CreateCityAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(int id, CityCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _cityService.UpdateCityAsync(id, dto);
            if (!result.IsSuccess) return StatusCode(result.StatusCode, result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var result = await _cityService.DeleteCityAsync(id);
            if (!result.IsSuccess) return StatusCode(result.StatusCode, result);
            return Ok(result);
        }
    }
}
