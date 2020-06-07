using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganografia
{
    class FileManager
    {
        public Bitmap MyImage1 { get; set; }
        public long ImageSize { get; set; }
        public string FileContent { get; set; }
        public string FileName { get; set; }

        private string imageName=null, imagePath;

        public void OpenTextFile()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Open Text File";
                dialog.Filter = ".txt files|*.txt"; //"Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG"; 
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Stream fileStream = dialog.OpenFile();
                    FileName = dialog.FileName;
                    using (StreamReader reader= new StreamReader(fileStream))
                    {
                        FileContent = reader.ReadToEnd();
                    }
                }
            }
        }

        public void OpenImage()
        {
            imageName = null;
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Open Image";
                dialog.Filter = "Image files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG"; //"Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG (*.bmp)|*.bmp"; 
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ImageSize = new FileInfo(dialog.FileName).Length;
                    MyImage1 = new Bitmap(dialog.FileName);
                }
            }
        }

        public void SaveImageAs()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Save Image",
                Filter = "BMP files (*.bmp)|*.BMP| (*.jpg)|*.JPG;*.JPEG| (*.png)|*.PNG" // .bmp files (*.bmp)|*.bmp
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                imagePath = Path.GetDirectoryName(dialog.FileName);
                imageName = dialog.FileName;
                MyImage1.Save(dialog.FileName, GetImageFormat(dialog.FileName));
            }
        }

        public void SaveImage()
        {
            if(imageName == null && imageName != string.Empty && MyImage1 != null)
            {
                SaveImageAs();
            }else{
                MyImage1.Save(imageName, System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }

        ImageFormat GetImageFormat(string name) {
            ImageFormat format = ImageFormat.Bmp;
            string extension = System.IO.Path.GetExtension(name);
            switch (extension)
            {
                case ".jpeg":
                case ".jpg":
                    format = ImageFormat.Jpeg;
                    break;
                case ".bmp":
                    format = ImageFormat.Bmp;
                    break;
                case ".png":
                    format = ImageFormat.Png;
                    break;
            }
            return format;
        }
    }
}
