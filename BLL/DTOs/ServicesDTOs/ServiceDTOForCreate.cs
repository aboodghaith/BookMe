using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.ServicesDTOs
{
    public class ServiceDTOForCreate
    {
        [Required(ErrorMessage = "Service name is required")]
        [MaxLength(100, ErrorMessage = "Service name cannot exceed 100 characters")]
        [MinLength(3, ErrorMessage = "Service name must be at least 3 characters")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Service description is required")]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }


        [Range(1, 100000, ErrorMessage = "Price must be between 1 and 100000")]
        public decimal Price { get; set; }


        [Range(1, 1440, ErrorMessage = "Estimated duration must be between 1 and 1440 minutes")]
        public int EstimatedDuration { get; set; }


        [Required(ErrorMessage = "Start work time is required")]
        public TimeSpan StartWork { get; set; }


        [Required(ErrorMessage = "End work time is required")]
        public TimeSpan EndWork { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Please select a city")]
        public int CityId { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }

        public string? ImagePath { get; set; }

    }
}
