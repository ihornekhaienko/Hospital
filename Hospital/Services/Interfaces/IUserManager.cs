using Hospital.Models.Identity;

namespace Hospital.Services.Interfaces
{
    public interface IUserManager
    {
        public void ChangePassword(User user, string newPassword);
        public User GetUser(int id);
        public User GetUser(string username);
        public string GetHashPassword(string password);
        public string RandomizeUsername();
        public string RandomizePassword();
        public void RestorePassword(string username);
        public void UpdateUserInfo(User user, int id);
        public bool Validate(string username, string password);
    }
}
