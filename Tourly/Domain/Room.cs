using Tourly.Constants;
using Tourly.Enums;
using Tourly.Helpers;
namespace Tourly.BookingModels;
public class Room
{
    public Room()
    {
        Id = GeneratorHelper.GenerateId(PathHolder.RoomsFilesPath);
    }
    public int Id { get; set; }    
    public RoomType RoomType { get; set; }               
    public RoomStatus Status { get; set; }       
    public decimal PricePerNight { get; set; }   

    public override string ToString()
    {
        return $"{Id},{RoomType},{Status},{PricePerNight}";
    }
    
}
