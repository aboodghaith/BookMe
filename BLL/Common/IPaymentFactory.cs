using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Common
{
    public interface IPaymentFactory
    {
        IPaymentProcess GetPaymentStrategy(PaymentMethod method = PaymentMethod.DemoPayment);
    }
}
