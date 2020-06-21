using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;

namespace WpfApp5
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static void SaveClipboardImageToFile(string filePath,BitmapSource image)
        {
        
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fileStream);
            }
        }
        private void Scaling(System.Windows.Controls.Image image)
        {
            double MaxWidth = 300;
            double MaxHeight = 300;
            double DiferneceSourceWidthAndMaxWidth= image.Source.Width / MaxWidth; ;
            double DiferneceSourceHeightAndMaxHeight= image.Source.Height / MaxHeight;

            if (DiferneceSourceWidthAndMaxWidth >= DiferneceSourceHeightAndMaxHeight)
            {
                image.Width = image.Source.Width / DiferneceSourceWidthAndMaxWidth;
                image.Height = image.Source.Height / DiferneceSourceWidthAndMaxWidth;
            }
            else
            {
                image.Width = image.Source.Width / DiferneceSourceHeightAndMaxHeight;
                image.Height = image.Source.Height / DiferneceSourceHeightAndMaxHeight;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog
            {
                Title = "Select a picture",
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png"
            };
            if (op.ShowDialog() == true)
            {
                ObrazekOrginalny.Source = new BitmapImage(new Uri(op.FileName));
                long length = new System.IO.FileInfo(op.FileName).Length;
                Scaling(ObrazekOrginalny);

                ObrazekZakodowany.Height = ObrazekOrginalny.Height;
                ObrazekZakodowany.Width = ObrazekOrginalny.Width;
                RamkaOrginalny.UpdateLayout();
                RamkaZaodowany.UpdateLayout();

            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog
            {
                Title = "Select a picture",
                Filter = "text files (*.txt)|*.txt"
            };
            if (op.ShowDialog() == true)
            {
                string text = File.ReadAllText(op.FileName);
                TextDoZakodowania.Text = text;
            }
        }

        private void Zakoduj_Click(object sender, RoutedEventArgs e)
        {

            BitmapSource zakodowany =Stereografymoja.CryptoMesseg(ObrazekOrginalny.Source as BitmapImage,TextDoZakodowania.Text);
            SaveClipboardImageToFile(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"\\temporary.png",zakodowany);
            ObrazekZakodowany.Source = new BitmapImage(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\temporary.png"));
        }

        private void Odkoduj_Click(object sender, RoutedEventArgs e)
        {
            string textOdkoodwany;
            if (Stereografymoja.IsEncoded(ObrazekOrginalny.Source as BitmapImage))
            {
                textOdkoodwany = Stereografymoja.DecodeMesseg(ObrazekZakodowany.Source as BitmapImage);
            }
        }
    }
}
