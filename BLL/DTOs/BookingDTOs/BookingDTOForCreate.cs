using System.ComponentModel.DataAnnotations;


namespace BLL.DTOs.BookingDTOs
{
    public class BookingDTOForCreate
    {


        [Required(ErrorMessage = "The StartDateTime is Required")]
        public DateTime StartDateTime { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ServiceId must be greater than 0")]
        public int ServiceId { get; set; }



    }
}
