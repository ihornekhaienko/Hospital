using Hospital.Data;
using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Services.Implementations
{
    public class StudentService : IStudentService
    {
        readonly Db db;
        readonly IEmailService emailService;

        public StudentService(Db db)
        {
            this.db = db;
            emailService = App.emailService;
        }

        public void AddStudent(Student student, string password)
        {
            db.Students.Add(student);
            db.SaveChanges();
            emailService.NotifyAdd(student.Email, student.UserName, password);
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return db.Students
                    .Include(s => s.Address)
                    .Include(s => s.Group)
                        .ThenInclude(g => g.Faculty).ToList();
        }

        public Student GetStudent(int id)
        {
            return db.Students.FirstOrDefault(s => s.UserId == id);
        }

        public bool HasRecords(int id)
        {
            Student student = GetStudent(id);
            return db.Records.Any(r => r.Student == student);
        }

        public void RemoveRecords(Student student)
        {
            var records = db.Records.Where(r => r.Student == student);
            db.Records.RemoveRange(records);
            db.SaveChanges();
        }

        public void RemoveStudent(Student student)
        {
            RemoveRecords(student);
            db.Students.Remove(student);
            db.SaveChanges();
            emailService.NotifyDelete(student.Email, student.UserName);
        }

        public void RemoveStudent(int id)
        {
            Student student = GetStudent(id);
            RemoveStudent(student);
        }

        public Sex StringToSex(string sex)
        {
            if (sex == "Male")
            {
                return Sex.Male;
            }
            else
            {
                return Sex.Female;
            }
        }

        public void UpdateStudent(Student student, int id)
        {
            var old = GetStudent(id);
            old.Email = student.Email;
            old.Phone = student.Phone;
            old.Name = student.Name;
            old.Surname = student.Surname;
            old.Address = student.Address;
            old.BirthDate = student.BirthDate;
            old.Group = student.Group;
            old.Sex = student.Sex;
            old.Image = student.Image;
            db.SaveChanges();
            emailService.NotifyUpdate(old.Email, old.UserName);
        }
    }
}
