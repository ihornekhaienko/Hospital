using Hospital.Models.Identity;
using System;

namespace Hospital.Models
{
    public enum State
    {
        Planned,
        Active,
        Completed,
        Canceled,
        Missed
    };
    public class Record
    {
        public int RecordId { get; set; }
        public string Diagnosis { get; set; }
        public string Appointment { get; set; }
        public string AdditionalInfo { get; set; }
        public DateTime RecordDate { get; set; }
        public TimeSpan RecordTime { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public State State { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual Student Student { get; set; }
    }
}
