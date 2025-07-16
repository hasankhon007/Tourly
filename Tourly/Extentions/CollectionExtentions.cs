using Tourly.Domain;

namespace Tourly.Extentions;

public static class CollectionExtentions
{
<<<<<<< HEAD
    public static List<string> ConvertToString(this List<User> users)
    {
        var convertedUser = new List<string>();

        foreach (var user in users)
        {
            convertedUser.Add(user.ToString());
        }

        return convertedUser;
    }

    public static List<string> ConvertToString(this List<Booking> bookings)
    {
        var convertedBooking = new List<string>();

        foreach (var booking in bookings)
        {
            convertedBooking.Add(booking.ToString());
        }

        return convertedBooking;
=======
    public static List<string> Convert<T>(this List<T> list)
    {
        var convertedList = new List<string>();
        foreach (var item in list)
        {
            convertedList.Add(item.ToString());
        }
        return convertedList;
>>>>>>> da2b102d3baeae39fa678fc9dee539ca1b74efbf
    }
}
