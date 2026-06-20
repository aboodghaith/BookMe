using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.CategoryDTOs
{
    public class CategoryReadDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; } = string.Empty;

        public string ImagePath { get; set; } = string.Empty;

    }
}
