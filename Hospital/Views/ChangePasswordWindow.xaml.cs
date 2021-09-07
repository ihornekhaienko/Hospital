using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using System;
using System.Windows;
using System.Windows.Input;

namespace Hospital.Views
{
    /// <summary>
    /// Логика взаимодействия для ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        readonly User currentUser;
        readonly IUserManager userManager;
        public ChangePasswordWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            userManager = App.userManager;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string oldPass = OldPasswordTB.Text;
                string newPass1 = NewPasswordTB1.Text;
                string newPass2 = NewPasswordTB2.Text;

                if (string.IsNullOrEmpty(oldPass) || string.IsNullOrEmpty(newPass1) || string.IsNullOrEmpty(newPass2))
                {
                    throw new Exception("Fields cannot be empty");
                }

                if (!userManager.Validate(currentUser.UserName, oldPass))
                {
                    throw new Exception("Wrong old password");
                }

                if (newPass1 != newPass2)
                {
                    throw new Exception("New passwords don't match");
                }

                userManager.ChangePassword(currentUser, newPass1);

                this.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            OldPasswordTB.Clear();
            NewPasswordTB1.Clear();
            NewPasswordTB2.Clear();
        }
        private void KeyPress(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.A && e.Key <= Key.Z) || (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back)
            {
                return;
            }
            e.Handled = true;
        }
    }
}
