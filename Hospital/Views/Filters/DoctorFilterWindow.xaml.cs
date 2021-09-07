using Hospital.Services.Interfaces;
using Hospital.ViewModels;
using System;
using System.Linq;
using System.Windows;
using static Hospital.App;

namespace Hospital.Views.Filters
{
    /// <summary>
    /// Логика взаимодействия для DoctorFilterWindow.xaml
    /// </summary>
    public partial class DoctorFilterWindow : Window
    {
        readonly Refresh refresh;
        readonly IDoctorService doctorService;
        readonly ISpecialtyService specialtyService;
        public DoctorFilterWindow(Refresh refresh)
        {
            InitializeComponent();

            this.refresh = refresh;
            doctorService = App.doctorService;
            specialtyService = App.specialtyService;

            SpecialtyCB.ItemsSource = specialtyService.GetAllSpecialties()
                                                            .Select(s => s.SpecialtyName);
            Clear();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
           try
           {
                var doctors = doctorService.GetAllDoctors();

                if (!string.IsNullOrWhiteSpace(NameTB.Text))
                {
                    doctors = doctors.Where(d => d.Name == NameTB.Text);
                }
                if (!string.IsNullOrWhiteSpace(SurnameTB.Text))
                {
                    doctors = doctors.Where(d => d.Surname == SurnameTB.Text);
                }
                if (DayCB.SelectedItem != null)
                {
                    doctors = doctors.Where(d => d.Schedules.Any(s => s.DayOfWeek == (DayOfWeek)DayCB.SelectedIndex));
                }
                if (SpecialtyCB.SelectedItem != null)
                {
                    doctors = doctors.Where(d => d.Specialty.SpecialtyName == SpecialtyCB.Text);
                }

                if (FromTP.Value != DateTime.MinValue || ToTP.Value != DateTime.MaxValue)
                {
                    doctors = doctors.Where(d => d.Schedules.Any(s => s.StartTime >= FromTP.Value?.TimeOfDay 
                                                                      && s.EndTime >= ToTP.Value?.TimeOfDay));
                }
                if (doctors.Any())
                {
                    var source = doctors.Select(d => new DoctorAdminViewModel
                    {
                        DoctorId = d.UserId,
                        FullName = d.ToString(),
                        Email = d.Email,
                        Phone = d.Phone,
                        Specialty = d.Specialty.SpecialtyName
                    });
                    refresh(source);
                }
                else
                {
                    throw new Exception("No doctors found for your search");
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
            NameTB.Clear();
            SurnameTB.Clear();
            DayCB.SelectedItem = null;
            SpecialtyCB.SelectedItem = null;

            FromTP.Value = DateTime.MinValue;
            ToTP.Value = DateTime.MaxValue;

            refresh(null);
        }
    }
}
