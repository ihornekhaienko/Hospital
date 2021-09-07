using Hospital.Models;
using Hospital.Services.Interfaces;
using Hospital.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hospital.Views.AdminUI
{
    /// <summary>
    /// Логика взаимодействия для GroupPage.xaml
    /// </summary>
    public partial class GroupPage : Page
    {
        private GroupAdminViewModel selected;
        readonly IGroupService groupService;
        readonly IFacultyService facultyService;
        public GroupPage()
        {
            InitializeComponent();

            groupService = App.groupService;
            facultyService = App.facultyService;
            RefreshDataGrid();

            FacultyCB.ItemsSource = facultyService.GetAllFaculties()
                                                        .Select(f => f.FacultyName);
        }

        private void AddUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(GroupTB.Text) || string.IsNullOrWhiteSpace(CourseTB.Text) || FacultyCB.SelectedItem == null)
                {
                    throw new Exception("Fields should not be empty");
                }
                if (groupService.IsExist(FacultyCB.Text, GroupTB.Text))
                {
                    throw new Exception("This group already exists");
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

                groupService.RemoveGroup(selected.GroupId);
                Clear();
                RefreshDataGrid();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void RowSelected(object sender, RoutedEventArgs e)
        {
            selected = (GroupAdminViewModel)GroupsDG.SelectedItem;
            if (selected != null)
            {
                AddUpdateButton.Content = "Update";
                GroupTB.Text = selected.GroupName;
                CourseTB.Text = selected.Course.ToString();
                FacultyCB.SelectedIndex = FacultyCB.Items.IndexOf(selected.FacultyName);
            }
        }

        void Add()
        {
            Group group = new Group
            {
                GroupName = GroupTB.Text.Trim(),
                Course = Convert.ToUInt32(CourseTB.Text),
                Faculty = facultyService.GetFaculty(FacultyCB.Text)
            };
            groupService.AddGroup(group);
            Clear();
            RefreshDataGrid();
        }

        void Update()
        {
            Group group = new Group
            {
                GroupName = GroupTB.Text,
                Course = Convert.ToUInt32(CourseTB.Text),
                Faculty = facultyService.GetFaculty(FacultyCB.Text)
            };
            groupService.UpdateGroup(group, selected.GroupId);
            Clear();
        }

        public void Clear()
        {
            AddUpdateButton.Content = "Add";
            GroupsDG.SelectedItem = null;

            GroupTB.Clear();
            CourseTB.Clear();
            FacultyCB.SelectedItem = null;
            selected = null;

            RefreshDataGrid();
        }

        private void CourseTB_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back)
            {
                return;
            }
            e.Handled = true;
        }

        void RefreshDataGrid()
        {
            DataContext = null;
            DataContext = groupService.GetAllGroups().Select(g => new GroupAdminViewModel
            {
                GroupId = g.GroupId,
                GroupName = g.GroupName,
                Course = g.Course,
                FacultyName = g.Faculty.FacultyName,
                Students = groupService.GetStudentCount(g)
            });
        }
    }
}
