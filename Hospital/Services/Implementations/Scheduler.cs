using Hospital.Data;
using Hospital.Models;
using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using System;
using System.Linq;

namespace Hospital.Services.Implementations
{
    public class Scheduler : IScheduler
    {
        readonly Db db;

        public Scheduler(Db db)
        {
            this.db = db;
        }
        public void AddSchedule(Schedule schedule)
        {
            db.Schedules.Add(schedule);
            db.SaveChanges();
        }

        public Schedule GetSchedule(Doctor doctor, DayOfWeek dayOfWeek)
        {
            return db.Schedules.SingleOrDefault(s => s.Doctor == doctor 
                                                     && s.DayOfWeek == dayOfWeek);
        }

        public bool HasPlannedAppointments(Schedule schedule)
        {
            return db.Records.Any(r => r.DayOfWeek == schedule.DayOfWeek 
                                       && r.Doctor == schedule.Doctor 
                                       && r.RecordTime >= schedule.StartTime 
                                       && r.RecordTime <= schedule.EndTime 
                                       && (r.State == State.Planned || r.State == State.Active));
        }

        public bool HasSchedule(Doctor doctor, DayOfWeek dayOfWeek)
        {
            return db.Schedules.Any(s => s.Doctor == doctor 
                                         && s.DayOfWeek == dayOfWeek);
        }

        public DayOfWeek ParseStringToDayOfWeek(string day)
        {
            return day switch
            {
                "Monday" => DayOfWeek.Monday,
                "Tuesday" => DayOfWeek.Tuesday,
                "Wednesday" => DayOfWeek.Wednesday,
                "Thursday" => DayOfWeek.Thursday,
                "Friday" => DayOfWeek.Friday,
                "Saturday" => DayOfWeek.Saturday,
                "Sunday" => DayOfWeek.Sunday,
                _ => throw new Exception($"Incorrect title of day: {day}"),
            };
        }

        public void RemoveSchedule(Schedule schedule)
        {
            Schedule temp = db.Schedules.FirstOrDefault(s => s.Doctor == schedule.Doctor 
                                                                      && s.DayOfWeek == schedule.DayOfWeek);
            db.Schedules.Remove(temp);
            db.SaveChanges();
        }

        public void UpdateSchedule(Schedule schedule)
        {
            Schedule old = db.Schedules.FirstOrDefault(s => s.Doctor == schedule.Doctor 
                                                            && s.DayOfWeek == schedule.DayOfWeek);
            old.StartTime = schedule.StartTime;
            old.EndTime = schedule.EndTime;
            db.SaveChanges();
        }
    }
}
