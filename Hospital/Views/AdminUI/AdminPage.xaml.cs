using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using Hospital.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Hospital.Views.AdminUI
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        readonly User currentUser;
        private AdminAdminViewModel selected;
        private byte[] image;
        readonly IAdminService adminService;
        readonly IFileManager fileManager;
        readonly IUserManager userManager;
        public AdminPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            adminService = App.adminService;
            fileManager = App.fileManager;
            userManager = App.userManager;

            RefreshDataGrid();
        }
        private void AddUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LoginTB.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password)
                    || string.IsNullOrWhiteSpace(NameTB.Text) || string.IsNullOrWhiteSpace(SurnameTB.Text)
                    || string.IsNullOrWhiteSpace(EmailTB.Text) || string.IsNullOrWhiteSpace(PhoneTB.Text))
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
                        var admin = adminService.GetAdmin(selected.AdminId);
                        if (admin.UserName != LoginTB.Text)
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

                adminService.RemoveAdmin(selected.AdminId);
                Clear();
                RefreshDataGrid();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
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

        private void UserNameKeyPress(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.A && e.Key <= Key.Z) || (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back)
            {
                return;
            }
            e.Handled = true;
        }

        private void NameKeyPress(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.A && e.Key <= Key.Z) || e.Key == Key.Back)
            {
                return;
            }
            e.Handled = true;
        }

        private void RowSelected(object sender, RoutedEventArgs e)
        {
            selected = (AdminAdminViewModel)AdminsDG.SelectedItem;
            if (selected != null)
            {
                var admin = adminService.GetAdmin(selected.AdminId);
                AddUpdateButton.Content = "Update";

                image = admin.Image;
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

                LoginTB.Text = admin.UserName;
                PasswordBox.Password = admin.HashPassword;
                NameTB.Text = admin.Name;
                SurnameTB.Text = admin.Surname;
                EmailTB.Text = admin.Email;
                PhoneTB.Text = admin.Phone;
            }
        }

        void Add()
        {
            Admin admin = new Admin
            {
                Name = NameTB.Text,
                Surname = SurnameTB.Text,
                Email = EmailTB.Text,
                Phone = PhoneTB.Text,
                UserName = LoginTB.Text,
                HashPassword = userManager.GetHashPassword(PasswordBox.Password),
                Role = Role.Admin,
                Image = UserImage.Tag == null ? null : image
            };
            adminService.AddAdmin(admin, PasswordBox.Password);
            Clear();
            RefreshDataGrid();
        }

        void Update()
        {
            Admin admin = new Admin
            {
                Name = NameTB.Text,
                Surname = SurnameTB.Text,
                Email = EmailTB.Text,
                Phone = PhoneTB.Text,
                Image = UserImage.Tag == null ? null : image
            };
            adminService.UpdateAdmin(admin, selected.AdminId);
            Clear();
        }

        public void Clear()
        {
            AddUpdateButton.Content = "Add";
            AdminsDG.SelectedItem = null;

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
            selected = null;

            RefreshDataGrid();
        }

        void RefreshDataGrid()
        {
            DataContext = null;
            DataContext = adminService.GetAllAdmins().Where(a => a.UserId != currentUser.UserId).Select(a => new AdminAdminViewModel
            {
                AdminId = a.UserId,
                UserName = a.UserName,
                HashPassword = a.HashPassword,
                FullName = a.ToString(),
                Email = a.Email,
                Phone = a.Phone
            });
        }
    }
}
