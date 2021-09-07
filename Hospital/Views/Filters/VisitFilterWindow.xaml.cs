using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using Hospital.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static Hospital.App;

namespace Hospital.Views.Filters
{
    /// <summary>
    /// Логика взаимодействия для VisitFilterWindow.xaml
    /// </summary>
    public partial class VisitFilterWindow : Window
    {
        readonly Refresh refresh;
        readonly IRecordService recordService;
        readonly Role role;
        public VisitFilterWindow(Role role, Refresh refresh)
        {
            InitializeComponent();

            this.refresh = refresh;
            recordService = App.recordService;
            this.role = role;

            Clear();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var records = recordService.GetAllRecords();

                if (StateCB.SelectedItem != null)
                {
                    records = records.Where(r => r.State.ToString() == StateCB.Text);
                }
                if (FromDP.SelectedDate.Value != DateTime.MinValue || ToDP.SelectedDate.Value != DateTime.MaxValue)
                {
                    records = records.Where(r => r.RecordDate >= FromDP.SelectedDate.Value.Date 
                                                 && r.RecordDate <= ToDP.SelectedDate.Value.Date);
                }

                if (records.Any())
                {
                    IEnumerable<VisitViewModel> source;
                    if (role == Role.Doctor)
                    {
                        source = records.Select(r => new VisitDoctorViewModel
                        {
                            RecordId = r.RecordId,
                            Student = r.Student.ToString(),
                            Faculty = r.Student.Group.Faculty.FacultyName,
                            Group = r.Student.Group.GroupName,
                            Course = r.Student.Group.Course,
                            Date = r.RecordDate,
                            Time = r.RecordTime,
                            State = r.State.ToString()
                        });
                    }
                    else
                    {
                        source = records.Select(r => new VisitStudentViewModel
                        {
                            RecordId = r.RecordId,
                            Doctor = r.Doctor.ToString(),
                            Specialty = r.Doctor.Specialty.SpecialtyName,
                            Date = r.RecordDate,
                            Time = r.RecordTime,
                            State = r.State.ToString(),
                            Diagnosis = string.IsNullOrEmpty(r.Diagnosis) ? "-" : r.Diagnosis,
                        });
                    }

                    refresh(source);
                }
                else
                {
                    throw new Exception("No records found for your search");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            StateCB.SelectedItem = null;
            FromDP.SelectedDate = DateTime.MinValue;
            ToDP.SelectedDate = DateTime.MaxValue;

            refresh(null);
        }
    }
}
