namespace Tourly.Constants;
public class PathHolder
{
    private static readonly string parentRoot = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
    public static readonly string UserFilesPath = Path.Combine(parentRoot,"Data", "users.txt");
    public static readonly string HotelsFilesPath = Path.Combine(parentRoot, "Data", "hotels.txt");
    public static readonly string BookingFilesPath = Path.Combine(parentRoot, "Data", "bokkings.txt");
}
