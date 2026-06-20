using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.CityDTOs
{
    public class CityCreateDTO
    {
        [Required(ErrorMessage = "The City Name is Required")]
        public string CityName { get; set; } 
    }
}
