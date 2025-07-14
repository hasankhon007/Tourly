using Tourly.Domain;

namespace Tourly.Extentions;

public static class Converter
{
    public static List<User> ToUser(this string text)
    {
        List<User> users = new();

        string[] lines = text.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');

            users.Add(new User
            {
                Id = int.Parse(parts[0]),
                FirstName = parts[1],
                LastName = parts[2],
                PhoneNumber = parts[3],
                Password = parts[4],
            });
        }

        return users;
    }
}
