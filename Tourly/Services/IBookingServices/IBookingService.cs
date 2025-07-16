<<<<<<< HEAD
﻿using Tourly.Domain;
using Tourly.Models.BookingModels;
namespace Tourly.Services.IBookingServices;
public interface IBookingService
{
    List<Hotel> SearchAllHotels(Hotel hotel, string search);
    void BookHotel(Hotel hotelBookingModel, int userId);
    void CancelBooking(int bookingId);
=======
﻿using Tourly.BookingModels;
using Tourly.Domain;
namespace Tourly.IServices.IBookingServices;
public interface IBookingService
{
    List<Hotel> SearchAllHotels(Hotel hotel, string search);
    void BookHotel(HotelBookingModel hotelBookingModel);
    void CancelBooking(HotelBookingModel hotelBookingModel);
>>>>>>> da2b102d3baeae39fa678fc9dee539ca1b74efbf
    List<HotelBookingModel> GetBookings();
    void ChangeBooking(HotelBookingModel hotelBookingModel);
    HotelModelView View(Guid hotelId);
    bool CheckAvailability(HotelBookingModel hotelBookingModel);
    void CalculateTotalPrizeOfBooking(HotelBookingModel hotelBookingModel);
}
