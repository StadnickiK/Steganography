using System;
using System.Drawing;
using System.Windows.Forms;

namespace Steganografia
{
    public partial class Form1 : Form
    {
        private Coder coder = new Coder();
        private Messenger messenger = new Messenger();
        private FileManager fileManager = new FileManager();

        public Form1()
        {
            InitializeComponent();
        }

        double FreeSpace()
        {
            double size = 0;
            if (fileManager.FileContent != null)
            {
                if (fileManager.FileContent.Length < (Converter.CountTrueBits(coder.Channels) / 3.0) * fileManager.ImageSize - 2)
                {
                    size = fileManager.ImageSize - 2 - fileManager.FileContent.Length * Converter.CountTrueBits(coder.Channels) / 3.0;
                }
            }else if(fileManager.ImageSize > 0)
            {
                size = (fileManager.ImageSize -2) * Converter.CountTrueBits(coder.Channels) / 3.0;
            }
            return size;
        }

        double GetAvailableSpace()
        {
            double size = 0;
            double tempR = coder.BitsToChange[0] = trackBar1.Value;
            tempR *= 12.5;
            size = fileManager.ImageSize / 3.0 * (tempR/100.0);
            tempR = coder.BitsToChange[1] = trackBar2.Value;
            tempR *= 12.5;
            size += fileManager.ImageSize / 3.0 * (tempR / 100.0);
            tempR = coder.BitsToChange[2] = trackBar3.Value;
            tempR *= 12.5;
            size += fileManager.ImageSize / 3.0 * (tempR / 100.0);
            return size;
        }

        void UpdateChart()
        {
            chart1.Series["Series1"].Points.Clear();
            long required = fileManager.FileContent == null ? 0 : fileManager.FileContent.Length;
            int percentReq, percentFree;
            double imageSize = GetAvailableSpace();
            percentReq = imageSize > required ? (int)(required / imageSize*100) : 100;
            percentFree = 100 - percentReq;
            //if ((int)FreeSpace() != 0) { }
            chart1.Series["Series1"].Points.AddXY(percentFree.ToString() + "%", percentFree);
            chart1.Series["Series1"].Points.AddXY(percentReq.ToString() + "%", percentReq);
            label1.Text = "Free space left " + (int)FreeSpace() + " Bytes";
        }

        void UpdateSpaceAvailable()
        {
            label5.Text = "Space Available " +
            GetAvailableSpace()
            + " Bytes";
        }

        void CheckIfEnoughSpace()
        {
            if (fileManager.FileContent != null )
            {
                label7.Text = fileManager.FileContent.Length > GetAvailableSpace()
                    ? "Warning not enough space, possible lose of data."
                    : "";
            }
        }

        void EncodeImage(string s)
        {
            if (pictureBox1.Image != null)
            {
                Bitmap tempBMP = (Bitmap)pictureBox1.Image.Clone();
                fileManager.MyImage1 = coder.EncodeBitmap(tempBMP, s);
                PictureBox2.Image = fileManager.MyImage1;
                //PictureBox2.Height = fileManager.MyImage1.Height;
                //PictureBox2.Width = fileManager.MyImage1.Width;
            }
        }

        private void Button1_Click(object sender, EventArgs e) //encode image
        {
            
            string s = textBox1.Text;
            if (s!=null && s != "" && s != " " && s != string.Empty && fileManager.MyImage1 != null)
            {
                EncodeImage(s);
            }else{
                if(fileManager.FileContent != null)
                {
                    EncodeImage(fileManager.FileContent);
                }
                else
                {
                    messenger.Error("Error nothing to encode. Please select a file or write your text in textbox.");
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e) // decode image
        {
            string s = null;
            if (fileManager.MyImage1 != null)
            {
                s = coder.DecodeBitmap(fileManager.MyImage1);
                //textBox1.Text = s;
            }
            else
            {
                messenger.ErrorNoFileSelected();
                fileManager.OpenImage();
            }
        }

        private void LoadFileToolStripMenuItem_Click(object sender, EventArgs e) // load image
        {
            fileManager.OpenImage();
            Image temp = (Image)fileManager.MyImage1.Clone();
            PictureBox1.Image = temp;
            label5.Text = "Space Available "+ (fileManager.ImageSize/8-2)+" Bytes";
            if(fileManager.FileContent != null)
            {
                double size = FreeSpace();
                label1.Text = "Free space left " + size + " Bytes";
            }
            UpdateChart();
            CheckIfEnoughSpace();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileManager.SaveImageAs();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileManager.SaveImage();
        }

        private void LoadFileToolStripMenuItem1_Click(object sender, EventArgs e) // load file
        {
            fileManager.OpenTextFile();
            if (fileManager.FileContent != null)
            {
                label6.Text = "Space required " + fileManager.FileContent.Length + " Bytes";
                label2.Text = "File " + fileManager.FileName;
                CheckIfEnoughSpace();
            }
            UpdateChart();
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)   // Bits to change
        {
            double temp = coder.BitsToChange[0] = trackBar1.Value;
            label4.Text = "Bits to use "+temp;
            UpdateSpaceAvailable();
            UpdateChart();
        }

        private void TrackBar2_Scroll(object sender, EventArgs e)
        {
            double temp = coder.BitsToChange[1] = trackBar2.Value;
            label3.Text = "Bits to use " + temp;
            UpdateSpaceAvailable();
            UpdateChart();
        }

        private void TrackBar3_Scroll(object sender, EventArgs e)
        {
            double temp = coder.BitsToChange[2] = trackBar3.Value;
            label8.Text = "Bits to use " + temp;
            UpdateSpaceAvailable();
            UpdateChart();
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)   // Space required
        {
            label6.Text = "Space required " + textBox1.Text.Length + " Bytes";
            fileManager.FileContent = textBox1.Text;
            UpdateChart();
            CheckIfEnoughSpace();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)   // R channel
        {
            if(checkBox1.Checked == true)
            {
                coder.Channels[2] = true;
            }
            else
            {
                coder.Channels[2] = false;
            }
            UpdateSpaceAvailable();
            UpdateChart();
            CheckIfEnoughSpace();
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)   // G channel
        {
            if (checkBox2.Checked == true)
            {
                coder.Channels[1] = true;
            }
            else
            {
                coder.Channels[1] = false;
            }
            UpdateSpaceAvailable();
            UpdateChart();
            CheckIfEnoughSpace();
        }

        private void CheckBox3_CheckedChanged(object sender, EventArgs e)   // B channel
        {
            if (checkBox3.Checked == true)
            {
                coder.Channels[0] = true;
            }
            else
            {
                coder.Channels[0] = false;
            }
            UpdateSpaceAvailable();
            UpdateChart();
            CheckIfEnoughSpace();
        }
    }
}
