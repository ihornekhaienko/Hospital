using Hospital.Models.Identity;
using System.Collections.Generic;

namespace Hospital.Models
{
    public class Address
    {
        public Address()
        {
            Students = new List<Student>();
        }
        public int AddressId { get; set; }
        public string Region { get; set; }
        public string Locality { get; set; }
        public string Street { get; set; }

        public virtual IEnumerable<Student> Students { get; set; }
        
        public override string ToString()
        {
            return $"{Street}, {Locality}, {Region}";
        }
    }
}
