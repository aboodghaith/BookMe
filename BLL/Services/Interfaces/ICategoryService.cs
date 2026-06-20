using BLL.Common;
using BLL.DTOs.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<List<CategoryReadDTO>>> GetAllCategoriesAsync();
        Task<ApiResponse<CategoryReadDTO>> GetCategoryByIdAsync(int id);
        Task<ApiResponse<bool>> CreateCategoryAsync(CategoryCreateUpdateDTO dto);
        Task<ApiResponse<bool>> UpdateCategoryAsync(int id, CategoryCreateUpdateDTO dto);
        Task<ApiResponse<bool>> DeleteCategoryAsync(int id);
    }
}
