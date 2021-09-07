using Hospital.Models.Identity;
using System.Collections.Generic;

namespace Hospital.Services.Interfaces
{
    interface IStudentService
    {
        public void AddStudent(Student student, string password);
        public Student GetStudent(int id);
        public IEnumerable<Student> GetAllStudents();
        public bool HasRecords(int id);
        public void RemoveStudent(Student student);
        public void RemoveStudent(int id);
        public void RemoveRecords(Student student);
        public Sex StringToSex(string sex);
        public void UpdateStudent(Student student, int id);
    }
}
