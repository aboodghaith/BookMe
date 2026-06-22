using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Common
{
    public class DemoPayment : IPaymentProcess
    {
        public async Task<bool> Process()
        {
            return await Task.FromResult(true);
        }
    }
}
