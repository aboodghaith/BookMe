using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.ServicesDTOs
{
    public  class ServiceDTOForRead
    {     
        public int ServiceId { get; set; } 


        public string ServiceName { get; set; }

  
        public string Description { get; set; }


        
        public decimal Price { get; set; }


       
        public int EstimatedDuration { get; set; }

        public TimeSpan StartWork { get; set; }
        public TimeSpan EndWork { get; set; }


        public string ServiceProviderId { get; set; }
        public string ProviderName { get; set; } 
        public string CityName { get; set; } 

        public string CategoryName { get; set; } 
        public string ImagePath { get; set; }
    }
}
