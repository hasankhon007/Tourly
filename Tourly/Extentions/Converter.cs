using Tourly.Constants;
using Tourly.Domain;
using Tourly.Helpers;
using Tourly.Models.BookingModels;

namespace Tourly.Extentions;

public static class Converter
{
    #region ToUser
    public static List<User> ToUser(this string text)
    {
        List<User> users = new();

        string[] lines = text.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');

            users.Add(new User
            {
                Id = int.Parse(parts[0]),
                FirstName = parts[1],
                LastName = parts[2],
                PhoneNumber = parts[3],
                Password = parts[4],
            });
        }

        return users;
    }
    #endregion

    #region ConvertTextToList
    public static List<Booking> ConvertTextToList(this string text)
    {
        List<Booking> bookings = new List<Booking>();

        string[] lines = text.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');

            bookings.Add(new Booking
            {
                ID = int.Parse(parts[0]),
                UserId = int.Parse(parts[1]),
                HotelId = int.Parse(parts[2]),
                Price = decimal.Parse(parts[3]),
                StartDate = DateOnly.Parse(parts[4]),
                EndDate = DateOnly.Parse(parts[5]),
            });
        }
        return bookings;
    }
    #endregion

    #region ToHotelViewModel
    public static HotelModelView ToHotelViewModel(this Hotel booking)
    {
        string hotelInfoInTextFormat = FileHelper.ReadFromFile(PathHolder.HotelsFilesPath);

        List<Hotel> hotels = hotelInfoInTextFormat.ToHotelList();

        Hotel hotel = hotels.Find(x => x.ID == booking.ID);

        if (hotel == null)
        {
            throw new Exception("Stadium was not found!");
        }
        
        return new HotelModelView
        {
           
        };
    }
    #endregion

    #region ToHotelList
    public static List<Hotel> ToHotelList(this string text)
    {
        List<Hotel> hotels = new List<Hotel>();

        string[] lines = text.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');

            hotels.Add(new Hotel
            {
                Name = parts[0],
                Description = parts[1],
                Price = decimal.Parse(parts[2]),
                Status = (Enums.HotelStatus)Enum.Parse(typeof(Enums.HotelStatus), parts[3]),
                StartDate = DateOnly.Parse(parts[4]),
                EndDate = DateOnly.Parse(parts[5])
            });
        }
        return hotels;
    }
    #endregion
}