using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganografia
{
    public static class Converter
    {
        static byte[] Remove0BytesFromArray(byte[] bytes)
        {
            int i = bytes.Length - 1;
            while (bytes[i] == 0 && i != 0) { --i; }
            byte[] cleanBytes = new byte[i + 1];
            Array.Copy(bytes, cleanBytes, i + 1);
            return cleanBytes;
        }

        public static BitArray StringToBits(string s)
        {
            // 1 bajt ma przechowywać startBit i step
            BitArray asciiBits = null;

            if (s != null)
            {
                asciiBits = new BitArray(Encoding.UTF8.GetBytes(s));
            }
            return asciiBits;
        }

        public static string BitsToString(BitArray bits)
        {
            string s = null;
            if (bits != null)
            {
                byte[] bytes = new byte[(bits.Length - 1) / 8 + 1];
                bits.CopyTo(bytes, 0);
                bytes = Remove0BytesFromArray(bytes);
                s = System.Text.Encoding.UTF8.GetString(bytes);
            }
            return s;
        }

        public static byte[] BMPtoBytes(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bitmapData = bmp.LockBits(
                rect,
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            Bitmap tempBMP = (Bitmap)bmp.Clone();
            IntPtr ptr = bitmapData.Scan0;

            int bytes = Math.Abs(bitmapData.Stride) * tempBMP.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            bmp.UnlockBits(bitmapData);
            return rgbValues;
        }

        public static int BitArrayToInt(BitArray bitArray)
        {
            if (bitArray.Length > 32)
            {
                throw new ArgumentException("Maximum length of BitArray should be 32 bits.");
            }
            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }

        public static int CountTrueBits(BitArray bits)
        {
            int count = 0;
            for(int i = 0; i < bits.Length; i++)
            {
                count += bits[i] == true ? 1 : 0; 
            }
            return count;
        }

        public static int ReturnSmallerInt(int a, int b)
        {
            if (a < b) return a;
            return b;
        }

    }
}
