using Hospital.Data;
using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Hospital.Services.Implementations
{
    public class AdminService : IAdminService
    {
        readonly Db db;
        readonly IEmailService emailService;
        public AdminService(Db db)
        {
            this.db = db;
            emailService = App.emailService;
        }
        public void AddAdmin(Admin admin, string password)
        {
            db.Admins.Add(admin);
            db.SaveChanges();
            emailService.NotifyAdd(admin.Email, admin.UserName, password);
        }

        public Admin GetAdmin(int id)
        {
            return db.Admins.SingleOrDefault(a => a.UserId == id);
        }

        public IEnumerable<Admin> GetAllAdmins()
        {
            return db.Admins.ToList();
        }

        public void RemoveAdmin(Admin admin)
        {
            db.Admins.Remove(admin);
            db.SaveChanges();
            emailService.NotifyDelete(admin.Email, admin.UserName);
        }

        public void RemoveAdmin(int id)
        {
            Admin admin = GetAdmin(id);
            RemoveAdmin(admin);
        }

        public void UpdateAdmin(Admin admin, int id)
        {
            var old = GetAdmin(id);
            old.Email = admin.Email;
            old.Phone = admin.Phone;
            old.Name = admin.Name;
            old.Surname = admin.Surname;
            old.Image = admin.Image;
            db.SaveChanges();
            emailService.NotifyUpdate(admin.Email, admin.UserName);
        }
    }
}
