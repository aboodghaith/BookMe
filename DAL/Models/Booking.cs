using DAL.Enums;


namespace DAL.Models
{
    public class Booking : BaseModel
    {
        

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public string CustomerId { get; set; }
        public User Customer { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }


        // SnapShot for booking from service 
        public string ServiceName { get; set; }

        public string? ServiceDescription { get; set; }

        public decimal ServicePrice { get; set; }

        public int EstimatedDuration { get; set; }

    }
}
