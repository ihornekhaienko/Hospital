using System;

namespace Hospital.ViewModels
{
    public class StudentAdminViewModel
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public string Faculty { get; set; }
        public string Group { get; set; }
        public string Course { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        public string Sex { get; set; }
    }
}
