using Hospital.Models.Identity;
using System.Collections.Generic;

namespace Hospital.Models
{
    public class Group
    {
        public Group()
        {
            Students = new List<Student>();
        }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public uint Course { get; set; }

        public int FacultyId { get; set; }
        public virtual Faculty Faculty { get; set; }
        public virtual IEnumerable<Student> Students { get; set; }
    }
}
