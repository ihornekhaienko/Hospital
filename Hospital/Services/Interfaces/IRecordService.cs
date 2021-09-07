using Hospital.Models;
using Hospital.Models.Identity;
using System.Collections.Generic;

namespace Hospital.Services.Interfaces
{
    interface IRecordService
    {
        public void AddRecord(Record record);
        public IEnumerable<Record> GetAllRecords();
        public IEnumerable<Record> GetCompletedRecords();
        public Record GetRecord(int id);
        public void RefreshRecords(Doctor doctor);
        public void RefreshRecords(Student student);
        public void RemoveRecord(Schedule schedule);
        public void SetState(Record record, State state);
        public void SetState(int id, State state);
        public void UpdateRecord(Record record, int id);
    }
}
