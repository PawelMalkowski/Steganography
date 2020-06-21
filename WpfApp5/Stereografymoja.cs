using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace WpfApp5
{
    class Stereografymoja
    {

        public static byte[] ConvertBitmapToBytesArray(BitmapImage Image)
        {
            int stride = (int)Image.PixelWidth * (Image.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[Image.PixelHeight * stride];
            Image.CopyPixels(pixels, stride, 0);
            return pixels;
        }
        public static BitmapSource ConvertByteArrayToBitmsapImage(BitmapImage OrginalImage, byte[] pixels)
        {
            int stride = (int)OrginalImage.PixelWidth * (OrginalImage.Format.BitsPerPixel / 8);
            var bitmap = BitmapSource.Create((int)OrginalImage.Width, (int)OrginalImage.Height, OrginalImage.DpiX, OrginalImage.DpiY,
                                 OrginalImage.Format, null, pixels, stride);
            return bitmap;

        }
        public static bool IsEncoded(BitmapImage Image)
        {
            byte[] pixels = ConvertBitmapToBytesArray(Image);
            bool isEncoded = true;
            int[] defaultCodedPrefix = { 101, 110, 99, 111, 100, 101, 100 } ;
            int licznik = 0;
            int licznikTablicy=0;
            int przesuniecie = 0;
            for (int i = 0; i < 56; i += 8)
            {
                int dec_value = 0;
                int base1 = 128;
                for (int j = 7;isEncoded && j >= 0; j--)
                {
                    if (licznik == 3)
                    {
                        licznik = 0;
                        przesuniecie++;
                    }
                    dec_value += ((pixels[i + 7 - j + przesuniecie]) % 2) * base1;

                    base1 = base1 / 2;
                    licznik++;
                }
                if (dec_value != defaultCodedPrefix[licznikTablicy])
                {
                    isEncoded = false;
                }
                licznikTablicy++;
            }
                return isEncoded;
        }

        public static BitmapSource CryptoMesseg(BitmapImage OrginalImage, string TextToEncoding)
        {
            int pozycja = 1;
            long lastword = 0;
            TextToEncoding = "encoded" + TextToEncoding;
            byte[] TextInBytes = Encoding.GetEncoding("Windows-1250").GetBytes(TextToEncoding);
            byte[] pixels = ConvertBitmapToBytesArray(OrginalImage);
            int licznik;
            long codedwords=0;
            long maxindeks = pixels.Length - pixels.Length % 32;
            while (codedwords != TextInBytes.Length)
            {
                licznik = 0;
                for (int i = 0; i < pixels.Length; i++)
                {
                    if (licznik == 3)
                    {
                        licznik = 0;
                    }
                    else
                    {
                        int a = (int)pixels[i];
                        a = a & (255 - pozycja);
                        pixels[i] = (byte)a;
                        licznik++;
                    }
                }
                licznik = 0;
                long przeskok = 0;
                for (lastword = 0; lastword < TextToEncoding.Length-codedwords && lastword * 8 < maxindeks - przeskok - 1; lastword++)
                {
                    int n = TextInBytes[lastword + codedwords];
                    for (int j = 7; j >= 0; j--)
                    {
                        if (licznik == 3)
                        {
                            licznik = 0;
                            przeskok++;
                        }
                        int a = (int)pixels[(lastword + 1) * 8 - (j + 1) + przeskok];
                        int k = n >> j;
                        if( (k & 1)==1)
                        {
                            a += pozycja;
                        }
                        pixels[(lastword + 1) * 8 - (j + 1) + przeskok] = (byte)a;
                        licznik++;

                    }
                }
                codedwords += lastword;
                pozycja *= 2;
            }
            byte b= TextInBytes[lastword-1];
            byte[] temporary = new byte[100];
            for(int i = 100; i > 0; i--)
            {
                temporary[100 - i] = pixels[pixels.Length - i];
            }
            BitmapSource bitmap= ConvertByteArrayToBitmsapImage(OrginalImage,pixels);    
            return bitmap;
        }

        public static string DecodeMesseg(BitmapImage OrginalImage)
        {
            Queue<byte> Litery= new Queue<byte>();
            int h = OrginalImage.PixelHeight;
            int w = OrginalImage.PixelWidth;
            byte[] pixels = ConvertBitmapToBytesArray(OrginalImage);
            long maxindeks = pixels.Length - pixels.Length % 32;
            int przesuniecie;
            int licznik = 2;
            bool koniec = false;
            int pozycja = 0;
            int i = 74;
            while (!koniec) {     
                przesuniecie = 0;
                for (; !koniec && i < maxindeks - przesuniecie - 1; i += 8)
                { 
                    int dec_value = 0;
                    int base1 = 128;
                    for (int j = 7; j >= 0; j--)
                    {
                        if (licznik == 3)
                        {
                            licznik = 0;
                            przesuniecie++;
                        }
                        dec_value += ((pixels[i + 7 - j + przesuniecie]>>pozycja) % 2) * base1;

                        base1 = base1 / 2;
                        licznik++;
                    }
                    if (dec_value != 0)
                    {
                        Litery.Enqueue(((byte)dec_value));
                    }
                    else
                    {
                           koniec = true;
                    }
                }
                i = 0;
                licznik = 0;
                pozycja++;
            }
            string odkodowane= Encoding.GetEncoding("Windows-1250").GetString(Litery.ToArray());
            return odkodowane;
        }
    }
}
