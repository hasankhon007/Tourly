using Tourly.Constants;
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
    public BookingService(HotelBookingModel hotelBookingModel,HotelModelView hotelModelView , UserService userService)
    {
        this.hotelBookingModel = hotelBookingModel;
        this.hotelModelView = hotelModelView;
        this.userServices = userService;
    }

    public void BookHotel(HotelBookingModel hotelBookingModel)
    {
        string text = FileHelper.ReadFromFile(PathHolder.BookingFilesPath);

        List<Booking> bookings = text.ConvertTextToList();


    }

    public void CalculateTotalPrizeOfBooking(HotelBookingModel hotelBookingModel)
    {

    }

    public void CancelBooking(HotelBookingModel hotelBookingModel)
    {

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
    // 
    public HotelModelView View(int hotelId)
    {
        string text = FileHelper.ReadFromFile(PathHolder.BookingFilesPath);

        List<Booking> convertedBookings = text.ConvertTextToList();

        var existBooking = convertedBookings.Find(x => x.HotelId == hotelId);

        if (existBooking == null)
        {
            throw new Exception("Booking is not found");
        }
        return existBooking.ToHotelViewModel();
    }
}
