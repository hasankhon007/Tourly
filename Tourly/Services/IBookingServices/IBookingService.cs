using Tourly.BookingModels;
using Tourly.Domain;
namespace Tourly.IServices.IBookingServices;
public interface IBookingService
{
    List<Hotel> SearchAllHotels(Hotel hotel, string search);
    void BookHotel(HotelBookingModel hotelBookingModel);
    void CancelBooking(HotelBookingModel hotelBookingModel);
    List<HotelBookingModel> GetBookings();
    void ChangeBooking(HotelBookingModel hotelBookingModel);
    HotelModelView View(Guid hotelId);
    bool CheckAvailability(HotelBookingModel hotelBookingModel);
    void CalculateTotalPrizeOfBooking(HotelBookingModel hotelBookingModel);
}
