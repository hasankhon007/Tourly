namespace Tourly.Models.HotelModels;

public class HotelUpdateModel
{
    public int Id { get; set; }
    public string Location { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Description { get; set; }
    public byte StarsCount { get; set; }
}
