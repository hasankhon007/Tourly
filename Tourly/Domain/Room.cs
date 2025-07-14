using Tourly.Enums;
namespace Tourly.BookingModels;
public class Room
{
    public Guid Id { get; set; }
    public string RoomNumber { get; set; }       
    public RoomType RoomType { get; set; }         
    public int Capacity { get; set; }            
    public RoomStatus Status { get; set; }       
    public decimal PricePerNight { get; set; }   

    public override string ToString()
    {
        return $"{Id}|{RoomNumber}|{RoomType}|{Capacity}|{Status}|{PricePerNight}";
    }
    public static Room FromString(string line)
    {
        var parts = line.Split('|');
        return new Room
        {
            Id = Guid.Parse(parts[0]),
            RoomNumber = parts[1],
            RoomType = (RoomType)Enum.Parse(typeof(RoomType), parts[2]),
            Capacity = int.Parse(parts[3]),
            Status = Enum.Parse<RoomStatus>(parts[4]),
            PricePerNight = decimal.Parse(parts[5])
        };
    }
}
