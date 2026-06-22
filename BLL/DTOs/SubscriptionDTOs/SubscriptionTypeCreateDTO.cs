using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.SubscriptionDTOs
{
    public class SubscriptionTypeCreateDTO
    {
        [Required(ErrorMessage = "Subscription name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Duration days is required")]
        [Range(1, 366, ErrorMessage = "Duration must be between 1 and 366 days")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Price is required")]
        
        public decimal Price { get; set; }
    }
}
