using Tourly.Domain;

namespace Tourly.Extentions;

public static class CollectionExtentions
{
    public static List<string> ConvertToString(this List<User> users)
    {
        var convertedUser = new List<string>();

        foreach (var user in users)
        {
            convertedUser.Add(user.ToString());
        }

        return convertedUser;
    }
}
