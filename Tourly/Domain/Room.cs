using Tourly.Enums;
namespace Tourly.BookingModels;
public class Room
{
    public int Id { get; set; }
    public byte RoomNumber { get; set; }       
    public RoomType RoomType { get; set; }               
    public RoomStatus Status { get; set; }       
    public decimal PricePerNight { get; set; }   

    public override string ToString()
    {
        return $"{Id},{RoomNumber},{RoomType},{Status},{PricePerNight}";
    }
    
}
