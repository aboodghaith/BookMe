using BLL.Common;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<List<Payment>>> GetAllPaymentsAsync();
        Task<ApiResponse<List<Payment>>> GetPaymentsByUserNameAsync(string userName);
        Task<bool> CreatePaymentAsync(Payment payment);
    }
}
