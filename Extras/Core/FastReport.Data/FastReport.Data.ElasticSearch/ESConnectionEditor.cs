using FastReport.Data.ElasticSearch;
using FastReport.Data.JsonConnection;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FastReport.Data.ConnectionEditors
{
    public partial class ESConnectionEditor : ConnectionEditorBase
    {

        public ESConnectionEditor()
        {
            InitializeComponent();
        }

        protected override string GetConnectionString()
        {
            ESDataSourceConnectionStringBuilder builder = new ESDataSourceConnectionStringBuilder();
            builder.EndPoint = tbEndPoint.Text;
            for (int i = 0; i < dgvHeaders.Rows.Count; i++)
            {
                DataGridViewRow row = dgvHeaders.Rows[i];
                if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                {
                    var headerName = row.Cells[0].Value.ToString();
                    var headerData = row.Cells[1].Value.ToString();
                    builder.Headers.Add(headerName, headerData);
                }
            }

            return builder.ToString();
        }

        protected override void SetConnectionString(string value)
        {
            ESDataSourceConnectionStringBuilder builder = new ESDataSourceConnectionStringBuilder(value);
            tbEndPoint.Text = builder.EndPoint;

            foreach (KeyValuePair<string, string> header in builder.Headers)
            {
                dgvHeaders.Rows.Add(header.Key, header.Value);
            }
        }
    }
}
