using Hospital.Models.Identity;
using System;

namespace Hospital.ViewModels
{
    public class StatisticsViewModel
    {
        public int RecordId { get; set; }
        public string Doctor { get; set; }
        public string Student { get; set; }
        public string Diagnosis { get; set; }
        public DateTime BirthDate { get; set; }
        public Sex Sex { get; set; }
        public DateTime RecordDate { get; set; }
        public string Faculty { get; set; }
        public string Group { get; set; }
        public string Course { get; set; }
    }
}
