using System;
using System.Data;
using System.IO;

namespace MVC.Service
{
    public class DataSetService
    {
        public string ReportsPath { get; private set; }
        public DataSet DataSet { get; private set; } = new DataSet();

        public DataSetService()
        {
            SetReportsFolder();
            SetDataSet();
        }

        private void SetReportsFolder() => ReportsPath = FindReportsFolder(Environment.CurrentDirectory);
        private void SetDataSet() => DataSet.ReadXml(Path.Combine(ReportsPath, "nwind.xml"));

        private string FindReportsFolder(string startDir)
        {
            string directory = Path.Combine(startDir, "Reports");
            if (Directory.Exists(directory))
                return directory;

            for (int i = 0; i < 6; i++)
            {
                startDir = Path.Combine(startDir, "..");
                directory = Path.Combine(startDir, "Reports");
                if (Directory.Exists(directory))
                    return directory;
            }

            throw new Exception("Demos/Reports directory is not found");
        }
    }
}