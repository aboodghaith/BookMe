using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Common
{
    public class PaymentFactory : IPaymentFactory
    {
        public IPaymentProcess GetPaymentStrategy(PaymentMethod method = PaymentMethod.DemoPayment)
        {
            switch (method) { 
            
            case PaymentMethod.DemoPayment: return new DemoPayment();

                default:
                    throw new Exception("Invalid Payment Method");


            } 
        }
    }
}
