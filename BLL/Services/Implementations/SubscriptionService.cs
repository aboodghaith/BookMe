using BLL.Common;
using BLL.DTOs.SubscriptionDTOs;
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
    public class SubscriptionService : ISubscriptionService
    {

        private readonly IUnitOfWork _unitOfWork;
       

        public SubscriptionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<bool>> CreateOrUpgradeSubscription(int subscriptionType, string user)
        {
           var Subscription = await _unitOfWork.SubscriptionRepository.GetAsync(
               s => s.ServiceProviderID == user && s.IsActive == true);
            using var Transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                if (Subscription != null)
                {

                    Subscription.IsActive = false;
                }

                var SubscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetAsync(st => st.Id == subscriptionType);
                if(SubscriptionType == null)
                {
                    return ApiResponseHelper.Fail<bool>("The Subscription Type Not Found" , 404);
                }
                var StartDate = DateTime.Now;
                var EndDate = DateTime.Now.AddDays(SubscriptionType.DurationDays);
                var NewSubscription = new Subscription
                {
                     StartDate = StartDate, 
                     EndDate = EndDate, 
                     ServiceProviderID = user , 
                     IsActive = true , 
                     SubscriptionTypeId = SubscriptionType.Id, 
                     SubscriptionTypeName = SubscriptionType.Name, 
                     SubscriptionPrice = SubscriptionType.Price,
                };



            }
            catch (Exception ex) { 
            
            
            } 
        }

        public Task<ApiResponse<SubscriptionReadDTO>> GetSubscriptionById(int subscriptionId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> IsActiveSubscription(string user)
        {
            throw new NotImplementedException();
        }
    }
}
