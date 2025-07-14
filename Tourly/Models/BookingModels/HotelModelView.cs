using Tourly.Enums;

namespace Tourly.Models.BookingModels;
public class HotelModelView
{
    public int Id { get; set; } 
    public string HotelName { get; set; }
    public string HotelDescription { get; set; }
    public HotelStatus HotelStatus { get; set; }
    public decimal HotelPrice { get; set; }
    public DateTime StartDate { get; set; } 
    public DateTime EndDate { get; set; }
}
