using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FastReport.Data.ConnectionEditors;
using FastReport.Forms;
using FirebirdSql.Data.FirebirdClient;
using FastReport.Utils;

namespace FastReport.Data
{
  public partial class FirebirdConnectionEditor : ConnectionEditorBase
  {
    private string FConnectionString;

    private void tbDatabase_ButtonClick(object sender, EventArgs e)
    {
      using (OpenFileDialog dialog = new OpenFileDialog())
      {
        dialog.Filter = Res.Get("FileFilters,FdbFile");
        if (dialog.ShowDialog() == DialogResult.OK)
          tbDatabase.Text = dialog.FileName;
      }
    }

    private void btnAdvanced_Click(object sender, EventArgs e)
    {
      using (AdvancedConnectionPropertiesForm form = new AdvancedConnectionPropertiesForm())
      {
        FbConnectionStringBuilder builder = new FbConnectionStringBuilder(ConnectionString);
        form.AdvancedProperties = builder;
        if (form.ShowDialog() == DialogResult.OK)
          ConnectionString = form.AdvancedProperties.ToString();
      }
    }

    private void Localize()
    {
      MyRes res = new MyRes("ConnectionEditors,Common");
      gbDatabase.Text = res.Get("Database");
      lblDataFile.Text = res.Get("DatabaseFile");
      lblUserName.Text = res.Get("UserName");
      lblPassword.Text = res.Get("Password");
      btnAdvanced.Text = Res.Get("Buttons,Advanced");
      tbDatabase.Image = Res.GetImage(1);
    }

    protected override string GetConnectionString()
    {
      FbConnectionStringBuilder builder = new FbConnectionStringBuilder(FConnectionString);
      
      builder.Database = tbDatabase.Text;
      builder.UserID = tbUserName.Text;
      builder.Password = tbPassword.Text;
      
      return builder.ToString();
    }

    protected override void SetConnectionString(string value)
    {
      FConnectionString = value;
      FbConnectionStringBuilder builder = new FbConnectionStringBuilder(value);
      try
      {
        tbDatabase.Text = builder.Database;
      }
      catch
      {
        tbDatabase.Text = "";
      }
      try
      {
        tbUserName.Text = builder.UserID;
      }
      catch
      {
        tbUserName.Text = "";
      }
      try
      {
        tbPassword.Text = builder.Password;
      }
      catch
      {
        tbPassword.Text = "";
      }
    }

    public FirebirdConnectionEditor()
    {
      InitializeComponent();
      Localize();
    }
  }
}
