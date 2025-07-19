using Tourly.Constants;
using Tourly.Domain;
using Tourly.Enums;
using Tourly.Extentions;
using Tourly.Helpers;

namespace Tourly.BookingModels;
public class HotelBookingModel
{
    public int HotelId { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public RoomType RoomType { get; set; }
    public string HotelName { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal Price { get; set; }
    public int idFinder(int id)
    {
        List<Booking> bookings = FileHelper.ReadFromFile(PathHolder.BookingFilesPath).Convert<Booking>();

        if(bookings == null)
        {
            return 1;
        }

        Booking booking = bookings.FirstOrDefault(x=>x.ID == id);
        return booking.ID;
    }
}
