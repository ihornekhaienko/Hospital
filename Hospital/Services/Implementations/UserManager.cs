using Hospital.Data;
using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Hospital.Services.Implementations
{
    public class UserManager : IUserManager
    {
        readonly Db db;
        readonly IEmailService emailService;
        readonly string symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public UserManager(Db db)
        {
            this.db = db;
            emailService = App.emailService;
        }
        public void ChangePassword(User user, string newPassword)
        {
            user.HashPassword = GetHashPassword(newPassword);
            db.SaveChanges();
            emailService.NotifyChangePassword(user.Email, user.UserName, newPassword);
        }
        public string GetHashPassword(string password)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(password);

            MD5CryptoServiceProvider CSP = new MD5CryptoServiceProvider();

            byte[] byteHash = CSP.ComputeHash(bytes);

            string hash = string.Empty;

            foreach (byte b in byteHash)
                hash += string.Format("{0:x2}", b);

            return hash;
        }

        public User GetUser(int id)
        {
            return db.Users.FirstOrDefault(user => user.UserId == id);
        }

        public User GetUser(string username)
        {
            return db.Users.FirstOrDefault(user => user.UserName == username);
        }

        public Role GetUserRole(string username)
        {
            User user = GetUser(username);
            return user.Role;
        }

        public string RandomizePassword()
        {
            string password = string.Empty;

            for (int i = 0; i < App.random.Next(5, 15); i++)
            {
                password += symbols[App.random.Next(symbols.Length)];
            }

            return password;
        }

        public string RandomizeUsername()
        {
            string username;

            while (true)
            {
                username = string.Empty;
                for (int i = 0; i < App.random.Next(5, 15); i++)
                {
                    username += symbols[App.random.Next(symbols.Length)];
                }
                if (!db.Users.Any(u => u.UserName == username))
                {
                    break;
                }
            }

            return username;
        }

        public void RestorePassword(string username)
        {
            User user = GetUser(username);
            string newPassword = RandomizePassword();
            user.HashPassword = GetHashPassword(newPassword);
            db.SaveChanges();
            emailService.NotifyChangePassword(user.Email, username, newPassword);
        }

        public void UpdateUserInfo(User user, int id)
        {
            User old = GetUser(id);
            old.UserName = user.UserName;
            old.Email = user.Email;
            old.Phone = user.Phone;
            old.Image = user.Image;
            db.SaveChanges();
        }

        public bool Validate(string username, string password)
        {
            User user = GetUser(username);

            if (user == null || user.HashPassword != GetHashPassword(password))
            {
                return false;
            }
            return true;
        }
    }
}
