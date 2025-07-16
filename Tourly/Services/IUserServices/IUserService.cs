using Tourly.Models.UserModels;

namespace Tourly.Services.IUserServices;
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
