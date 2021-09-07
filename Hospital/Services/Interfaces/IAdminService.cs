using Hospital.Models.Identity;
using System.Collections.Generic;

namespace Hospital.Services.Interfaces
{
    interface IAdminService
    {
        public void AddAdmin(Admin admin, string password);
        public Admin GetAdmin(int id);
        public IEnumerable<Admin> GetAllAdmins();
        public void RemoveAdmin(Admin admin);
        public void RemoveAdmin(int id);
        public void UpdateAdmin(Admin admin, int id);
    }
}
