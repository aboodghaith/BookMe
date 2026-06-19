using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string? ImagePath { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreateAt { get; set; } = DateTime.Now;

        public string? Description { get; set; }


        public string? Address { get; set; }

        public ICollection<Service>? Services { get; set; }
    }
}
