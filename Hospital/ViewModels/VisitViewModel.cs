using System;

namespace Hospital.ViewModels
{
    public class VisitViewModel
    {
        public int RecordId { get; set; }
        public string State { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }
}
