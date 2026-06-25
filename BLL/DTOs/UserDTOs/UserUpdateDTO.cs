using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.UserDTOs
{
    public class UserUpdateDTO
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(70, MinimumLength = 3, ErrorMessage = "The FirstName must be between 3 and 100 characters")]

        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(70, MinimumLength = 3, ErrorMessage = "The LastName must be between 3 and 100 characters")]

        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(20, MinimumLength = 11, ErrorMessage = "The PhoneNumber must be between 11 and 20 characters")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [MinLength(2, ErrorMessage = "MinLength is 2 characters")]
        public string Address { get; set; }

        public IFormFile? ImagePath { get; set; }

        public string? Description{ get; set; }
    }
}
