using Hospital.Models;
using Hospital.Services.Interfaces;
using System;
using System.Windows;
using System.Windows.Documents;

namespace Hospital.Views
{
    /// <summary>
    /// Логика взаимодействия для RecordWindow.xaml
    /// </summary>
    public partial class RecordWindow : Window
    {
        readonly Record selected;
        readonly IFileManager fileManager;
        readonly IRecordService recordService;
        public RecordWindow(int recordId)
        {
            InitializeComponent();

            fileManager = App.fileManager;
            recordService = App.recordService;
            selected = recordService.GetRecord(recordId);
            recordService.SetState(selected, State.Active);

            PatientTB.Text = $"{selected.Student.Name} {selected.Student.Surname}";
            SexTB.Text = selected.Student.Sex.ToString();
            DoctorTB.Text = $"{selected.Doctor.Name} {selected.Doctor.Surname}";
            SpecialtyTB.Text = selected.Doctor.Specialty.SpecialtyName;
            DateTB.Text = selected.RecordDate.Date.ToString(@"yyyy-MM-dd");
            TimeTB.Text = selected.RecordTime.ToString(@"hh\:mm");
            PrintCheck.IsChecked = false;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DiagnosisTB.Text))
                {
                    throw new Exception("Empty diagnosis given");
                }

                Record record = new Record
                {
                    Diagnosis = DiagnosisTB.Text,
                    Appointment = new TextRange(AppointmentRTB.Document.ContentStart, AppointmentRTB.Document.ContentEnd).Text,
                    AdditionalInfo = new TextRange(AddInfoRTB.Document.ContentStart, AddInfoRTB.Document.ContentEnd).Text
                };
                recordService.UpdateRecord(record, selected.RecordId);

                if ((bool)PrintCheck.IsChecked)
                {
                    string path = fileManager.SaveToPdf();
                    fileManager.PrintConclusion(selected, path);
                }

                this.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}
