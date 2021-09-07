using System.Collections.Generic;

namespace Hospital.Models
{
    public class Faculty
    {
        public Faculty()
        {
            Groups = new List<Group>();
        }
        public int FacultyId { get; set; }
        public string FacultyName { get; set; }

        public virtual IEnumerable<Group> Groups { get; set; }
    }
}
