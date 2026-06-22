using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Payment : BaseModel
    {
        public Subscription? Subscription { get; set; }

        public int SubscriptionId { get; set; } 

        public string? UserName { get; set; } 


        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.DemoPayment;

        public PaymentStatus PaymentStatus { get; set; }

        public string? SubscriptionTypeName { get; set; }

    }
}
