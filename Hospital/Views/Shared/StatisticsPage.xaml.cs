using Hospital.Services.Interfaces;
using Hospital.ViewModels;
using Hospital.Views.Filters;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hospital.Views.Shared
{
    /// <summary>
    /// Логика взаимодействия для StatisticsPage.xaml
    /// </summary>
    public partial class StatisticsPage : Page
    {
        private StatisticsViewModel selected;
        readonly IRecordService recordService;
        public StatisticsPage()
        {
            InitializeComponent();
            recordService = App.recordService;

            RefreshDataGrid();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            Window filter = new RecordFilterWindow(RefreshDataGrid);
            filter.Show();
        }
        private void RowSelected(object sender, RoutedEventArgs e)
        {
            selected = (StatisticsViewModel)StatisticsDG.SelectedItem;
            if (selected != null)
            {
                StudentTB.Text = selected.Student;
                DoctorTB.Text = selected.Doctor;
                SexTB.Text = selected.Sex.ToString();
                BirthDP.SelectedDate = selected.BirthDate;
                RecordDP.SelectedDate = selected.RecordDate;
                DiagnosisTB.Text = selected.Diagnosis;
                FacultyTB.Text = selected.Faculty;
                GroupTB.Text = selected.Group;
                CourseTB.Text = selected.Course.ToString();
            }
        }
        void RefreshDataGrid(object source = null)
        {
            DataContext = null;
            if (source == null)
            {
                DataContext = recordService.GetCompletedRecords().Select(r => new StatisticsViewModel
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
            }
            else
            {
                DataContext = source;
            } 
        }
    }
}
