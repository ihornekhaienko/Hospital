using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using Hospital.Views.AdminUI;
using Hospital.Views.DoctorUI;
using Hospital.Views.StudentUI;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Hospital
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly IUserManager userManager;

        public MainWindow()
        {
            InitializeComponent();
            this.userManager = App.userManager;

            LoginTB.Text = "Master";
            PasswordBox.Password = "pass";
        }

        private void SigninButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = LoginTB.Text;
                string password = PasswordBox.Password;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    throw new Exception("Empty username or password");
                }

                bool success = userManager.Validate(username, password);
                if (!success)
                {
                    throw new Exception("Wrong username or password");
                }

                Window mainWindow = this;
                User current = userManager.GetUser(username);

                switch (current.Role)
                {
                    case Role.Admin:
                        mainWindow = new AdminWindow(this, current);
                        break;
                    case Role.Doctor:
                        mainWindow = new DoctorWindow(this, current);
                        break;
                    case Role.Student:
                        mainWindow = new StudentWindow(this, current);
                        break;
                }

                this.Hide();
                mainWindow.Show();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ForgotPasswordHL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = LoginTB.Text;
                if (string.IsNullOrEmpty(username))
                {
                    throw new Exception("Empty username");
                }

                userManager.RestorePassword(username);
                MessageBox.Show("new account details have been sent to your email");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}
