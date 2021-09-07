using Hospital.Models;
using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using Hospital.ViewModels;
using Hospital.Views.Filters;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Hospital.Views.DoctorUI
{
    /// <summary>
    /// Логика взаимодействия для VisitPage.xaml
    /// </summary>
    public partial class VisitPage : Page
    {
        readonly User currentUser;
        private VisitDoctorViewModel selected;
        readonly IFileManager fileManager;
        readonly IRecordService recordService;

        public VisitPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            recordService = App.recordService;
            fileManager = App.fileManager;

            GetVisitsForToday();
        }

        private void DiagnoseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selected == null)
                {
                    throw new Exception("First select the row");
                }
                if (selected.Date != DateTime.Today.Date)
                {
                    throw new Exception("You can only diagnose the appointments scheduled for today");
                }
                if (selected.State != State.Planned.ToString())
                {
                    throw new Exception("You can only diagnose planned appointments");
                }

                RecordWindow recordWindow = new RecordWindow(selected.RecordId);
                recordWindow.Show();
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show(err.Message);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selected == null)
                {
                    throw new Exception("First select the row");
                }

                if (selected.State != State.Planned.ToString())
                {
                    throw new Exception("You can cancel only planned visits");
                }

                recordService.SetState(selected.RecordId, State.Canceled);
                Clear();
                RefreshDataGrid();
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show(err.Message);
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            Window filter = new VisitFilterWindow(currentUser.Role, RefreshDataGrid);
            filter.Show();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selected == null)
                {
                    throw new Exception("First select the row");
                }
                if (selected.State != State.Completed.ToString())
                {
                    throw new Exception("You can print only completed visits");
                }

                Record record = recordService.GetRecord(selected.RecordId);
                string path = fileManager.SaveToPdf();
                fileManager.PrintConclusion(record, path);
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show(err.Message);
            }
        }

        private void RowSelected(object sender, RoutedEventArgs e)
        {
            selected = (VisitDoctorViewModel)VisitsDG.SelectedItem;
            if (selected != null)
            {
                StudentTB.Text = selected.Student;
                FacultyTB.Text = selected.Faculty;
                GroupTB.Text = selected.Group;
                CourseTB.Text = selected.Course.ToString();
                VisitDateDP.SelectedDate = selected.Date;
                VisitTimeTP.SetValue(DateTimePicker.ValueProperty, new DateTime(1, 1, 1) + selected.Time);
                StateTB.Text = selected.State;
            }
        }

        private void Clear()
        {
            VisitsDG.SelectedItem = null;

            selected = null;
            StudentTB.Text = string.Empty;
            FacultyTB.Text = string.Empty;
            GroupTB.Text = string.Empty;
            CourseTB.Text = string.Empty;
            VisitDateDP.SelectedDate = DateTime.Now;
            VisitTimeTP.SetValue(DateTimePicker.ValueProperty, DateTime.Now);
            StateTB.Text = string.Empty;

            RefreshDataGrid();
        }

        void RefreshDataGrid(object source = null)
        {
            recordService.RefreshRecords(currentUser as Doctor);
            DataContext = null;
            if (source == null)
            {
                DataContext = recordService.GetAllRecords().Select(r => new VisitDoctorViewModel
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
                DataContext = source;
            }
        }

        void GetVisitsForToday()
        {
            var source = recordService.GetAllRecords().Where(r => r.RecordDate.Date == DateTime.Today).Select(r => new VisitDoctorViewModel
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

            RefreshDataGrid(source);
        } 
    }
}
