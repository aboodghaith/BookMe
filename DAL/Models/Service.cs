using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Service : BaseModel
    {
       
        public string Name { get; set; }

        public string Description { get; set; } 

        public decimal Price { get; set; }

        public int EstimatedDuration { get; set; }

        public TimeSpan StartWork { get; set; }

        public TimeSpan EndWork { get; set; }


        public string? ImagePath { get; set; }
        
        public int CityId { get; set; }

        public City City { get; set; }

        public string ServiceProviderId { get; set; }
        public User ServiceProvider { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }


        
    }
}
