using Tourly.BookingModels;
using Tourly.Domain;
using Tourly.Enums;
using Tourly.Models.BookingModels;
namespace Tourly.Services.IBookingServices;
public interface IBookingService
{
    List<HotelModelView> SearchAllHotels(List<Hotel> hotels, string? search);
    Booking? BookRoom(int userId, Hotel hotel, RoomType desiredType, DateOnly start, DateOnly end);
    void CancelBooking(int bookingId);
    bool IsRoomAvailable(Hotel hotel, Room room, DateOnly start, DateOnly end);
    HotelModelView View(int hotelId);
    List<HotelBookingModel> GetBookings(int userId);
    void ChangeBooking(HotelBookingModel updatedBooking); 
}
