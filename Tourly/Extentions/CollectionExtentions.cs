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
}
