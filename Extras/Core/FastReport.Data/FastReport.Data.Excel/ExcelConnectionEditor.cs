using System;
using System.Windows.Forms;
using FastReport.Data.ConnectionEditors;
using FastReport.Utils;


namespace FastReport.Data
{
    public partial class ExcelConnectionEditor : ConnectionEditorBase
    {

        private void Localize()
        {
            gbSelect.Text = Res.Get("ConnectionEditors,Common,Database");
        }

        protected override string GetConnectionString()
        {
            return tbExcelFile.Text;
        }

        private void TbExcelFile_ButtonClick(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "*.xlsx|*.xlsx|*.*|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    tbExcelFile.Text = dialog.FileName;
                }
            }
        }

        public override void UpdateDpiDependencies()
        {
            base.UpdateDpiDependencies();
            tbExcelFile.Image = this.GetImage(1);
        }

        protected override void SetConnectionString(string value)
        {
            tbExcelFile.Text = value;
        }

        public ExcelConnectionEditor()
        {
            InitializeComponent();
            Localize();
        }
    }
}
