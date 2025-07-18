using Tourly.BookingModels;
using Tourly.Constants;
using Tourly.Domain;
using Tourly.Enums;
using Tourly.Extentions;

namespace Tourly.Helpers;

public  static class CreateRooms
{
    public static List<Room> Create(this RoomType type, int count, int hotelid)
    {
        var rooms = new List<Room>();
        for(int i = 0; i < count; i++)
        {
            rooms.Add(new Room
            {
                HotelId = hotelid,
                Status = RoomStatus.Available,
                PricePerNight = type switch
                {
                    RoomType.Single => 50,
                    RoomType.Double => 100,
                    RoomType.VIP => 200,
                    RoomType.Family => 150,
                    RoomType.Deluxe => 180,
                },
                RoomType = type
            });
        }
        var lst = FileHelper.ReadFromFile(PathHolder.RoomsFilesPath).Convert<Room>();
        lst.AddRange(rooms);
        FileHelper.WriteToFile(PathHolder.RoomsFilesPath, lst.Convert());
        return rooms;

    }
}
