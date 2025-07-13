using Tourly.Constants;
using Tourly.Helpers;

namespace Tourly.Domain;

public class Booking
{
    public Booking()
    {
        ID = GeneratorHelper.GenerateId(PathHolder.BookingFilesPath);
    }

    public int ID { get; set; }
    public int UserId {get; set; }
    public int HotelId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal Price { get; set; }

    public override string ToString()
    {
        return $"{ID},{UserId},{HotelId},{StartDate},{EndDate},{Price}";
    }
}
