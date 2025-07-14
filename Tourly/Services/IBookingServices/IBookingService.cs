using Tourly.Domain;
using Tourly.Models.BookingModels;
namespace Tourly.Services.IBookingServices;
public interface IBookingService
{
    List<Hotel> SearchAllHotels(Hotel hotel, string search);
    void BookHotel(HotelBookingModel hotelBookingModel, HotelModelView hotelModelView);
    void CancelBooking(HotelBookingModel hotelBookingModel);
    List<HotelBookingModel> GetBookings();
    void ChangeBooking(HotelBookingModel hotelBookingModel);
    HotelModelView View(Guid hotelId);
    bool CheckAvailability(HotelBookingModel hotelBookingModel);
    void CalculateTotalPrizeOfBooking(HotelBookingModel hotelBookingModel);
}
