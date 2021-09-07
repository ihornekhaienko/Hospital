using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using Hospital.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static Hospital.App;

namespace Hospital.Views.Filters
{
    /// <summary>
    /// Логика взаимодействия для RecordFilterWindow.xaml
    /// </summary>
    public partial class RecordFilterWindow : Window
    {
        readonly Refresh refresh;
        readonly IRecordService recordService;
        readonly IFacultyService facultyService;
        readonly IGroupService groupService;
        public RecordFilterWindow(Refresh refresh)
        {
            InitializeComponent();

            this.refresh = refresh;
            recordService = App.recordService;
            facultyService = App.facultyService;
            groupService = App.groupService;

            FacultyCB.ItemsSource = facultyService.GetAllFaculties()
                                                        .Select(f => f.FacultyName);
            Clear();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var records = recordService.GetCompletedRecords();

                if (!string.IsNullOrWhiteSpace(StudentNameTB.Text))
                {
                    records = records.Where(r => r.Student.Name == StudentNameTB.Text);
                }
                if (!string.IsNullOrWhiteSpace(StudentSurnameTB.Text))
                {
                    records = records.Where(r => r.Student.Surname == StudentSurnameTB.Text);
                }
                if (SexCB.SelectedItem != null)
                {
                    records = records.Where(r => r.Student.Sex == (Sex)SexCB.SelectedIndex);
                }
                if (BirthFromDP.SelectedDate.Value != DateTime.MinValue || BirthToDP.SelectedDate.Value != DateTime.MaxValue)
                {
                    records = records.Where(r => r.Student.BirthDate >= BirthFromDP.SelectedDate.Value.Date
                                                 && r.Student.BirthDate <= BirthToDP.SelectedDate.Value.Date);
                }

                if (FacultyCB.SelectedItem != null)
                {
                    records = records.Where(r => r.Student.Group.Faculty.FacultyName == FacultyCB.Text);
                }
                if (GroupCB.SelectedItem != null)
                {
                    records = records.Where(r => r.Student.Group.GroupName == GroupCB.Text);
                }
                if (!string.IsNullOrWhiteSpace(CourseTB.Text))
                {
                    records = records.Where(r => r.Student.Group.Course.ToString() == CourseTB.Text);
                }
                if (!string.IsNullOrWhiteSpace(DoctorNameTB.Text))
                {
                    records = records.Where(r => r.Doctor.Name == DoctorNameTB.Text);
                }
                if (!string.IsNullOrWhiteSpace(DoctorSurnameTB.Text))
                {
                    records = records.Where(r => r.Doctor.Surname == DoctorSurnameTB.Text);
                }
                if (!string.IsNullOrWhiteSpace(DiagnosisTB.Text))
                {
                    records = records.Where(r => r.Diagnosis == DiagnosisTB.Text);
                }
                if (RecordFromDP.SelectedDate.Value != DateTime.MinValue || RecordToDP.SelectedDate.Value != DateTime.MaxValue)
                {
                    records = records.Where(r => r.RecordDate >= RecordFromDP.SelectedDate.Value.Date 
                                                 && r.RecordDate <= RecordToDP.SelectedDate.Value.Date);
                }

                if (records.Any())
                {
                    var source = records.Select(r => new StatisticsViewModel
                    {
                        RecordId = r.RecordId,
                        Student = r.Student.ToString(),
                        Doctor = r.Doctor.ToString(),
                        Sex = r.Student.Sex,
                        BirthDate = r.Student.BirthDate,
                        RecordDate = r.RecordDate,
                        Diagnosis = r.Diagnosis,
                        Faculty = r.Student.Group.Faculty.FacultyName,
                        Group = r.Student.Group.GroupName,
                        Course = r.Student.Group.Course.ToString()
                    });
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

        private void FacultyCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GroupCB.ItemsSource = groupService.GetAllGroups()
                                                    .Where(g => g.Faculty.FacultyName == (string)FacultyCB.SelectedItem)
                                                        .Select(g => g.GroupName);
        }

        private void Clear()
        {
            StudentNameTB.Clear();
            StudentSurnameTB.Clear();
            DoctorNameTB.Clear();
            DoctorSurnameTB.Clear();
            CourseTB.Clear();
            DiagnosisTB.Clear();
            FacultyCB.SelectedItem = null;
            GroupCB.SelectedItem = null;
            GroupCB.ItemsSource = null;
            BirthFromDP.SelectedDate = DateTime.MinValue;
            BirthToDP.SelectedDate = DateTime.MaxValue;
            RecordFromDP.SelectedDate = DateTime.MinValue;
            RecordToDP.SelectedDate = DateTime.MaxValue;

            refresh(null);
        }
    }
}
