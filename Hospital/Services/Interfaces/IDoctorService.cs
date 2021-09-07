using Hospital.Models;
using Hospital.Models.Identity;
using System.Collections.Generic;

namespace Hospital.Services.Interfaces
{
    interface IDoctorService
    {
        public void AddDoctor(Doctor doctor, string password);
        public IEnumerable<Doctor> GetAllDoctors();
        public Doctor GetDoctor(int id);
        public bool HasRecords(int id);
        public bool HasSchedules(int id);
        public void RemoveDoctor(Doctor doctor);
        public void RemoveDoctor(int id);
        public void RemoveRecords(Doctor doctor);
        public IEnumerable<Schedule> GetSchedules(Doctor doctor);
        public void UpdateDoctor(Doctor doctor, int id);
    }
}
