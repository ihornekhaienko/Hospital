using Hospital.Models;
using Hospital.Models.Identity;
using System.Collections.Generic;

namespace Hospital.Services.Interfaces
{
    interface ISpecialtyService
    {
        public void AddSpecialty(Specialty specialty);
        public Specialty AddSpecialty(string specialty);
        public IEnumerable<Specialty> GetAllSpecialties();
        public Specialty GetSpecialty(int id);
        public Specialty GetSpecialty(string name);
        public bool IsExist(string specialty);
        public IEnumerable<Doctor> GetDoctors(Specialty specialty);
    }
}
