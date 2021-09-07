using Hospital.Data;
using Hospital.Models;
using Hospital.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Hospital.Services.Implementations
{
    public class FacultyService : IFacultyService
    {
        readonly Db db;
        public FacultyService(Db db)
        {
            this.db = db;
        }

        public void AddFaculty(Faculty faculty)
        {
            db.Faculties.Add(faculty);
            db.SaveChanges();
        }

        public IEnumerable<Faculty> GetAllFaculties()
        {
            return db.Faculties.ToList();
        }

        public Faculty GetFaculty(string name)
        {
            return db.Faculties.SingleOrDefault(f => f.FacultyName == name);
        }

        public Faculty GetFaculty(int id)
        {
            return db.Faculties.SingleOrDefault(f => f.FacultyId == id);
        }

        public int GetGroupCount(Faculty faculty)
        {
            return db.Groups.Count(g => g.Faculty == faculty);
        }

        public int GetStudentCount(Faculty faculty)
        {
            return db.Students.Count(s => s.Group.Faculty == faculty);
        }

        public bool IsExist(string faculty)
        {
            return db.Faculties.Any(f => f.FacultyName == faculty);
        }

        public void RemoveFaculty(int id)
        {
            db.Faculties.Remove(GetFaculty(id));
            db.SaveChanges();
        }

        public void UpdateFaculty(Faculty faculty, int id)
        {
            var old = GetFaculty(id);
            old.FacultyName = faculty.FacultyName;
            db.SaveChanges();
        }
    }
}
