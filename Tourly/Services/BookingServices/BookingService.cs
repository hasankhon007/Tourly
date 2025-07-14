using Tourly.BookingModels;
using Tourly.Domain;
using Tourly.IServices.IBookingServices;
namespace Tourly.Services.BookingServices;
public class BookingService : IBookingService
{
    public void BookHotel(HotelBookingModel hotelBookingModel)
    {
        throw new NotImplementedException();
    }

    public void CalculateTotalPrizeOfBooking(HotelBookingModel hotelBookingModel)
    {
        throw new NotImplementedException();
    }

    public void CancelBooking(HotelBookingModel hotelBookingModel)
    {
        throw new NotImplementedException();
    }

    public void ChangeBooking(HotelBookingModel hotelBookingModel)
    {
        throw new NotImplementedException();
    }

    public bool CheckAvailability(HotelBookingModel hotelBookingModel)
    {
        throw new NotImplementedException();
    }

    public List<HotelBookingModel> GetBookings()
    {
        throw new NotImplementedException();
    }

    public List<Hotel> SearchAllHotels(Hotel hotel, string search)
    {
        throw new NotImplementedException();
    }

    public HotelModelView View(Guid hotelId)
    {
        throw new NotImplementedException();
    }
}
