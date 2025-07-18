using Tourly.Constants;
using Tourly.Domain;
using Tourly.Extentions;
using Tourly.Helpers;
using Tourly.Models.UserModels;


namespace Tourly.Services.UserServices;
public class UserService : IUserService
{
    public void ChangePassword(int userId, string oldPassword, string newPassword)
    {
        var text = FileHelper.ReadFromFile(PathHolder.UserFilesPath);

        var users = text.Convert<User>();

        var existUser = users.Find(u => u.Id == userId)
            ?? throw new Exception("User is not found.");

        if (existUser.Password != oldPassword)
            throw new Exception("Passwords do not match.");

        existUser.Password = newPassword;


        FileHelper.WriteToFile(PathHolder.UserFilesPath, users.Convert());
    }

    public void Delete(int id)
    {
        var text = FileHelper.ReadFromFile(PathHolder.UserFilesPath);

        var users = text.Convert<User>();

        var existUser = users.Find(u => u.Id == id)
            ?? throw new Exception("User is not found.");

        users.Remove(existUser);


        FileHelper.WriteToFile(PathHolder.UserFilesPath, users.Convert());
    }

    public UserViewModel Get(int id)
    {
        var text = FileHelper.ReadFromFile(PathHolder.UserFilesPath);

        var users = text.Convert<User>();

        var existUser = users.Find(u => u.Id == id)
            ?? throw new Exception("User is not found.");

        return new UserViewModel()
        {
            Id = existUser.Id,
            FirstName = existUser.FirstName,
            LastName = existUser.LastName,
            PhoneNumber = existUser.PhoneNumber
        };
    }

    public List<UserViewModel> GetAll(string search)
    {
        var text = FileHelper.ReadFromFile(PathHolder.UserFilesPath);

        var users = text.Convert<User>();

        var result = new List<UserViewModel>();

        if (!string.IsNullOrEmpty(search))
        {
            users = Search(users, search);
        }

        foreach (var user in users)
        {
            result.Add(new UserViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            });
        }

        return result;
    }

    public void Register(UserRegisterModel model)
    {
        var text = FileHelper.ReadFromFile(PathHolder.UserFilesPath);

        var users = text.Convert<User>();

        var existUser = users.Find(u => u.PhoneNumber == model.PhoneNumber);
        if (existUser != null)
        {
            throw new Exception("User with this phone number already exists.");
        }

        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            Password = model.Password
        };

        users.Add(user);


        FileHelper.WriteToFile(PathHolder.UserFilesPath, users.Convert());
    }

    public int Login(UserLoginModel model)
    {
        var text = FileHelper.ReadFromFile(PathHolder.UserFilesPath);

        var users = text.Convert<User>();

        var existUser = users.Find(u => u.PhoneNumber == model.PhoneNumber)
            ?? throw new Exception("Phone or password is incorrect.");

        if (existUser.Password != model.Password)
        {
            throw new Exception("Phone or password is incorrect.");
        }

        return existUser.Id;
    }

    public void Update(UserUpdateModel model)
    {
        var text = FileHelper.ReadFromFile(PathHolder.UserFilesPath);

        var users = text.Convert<User>();

        var existUser = users.Find(u => u.Id == model.Id)
            ?? throw new Exception("User is not found.");

        var alreadyExistUser = users.Find(u => u.PhoneNumber == model.PhoneNumber);

        if (alreadyExistUser != null)
        {
            throw new Exception("User already exists with this phone number.");
        }

        existUser.FirstName = model.FirstName;
        existUser.LastName = model.LastName;
        existUser.PhoneNumber = model.PhoneNumber;

        FileHelper.WriteToFile(PathHolder.UserFilesPath, users.Convert());
    }

    public List<User> GetALLforAdmin()
    {
        var text = FileHelper.ReadFromFile(PathHolder.UserFilesPath);
        var users = text.Convert<User>();
        
        return users; // Assuming you want to return the first user for admin purposes
    }

    private List<User> Search(List<User> users, string search)
    {
        var result = new List<User>();

        if (!string.IsNullOrEmpty(search))
        {
            foreach (var user in users)
            {
                if (
                    user.FirstName.ToLower().Contains(search.ToLower()) ||
                    user.LastName.ToLower().Contains(search.ToLower()) ||
                    user.PhoneNumber.ToLower().Contains(search.ToLower()))
                {
                    result.Add(user);
                }
            }
        }

        return result;
    }

    public async Task SaveToFileAsync(UserRegisterModel user)
    {
        user.Id = await GetNextUserIdAsync();
        var line = $"{user.Id},{user.FirstName},{user.LastName},{user.PhoneNumber},{user.Password}";
        await File.AppendAllTextAsync(PathHolder.UserFilesPath, line + Environment.NewLine);
    }

    private async Task<int> GetNextUserIdAsync()
    {
        if (!File.Exists(PathHolder.UserFilesPath))
            return 1;

        var lines = await File.ReadAllLinesAsync(PathHolder.UserFilesPath);
        if (lines.Length == 0)
            return 1;

        var lastLine = lines.Last();
        var lastId = int.Parse(lastLine.Split(',')[0]);
        return lastId + 1;
    }

    public async Task<List<UserRegisterModel>> GetAllUsersAsync()
    {
        var users = new List<UserRegisterModel>();

        if (!File.Exists(PathHolder.UserFilesPath)) return users;

        var lines = await File.ReadAllLinesAsync(PathHolder.UserFilesPath);
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length < 5) continue;

            users.Add(new UserRegisterModel
            {
                Id = int.Parse(parts[0]),
                FirstName = parts[1],
                LastName = parts[2],
                PhoneNumber = parts[3],
                Password = parts[4]
            });
        }

        return users;
    }

    public async Task<UserRegisterModel?> LoginAsync(string phone, string password)
    {
        if (!File.Exists(PathHolder.UserFilesPath))
            return null;

        string[] lines = await File.ReadAllLinesAsync(PathHolder.UserFilesPath);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');

            if (parts.Length >= 5)
            {
                string filePhone = parts[3].Trim();
                string filePassword = parts[4].Trim();

                if (filePhone == phone && filePassword == password)
                {
                    return new UserRegisterModel
                    {
                        Id = int.Parse(parts[0]),
                        FirstName = parts[1],
                        LastName = parts[2],
                        PhoneNumber = filePhone,
                        Password = filePassword
                    };
                }
            }
        }

        return null;
    }

}

