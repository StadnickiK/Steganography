using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Steganografia
{
    class Coder
    {
        private readonly int startBit;
        private readonly int step;
        private readonly int correction;
        public int[] BitsToChange { get; set; }
        public BitArray Channels { get; set; }
        private int[] changedBits = new int[3];

        public Coder()
        {
            BitsToChange = new int[3];
            changedBits[0] = changedBits[1] = changedBits[2] = 1;
            startBit = 96;   // 07 815 1623 2431 3239 4047 48
            correction = startBit / 8;
            Channels = new BitArray(3, true);   // channels RGB
            step = 8;   // 1 byte = 8 bits
        }

        Bitmap UpdateBitmap(Bitmap bmp, byte[] rgbValues)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bitmapData = bmp.LockBits(
                rect,
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            Bitmap tempBMP = (Bitmap)bmp.Clone();
            IntPtr ptr = bitmapData.Scan0;
            int bytes = Math.Abs(bitmapData.Stride) * tempBMP.Height;
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            bmp.UnlockBits(bitmapData);
            return bmp;
        }

        void DecodeChangedBits(BitArray rgbBits)
        {
            for (int i = 0; i < 71; i += 24)
            {
                BitArray temp = new BitArray(3, false);
                for (int j = 0; j < 24; j += 8)
                {
                    if (rgbBits[i+j] == true) { temp[j/8] = true; }
                }
                changedBits[i/24] = Converter.BitArrayToInt(temp);
                if (changedBits[i/24] == 0) changedBits[i/24] = 8;
            }
        }

        BitArray EncodeChangedBits(BitArray rgbBits)
        {
            BitArray bits = new BitArray(9, false);
            BitArray temp = new BitArray(new int[] { BitsToChange[0] });
            for(int i = 0; i < 3; i++)
            {
                bits[i] = temp[i];
            }
            temp = new BitArray(new int[] { BitsToChange[1] });
            for (int i = 3; i < 6; i++)
            {
                bits[i] = temp[i - 3];
            }
            temp = new BitArray(new int[] { BitsToChange[2] });
            for (int i = 6; i < 9; i++)
            {
                bits[i] = temp[i-6];
            }

            for (int i = 0; i < 71; i += step)
            {
                if (bits[(i / step)] == true) { rgbBits[i] = true; } else
                {
                    rgbBits[i] = false;
                }
            }
            return rgbBits;
        }

        BitArray EncodeChannels(BitArray rgbBits)
        {
            for (int i = 72; i < 95; i += step)
            {
                if (Channels[(i / step) - 9] == true) { rgbBits[i] = true; }
                else
                {
                    rgbBits[i] = false;
                }
                
            }
            return rgbBits;
        }

        void DecodeChannels(BitArray rgbBits)
        {
            for (int i = 72; i < 95; i += step)
            {
                if (rgbBits[i] == true) { Channels[(i / step)-9] = true; } else
                {
                    Channels[(i / step)-9] = false;
                }
            }
        }

        public Bitmap EncodeBitmap(Bitmap bmp, string s)
        {
            byte[] rgbValues = Converter.BMPtoBytes(bmp);
            BitArray rgbBits = new BitArray(rgbValues);
            BitArray asciiBits = Converter.StringToBits(s);
            rgbBits = EncodeChangedBits(rgbBits);
            rgbBits = EncodeChannels(rgbBits);
            for (int i = startBit; i < rgbBits.Length-startBit; i += step) //07 815 1623 2431 
            { 
                for(int c = 0; c < 3; c++) // channel loop up to 3
                {
                    if (c > 0)
                    {
                        i += step;
                    }
                    if (Channels[c] == true)
                    {
                        for (int j = 0; j < BitsToChange[c]; j++) // bit loop, up to 8
                        {
                            if (((i / 8) - (correction+c)) + j < asciiBits.Length && (i+j)<rgbBits.Length)
                            {
                                if (rgbBits[i + j] == true && asciiBits[((i / step) - (correction + c)) + j] == false)
                                {
                                    rgbBits[i + j] = false;
                                }
                                else
                                {
                                    if (asciiBits[(i / step) - (correction + c) + j] == true)
                                    {
                                        rgbBits[i + j] = true;
                                    }
                                }
                            }
                            else { rgbBits[i + j] = false; }
                        }
                    }
                }
            }
            rgbBits.CopyTo(rgbValues, 0);
            Bitmap tempBMP = UpdateBitmap(bmp, rgbValues);
            return tempBMP;
        }

        public string DecodeBitmap(Bitmap bmp)
        {
            string s = null;

            byte[] rgbValues = Converter.BMPtoBytes(bmp);

            BitArray rgbBits = new BitArray(rgbValues);
            BitArray asciiBits = new BitArray(rgbBits.Length,false);
            DecodeChangedBits(rgbBits);
            DecodeChannels(rgbBits);
            for (int i = startBit; i < rgbBits.Length-startBit; i += step)
            {
                for (int c = 0; c < 3; c++) // channel loop up to 3
                {
                    if (c > 0)
                    {
                        i += step;
                    }
                    for (int j = 0; j <changedBits[c]; j++)
                    {

                        if (Channels[c] == true)
                        {
                            if (rgbBits[i + j] == true)
                            {
                                asciiBits[(i / step) - (correction + c) + j] = true;
                            }
                        }
                    }
                }
            }
            s = Converter.BitsToString(asciiBits);
            Console.WriteLine(s);
            return s;
        }
    }
}
