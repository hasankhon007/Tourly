<<<<<<< HEAD
﻿using Tourly.Constants;
using Tourly.Domain;
using Tourly.Helpers;
using Tourly.Models.BookingModels;
=======
﻿using Tourly.BookingModels;
using Tourly.Domain;
using Tourly.Enums;
>>>>>>> da2b102d3baeae39fa678fc9dee539ca1b74efbf

namespace Tourly.Extentions;

public static class Converter
{
<<<<<<< HEAD
    #region ToUser
=======
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
            else if(typeof(T) == typeof(Booking))
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
            else if(typeof(T) == typeof(Hotel))
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
            else if(typeof(T) == typeof(Room))
            {
                items.Add((T)(object)new Room
                {
                    Id = int.Parse(parts[0]),
                    RoomNumber = byte.Parse(parts[1]),
                    RoomType = (RoomType)Enum.Parse(typeof(RoomType), parts[2]),
                    Status = Enum.Parse<RoomStatus>(parts[3]),
                    PricePerNight = decimal.Parse(parts[4]),
                });
            }
        }
        return items;
    }
>>>>>>> da2b102d3baeae39fa678fc9dee539ca1b74efbf
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
<<<<<<< HEAD
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
=======
}
>>>>>>> da2b102d3baeae39fa678fc9dee539ca1b74efbf
