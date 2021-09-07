using Hospital.Models;
using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Hospital.Views
{
    /// <summary>
    /// Логика взаимодействия для CreateAppointmentWindow.xaml
    /// </summary>
    public partial class CreateAppointmentWindow : Window
    {
        readonly Doctor doctor;
        readonly Student student;
        readonly IDoctorService doctorService;
        readonly IRecordService recordService;
        readonly List<DayOfWeek> workingDays;
        private List<DateTime> boldedDays;
        public CreateAppointmentWindow(Doctor doctor, Student student)
        {
            InitializeComponent();

            this.doctor = doctor;
            this.student = student;
            recordService = App.recordService;
            doctorService = App.doctorService;

            workingDays = doctorService.GetSchedules(doctor).Select(s => s.DayOfWeek).ToList();
            boldedDays = GetBoldedDates(Calendar.DisplayDate.Year, Calendar.DisplayDate.Month);

            if (workingDays.Count() == 0)
            {
                MessageBox.Show("There is no schedule");
                this.Close();
            }

            Calendar.SelectedDate = DateTime.Today;
        }

        private void MakeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Calendar.SelectedDate <= DateTime.Today)
            {
                MessageBox.Show("Incorrect date");
                return;
            }
            if (TimeCB.SelectedItem == null)
            {
                MessageBox.Show("Choose the time of appointment");
                return;
            }

            Record record = new Record
            {
                Doctor = doctor,
                Student = student,
                RecordDate = Calendar.SelectedDate.Value.Date,
                RecordTime = TimeSpan.Parse(TimeCB.SelectedItem.ToString()),
                DayOfWeek = Calendar.SelectedDate.Value.DayOfWeek,
                State = State.Planned
            };

            if (recordService.GetAllRecords().Any(r => r.Doctor == doctor
                                                       && r.RecordDate.Date == record.RecordDate
                                                       && r.RecordTime == record.RecordTime))
            {
                MessageBox.Show("This time is not available");
                return;
            }

            recordService.AddRecord(record);
            MessageBox.Show($"Recording successful: {record.RecordTime:hh\\:mm}, {record.RecordDate.Date:MM\\.dd\\.yy}");
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                TimeCB.Items.Clear();
                DateTime selectedDate = Calendar.SelectedDate.Value;
                if (boldedDays.Contains(selectedDate.Date))
                {
                    DayOfWeek day = selectedDate.DayOfWeek;
                    Schedule schedule = doctorService.GetSchedules(doctor).SingleOrDefault(s => s.DayOfWeek == day);

                    TimeSpan step = schedule.StartTime;

                    while (step < schedule.EndTime)
                    {
                        if (!recordService.GetAllRecords().Any(r => r.Doctor == doctor 
                                                                    && r.RecordDate.Date == selectedDate 
                                                                    && r.RecordTime == step))
                        {
                            TimeCB.Items.Add(step.ToString(@"hh\:mm"));
                        }

                        step += TimeSpan.FromMinutes(20);
                    }
                    TimeCB.SelectedItem = null;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private List<DateTime> GetBoldedDates(int year, int month)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                                .Select(day => new DateTime(year, month, day))
                                    .Where(d => workingDays.Contains(d.DayOfWeek))
                                        .ToList();
        }

        private void HighlightDay(CalendarDayButton button, DateTime date)
        {
            if (boldedDays.Contains(date))
                button.Background = Brushes.LightBlue;
            else
                button.Background = Brushes.White;
        }

        private void CalendarButton_Loaded(object sender, EventArgs e)
        {
            CalendarDayButton button = (CalendarDayButton)sender;
            DateTime date = (DateTime)button.DataContext;
            HighlightDay(button, date);
            button.DataContextChanged += new DependencyPropertyChangedEventHandler(CalendarButton_DataContextChanged);
        }

        private void CalendarButton_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CalendarDayButton button = (CalendarDayButton)sender;
            DateTime date = (DateTime)button.DataContext;
            HighlightDay(button, date);
        }

        private void Calendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            boldedDays = GetBoldedDates(Calendar.DisplayDate.Year, Calendar.DisplayDate.Month);
        }
    }
}
