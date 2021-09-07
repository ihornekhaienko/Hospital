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
    /// Логика взаимодействия для StudentFilterWindow.xaml
    /// </summary>
    public partial class StudentFilterWindow : Window
    {
        readonly Refresh refresh;
        readonly IStudentService studentService;
        readonly IFacultyService facultyService;
        readonly IGroupService groupService;
        public StudentFilterWindow(Refresh refresh)
        {
            InitializeComponent();

            this.refresh = refresh;
            studentService = App.studentService;
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
                var students = studentService.GetAllStudents();

                if (!string.IsNullOrWhiteSpace(NameTB.Text))
                {
                    students = students.Where(s => s.Name == NameTB.Text);
                }
                if (!string.IsNullOrWhiteSpace(SurnameTB.Text))
                {
                    students = students.Where(s => s.Surname == SurnameTB.Text);
                }
                if (SexCB.SelectedItem != null)
                {
                    students = students.Where(s => s.Sex.ToString() == SexCB.Text);
                }
                if (FromDP.SelectedDate.Value != DateTime.MinValue || ToDP.SelectedDate.Value != DateTime.MaxValue)
                {
                    students = students.Where(s => s.BirthDate >= FromDP.SelectedDate.Value.Date
                                                   && s.BirthDate <= ToDP.SelectedDate.Value.Date);
                }
                if (FacultyCB.SelectedItem != null)
                {
                    students = students.Where(s => s.Group.Faculty.FacultyName == FacultyCB.Text);
                }
                if (GroupCB.SelectedItem != null)
                {
                    students = students.Where(s => s.Group.GroupName == GroupCB.Text);
                }
                if (!string.IsNullOrWhiteSpace(CourseTB.Text))
                {
                    students = students.Where(s => s.Group.Course.ToString() == CourseTB.Text);
                }

                if (students.Any())
                {
                    var source = students.Select(s => new StudentAdminViewModel
                    {
                        StudentId = s.UserId,
                        FullName = s.ToString(),
                        Address = s.Address.ToString(),
                        Faculty = s.Group.Faculty.FacultyName,
                        Group = s.Group.GroupName,
                        Course = s.Group.Course.ToString(),
                        BirthDate = s.BirthDate,
                        Sex = s.Sex.ToString()
                    });
                    refresh(source);
                }
                else
                {
                    throw new Exception("No students found for your search");
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
            NameTB.Clear();
            SurnameTB.Clear();
            CourseTB.Clear();
            FacultyCB.SelectedItem = null;
            GroupCB.SelectedItem = null;
            GroupCB.ItemsSource = null;
            FromDP.SelectedDate = DateTime.MinValue;
            ToDP.SelectedDate = DateTime.MaxValue;

            refresh(null);
        }
    }
}
