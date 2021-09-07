using Hospital.Data;
using Hospital.Models;
using Hospital.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Hospital.Services.Implementations
{
    public class GroupService : IGroupService
    {
        readonly Db db;
        public GroupService(Db db)
        {
            this.db = db;
        }
        public void AddGroup(Group group)
        {
            db.Groups.Add(group);
            db.SaveChanges();
        }

        public IEnumerable<Group> GetAllGroups()
        {
            return db.Groups.Include(g => g.Faculty).ToList();
        }

        public Group GetGroup(int id)
        {
            return db.Groups.SingleOrDefault(g => g.GroupId == id);
        }

        public Group GetGroup(string facultyName, string groupName)
        {
            return db.Groups.SingleOrDefault(g => g.GroupName == groupName 
                                                  && g.Faculty.FacultyName == facultyName);
        }

        public int GetStudentCount(Group group)
        {
            return db.Students.Count(s => s.Group == group);
        }

        public bool IsExist(string faculty, string group)
        {
            return db.Groups.Any(g => g.Faculty.FacultyName == faculty 
                                      && g.GroupName == group);
        }

        public void RemoveGroup(int id)
        {
            db.Groups.Remove(GetGroup(id));
            db.SaveChanges();
        }

        public void UpdateGroup(Group group, int id)
        {
            var old = GetGroup(id);
            old.GroupName = group.GroupName;
            old.Faculty = group.Faculty;
            db.SaveChanges();
        }
    }
}
