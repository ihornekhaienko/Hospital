using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models.Identity
{
    public class Doctor : User
    {
        public Doctor()
        {
            Records = new List<Record>();
            Schedules = new List<Schedule>();
        }
        public int SpecialtyId { get; set; }
        public virtual Specialty Specialty { get; set; }
        public virtual IEnumerable<Record> Records { get; set; }
        public virtual IEnumerable<Schedule> Schedules { get; set; }
    }
}
