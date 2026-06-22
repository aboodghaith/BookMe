using BLL.Common;
using BLL.DTOs.SubscriptionDTOs;
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
        Task<ApiResponse<bool>> CreateOrUpgradeSubscription(int subscriptionType , string user);


        Task<ApiResponse<bool>> IsActiveSubscription(string user); 

        Task<ApiResponse<SubscriptionReadDTO>> GetSubscriptionById(int subscriptionId);
    }
}
