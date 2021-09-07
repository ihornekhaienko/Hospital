using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using System;
using System.Windows;
using Hospital.Views.Shared;

namespace Hospital.Views.DoctorUI
{
    /// <summary>
    /// Логика взаимодействия для DoctorWindow.xaml
    /// </summary>
    public partial class DoctorWindow : Window
    {
        readonly Window parent;
        readonly User currentUser;
        readonly IFileManager fileManager;

        public DoctorWindow(Window parent, User user)
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

        private void StudentButton_Click(object sender, RoutedEventArgs e)
        {
            WindowContent.Navigate(new StudentPage(currentUser));
        }

        private void VisitsButton_Click(object sender, RoutedEventArgs e)
        {
            WindowContent.Navigate(new VisitPage(currentUser));
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            WindowContent.Navigate(new StatisticsPage());
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
