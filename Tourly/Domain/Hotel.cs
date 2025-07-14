﻿using Tourly.Constants;
using Tourly.Enums;
using Tourly.Helpers;
namespace Tourly.Domain;

public class Hotel
{
    public Hotel()
    {
        ID = GeneratorHelper.GenerateId(PathHolder.HotelsFilesPath);
    }

    public int ID { get; set; }
    public string Location { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public HotelStatus Status { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public byte StarsCount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public override string ToString()
    {
        return $"{ID},{Name},{Price},{Location},{PhoneNumber},{Description},{StarsCount}";
    }
}
