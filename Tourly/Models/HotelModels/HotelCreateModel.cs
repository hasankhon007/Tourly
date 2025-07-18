using Tourly.BookingModels;
using Tourly.Domain;

namespace Tourly.Models.HotelModels;

public class HotelCreateModel
{
    public string Location { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Description { get; set; }
    public byte StarsCount { get; set; }

    public List<Room>? Rooms { get; set; }
}
