using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Hospital.Views.Shared
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        readonly User currentUser;
        readonly IFileManager fileManager;
        readonly IUserManager userManager;
        private byte[] image;

        public ProfilePage(User user)
        {
            InitializeComponent();
            currentUser = user;
            userManager = App.userManager;
            fileManager = App.fileManager;

            LoginTB.Text = currentUser.UserName;
            NameTB.Text = currentUser.Name;
            SurnameTB.Text = currentUser.Surname;
            EmailTB.Text = currentUser.Email;
            PhoneTB.Text = currentUser.Phone;

            if (currentUser.Image != null)
            {
                UserImage.Source = fileManager.ByteToImageSource(currentUser.Image);
            }
        }
        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = LoginTB.Text;
                string email = EmailTB.Text;
                string phone = PhoneTB.Text;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone))
                {
                    throw new Exception("Fields cannot be empty");
                }

                if (userManager.GetUser(username) != null && username != currentUser.UserName)
                {
                    throw new Exception("This login is unavailable");
                }

                if (!Regex.IsMatch(phone, @"((\+)?\b(8|38)?(0[\d]{2}))([\d-]{5,8})([\d]{2})"))
                {
                    throw new Exception("Incorrect phone number format");
                }

                if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
                {
                    throw new Exception("Incorrect email format");
                }

                User user = new User
                {
                    UserName = username,
                    Email = email,
                    Phone = phone,
                    Image = UserImage.Tag == null ? null : image
                };
                userManager.UpdateUserInfo(user, currentUser.UserId);

                AcceptButton.Visibility = Visibility.Hidden;
                CancelButton.Visibility = Visibility.Hidden;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            LoginTB.Text = currentUser.UserName;
            NameTB.Text = currentUser.Name;
            SurnameTB.Text = currentUser.Surname;
            EmailTB.Text = currentUser.Email;
            PhoneTB.Text = currentUser.Phone;

            image = currentUser.Image;
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

            AcceptButton.Visibility = Visibility.Hidden;
            CancelButton.Visibility = Visibility.Hidden;
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

            AcceptButton.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Visible;
        }

        private void KeyPress(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.A && e.Key <= Key.Z) || (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back)
            {
                TextChanged(sender, e);
                return;
            }
            e.Handled = true;
        }

        private void TextChanged(object sender, KeyEventArgs e)
        {
            AcceptButton.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Visible;
        }

        private void ChangePasswordHL_Click(object sender, RoutedEventArgs e)
        {
            Window cpw = new ChangePasswordWindow(currentUser);
            cpw.Show();
        }
    }
}
