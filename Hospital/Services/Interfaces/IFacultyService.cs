using Hospital.Models;
using System.Collections.Generic;

namespace Hospital.Services.Interfaces
{
    interface IFacultyService
    {
        public void AddFaculty(Faculty faculty);
        public int GetGroupCount(Faculty faculty);
        public int GetStudentCount(Faculty faculty);
        public Faculty GetFaculty(int id);
        public Faculty GetFaculty(string name);
        public IEnumerable<Faculty> GetAllFaculties();
        public bool IsExist(string faculty);
        public void RemoveFaculty(int id);
        public void UpdateFaculty(Faculty faculty, int id);
    }
}
