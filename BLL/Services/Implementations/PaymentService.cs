using BLL.Common;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<Payment>>> GetAllPaymentsAsync()
        {
            var payments = await _unitOfWork.PaymentRepository.GetAllAsync();
            if (payments == null || !payments.Any())
            {
                return ApiResponseHelper.Fail<List<Payment>>("No payments found", 404);
            }
            return ApiResponseHelper.Success(payments, "Payments retrieved successfully", 200);
        }

        public async Task<ApiResponse<List<Payment>>> GetPaymentsByUserNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return ApiResponseHelper.Fail<List<Payment>>("Username cannot be empty", 400);
            }

            var payments = await _unitOfWork.PaymentRepository.FindAllAsync(p => p.UserName == userName);
            if (payments == null || !payments.Any())
            {   
                return ApiResponseHelper.Fail<List<Payment>>($"No payments found for user: {userName}", 404);
            }
            return ApiResponseHelper.Success(payments, $"Payments for user {userName} retrieved successfully", 200);
        }

        public async Task<bool> CreatePaymentAsync(Payment payment)
        {
            _unitOfWork.PaymentRepository.Add(payment);
            return await _unitOfWork.SaveChanges() > 0;
        }
    }
}
