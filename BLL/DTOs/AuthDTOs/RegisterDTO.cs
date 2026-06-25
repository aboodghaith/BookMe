using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.AuthDTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Please Enter The FirstName")]
        [StringLength(70, MinimumLength = 3, ErrorMessage = "The FirstName must be between 3 and 100 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter The LastName")]
        [StringLength(70, MinimumLength = 3, ErrorMessage = "The LastName must be between 3 and 100 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter The Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address Format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter The Password")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]

        public string Password { get; set; }

        [Required(ErrorMessage = "Please Confirm Your Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please Enter Your Address")]
        [MinLength(2, ErrorMessage = "MinLength is 2 characters")]
        public string Address { get; set; }


        [Required(ErrorMessage = "Please Enter Your PhoneNumber")]
        [StringLength(20, MinimumLength = 11, ErrorMessage = "The PhoneNumber must be between 11 and 20 characters")]
        [Phone(ErrorMessage = "Invalid phone number format.")]

        public string PhoneNumber { get; set; }


        public IFormFile? ImagePath { get; set; }

        public string? Description { get; set; }

    }
}
