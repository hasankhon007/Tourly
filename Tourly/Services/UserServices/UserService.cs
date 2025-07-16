using Tourly.Constants;
using Tourly.Domain;
using Tourly.Extentions;
using Tourly.Helpers;
using Tourly.IServices.IUserServices;
using Tourly.Models.UserModels;
using Tourly.UserModels;

namespace Tourly.Services.UserServices;
    public class UserService : IUserService
{
    private readonly string _path = PathHolder.UserFilesPath;

        FileHelper.WriteToFile(PathHolder.UserFilesPath, users.Convert());
    }

    public void Delete(int id)

    private void EnsureFileExists()
    {
        var dir = Path.GetDirectoryName(_path);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir!);

        var users = text.ToUser();

        var existUser = users.Find(u => u.Id == id)
            ?? throw new Exception("User is not found.");

        users.Remove(existUser);

        FileHelper.WriteToFile(PathHolder.UserFilesPath, users.Convert());

        if (!File.Exists(_path))
            File.Create(_path).Close();
    }

    private void ValidateInput(
        string firstName,
        string lastName,
        string country,
        string phone,
        string password)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty");

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty");

        if (!phone.StartsWith("+998") || phone.Length != 13)
            throw new ArgumentException("Phone must start with +998 and be 13 characters long");

        if (string.IsNullOrWhiteSpace(password) || password.Length < 4)
            throw new ArgumentException("Password must be at least 4 characters long");
    }
    private void UpdateUser(User updated, List<User> allUsers)
    {
        for (int i = 0; i < allUsers.Count; i++)
        {
            if (allUsers[i].Id == updated.Id)
            {
                allUsers[i] = updated;
                break;
            }
        }

        File.WriteAllText(_path, string.Empty);
        foreach (var u in allUsers)
            File.AppendAllText(_path, u.ToString() + "\n");
    }

    public void Register(UserRegisterModel model)
    {
        ValidateInput(
            model.FirstName,
            model.LastName,
            model.Country,
            model.PhoneNumber,
            model.Password);
        EnsureFileExists();

        var users = LoadUsers();

        var existUser = users.FirstOrDefault(u => u.PhoneNumber == model.PhoneNumber);
        if (existUser != null)
        {
            existUser.VisitCount++;
            UpdateUser(existUser, users);
            return;
        }

        var newUser = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Country = model.Country,
            PhoneNumber = model.PhoneNumber,
            Password = model.Password
        };

        users.Add(user);

        FileHelper.WriteToFile(PathHolder.UserFilesPath, users.Convert());

        users.Add(newUser);
        SaveAllUsers(users);
    }

    public int Login(UserLoginModel model)
    {
        var users = LoadUsers();
        var user = users.FirstOrDefault(u => u.PhoneNumber == model.PhoneNumber)
            ?? throw new Exception("Phone or password is incorrect.");

        if (user.Password != model.Password)
            throw new Exception("Phone or password is incorrect.");

        return user.Id;
    }

    public void ChangePassword(
        int userId, 
        string oldPassword, 
        string newPassword)
    {
        var users = LoadUsers();
        var user = users.FirstOrDefault(u => u.Id == userId)
            ?? throw new Exception("User is not found.");

        if (user.Password != oldPassword)
            throw new Exception("Passwords do not match.");

        user.Password = newPassword;
        SaveAllUsers(users);
    }

    public void Delete(int id)
    {
        var users = LoadUsers();
        var user = users.FirstOrDefault(u => u.Id == id)
            ?? throw new Exception("User is not found.");

        users.Remove(user);
        SaveAllUsers(users);
    }

    public void Update(UserUpdateModel model)
    {
        var users = LoadUsers();
        var user = users.FirstOrDefault(u => u.Id == model.Id)
            ?? throw new Exception("User is not found.");

        var duplicate = users.FirstOrDefault(u => u.PhoneNumber == model.PhoneNumber && u.Id != model.Id);
        if (duplicate != null)
            throw new Exception("User with this phone already exists.");

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.PhoneNumber = model.PhoneNumber;

        SaveAllUsers(users);
    }

    public UserViewModel Get(int id)
    {
        var user = LoadUsers().FirstOrDefault(u => u.Id == id)
            ?? throw new Exception("User is not found.");

        return new UserViewModel()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        };
    }

    public List<UserViewModel> GetAll(string search)
    {
        var users = LoadUsers();
        var result = string.IsNullOrWhiteSpace(search) ? users : Search(users, search);

        FileHelper.WriteToFile(PathHolder.UserFilesPath, users.Convert());

        return result.Select(user => new UserViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        }).ToList();
    }

    public bool IsVIPGuest(int userId)
    {
        var user = LoadUsers().FirstOrDefault(u => u.Id == userId);
        return user != null && user.VisitCount >= 5;
    }

    private List<User> LoadUsers()
    {
        EnsureFileExists();
        var lines = File.ReadAllLines(_path);
        var users = new List<User>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Split(',');
            if (parts.Length != 7) continue;

            users.Add(new User
            {
                Id = int.Parse(parts[0]),
                FirstName = parts[1],
                LastName = parts[2],
                Country = parts[3],
                VisitCount = int.Parse(parts[4]),
                PhoneNumber = parts[5],
                Password = parts[6]
            });
        }

        return users;
    }
    private void SaveAllUsers(List<User> users)
    {
        File.WriteAllText(_path, string.Empty);
        foreach (var user in users)
        {
            File.AppendAllText(_path, user.ToString() + "\n");
        }
    }
    private List<User> Search(List<User> users, string search)
    {
        return users.Where(user =>
            user.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            user.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            user.PhoneNumber.Contains(search, StringComparison.OrdinalIgnoreCase)
        ).ToList();
    }
}

