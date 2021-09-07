using Hospital.Data;
using Hospital.Models;
using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Hospital.Services.Implementations
{
    public class SpecialtyService : ISpecialtyService
    {
        readonly Db db;

        public SpecialtyService(Db db)
        {
            this.db = db;
        }
        public Specialty AddSpecialty(string name)
        {
            Specialty specialty;
            if (IsExist(name))
            {
                specialty = GetSpecialty(name);
            }
            else
            {
                specialty = new Specialty { SpecialtyName = name };

                db.Specialties.Add(specialty);
                db.SaveChanges();
            }

            return specialty;
        }

        public void AddSpecialty(Specialty specialty)
        {
            db.Specialties.Add(specialty);
            db.SaveChanges();
        }

        public IEnumerable<Specialty> GetAllSpecialties()
        {
            return db.Specialties.ToList();
        }

        public IEnumerable<Doctor> GetDoctors(Specialty specialty)
        {
            return db.Doctors.Where(d => d.Specialty == specialty).ToList();
        }

        public Specialty GetSpecialty(int id)
        {
            return db.Specialties.SingleOrDefault(s => s.SpecialtyId == id);
        }

        public Specialty GetSpecialty(string name)
        {
            return db.Specialties.SingleOrDefault(s => s.SpecialtyName == name);
        }

        public bool IsExist(string specialty)
        {
            return db.Specialties.Any(s => s.SpecialtyName == specialty);
        }
    }
}
