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
using MySql.Data.MySqlClient;

namespace FastReport.Data
{
  public partial class MySqlConnectionEditor : ConnectionEditorBase
  {
    private string FConnectionString;

    private void btnAdvanced_Click(object sender, EventArgs e)
    {
      using (AdvancedConnectionPropertiesForm form = new AdvancedConnectionPropertiesForm())
      {
        MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(ConnectionString);
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

      gbDatabase.Text = res.Get("Database");
      lblDatabase.Text = res.Get("DatabaseName");
      btnAdvanced.Text = Res.Get("Buttons,Advanced");
    }

    protected override string GetConnectionString()
    {
      MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(FConnectionString);
      
      builder.Server = tbServer.Text;
      builder.UserID = tbUserName.Text;
      builder.Password = tbPassword.Text;
      builder.Database = tbDatabase.Text;
      
      return builder.ToString();
    }

    protected override void SetConnectionString(string value)
    {
      FConnectionString = value;
      MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(value);
      
      tbServer.Text = builder.Server;
      tbUserName.Text = builder.UserID;
      tbPassword.Text = builder.Password;
      tbDatabase.Text = builder.Database;
    }

    public MySqlConnectionEditor()
    {
      InitializeComponent();
      Localize();
    }
  }
}
