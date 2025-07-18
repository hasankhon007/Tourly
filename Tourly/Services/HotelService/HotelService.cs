namespace Tourly.Services.HotelService;

using System;
using System.Collections.Generic;
using Tourly.Constants;
using Tourly.Domain;
using Tourly.Extentions;
using Tourly.Helpers;
using Tourly.Models.HotelModels;
using Tourly.Services.BookingServices;
using Tourly.Services.IHotelService;

public class HotelService : IHotelService
{
    private readonly BookingService bookingService;
    public void Create(HotelCreateModel hotelCreateModel)
    {
        string text = File.ReadAllText(PathHolder.HotelsFilesPath);

        var convertedhotels = text.Convert<Hotel>();

        var existinghotel = convertedhotels.Find(x => x.Name == hotelCreateModel.Name);

        if (existinghotel != null)
        {
            throw new Exception($"Hotel with this name <{hotelCreateModel.Name}> already exists");
        }

        if (string.IsNullOrEmpty(hotelCreateModel.PhoneNumber))
        {
            throw new Exception("Phone should not be null or empty");
        }

        if (hotelCreateModel.PhoneNumber.Length != 13)
        {
            throw new Exception("Phone number should be 13 characters");
        }

        if (!hotelCreateModel.PhoneNumber.StartsWith("+998"))
        {
            throw new Exception("Phone number should start with '+998'");
        }

        convertedhotels.Add(new Hotel
        {
            ID = GeneratorHelper.GenerateId(PathHolder.HotelsFilesPath),
            Name = hotelCreateModel.Name,
            Location = hotelCreateModel.Location,
            PhoneNumber = hotelCreateModel.PhoneNumber,
            Description = hotelCreateModel.Description,
            StarsCount = hotelCreateModel.StarsCount,
            Rooms = hotelCreateModel.Rooms
        });
        FileHelper.WriteToFile(PathHolder.HotelsFilesPath, convertedhotels.Convert());
    }

    public void Update(HotelUpdateModel model)
    {
        var text = FileHelper.ReadFromFile(PathHolder.HotelsFilesPath);
        var hotels = text.Convert<Hotel>();
        var existHotel = hotels.Find(x => x.ID == model.Id)
            ?? throw new Exception("Hotel is not found");


        var alreadyExistHotel = hotels.Find(x => x.Name == model.Name);

        if (alreadyExistHotel != null)
        {
            throw new Exception($"Stadium already exists with this name = {model.Name}");
        }

        if (string.IsNullOrEmpty(model.PhoneNumber))
        {
            throw new Exception("Phone should not be null or empty");
        }

        if (model.PhoneNumber.Length != 13)
        {
            throw new Exception("Phone number should be 13 characters");
        }

        if (!model.PhoneNumber.StartsWith("+998"))
        {
            throw new Exception("Phone number should start with '+998'");
        }
        existHotel.Name = model.Name;
        existHotel.Location = model.Location;
        existHotel.PhoneNumber = model.PhoneNumber;
        existHotel.Description = model.Description;
        existHotel.StarsCount = model.StarsCount;

        FileHelper.WriteToFile(PathHolder.HotelsFilesPath, hotels.Convert());
    }

    public void Delete(int id)
    {
        var text = FileHelper.ReadFromFile(PathHolder.HotelsFilesPath);
        var hotels = text.Convert<Hotel>();
        var existHotel = hotels.Find(x => x.ID == id)
            ?? throw new Exception("Hotel is not found");

        hotels.Remove(existHotel);

        FileHelper.WriteToFile(PathHolder.HotelsFilesPath, hotels.Convert());
    }

    public Hotel Get(int id)
    {
        var text = FileHelper.ReadFromFile(PathHolder.HotelsFilesPath);
        var hotels = text.Convert<Hotel>();
        var existHotel = hotels.Find(x => x.ID == id)
            ?? throw new Exception("Hotel is not found");

        return existHotel;
    }

    public List<Hotel> GetAll(string search)
    {
        var text = FileHelper.ReadFromFile(PathHolder.HotelsFilesPath);
        var hotels = text.Convert<Hotel>();

        if (!string.IsNullOrEmpty(search))
        {
            hotels = Search(search);
        }
        return hotels;
    }
    
    public List<Hotel> GetAllByLocation(string location)
    {
        var text = FileHelper.ReadFromFile(PathHolder.HotelsFilesPath);
        var hotels = text.Convert<Hotel>();
        var result = new List<Hotel>();

        foreach (var hotel in hotels)
        {
            if (hotel.Location.ToLower().Contains(location.ToLower()))
            {
                result.Add(hotel);
            }
        }

        return result;
    }           
      
    private List<Hotel> Search(string search)
    {
        var text = FileHelper.ReadFromFile(PathHolder.HotelsFilesPath);
        var hotels = text.Convert<Hotel>();
        var result = new List<Hotel>();

        if (!string.IsNullOrEmpty(search))
        {
            string trimedString = search.TrimStart(' ').ToLower();

            foreach (var hotel in hotels)
            {
                if (hotel.Name.ToLower().Contains(trimedString))
                {
                    result.Add(hotel);
                }
            }
        }

        return result;
    }
}
