namespace Tourly.Models.BookingModels;
public class HotelModelView
{
    public int Id { get; set; } 
    public string HotelName { get; set; }
    public string HotelDescription { get; set; }
    public string Location { get; set; }
    public decimal HotelPrice { get; set; }
    public DateOnly StartDate { get; set; } 
    public DateOnly EndDate { get; set; }
}
