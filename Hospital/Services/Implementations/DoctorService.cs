using Hospital.Data;
using Hospital.Models;
using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Hospital.Services.Implementations
{
    public class DoctorService : IDoctorService
    {
        readonly Db db;
        readonly IEmailService emailService;
        public DoctorService(Db db)
        {
            this.db = db;
            emailService = App.emailService;
        }
        public void AddDoctor(Doctor doctor, string password)
        {
            db.Doctors.Add(doctor);
            db.SaveChanges();
            emailService.NotifyAdd(doctor.Email, doctor.UserName, password);
        }

        public IEnumerable<Doctor> GetAllDoctors()
        {
            return db.Doctors.Include(d => d.Specialty).ToList();
        }

        public Doctor GetDoctor(int id)
        {
            return db.Doctors.FirstOrDefault(d => d.UserId == id);
        }

        public IEnumerable<Schedule> GetSchedules(Doctor doctor)
        {
            return db.Schedules.Where(s => s.Doctor == doctor).ToList();
        }

        public bool HasRecords(int id)
        {
            Doctor doctor = GetDoctor(id);
            return db.Records.Any(r => r.Doctor == doctor);
        }

        public bool HasSchedules(int id)
        {
            Doctor doctor = GetDoctor(id);
            return db.Schedules.Any(s => s.Doctor == doctor);
        }

        public void RemoveDoctor(Doctor doctor)
        {
            RemoveRecords(doctor);
            db.Doctors.Remove(doctor);
            db.SaveChanges();
            emailService.NotifyDelete(doctor.Email, doctor.UserName);
        }

        public void RemoveDoctor(int id)
        {
            Doctor doctor = GetDoctor(id);
            RemoveDoctor(doctor);
        }

        public void RemoveRecords(Doctor doctor)
        {
            var records = db.Records.Where(r => r.Doctor == doctor);
            db.Records.RemoveRange(records);
            db.SaveChanges();
        }

        public void UpdateDoctor(Doctor doctor, int id)
        {
            var old = GetDoctor(id);
            old.Email = doctor.Email;
            old.Phone = doctor.Phone;
            old.Name = doctor.Name;
            old.Surname = doctor.Surname;
            old.Specialty = doctor.Specialty;
            old.Image = doctor.Image;
            db.SaveChanges();
            emailService.NotifyUpdate(old.Email, old.UserName);
        }
    }
}
