using Hospital.Models;
using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Hospital.Views
{
    /// <summary>
    /// Логика взаимодействия для AddScheduleWindow.xaml
    /// </summary>
    public partial class AddScheduleWindow : Window
    {
        readonly Doctor doctor;
        readonly IRecordService recordService;
        readonly IScheduler scheduler;
        public AddScheduleWindow(Doctor doctor)
        {
            InitializeComponent();
            scheduler = App.scheduler;
            recordService = App.recordService;
            this.doctor = doctor;

            DoctorTB.Text = $"{doctor.Name} {doctor.Surname}";

            StartDTP.Minimum = DateTime.Parse("09:00:00");
            EndDTP.Minimum = DateTime.Parse("09:00:00");
            StartDTP.Maximum = DateTime.Parse("19:00:00");
            EndDTP.Maximum = DateTime.Parse("19:00:00");

            Clear();
        }

        private void AddUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DayOfWeekCB.SelectedItem == null)
                {
                    throw new Exception("Choose a day of the week");
                }

                if (StartDTP.Value >= EndDTP.Value)
                {
                    throw new Exception("Incorrect shift interval");
                }
                Schedule schedule = new Schedule
                {
                    DayOfWeek = scheduler.ParseStringToDayOfWeek(DayOfWeekCB.Text),
                    StartTime = StartDTP.Value.Value.TimeOfDay,
                    EndTime = EndDTP.Value.Value.TimeOfDay,
                    Doctor = doctor
                };

                if (scheduler.HasSchedule(schedule.Doctor, schedule.DayOfWeek))
                {
                    Update();
                }
                else
                {
                    Add();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Schedule schedule = new Schedule
            {
                DayOfWeek = scheduler.ParseStringToDayOfWeek(DayOfWeekCB.Text),
                StartTime = StartDTP.Value.Value.TimeOfDay,
                EndTime = EndDTP.Value.Value.TimeOfDay,
                Doctor = doctor
            };

            if (scheduler.HasPlannedAppointments(schedule))
            {
                MessageBoxResult result = MessageBox.Show("There are scheduled visits for this day, are you sure you want to delete the schedule?", "Deleting schedule", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
            recordService.RemoveRecord(schedule);
            scheduler.RemoveSchedule(schedule);
            DayOfWeekCB.SelectedItem = null;

            Clear();
        }

        private void DayOfWeekCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DayOfWeekCB.SelectedItem != null)
            {
                string day = (DayOfWeekCB.SelectedItem as TextBlock).Text;
                if (!string.IsNullOrWhiteSpace(day) && scheduler.HasSchedule(doctor, scheduler.ParseStringToDayOfWeek(day)))
                {
                    Schedule schedule = scheduler.GetSchedule(doctor, scheduler.ParseStringToDayOfWeek(day));

                    StartDTP.Value = DateTime.Today.Date + schedule.StartTime;
                    EndDTP.Value = DateTime.Today.Date + schedule.EndTime;

                    AddUpdateButton.Content = "Update";
                }
                else
                {
                    Clear();
                }
            }
        }

        private void Add()
        {
            Schedule schedule = new Schedule
            {
                DayOfWeek = scheduler.ParseStringToDayOfWeek(DayOfWeekCB.Text),
                StartTime = StartDTP.Value.Value.TimeOfDay,
                EndTime = EndDTP.Value.Value.TimeOfDay,
                Doctor = doctor
            };

            scheduler.AddSchedule(schedule);
            Clear();
            DayOfWeekCB.SelectedItem = null;
        }

        private void Update()
        {
            Schedule schedule = new Schedule
            {
                DayOfWeek = scheduler.ParseStringToDayOfWeek(DayOfWeekCB.Text),
                StartTime = StartDTP.Value.Value.TimeOfDay,
                EndTime = EndDTP.Value.Value.TimeOfDay,
                Doctor = doctor
            };

            scheduler.UpdateSchedule(schedule);
        }
        private void Clear()
        {
            AddUpdateButton.Content = "Add";

            StartDTP.Value = StartDTP.Minimum;
            EndDTP.Value = EndDTP.Maximum;
        }
    }
}
