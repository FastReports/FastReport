using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FastReport;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using FastReport.Export.Image;

namespace OpenSourceCustomViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            imHeight = im.Height;
            imWidth = im.Width;
        }

        public List<BitmapImage> pages = new List<BitmapImage>();
        public Report Report
        {
            get { return report; }
            set
            {
                ex = new FastReport.Export.Image.ImageExport();
                ex.HasMultipleFiles = HasMultipleFiles;
                report = value;
                SetContent(report);
                SetImage();
            }
        }

        public void SetImage()
        {
            im.Source = pages[CurrentPage];
            im.Height = imHeight;
            im.Width = imWidth;
            PageNumber.Text = (CurrentPage + 1).ToString();
        }

        public int CurrentPage
        {
            get { return currentPage;  }
            set {
                if (value >= 0 && value < pages.Count())
                currentPage = value;
            }
        }
        public bool HasMultipleFiles
        {
            get { return hasMultipleFiles; }
            set
            {
                hasMultipleFiles = value;
                ex.HasMultipleFiles = value;
            }
        }

        private void SetContent(Report report)
        {
            DeleteTempFiles();
            ex.ImageFormat = ImageExportFormat.Png;
            ex.ResolutionX = 96;
            ex.ResolutionY = 96;
            Random rnd = new Random();
            ex.Export(report, Directory.GetCurrentDirectory() + "/test." + rnd.Next(100) + ".png");
            foreach (string file in ex.GeneratedFiles)
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(file);
                image.EndInit();
                pages.Add(image);                    
            }
            CurrentPage = 0;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenReportFile();
        }

        public void DeleteTempFiles()
        {
            FileInfo[] path = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("*test*", SearchOption.AllDirectories);
            pages.Clear();
            foreach (FileInfo file in path)
            {
                 File.Delete(file.FullName);
            }
         }

        void OpenReportFile()
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = "Подготовленные отчёты(*.FPX)|*.fpx;";
            myDialog.CheckFileExists = true;
            myDialog.Multiselect = false;
            if (myDialog.ShowDialog() == true)
            {
                LoadReport(myDialog.FileName);
            }
        }

        void LoadReport(string report_name)
        {
            Report rep = new Report();
            rep.LoadPrepared(report_name);
            Report = rep;
        }

        private Report report;
        private FastReport.Export.Image.ImageExport ex;
        private bool hasMultipleFiles;
        private int currentPage = 0;
        private double imHeight;
        private double imWidth;

        private void Zoom_in_Click(object sender, RoutedEventArgs e)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (ex != null)
                {
                    ex.ImageFormat = ImageExportFormat.Png;
                    ex.Resolution += 25;
                    ex.PageNumbers = (CurrentPage + 1).ToString();
                    Report.Export(ex, stream);

                    if (CurrentPage >= 0 && CurrentPage < pages.Count())
                        im.Source = LoadImage(stream);
                    PageNumber.Text = (CurrentPage + 1).ToString();
                }
            }
            im.Width += 50;
            im.Height += 50;
        }

        private static BitmapImage LoadImage(Stream stream)
        {
            var image = new BitmapImage();

            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();
            return image;
        }

        private void First_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = 0;
            SetImage();
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage--;
            SetImage();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage++;
            SetImage();
        }

        private void Last_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = pages.Count() - 1;
            SetImage();
        }

        private void PageNumber_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                if (int.Parse(PageNumber.Text) > 0)
                {
                    CurrentPage = int.Parse(PageNumber.Text) - 1;
                    SetImage();
                }
            }
        }

        private void Zoom_out_Click(object sender, RoutedEventArgs e)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (ex != null)
                {
                    ex.ImageFormat = ImageExportFormat.Png;
                    ex.PageNumbers = (CurrentPage + 1).ToString();
                    Report.Export(ex, stream);

                    if (CurrentPage >= 0 && CurrentPage < pages.Count())
                        im.Source = LoadImage(stream);
                    PageNumber.Text = (CurrentPage + 1).ToString();
                }
            }
            im.Width -= 50;
            im.Height -= 50;
        }

    }
}
