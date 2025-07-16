using Tourly.BookingModels;
using Tourly.Constants;
using Tourly.Domain;
using Tourly.Enums;
using Tourly.Extentions;
using Tourly.Helpers;
using Tourly.Models.BookingModels;
using Tourly.Services.IBookingServices;
using Tourly.Services.UserServices;
namespace Tourly.Services.BookingServices;
public class BookingService : IBookingService
{
    private UserService userServices;
    private HotelBookingModel hotelBookingModel;
    private HotelModelView hotelModelView;
    private int _bookingId;
    public BookingService(HotelBookingModel hotelBookingModel, HotelModelView hotelModelView)
    {
        this.hotelBookingModel = hotelBookingModel;
        this.hotelModelView = hotelModelView;
    }

    public Booking? BookRoom(int userId, Hotel hotel, RoomType desiredType, DateOnly start, DateOnly end)
    {
        var text = FileHelper.ReadFromFile(PathHolder.BookingFilesPath);
        var _bookings = text.Convert<Booking>();

        var availableRoom = hotel.Rooms.FirstOrDefault(r =>
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

        List<Booking> userBookings = text.Convert<Booking>();

        foreach (var item in userBookings)
        {

            var hotelViewModel = userBookings.Find(item => item.UserId == userId);

            if (hotelViewModel == null)
            {
                throw new Exception("Booking is not available");
            }
            userBookings.Add(item);
        }

        return userBookings.ConvertTo();
    }

    public HotelModelView View(int hotelId)
    {
        string fileInfo = FileHelper.ReadFromFile(PathHolder.HotelsFilesPath);

        List<Hotel> hotels = fileInfo.Convert<Hotel>();

        HotelModelView hotelModelView = new HotelModelView();

        foreach (var item in hotels)
        {
            if (item.ID == hotelId)
            {
                hotelModelView.HotelName = item.Name;
                hotelModelView.Location = item.Location;
                hotelModelView.HotelDescription = item.Description;
            }
        }
        return hotelModelView;
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
            throw new Exception("It i   s not booked yet");
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
