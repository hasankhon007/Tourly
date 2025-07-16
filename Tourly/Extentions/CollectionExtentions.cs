using Tourly.BookingModels;
using Tourly.Domain;

namespace Tourly.Extentions;

public static class CollectionExtentions
{
    public static List<string> Convert<T>(this List<T> list)
    {
        var convertedList = new List<string>();
        foreach (var item in list)
        {
            convertedList.Add(item.ToString());
        }
        return convertedList;
    }

    public static List<HotelBookingModel> ConvertTo(this List<Booking> list)
    {
        var convertedList = new List<HotelBookingModel>();

        foreach (var item in list)
        {
            convertedList.Add(new HotelBookingModel
            {
                UserId = item.UserId,
                HotelId = item.HotelId,
                HotelName = item.HotelName,
                RoomId = item.RoomId,
                RoomType = item.RoomType,
                StartDate = item.StartDate,
                EndDate = item.EndDate
            });
        }
        return convertedList;
    }
}
