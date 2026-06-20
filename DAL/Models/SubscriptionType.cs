using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SubscriptionType : BaseModel
    {
       
        public string Name { get; set; }


        public decimal Price { get; set; } 


        public int DurationDays { get; set; } 
    }
}
