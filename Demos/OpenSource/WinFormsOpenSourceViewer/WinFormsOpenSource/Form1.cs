using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastReport;
using FastReport.Export.Image;
using FastReport.Utils;

namespace WinFormsOpenSource
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadReportList();
            pictureBox1.Width = 793;
            pictureBox1.Height = 1122;
        }

        private Report report = new Report();
        public int width;
        public int height;
        public ImageExport exp = new ImageExport(); //Создаем экспорт отчета в формат изображения
        public string reportsPath = Config.ApplicationFolder + @"..\..\Reports\";
        public int CurrentPage = 0;
        public List<Image> pages = new List<Image>();

        public Report Report
        {
            get
            {
                return report;
            }
            set
            {
                report = value;
            }
        }

        public void LoadReportList()
        {
            List<string> filesname = Directory.GetFiles(reportsPath, "*.frx").ToList<string>();

            foreach (string file in filesname)
            {
                ReportsList.Items.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReportsList.SelectedItem != null)
            {
                    Report.Load(reportsPath + ReportsList.SelectedItem.ToString() + ".frx"); //Загружаем шаблон отчета
                    DataSet data = new DataSet(); //Создаем источник данных
                    data.ReadXml(reportsPath + "nwind.xml"); //Загружаем в источник данных базу
                    Report.RegisterData(data, "NorthWind"); //Регистрируем источник данных в отчете 
                    Report.Prepare(); //Выполняем предварительное построение отчета
                    ReportExport();
                    ShowReport();
            }
        }


        private void toolStripOpnBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Prapared reprt(*.fpx)|*.fpx";
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {                   
                    Report.LoadPrepared(dialog.FileName);
                    ReportExport();
                    ShowReport();
                }
            }
        }

        public void ReportExport()
        {
            DeleteTempFiles();
                exp.ImageFormat = ImageExportFormat.Png; //Устанавливаем для изображения формат png
                exp.Export(Report, Directory.GetCurrentDirectory() + "/" + System.Guid.NewGuid() + ".png"); //Выполняем экспорт отчета в файл
                foreach (string fileName in exp.GeneratedFiles)
                {
                    using (var img = File.OpenRead(fileName))
                    {
                        pages.Add(Image.FromStream(img));
                    }
                }
                CurrentPage = 0;
        }

        public void DeleteTempFiles()
        {
            pages.Clear();
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
            FileInfo[] path = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("*.png", SearchOption.AllDirectories);
            foreach (FileInfo file in path)
            {
                
                File.Delete(file.FullName);
            }
            
        }


        public void ShowReport()
        {
            if (CurrentPage >= 0 && CurrentPage < pages.Count)
            {
                pictureBox1.Image = pages[CurrentPage]; //Устанавливаем изображение
                toolStripPageNum.Text = (CurrentPage + 1).ToString();
            }
        }

        private void toolStripFirstBtn_Click(object sender, EventArgs e)
        {
            CurrentPage = 0;
            ShowReport();
        }

        private void toolStripPrewBtn_Click(object sender, EventArgs e)
        {
            CurrentPage--;
            ShowReport();
        }

        private void toolStripNextBtn_Click(object sender, EventArgs e)
        {
            CurrentPage++;
            ShowReport();
        }

        private void toolStripLastBtn_Click(object sender, EventArgs e)
        {
            CurrentPage = pages.Count - 1;
            ShowReport();
        }

        private void toolStripPageNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int pageNum = Convert.ToInt16(toolStripPageNum.Text) - 1;
                if (pageNum >= 0 && pageNum < pages.Count)
                {
                    CurrentPage = pageNum;
                    ShowReport();
                }
            }        
        }


        private void toolStripZoomIn_Click(object sender, EventArgs e)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                
                exp.ImageFormat = ImageExportFormat.Png; //Устанавливаем для изображения формат png
                exp.Resolution += 25;
                exp.PageNumbers = (CurrentPage+1).ToString();
                exp.Export(Report, stream); //Выполняем экспорт отчета в файл
                pictureBox1.Image = Image.FromStream(stream);
            }
            pictureBox1.Width += 25;
            pictureBox1.Height += 25;
        }

        private void toolStripZoomOut_Click(object sender, EventArgs e)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                exp.ImageFormat = ImageExportFormat.Png; //Устанавливаем для изображения формат png
                exp.PageNumbers = (CurrentPage + 1).ToString();
                exp.Export(Report, stream); //Выполняем экспорт отчета в файл
                pictureBox1.Image = Image.FromStream(stream);
            }
            pictureBox1.Width -= 25;
            pictureBox1.Height -= 25;
        }
    }
}
