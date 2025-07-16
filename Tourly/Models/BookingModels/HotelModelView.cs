namespace Tourly.BookingModels;
public class HotelModelView
{
    public string HotelName { get; set; }
    public string HotelDescription { get; set; }
    public string HotelStatus { get; set; }
    public decimal HotelPrice { get; set; }
    public DateOnly StartDate { get; set; } 
    public DateOnly EndDate { get; set; }
}
