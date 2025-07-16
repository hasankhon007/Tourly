using Tourly.Enums;
namespace Tourly.Domain;
public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; }       
    public RoomType RoomType { get; set; }         
    public int Capacity { get; set; }            
    public RoomStatus Status { get; set; }       
    public decimal PricePerNight { get; set; }   
        
    public override string ToString()
    {
        return $"{Id}|{RoomNumber}|{RoomType}|{Capacity}|{Status}|{PricePerNight}";
    }
}
