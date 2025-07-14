using Tourly.Constants;
using Tourly.Domain;
using Tourly.Helpers;
using Tourly.Models.BookingModels;

namespace Tourly.Extentions;

public static class Converter
{
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
                StartDate = DateTime.Parse(parts[4]),
                EndDate = DateTime.Parse(parts[5]),
            });
        }
        return bookings;
    }

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
            HotelName = hotel.Name,
            HotelDescription = hotel.Description,
            HotelPrice = hotel.Price,
            HotelStatus = hotel.Status,
            StartDate = hotel.StartDate,
            EndDate = hotel.EndDate
        };
    }

    public static List<HotelModelView> ToHotelList(this string text)
    {
        List<HotelModelView> hotels = new List<HotelModelView>();

        string[] lines = text.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');

            hotels.Add(new Hotel
            {
                Name = parts[1],
                Description = parts[2],
                Price = decimal.Parse(parts[3]),
                 = (Enums.HotelStatus)Enum.Parse(typeof(Enums.HotelStatus), parts[4]),
                StartDate = DateTime.Parse(parts[5]),
                EndDate = DateTime.Parse(parts[6])
            });
        }
        return hotels;
    }
}