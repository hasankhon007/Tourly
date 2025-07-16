<<<<<<< HEAD
﻿namespace Tourly.Models.UserModels;
=======
﻿namespace Tourly.UserModels;
>>>>>>> da2b102d3baeae39fa678fc9dee539ca1b74efbf

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
