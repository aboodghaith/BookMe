using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.SubscriptionDTOs
{
    public class SubscriptionReadDTO
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        
        public string? ServiceProviderId { get; set; }
        public string? ProviderName { get; set; }

        
        public int SubscriptionTypeId { get; set; }
        public string? SubscriptionTypeName { get; set; }
        public decimal PricePaid { get; set; }
    }
}
