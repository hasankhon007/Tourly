using Tourly.Constants;
using Tourly.Domain;
using Tourly.Helpers;
using Tourly.Models.BookingModels;
using Tourly.Enums;
using Tourly.BookingModels;
namespace Tourly.Extentions;

public static class Converter
{
    #region ToModel
    public static List<T> Convert<T>(this string text)
    {
        List<T> items = new();
        string[] lines = text.Split('\n');
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] parts = line.Split(',');
            if (typeof(T) == typeof(User))
            {
                items.Add((T)(object)new User
                {
                    Id = int.Parse(parts[0]),
                    FirstName = parts[1],
                    LastName = parts[2],
                    PhoneNumber = parts[3],
                    Password = parts[4],
                });
            }
            else if (typeof(T) == typeof(Booking))
            {
                items.Add((T)(object)new Booking
                {
                    ID = int.Parse(parts[0]),
                    UserId = int.Parse(parts[1]),
                    HotelId = int.Parse(parts[2]),
                    StartDate = DateOnly.Parse(parts[3]),
                    EndDate = DateOnly.Parse(parts[4]),
                    Price = decimal.Parse(parts[5]),
                });
            }
            else if (typeof(T) == typeof(Hotel))
            {
                items.Add((T)(object)new Hotel
                {
                    ID = int.Parse(parts[0]),
                    Name = parts[1],
                    Location = parts[2],
                    PhoneNumber = parts[3],
                    Description = parts[4],
                    StarsCount = byte.Parse(parts[5]),
                });
            }
            else if (typeof(T) == typeof(Room))
            {
                items.Add((T)(object)new Room
                {
                    Id = int.Parse(parts[0]),
                    HotelId = int.Parse(parts[1]),
                    RoomType = (RoomType)Enum.Parse(typeof(RoomType), parts[2]),
                    Status = Enum.Parse<RoomStatus>(parts[3]),
                    PricePerNight = decimal.Parse(parts[4]),
                });
            }
            
        }
        return items;
    }
    #endregion
}


