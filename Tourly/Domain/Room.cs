using Tourly.Enums;
<<<<<<< HEAD
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
=======
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
    
>>>>>>> da2b102d3baeae39fa678fc9dee539ca1b74efbf
}
