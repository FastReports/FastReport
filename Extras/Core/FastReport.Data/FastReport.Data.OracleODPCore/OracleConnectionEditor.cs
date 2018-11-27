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
using Oracle.ManagedDataAccess.Client;

namespace FastReport.Data
{
  public partial class OracleConnectionEditor : ConnectionEditorBase
  {
    private string FConnectionString;

    private void btnAdvanced_Click(object sender, EventArgs e)
    {
      using (AdvancedConnectionPropertiesForm form = new AdvancedConnectionPropertiesForm())
      {
        OracleConnectionStringBuilder builder = new OracleConnectionStringBuilder(ConnectionString);
        form.AdvancedProperties = builder;
        if (form.ShowDialog() == DialogResult.OK)
          ConnectionString = form.AdvancedProperties.ToString();
      }
    }

    private void Localize()
    {
      MyRes res = new MyRes("ConnectionEditors,Common");

      gbServer.Text = res.Get("ServerLogon");
      lblServer.Text = res.Get("Server");
      lblUserName.Text = res.Get("UserName");
      lblPassword.Text = res.Get("Password");

      btnAdvanced.Text = Res.Get("Buttons,Advanced");
    }

    private void InitServers()
    {
      OracleDataSourceEnumerator oraEnum = new OracleDataSourceEnumerator();
      try
      {
        DataTable dt = oraEnum.GetDataSources();
        foreach (DataRow row in dt.Rows)
        {
          cbxServer.Items.Add(row[0]);
        }
      }
      catch
      {
      }
    }

    protected override string GetConnectionString()
    {
      OracleConnectionStringBuilder builder = new OracleConnectionStringBuilder(FConnectionString);
      
      builder.DataSource = cbxServer.Text;
      builder.UserID = tbUserName.Text;
      builder.Password = tbPassword.Text;
      
      return builder.ToString();
    }

    protected override void SetConnectionString(string value)
    {
      FConnectionString = value;
      OracleConnectionStringBuilder builder = new OracleConnectionStringBuilder(value);
      
      cbxServer.Text = builder.DataSource;
      tbUserName.Text = builder.UserID;
      tbPassword.Text = builder.Password;
    }

    public OracleConnectionEditor()
    {
      InitializeComponent();
      Localize();
      InitServers();
    }
  }
}
