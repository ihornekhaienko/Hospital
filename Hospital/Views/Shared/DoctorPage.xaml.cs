using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using Hospital.ViewModels;
using Hospital.Views.Filters;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Hospital.Views.Shared
{
    /// <summary>
    /// Логика взаимодействия для DoctorPage.xaml
    /// </summary>
    public partial class DoctorPage : Page
    {
        readonly User currentUser;
        private DoctorAdminViewModel selected;
        private byte[] image;
        readonly IDoctorService doctorService;
        readonly IFileManager fileManager;
        readonly ISpecialtyService specialtyService;
        readonly IUserManager userManager;
        public DoctorPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            doctorService = App.doctorService;
            fileManager = App.fileManager;
            userManager = App.userManager;
            specialtyService = App.specialtyService;

            if (currentUser.Role != Role.Admin)
            {
                HideAdministration();
            }

            RefreshDataGrid();
        }
        private void AddUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LoginTB.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password)
                    || string.IsNullOrWhiteSpace(NameTB.Text) || string.IsNullOrWhiteSpace(SurnameTB.Text)
                    || string.IsNullOrWhiteSpace(EmailTB.Text) || string.IsNullOrWhiteSpace(PhoneTB.Text)
                    || string.IsNullOrWhiteSpace(SpecialtyTB.Text))
                {
                    throw new Exception("Fields should not be empty");
                }
                if (!Regex.IsMatch(PhoneTB.Text, @"((\+)?\b(8|38)?(0[\d]{2}))([\d-]{5,8})([\d]{2})"))
                {
                    throw new Exception("Incorrect phone number format");
                }

                if (!Regex.IsMatch(EmailTB.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
                {
                    throw new Exception("Incorrect email format");
                }

                if (userManager.GetUser(LoginTB.Text) != null)
                {
                    if (selected != null)
                    {
                        var doctor = doctorService.GetDoctor(selected.DoctorId);
                        if (doctor.UserName != LoginTB.Text)
                        {
                            throw new Exception("This login is unavailable");
                        }
                    }
                    else
                    {
                        throw new Exception("This login is unavailable");
                    }
                }

                if ((string)AddUpdateButton.Content == "Add")
                {
                    Add();
                }
                else
                {
                    Update();
                }

                RefreshDataGrid();
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selected == null)
                {
                    throw new Exception("First select the row");
                }

                if (doctorService.HasRecords(selected.DoctorId) || doctorService.HasSchedules(selected.DoctorId))
                {
                    MessageBoxResult result = MessageBox.Show("There are records or schedules associated with this doctors, are you sure you want to delete?", "Deleting doctor", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                doctorService.RemoveDoctor(selected.DoctorId);
                Clear();
                RefreshDataGrid();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            Window filter = new DoctorFilterWindow(RefreshDataGrid);
            filter.Show();
        }

        private void LoadPictureHL_Click(object sender, RoutedEventArgs e)
        {
            string path = fileManager.OpenImage();

            if (!string.IsNullOrWhiteSpace(path))
            {
                image = File.ReadAllBytes(path);
                UserImage.Source = fileManager.ByteToImageSource(image);
                UserImage.Tag = true;
            }
        }

        private void GoToScheduleHL_Click(object sender, RoutedEventArgs e)
        {
            if (selected == null)
            {
                MessageBox.Show("First select doctor");
                return;
            }

            if (!doctorService.HasSchedules(selected.DoctorId))
            {
                MessageBox.Show("There is no schedule for this doctor");
                return;
            }

            Window scheduleWindow;
            if (currentUser.Role == Role.Admin)
            {
                scheduleWindow = new AddScheduleWindow(doctorService.GetDoctor(selected.DoctorId));
            }
            else
            {
                scheduleWindow = new CreateAppointmentWindow(doctorService.GetDoctor(selected.DoctorId), currentUser as Student);
            }

            scheduleWindow.Show();
        }

        private void RandomUsernameButton_Click(object sender, RoutedEventArgs e)
        {
            LoginTB.Text = userManager.RandomizeUsername();
        }

        private void RandomPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = userManager.RandomizePassword();
        }

        private void NameKeyPress(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.A && e.Key <= Key.Z) || e.Key == Key.Back)
            {
                return;
            }
            e.Handled = true;
        }

        private void UserNameKeyPress(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.A && e.Key <= Key.Z) || (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back)
            {
                return;
            }
            e.Handled = true;
        }

        private void RowSelected(object sender, RoutedEventArgs e)
        {
            selected = (DoctorAdminViewModel)DoctorsDG.SelectedItem;
            if (selected != null)
            {
                var doctor = doctorService.GetDoctor(selected.DoctorId);
                AddUpdateButton.Content = "Update";

                image = doctor.Image;
                if (image != null)
                {
                    UserImage.Source = fileManager.ByteToImageSource(image);
                    UserImage.Tag = true;
                }
                else
                {
                    BitmapImage source = (BitmapImage)FindResource("UserIcon");
                    UserImage.Source = source;
                    UserImage.Tag = null;
                }

                LoginTB.Text = doctor.UserName;
                PasswordBox.Password = doctor.HashPassword;
                NameTB.Text = doctor.Name;
                SurnameTB.Text = doctor.Surname;
                EmailTB.Text = doctor.Email;
                PhoneTB.Text = doctor.Phone;
                SpecialtyTB.Text = doctor.Specialty.SpecialtyName;
            }
        }

        void Add()
        {
            Doctor doctor = new Doctor
            {
                Name = NameTB.Text,
                Surname = SurnameTB.Text,
                Email = EmailTB.Text,
                Phone = PhoneTB.Text,
                Specialty = specialtyService.AddSpecialty(SpecialtyTB.Text),
                UserName = LoginTB.Text,
                HashPassword = userManager.GetHashPassword(PasswordBox.Password),
                Role = Role.Doctor,
                Image = UserImage.Tag == null ? null : image
            };
            doctorService.AddDoctor(doctor, PasswordBox.Password);
            Clear();
            RefreshDataGrid();
        }

        void Update()
        {
            Doctor doctor = new Doctor
            {
                Name = NameTB.Text,
                Surname = SurnameTB.Text,
                Email = EmailTB.Text,
                Phone = PhoneTB.Text,
                Specialty = specialtyService.AddSpecialty(SpecialtyTB.Text),
                Image = UserImage.Tag == null ? null : image
            };
            doctorService.UpdateDoctor(doctor, selected.DoctorId);
            Clear();
        }

        public void Clear()
        {
            AddUpdateButton.Content = "Add";
            DoctorsDG.SelectedItem = null;

            image = null;
            BitmapImage source = (BitmapImage)FindResource("UserIcon");
            UserImage.Source = source;
            UserImage.Tag = null;

            LoginTB.Clear();
            PasswordBox.Clear();
            NameTB.Clear();
            SurnameTB.Clear();
            EmailTB.Clear();
            PhoneTB.Clear();
            SpecialtyTB.Clear();
            selected = null;

            RefreshDataGrid();
        }

        void RefreshDataGrid(object source = null)
        {
            DataContext = null;
            if (source == null)
            {
                DataContext = doctorService.GetAllDoctors().Select(d => new DoctorAdminViewModel
                {
                    DoctorId = d.UserId,
                    FullName = d.ToString(),
                    Email = d.Email,
                    Phone = d.Phone,
                    Specialty = d.Specialty.SpecialtyName
                });
            }
            else
            {
                DataContext = source;
            }
        }

        private void HideAdministration()
        {
            ImageTB.Visibility = Visibility.Hidden;
            AddUpdateButton.Visibility = Visibility.Hidden;
            ClearButton.Visibility = Visibility.Hidden;
            DeleteButton.Visibility = Visibility.Hidden;

            LoginLabel.Visibility = Visibility.Hidden;
            LoginTB.Visibility = Visibility.Hidden;
            RandomUsernameButton.Visibility = Visibility.Hidden;

            PasswordLabel.Visibility = Visibility.Hidden;
            PasswordBox.Visibility = Visibility.Hidden;
            RandomPasswordButton.Visibility = Visibility.Hidden;

            NameTB.IsReadOnly = true;
            SurnameTB.IsReadOnly = true;
            EmailTB.IsReadOnly = true;
            PhoneTB.IsReadOnly = true;
            SpecialtyTB.IsReadOnly = true;

            GoToScheduleTB.SetValue(Grid.RowProperty, 6);
            FilterButton.SetValue(Grid.RowProperty, 10);
            DoctorsDG.SetValue(Grid.RowProperty, 11);
            DoctorsDG.SetValue(Grid.RowSpanProperty, 2);

            if (currentUser.Role == Role.Doctor)
            {
                GoToScheduleTB.Visibility = Visibility.Hidden;
            }
        }
    }
}
