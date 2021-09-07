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
    /// Логика взаимодействия для StudentPage.xaml
    /// </summary>
    public partial class StudentPage : Page
    {
        readonly User currentUser;
        private StudentAdminViewModel selected;
        private byte[] image;
        readonly IAddressService addressService;
        readonly IStudentService studentService;
        readonly IFileManager fileManager;
        readonly IUserManager userManager;
        readonly IGroupService groupService;
        readonly IFacultyService facultyService;
        public StudentPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            studentService = App.studentService;
            fileManager = App.fileManager;
            userManager = App.userManager;
            addressService = App.addressService;
            groupService = App.groupService;
            facultyService = App.facultyService;

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
                    || string.IsNullOrWhiteSpace(RegionTB.Text) || string.IsNullOrWhiteSpace(LocalityTB.Text) 
                    || string.IsNullOrWhiteSpace(AddressTB.Text) || FacultyCB.SelectedItem == null 
                    || GroupCB.SelectedItem == null || SexCB.SelectedItem == null)
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

                if (!BirthDateDP.SelectedDate.HasValue)
                {
                    throw new Exception("You didn't select birth date");
                }
                else
                {
                    if (((DateTime)BirthDateDP.SelectedDate).Year >= DateTime.Now.Year - 14)
                    {
                        throw new Exception("Incorrect birth date");
                    }
                }


                if (userManager.GetUser(LoginTB.Text) != null)
                {
                    if (selected != null)
                    {
                        var student = studentService.GetStudent(selected.StudentId);
                        if (student.UserName != LoginTB.Text)
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

                if (studentService.HasRecords(selected.StudentId))
                {
                    MessageBoxResult result = MessageBox.Show("There are records associated with this student, are you sure you want to delete?", "Deleting student", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                studentService.RemoveStudent(selected.StudentId);
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
            Window filter = new StudentFilterWindow(RefreshDataGrid);
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
            selected = (StudentAdminViewModel)StudentsDG.SelectedItem;
            if (selected != null)
            {
                var student = studentService.GetStudent(selected.StudentId);
                AddUpdateButton.Content = "Update";

                image = student.Image;
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

                LoginTB.Text = student.UserName;
                PasswordBox.Password = student.HashPassword;
                NameTB.Text = student.Name;
                SurnameTB.Text = student.Surname;
                EmailTB.Text = student.Email;
                PhoneTB.Text = student.Phone;
                FacultyCB.SelectedIndex = FacultyCB.Items.IndexOf(student.Group.Faculty.FacultyName);
                GroupCB.SelectedIndex = GroupCB.Items.IndexOf(student.Group.GroupName);
                SexCB.SelectedItem = student.Sex == Sex.Male ? SexCB.Items[0] : SexCB.Items[1];
                BirthDateDP.SelectedDate = selected.BirthDate;
                RegionTB.Text = student.Address.Region;
                LocalityTB.Text = student.Address.Locality;
                AddressTB.Text = student.Address.Street;
            }         
        }

        private void FacultyCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string faculty = (string)FacultyCB.SelectedItem;
            GroupCB.ItemsSource = groupService.GetAllGroups()
                                                    .Where(g => g.Faculty.FacultyName == faculty)
                                                        .Select(g => g.GroupName);
        }

        void Add()
        {
            string sex = (SexCB.SelectedItem as TextBlock).Text;
            string facultyName = FacultyCB.SelectedItem as string;
            string groupName = GroupCB.SelectedItem as string;
            var group = groupService.GetGroup(facultyName, groupName);
            Student student = new Student
            {
                Name = NameTB.Text,
                Surname = SurnameTB.Text,
                Email = EmailTB.Text,
                Phone = PhoneTB.Text,
                UserName = LoginTB.Text,
                HashPassword = userManager.GetHashPassword(PasswordBox.Password),
                Role = Role.Student,
                Sex = studentService.StringToSex(sex),
                BirthDate = (DateTime)BirthDateDP.SelectedDate,
                Address = addressService.AddAddress(AddressTB.Text, LocalityTB.Text, RegionTB.Text),
                Group = group,
                Image = UserImage.Tag == null ? null : image
            };
            studentService.AddStudent(student, PasswordBox.Password);
            Clear();
            RefreshDataGrid();
        }

        void Update()
        {
            string sex = (SexCB.SelectedItem as TextBlock).Text;
            string facultyName = FacultyCB.SelectedItem as string;
            string groupName = GroupCB.SelectedItem as string;
            var group = groupService.GetGroup(facultyName, groupName);
            Student student = new Student
            {
                Name = NameTB.Text,
                Surname = SurnameTB.Text,
                Email = EmailTB.Text,
                Phone = PhoneTB.Text,
                Sex = studentService.StringToSex(sex),
                BirthDate = (DateTime)BirthDateDP.SelectedDate,
                Address = addressService.AddAddress(AddressTB.Text, LocalityTB.Text, RegionTB.Text),
                Group = group,
                Image = UserImage.Tag == null ? null : image
            };
            studentService.UpdateStudent(student, selected.StudentId);
            Clear();
        }

        public void Clear()
        {
            AddUpdateButton.Content = "Add";
            StudentsDG.SelectedItem = null;

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
            RegionTB.Clear();
            LocalityTB.Clear();
            AddressTB.Clear();
            BirthDateDP.SelectedDate = DateTime.Now;
            SexCB.SelectedItem = null;
            FacultyCB.SelectedItem = null;
            GroupCB.SelectedItem = null;
            GroupCB.ItemsSource = null;
            selected = null;

            RefreshDataGrid();
        }

        void RefreshDataGrid(object source = null)
        {
            DataContext = null;
            if (source == null)
            {
                DataContext = studentService.GetAllStudents().Select(s => new StudentAdminViewModel
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

                FacultyCB.ItemsSource = facultyService.GetAllFaculties().Select(f => f.FacultyName);
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

            NameTB.IsEnabled = false;
            SurnameTB.IsEnabled = false;
            EmailTB.IsEnabled = false;
            PhoneTB.IsEnabled = false;
            BirthDateDP.IsEnabled = false;
            SexCB.IsEnabled = false;
            RegionTB.IsEnabled = false;
            LocalityTB.IsEnabled = false;
            AddressTB.IsEnabled = false;
            FacultyCB.IsEnabled = false;
            GroupCB.IsEnabled = false;
        }
    }
}
