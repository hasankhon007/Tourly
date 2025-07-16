using Tourly.Domain;
using Tourly.Models.HotelModels;

namespace Tourly.Services.IHotelService;

public interface IHotelService
{
    void Create(HotelCreateModel hotelCreateModel);

    void Update(HotelUpdateModel hotelUpdateModel);

    void Delete(int id);

    Hotel Get(int id);

    List<Hotel> GetAll(string search);

    List<Hotel> GetAllByLocation(string location);
}
