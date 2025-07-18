using Tourly.BookingModels;
using Tourly.Constants;
using Tourly.Domain;
using Tourly.Enums;
using Tourly.Extentions;
using Tourly.Helpers;
using Tourly.Models.BookingModels;
using Tourly.Services.IBookingServices;
namespace Tourly.Services.BookingServices;
public class BookingService : IBookingService
{
    public Booking? BookRoom(int userId, Hotel hotel, RoomType desiredType, DateOnly start, DateOnly end)
    {
        var text = FileHelper.ReadFromFile(PathHolder.BookingFilesPath);
        var _bookings = text.Convert<Booking>();
        var rooms = FileHelper.ReadFromFile(PathHolder.RoomsFilesPath).Convert<Room>().Where(x => x.HotelId == hotel.ID);

        var availableRoom = rooms.FirstOrDefault(r =>
            r.RoomType == desiredType &&
            IsRoomAvailable(hotel, r, start, end));

        if (availableRoom == null)
        {
            Console.WriteLine("No available rooms found for the selected dates.");
            return null;
        }

        var nights = end.DayNumber - start.DayNumber;
        var totalPrice = nights * availableRoom.PricePerNight;

        var booking = new Booking
        {
            UserId = userId,
            HotelId = hotel.ID,
            StartDate = start,
            EndDate = end,
            Price = totalPrice
        };

        _bookings.Add(booking);

        FileHelper.WriteToFile(PathHolder.BookingFilesPath, _bookings.Convert());
        return booking;
    }
    public void CancelBooking(int bookingId)
    {
        string text = FileHelper.ReadFromFile(PathHolder.BookingFilesPath);

        List<Booking> convertedBookings = text.Convert<Booking>();

        var existBooking = convertedBookings.Find(x => x.ID == bookingId);

        if (existBooking == null)
        {
            throw new Exception("Booking was not found");
        }
        convertedBookings.Remove(existBooking);

        File.WriteAllLines(PathHolder.BookingFilesPath, convertedBookings.Convert());
    }
    public bool IsRoomAvailable(Hotel hotel, Room room, DateOnly start, DateOnly end)
    {
        var text = FileHelper.ReadFromFile(PathHolder.BookingFilesPath);
        var _bookings = text.Convert<Booking>();

        return !_bookings.Any(b =>
            b.HotelId == hotel.ID &&
            !(end <= b.StartDate || start >= b.EndDate));
    }
    public List<HotelBookingModel> GetBookings(int userId)
    {
        string text = FileHelper.ReadFromFile(PathHolder.BookingFilesPath);

        List<Booking> allBookings = text.Convert<Booking>();

        // Faqat foydalanuvchining bookinglari
        List<Booking> userBookings = allBookings
            .Where(b => b.UserId == userId)
            .ToList();

        if (!userBookings.Any())
            throw new Exception("Sizda hech qanday bron topilmadi.");

        return userBookings.ConvertTo();
    }

    //public HotelModelView View(int hotelId)
    //{
    //    string fileInfo = FileHelper.ReadFromFile(PathHolder.HotelsFilesPath);

    //    List<Hotel> hotels = fileInfo.Convert<Hotel>();

    //    HotelModelView hotelModelView = new HotelModelView();

    //    foreach (var item in hotels)
    //    {
    //        if (item.ID == hotelId)
    //        {
    //            hotelModelView.HotelName = item.Name;
    //            hotelModelView.Location = item.Location;
    //            hotelModelView.HotelDescription = item.Description;
    //        }
    //    }
    //    return hotelModelView;
    //}

    public HotelModelView View(int hotelId)
    {
        var hotels = FileHelper.ReadFromFile(PathHolder.HotelsFilesPath).Convert<Hotel>();

        var hotel = hotels.FirstOrDefault(h => h.ID == hotelId);

        if (hotel == null)
            throw new Exception("Hotel topilmadi.");

        return new HotelModelView
        {
            HotelName = hotel.Name,
            Location = hotel.Location,
            HotelDescription = hotel.Description
        };
    }

    public void ChangeBooking(HotelBookingModel updatedBooking)
    {
        var text = FileHelper.ReadFromFile(PathHolder.BookingFilesPath);
        var _bookings = text.Convert<Booking>();

        var existingBooking = _bookings.FirstOrDefault(b =>
            b.UserId == updatedBooking.UserId &&
            b.RoomId == updatedBooking.RoomId &&
            b.HotelId == updatedBooking.HotelId);

        if (existingBooking == null)
        {
            throw new Exception("It is not booked yet");
        }
        existingBooking.RoomType = updatedBooking.RoomType;
        existingBooking.HotelName = updatedBooking.HotelName;
        existingBooking.StartDate = updatedBooking.StartDate;
        existingBooking.EndDate = updatedBooking.EndDate;
    }
    public List<HotelModelView> SearchAllHotels(List<Hotel> hotels, string? search)
    {
        List<HotelModelView> result = new List<HotelModelView>();

        foreach (var item in hotels)
        {
            if (string.IsNullOrWhiteSpace(search) ||
                item.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                item.Location.Contains(search, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(new HotelModelView
                {
                    HotelName = item.Name,
                    Location = item.Location,
                });
            }
        }
        return result;
    }
}
