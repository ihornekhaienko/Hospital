using Hospital.Models;
using System.Windows.Media;

namespace Hospital.Services.Interfaces
{
    interface IFileManager
    {
        public ImageSource ByteToImageSource(byte[] byteImage);
        public string OpenImage();
        public string SaveToPdf();
        public void PrintConclusion(Record record, string fileName);
    }
}
