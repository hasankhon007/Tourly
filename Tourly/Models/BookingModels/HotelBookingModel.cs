namespace Tourly.Models.BookingModels;
public class HotelBookingModel
{
    public int HotelId { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public string HotelName { get; set; }
    public string HotelAddress { get; set; }
    public decimal HotelPrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
