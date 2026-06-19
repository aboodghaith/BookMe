using DAL.Enums;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.BookingDTOs
{
    public class BookingDTOForRead
    {
        public int BookingId { get; set; } 


        public string ServiceProviderId { get; set; }
        public string ProviderName { get; set; }

        public string? ProviderPhoneNumber { get; set; }


        public string CustomerId { get; set; }
        public string CustomerName { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }


        public int ServiceId { get; set; }
        public string ServiceName { get; set; }

        public string ServiceDescription { get; set; }

        public decimal ServicePrice { get; set; }

        public TimeSpan EstimatedServiceTime { get; set; }


        public BookingStatus Status { get; set; }


    }
}
