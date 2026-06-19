using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.AuthDTOs
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address Format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "New Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please Confirm Your New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
