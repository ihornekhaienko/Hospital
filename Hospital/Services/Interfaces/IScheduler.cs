using Hospital.Models;
using Hospital.Models.Identity;
using System;

namespace Hospital.Services.Interfaces
{
    interface IScheduler
    {
        public void AddSchedule(Schedule schedule);
        public Schedule GetSchedule(Doctor doctor, DayOfWeek dayOfWeek);
        public bool HasSchedule(Doctor doctor, DayOfWeek dayOfWeek);
        public bool HasPlannedAppointments(Schedule schedule);
        public void RemoveSchedule(Schedule schedule);
        public DayOfWeek ParseStringToDayOfWeek(string day);
        public void UpdateSchedule(Schedule schedule);

    }
}
