using Tourly.Constants;
using Tourly.Helpers;
namespace Tourly.Domain;
public class User
{
    public User()
    {
        Id = GeneratorHelper.GenerateId(PathHolder.UserFilesPath);
    }

    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }

    public override string ToString()
    {
        return $"{Id},{FirstName},{LastName},{PhoneNumber},{Password}";
    }
}
