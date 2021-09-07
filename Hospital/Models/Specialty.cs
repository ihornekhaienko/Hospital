using Hospital.Models.Identity;
using System.Collections.Generic;

namespace Hospital.Models
{
    public class Specialty
    {
        public Specialty()
        {
            Doctors = new List<Doctor>();
        }
        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; }

        public virtual IEnumerable<Doctor> Doctors { get; set; }
    }
}
