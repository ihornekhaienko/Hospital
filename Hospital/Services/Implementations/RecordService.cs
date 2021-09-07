using Hospital.Data;
using Hospital.Models;
using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hospital.Services.Implementations
{
    public class RecordService : IRecordService
    {
        readonly Db db;

        public RecordService(Db db)
        {
            this.db = db;
        }

        public void AddRecord(Record record)
        {
            db.Records.Add(record);
            db.SaveChanges();
        }

        public IEnumerable<Record> GetAllRecords()
        {
            return db.Records
                    .Include(r => r.Student)
                        .ThenInclude(s => s.Group)
                            .ThenInclude(g => g.Faculty)
                    .Include(r => r.Doctor)
                        .ThenInclude(d => d.Specialty).ToList();
        }

        public IEnumerable<Record> GetCompletedRecords()
        {
            return GetAllRecords().Where(r => r.State == State.Completed);
        }

        public Record GetRecord(int id)
        {
            return db.Records.SingleOrDefault(r => r.RecordId == id);
        }

        public void RefreshRecords(Doctor doctor)
        {
            foreach (var record in db.Records.Where(r => r.Doctor == doctor))
            {
                if (record.RecordDate.Date < DateTime.Today && record.State == State.Planned)
                {
                    record.State = State.Missed;
                }
            }
            db.SaveChanges();
        }

        public void RefreshRecords(Student student)
        {
            foreach (var record in db.Records.Where(r => r.Student == student))
            {
                if (record.RecordDate.Date < DateTime.Today && record.State == State.Planned)
                {
                    record.State = State.Missed;
                }
            }
            db.SaveChanges();
        }

        public void RemoveRecord(Schedule schedule)
        {
            db.Records.RemoveRange(db.Records.Where(r => r.DayOfWeek == schedule.DayOfWeek 
                                                         && (r.State == State.Planned || r.State == State.Active) 
                                                         && r.Doctor == schedule.Doctor));
            db.SaveChanges();
        }

        public void SetState(Record record, State state)
        {
            record.State = state;
            db.SaveChanges();
        }

        public void SetState(int id, State state)
        {
            Record record = GetRecord(id);
            record.State = state;
            db.SaveChanges();
        }

        public void UpdateRecord(Record record, int id)
        {
            Record old = GetRecord(id);
            old.Diagnosis = record.Diagnosis;
            old.Appointment = record.Appointment;
            old.AdditionalInfo = record.AdditionalInfo;
            old.State = State.Completed;

            db.SaveChanges();
        }
    }
}
