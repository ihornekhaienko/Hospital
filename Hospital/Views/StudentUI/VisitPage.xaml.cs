using Hospital.Models;
using Hospital.Models.Identity;
using Hospital.Services.Interfaces;
using Hospital.ViewModels;
using Hospital.Views.Filters;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Hospital.Views.StudentUI
{
    /// <summary>
    /// Логика взаимодействия для VisitPage.xaml
    /// </summary>
    public partial class VisitPage : Page
    {
        readonly User currentUser;
        private VisitStudentViewModel selected;
        readonly IFileManager fileManager;
        readonly IRecordService recordService;
        public VisitPage(User user)
        {
            InitializeComponent();
            currentUser = user;
            fileManager = App.fileManager;
            recordService = App.recordService;

            RefreshDataGrid();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selected == null)
                {
                    throw new Exception("First select the row");
                }

                if (selected.State != State.Planned.ToString())
                {
                    throw new Exception("You can cancel only planned visits");
                }

                recordService.SetState(selected.RecordId, State.Canceled);
                Clear();
                RefreshDataGrid();
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show(err.Message);
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            Window filter = new VisitFilterWindow(currentUser.Role, RefreshDataGrid);
            filter.Show();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selected == null)
                {
                    throw new Exception("First select the row");
                }
                if (selected.State != State.Completed.ToString())
                {
                    throw new Exception("You can print only completed visits");
                }

                Record record = recordService.GetRecord(selected.RecordId);
                string path = fileManager.SaveToPdf();
                fileManager.PrintConclusion(record, path);
            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show(err.Message);
            }
        }

        private void RowSelected(object sender, RoutedEventArgs e)
        {
            selected = (VisitStudentViewModel)VisitsDG.SelectedItem;
            if (selected != null)
            {
                DoctorTB.Text = selected.Doctor;
                SpecialtyTB.Text = selected.Specialty;
                VisitDateDP.SelectedDate = selected.Date;
                VisitTimeTP.SetValue(DateTimePicker.ValueProperty, new DateTime(1, 1, 1) + selected.Time);
                StateTB.Text = selected.State;
                DiagnosisTB.Text = selected.Diagnosis;
            }
        }

        private void Clear()
        {
            VisitsDG.SelectedItem = null;

            selected = null;
            DoctorTB.Text = string.Empty;
            SpecialtyTB.Text = string.Empty;
            VisitDateDP.SelectedDate = DateTime.Now;
            VisitTimeTP.SetValue(DateTimePicker.ValueProperty, DateTime.Now);
            StateTB.Text = string.Empty;
            DiagnosisTB.Text = string.Empty;

            RefreshDataGrid();
        }

        void RefreshDataGrid(object source = null)
        {
            recordService.RefreshRecords(currentUser as Student);
            DataContext = null;
            if (source == null)
            {
                DataContext = recordService.GetAllRecords().Select(r => new VisitStudentViewModel
                {
                    RecordId = r.RecordId,
                    Doctor = r.Doctor.ToString(),
                    Specialty = r.Doctor.Specialty.SpecialtyName,
                    Date = r.RecordDate,
                    Time = r.RecordTime,
                    State = r.State.ToString(),
                    Diagnosis = string.IsNullOrEmpty(r.Diagnosis) ? "-" : r.Diagnosis,
                });
            }
            else
            {
                DataContext = source;
            }
        }
    }
}
