using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using Hospital.Views.Shared;
using System.Windows;

namespace Hospital.Views.AdminUI
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        readonly User currentUser;
        readonly Window parent;
        readonly IFileManager fileManager;
        public AdminWindow(Window parent, User user)
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

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            WindowContent.Navigate(new AdminPage(currentUser));
        }

        private void DoctorButton_Click(object sender, RoutedEventArgs e)
        {
            WindowContent.Navigate(new DoctorPage(currentUser));
        }

        private void StudentButton_Click(object sender, RoutedEventArgs e)
        {
            WindowContent.Navigate(new StudentPage(currentUser));
        }

        private void FacultyButton_Click(object sender, RoutedEventArgs e)
        {
            WindowContent.Navigate(new FacultyPage());
        }

        private void GroupButton_Click(object sender, RoutedEventArgs e)
        {
            WindowContent.Navigate(new GroupPage());
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

        private void Window_Closed(object sender, System.EventArgs e)
        {
            parent.Close();
        }
    }
}
