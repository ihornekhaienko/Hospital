using Hospital.Models;
using Hospital.Services.Interfaces;
using Microsoft.Win32;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hospital.Services.Implementations
{
    public class FileManager : IFileManager
    {
        public ImageSource ByteToImageSource(byte[] byteImage)
        {
            MemoryStream byteStream = new MemoryStream(byteImage);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = byteStream;
            image.EndInit();
            return image;
        }
        public string OpenImage()
        {
            string path = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    path = openFileDialog.FileName;
                }
                catch
                {
                    MessageBox.Show("Failed to open image");
                }
            }
            return path;
        }
        public string SaveToPdf()
        {
            string path = string.Empty;
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Filter = "Pdf Files|*.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    path = saveFileDialog.FileName;
                }
                catch
                {
                    MessageBox.Show("Failed to save pdf");
                }
            }
            return path;
        }
        public void PrintConclusion(Record record, string fileName)
        {
            using PdfDocument document = new PdfDocument();
            PdfPage page = document.Pages.Add();
            PdfGraphics graphics = page.Graphics;
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14);


            graphics.DrawString("Results of the visit", font, PdfBrushes.Black, new PointF(200, 0));
            graphics.DrawString($"Record № {record.RecordId}", font, PdfBrushes.Black, new PointF(0, 50));
            graphics.DrawString($"Patient: {record.Student}", font, PdfBrushes.Black, new PointF(0, 100));
            graphics.DrawString($"Doctor: {record.Doctor}", font, PdfBrushes.Black, new PointF(0, 150));
            graphics.DrawString($"Doctor's specialty: {record.Doctor.Specialty.SpecialtyName}", font, PdfBrushes.Black, new PointF(0, 200));
            graphics.DrawString($"Appointment date: {record.RecordDate:yyyy-MM-dd}", font, PdfBrushes.Black, new PointF(0, 250));
            graphics.DrawString($"Appointment time: {record.RecordTime:hh\\:mm}", font, PdfBrushes.Black, new PointF(0, 300));
            graphics.DrawString($"Diagnosis: {record.Diagnosis}", font, PdfBrushes.Black, new PointF(0, 350));

            PointF p = new PointF(0, 400);
            if (!string.IsNullOrWhiteSpace(record.Appointment))
            {
                graphics.DrawString($"Appointment: {record.Appointment}", font, PdfBrushes.Black, p);
                p = new PointF(0, 450);
            }
            if (!string.IsNullOrWhiteSpace(record.AdditionalInfo))
            {
                graphics.DrawString($"Additional info: {record.AdditionalInfo}", font, PdfBrushes.Black, p);
            }

            document.Save(fileName);
        }
    }
}
