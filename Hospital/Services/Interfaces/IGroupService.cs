using Hospital.Models;
using System.Collections.Generic;

namespace Hospital.Services.Interfaces
{
    interface IGroupService
    {
        public void AddGroup(Group group);
        public int GetStudentCount(Group group);
        public Group GetGroup(int id);
        public Group GetGroup(string facultyName, string groupName);
        public IEnumerable<Group> GetAllGroups();
        public bool IsExist(string faculty, string group);
        public void RemoveGroup(int id);
        public void UpdateGroup(Group group, int id);
    }
}
