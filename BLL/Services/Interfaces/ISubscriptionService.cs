using BLL.Common;
using BLL.DTOs.SubscriptionDTOs;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface ISubscriptionService
    {
        // subscripe 
        // isActiveSubscripe 
        // get subscripe (subscripe & Type) 
        // update (subscripe & Type)
        // delete Subscriepe (type)
        Task<ApiResponse<bool>> CreateOrUpgradeSubscription(int subscriptionType , string user , PaymentMethod method = PaymentMethod.DemoPayment);


        Task<ApiResponse<bool>> IsTheSubscriptionValid(string user);

        Task<ApiResponse<List<SubscriptionReadDTO>>> GetAllSubscriptions();


        Task<ApiResponse<List<SubscriptionReadDTO>>> GetAllSubscriptions(int pageNumber = 1 , int PageSize = 10);



        Task<ApiResponse<List<SubscriptionTypeReadDTO>>> GetAllSubscriptionTypes();

        Task<ApiResponse<SubscriptionTypeReadDTO>> CreateSubscriptionType(SubscriptionTypeCreateDTO dto);


        Task<ApiResponse<SubscriptionTypeReadDTO>> UpdateSubscriptionType(int id, SubscriptionTypeCreateDTO dto);


        Task<ApiResponse<bool>> DeleteSubscriptionType(int id);

    }
}
