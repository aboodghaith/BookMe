using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Subscription : BaseModel
    {
        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime EndDate { get; set; }

        public string ServiceProviderID { get; set; }

        public bool IsActive { get; set; } = true; 
        public User? ServiceProvider  { get; set; }

        public int SubscriptionTypeId { get; set; } 

        public string SubscriptionTypeName { get; set; }

        public decimal SubscriptionPrice { get; set; } 
        public SubscriptionType? SubscriptionType { get; set; }


    }
}
