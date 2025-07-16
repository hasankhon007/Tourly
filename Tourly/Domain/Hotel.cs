using Tourly.BookingModels;
using Tourly.Constants;
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
    public string Description { get; set; }
    public byte StarsCount { get; set; } = 5;
    public List<Room> Rooms { get; set; }
    public override string ToString()
    {
        return $"{ID},{Name},{Location},{PhoneNumber},{Description}, {StarsCount}";
    }
}
