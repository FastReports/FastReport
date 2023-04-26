#undef FRCORE

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FastReport.Data.ConnectionEditors;
using FastReport.Forms;
using FastReport.Utils;
#if FRCORE
using Microsoft.Data.Sqlite;
#else
using System.Data.SQLite;
#endif

namespace FastReport.Data
{
    public partial class SQLiteConnectionEditor : ConnectionEditorBase
    {
        private string FConnectionString;

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            using (AdvancedConnectionPropertiesForm form = new AdvancedConnectionPropertiesForm())
            {
#if FRCORE
                var builder = new SqliteConnectionStringBuilder(ConnectionString);
#else
                var builder = new SQLiteConnectionStringBuilder(ConnectionString);
#endif
                form.AdvancedProperties = builder;
                if (form.ShowDialog() == DialogResult.OK)
                    ConnectionString = form.AdvancedProperties.ToString();
            }
        }

        private void Localize()
        {
            MyRes res = new MyRes("ConnectionEditors,Odbc");

            lblDataSource.Text = res.Get("DataSource");
			
			res = new MyRes("ConnectionEditors,Common");

            gbDatabase.Text = res.Get("Database");
            btnAdvanced.Text = Res.Get("Buttons,Advanced");
        }

        protected override string GetConnectionString()
        {
#if FRCORE
            var builder = new SqliteConnectionStringBuilder(FConnectionString);
#else
            var builder = new SQLiteConnectionStringBuilder(FConnectionString);
#endif
            builder.DataSource = tbDataSource.Text;

            return builder.ToString();
        }

        protected override void SetConnectionString(string value)
        {
            FConnectionString = value;
#if FRCORE
            var builder = new SqliteConnectionStringBuilder(value);
#else
            var builder = new SQLiteConnectionStringBuilder(value);
#endif

            tbDataSource.Text = builder.DataSource;
        }

        public SQLiteConnectionEditor()
        {
            InitializeComponent();
            Localize();
        }

        private void btBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "*.db|*.db|*.db3|*.db3|*.*|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                    tbDataSource.Text = dlg.FileName;
            }
        }
    }
}
