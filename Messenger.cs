using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganografia
{
    class Messenger
    {
        public void Error(string s)
        {
            MessageBox.Show(
                s,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
        }
        public void ErrorNoFileSelected()
        {
            MessageBox.Show(
                "Error no file selected. Please load an image.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
        }
    }
}
