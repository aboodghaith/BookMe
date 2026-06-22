using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.SubscriptionDTOs
{
    public class SubscriptionTypeReadDTO
    {

        public int Id { get; set; } 


        public string Name { get; set; }


        public int DurationDays { get; set; } 

        public decimal Price { get; set; } 
        
    }
}
