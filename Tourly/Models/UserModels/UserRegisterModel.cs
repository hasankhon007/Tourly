namespace Tourly.Models.UserModels;

public class UserRegisterModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}

public class UserLoginModel
{
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}
