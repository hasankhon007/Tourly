using Tourly.BookingModels;
using Tourly.Enums;

namespace Tourly.Helpers;

public  static class CreateRooms
{
    public static List<Room> Create(this RoomType type, int count)
    {
        var rooms = new List<Room>();
        for(int i = 0; i < count; i++)
        {
            rooms.Add(new Room
            {
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
        return rooms;
    }
}
