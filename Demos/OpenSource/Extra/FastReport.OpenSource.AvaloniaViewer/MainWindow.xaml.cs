using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FastReport;
using FastReport.Export.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Viewer
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            imHeight = img.Height;
            imWidth = img.Width;
        }

        private Button OpenBtn;
        private Image img;
        private TextBox PageNumber;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            OpenBtn = this.FindControl<Button>("Open");
            OpenBtn.Click += OpenFile;

            this.img = this.FindControl<Image>("img");
            this.PageNumber = this.FindControl<TextBox>("PageNumber");
        }

        public List<Avalonia.Media.Imaging.Bitmap> pages = new List<Avalonia.Media.Imaging.Bitmap>();
        public Report Report
        {
            get { return report; }
            set
            {
                ex = new FastReport.Export.Image.ImageExport();
                ex.HasMultipleFiles = true;
                report = value;
                SetContent(report);
                SetImage();
            }
        }

        public int CurrentPage
        {
            get { return currentPage; }
            set
            {
                if (value >= 0 && value < pages.Count)
                    currentPage = value;
            }
        }

        private void SetContent(Report report)
        {
            DeleteTempFiles();
            ex.ImageFormat = ImageExportFormat.Png;
            ex.Resolution = 96;
            Random rnd = new Random();
            ex.Export(report, Directory.GetCurrentDirectory() + "/test." + rnd.Next(100) + ".png");
            foreach (string file in ex.GeneratedFiles)
            {
                //Avalonia.Media.Imaging.Bitmap image = new Avalonia.Media.Imaging.Bitmap();
                //image.BeginInit();
                //image.CacheOption = BitmapCacheOption.OnLoad;
                //image.UriSource = new Uri(file);
                //image.EndInit();
                pages.Add(new Avalonia.Media.Imaging.Bitmap(file));
            }
            CurrentPage = 0;


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

        public void SetImage()
        {
            //if (CurrentPage >= 0 && CurrentPage < pages.Count())
            //{
            //    im.Source = null;
            img.Source = pages[CurrentPage];
            //}
            img.Height = imHeight;
            img.Width = imWidth;
            PageNumber.Text = (CurrentPage + 1).ToString();
        }

        public void FirstClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (pages.Any())
            {
                CurrentPage = 0;
                SetImage();
            }
        }

        public void Prev_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (currentPage > 0)
            {
                CurrentPage--;
                SetImage();
            }
        }

        public void Next_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (CurrentPage >= 0 && CurrentPage < pages.Count())
            {
                CurrentPage++;
                SetImage();
            }
        }

        public void Last_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (pages.Any())
            {
                CurrentPage = pages.Count - 1;
                SetImage();
            }
        }

        public void PageNumber_KeyDown(object sender, KeyEventArgs e)
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

        private async void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filters.Add(new FileDialogFilter() { Name = "Подготовленные отчёты(*.FPX)", Extensions = new List<string> { "fpx" } });
            myDialog.AllowMultiple = false;

            var result = await myDialog.ShowAsync(this);

            if (result != null && result.Length > 0)
                LoadReport(result[0]);
        }

        void LoadReport(string report_name)
        {
            Report rep = new Report();
            rep.LoadPrepared(report_name);
            Report = rep;
        }


        public void Zoom_in_Click(object sender, RoutedEventArgs e)
        {
            if (ex != null)
            {
                ex.ImageFormat = ImageExportFormat.Png;
                ex.Resolution += 25;
                ex.PageNumbers = (CurrentPage + 1).ToString();
                //ex.Zoom += 0.25f;
                ex.Export(Report, Directory.GetCurrentDirectory() + "/testZoom.png");
                //Report.Export(ex, stream);

                if (CurrentPage >= 0 && CurrentPage < pages.Count)
                    img.Source = LoadImage(Directory.GetCurrentDirectory() + "/testZoom.png");
                //new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "/testZoom.png"));
                PageNumber.Text = (CurrentPage + 1).ToString();
            }
            img.Width += 50;
            img.Height += 50;
        }

        private static Avalonia.Media.Imaging.Bitmap LoadImage(string file)
        {
            return new Avalonia.Media.Imaging.Bitmap(file);
        }
        public void Zoom_out_Click(object sender, RoutedEventArgs e)
        {

            if (ex != null)
            {
                ex.ImageFormat = ImageExportFormat.Png;
                ex.Resolution -= 25;
                ex.PageNumbers = (CurrentPage + 1).ToString();
                //ex.Zoom += 0.25f;
                ex.Export(Report, Directory.GetCurrentDirectory() + "/testZoom.png");
                //Report.Export(ex, stream);

                if (CurrentPage >= 0 && CurrentPage < pages.Count)
                    img.Source = LoadImage(Directory.GetCurrentDirectory() + "/testZoom.png");
                //new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "/testZoom.png"));
                PageNumber.Text = (CurrentPage + 1).ToString();
            }
            img.Width -= 50;
            img.Height -= 50;
        }

        private Report report;
        private FastReport.Export.Image.ImageExport ex;
        private int currentPage = 0;
        private readonly double imHeight;
        private readonly double imWidth;
    }
}