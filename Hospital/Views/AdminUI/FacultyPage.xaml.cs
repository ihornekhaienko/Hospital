using Hospital.Models;
using Hospital.Services.Interfaces;
using Hospital.ViewModels;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hospital.Views.AdminUI
{
    /// <summary>
    /// Логика взаимодействия для FacultyPage.xaml
    /// </summary>
    public partial class FacultyPage : Page
    {
        private FacultyAdminViewModel selected;
        readonly IFacultyService facultyService;
        public FacultyPage()
        {
            InitializeComponent();

            facultyService = App.facultyService;
            RefreshDataGrid();
        }

        private void AddUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FacultyTB.Text))
                {
                    throw new Exception("Fields should not be empty");
                }
                if (facultyService.IsExist(FacultyTB.Text))
                {
                    throw new Exception("This faculty already exists");
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

                facultyService.RemoveFaculty(selected.FacultyId);
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
            selected = (FacultyAdminViewModel)FacultiesDG.SelectedItem;
            if (selected != null)
            {
                AddUpdateButton.Content = "Update";
                FacultyTB.Text = selected.FacultyName;
            }
        }

        void Add()
        {
            Faculty faculty = new Faculty
            {
                FacultyName = FacultyTB.Text.Trim()
            };
            facultyService.AddFaculty(faculty);
            Clear();
            RefreshDataGrid();
        }

        void Update()
        {
            var faculty = new Faculty
            {
                FacultyName = selected.FacultyName
            };
            facultyService.UpdateFaculty(faculty, selected.FacultyId);
            Clear();
        }

        public void Clear()
        {
            AddUpdateButton.Content = "Add";
            FacultiesDG.SelectedItem = null;

            FacultyTB.Clear();
            FacultiesDG.SelectedItem = null;
            selected = null;

            RefreshDataGrid();
        }

        void RefreshDataGrid()
        {
            DataContext = null;
            DataContext = facultyService.GetAllFaculties().Select(f => new FacultyAdminViewModel
            {
                FacultyId = f.FacultyId,
                FacultyName = f.FacultyName,
                Groups = facultyService.GetGroupCount(f),
                Students = facultyService.GetStudentCount(f)
            });
        }
    }
}
