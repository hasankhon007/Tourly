using Tourly.Models.UserModels;
<<<<<<< HEAD

namespace Tourly.Services.IUserServices;
=======
using Tourly.UserModels;

namespace Tourly.IServices.IUserServices;
>>>>>>> da2b102d3baeae39fa678fc9dee539ca1b74efbf
public interface IUserService
{
    void Register(UserRegisterModel model);

    int Login(UserLoginModel model);

    UserViewModel Get(int id);

    void Update(UserUpdateModel model);

    void Delete(int id);

    List<UserViewModel> GetAll(string search);

    void ChangePassword(int userId, string oldPassword, string newPassword);
}
