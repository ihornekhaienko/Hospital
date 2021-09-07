using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using System;
using System.Windows;
using Hospital.Views.Shared;

namespace Hospital.Views.StudentUI
{
    /// <summary>
    /// Логика взаимодействия для StudentWindow.xaml
    /// </summary>
    public partial class StudentWindow : Window
    {
        readonly Window parent;
        readonly User currentUser;
        readonly IFileManager fileManager;

        public StudentWindow(Window parent, User user)
        {
            InitializeComponent();

            this.parent = parent;
            currentUser = user;

            WindowContent.Content = new ProfilePage(currentUser);

            UsernameTB.Text = $"{currentUser.Name} {currentUser.Surname}";
            fileManager = App.fileManager;
            if (currentUser.Image != null)
            {
                UserImage.Source = fileManager.ByteToImageSource(currentUser.Image);
            }
        }

        private void DoctorButton_Click(object sender, RoutedEventArgs e)
        {
            WindowContent.Navigate(new DoctorPage(currentUser));
        }

        private void VisitsButton_Click(object sender, RoutedEventArgs e)
        {
            WindowContent.Navigate(new VisitPage(currentUser));
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            WindowContent.Navigate(new ProfilePage(currentUser));
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            parent.Show();
            this.Hide();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            parent.Close();
        }
    }
}
