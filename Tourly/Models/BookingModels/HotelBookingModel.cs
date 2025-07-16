namespace Tourly.BookingModels;
public class HotelBookingModel
{
    public int HotelId { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public string HotelName { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
