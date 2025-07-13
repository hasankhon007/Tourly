using Tourly.Constants;
using Tourly.Helpers;

namespace Tourly.Domain;

public class User
{
<<<<<<< HEAD
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
=======
    public int Id { get; set; }

    ////ewferfvwefwefc

>>>>>>> 7640f47cd3d5c687f92720b98882e95c5ef33712
}
