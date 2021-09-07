using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models.Identity
{
    public enum Sex
    {
        Male,
        Female
    }
    public class Student : User
    {
        public Student()
        {
            Records = new List<Record>();
        }
        public Sex Sex { get; set; }
        public DateTime BirthDate { get; set; }
        public virtual Address Address { get; set; }
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }
        public virtual IEnumerable<Record> Records { get; set; }
    }
}
