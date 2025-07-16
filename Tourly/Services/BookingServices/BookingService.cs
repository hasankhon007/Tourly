<<<<<<< HEAD
﻿using Tourly.Constants;
using Tourly.Domain;
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
    public BookingService(HotelBookingModel hotelBookingModel, HotelModelView hotelModelView, UserService userService)
    {
        this.hotelBookingModel = hotelBookingModel;
        this.hotelModelView = hotelModelView;
        this.userServices = userService;
    }

    #region Booking Hotel
    public void BookHotel(Hotel hotelBookingModel, int userId)
    {
        string text = FileHelper.ReadFromFile(PathHolder.BookingFilesPath);

        List<Booking> bookings = new List<Booking>();
        bookings = text.ConvertTextToList();

        Booking newBooking = new Booking
        {
            UserId = userId,
            HotelId = hotelBookingModel.ID,
            BookingHotelName = hotelBookingModel.Name,
            BookingHotelAddress = hotelBookingModel.Location,
            Price = hotelBookingModel.Price,
            StartDate = hotelBookingModel.StartDate,
            EndDate = hotelBookingModel.EndDate,
        };

        bookings.Add(newBooking);

        FileHelper.WriteToFile(PathHolder.BookingFilesPath, bookings.ConvertToString());
    }
    #endregion

    public void CalculateTotalPrizeOfBooking(HotelBookingModel hotelBookingModel)
    {

    }

    #region Cancelling Booking
    public void CancelBooking(int bookingId)
    {
        string text = FileHelper.ReadFromFile(PathHolder.BookingFilesPath);

        List<Booking> convertedBookings = text.ConvertTextToList();

        var existBooking = convertedBookings.Find(x => x.ID == bookingId);

        if (existBooking == null)
        {
            throw new Exception("Booking was not found");
        }

        convertedBookings.Remove(existBooking);

        File.WriteAllLines(PathHolder.BookingFilesPath, convertedBookings.ConvertToString());
    }
    #endregion

    public void ChangeBooking(HotelBookingModel hotelBookingModel)
    {

    }

    public bool CheckAvailability(Hotel hotel)
    {

=======
﻿using Tourly.BookingModels;
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
>>>>>>> da2b102d3baeae39fa678fc9dee539ca1b74efbf
    }

    public List<HotelBookingModel> GetBookings()
    {
<<<<<<< HEAD
        string text = FileHelper.ReadFromFile(PathHolder.BookingFilesPath);

        List<Booking> convertedBookings = text.ConvertTextToList();


=======
        throw new NotImplementedException();
>>>>>>> da2b102d3baeae39fa678fc9dee539ca1b74efbf
    }

    public List<Hotel> SearchAllHotels(Hotel hotel, string search)
    {
<<<<<<< HEAD

    }

    // bookinglarni ko'rish
    public HotelModelView View(Hotel hotel ,int hotelId)
    {
        string fileInfo = FileHelper.ReadFromFile(PathHolder.HotelsFilesPath);

        List<Hotel> hotels = fileInfo.ConvertTextToList();



=======
        throw new NotImplementedException();
    }

    public HotelModelView View(Guid hotelId)
    {
        throw new NotImplementedException();
>>>>>>> da2b102d3baeae39fa678fc9dee539ca1b74efbf
    }
}
