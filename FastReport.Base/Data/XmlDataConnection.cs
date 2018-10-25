using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;
using FastReport.Utils;
using System.Data.Common;

namespace FastReport.Data
{
  /// <summary>
  /// Represents a connection to xml file-based database.
  /// </summary>
  /// <example>This example shows how to add a new connection to the report.
  /// <code>
  /// Report report1;
  /// XmlDataConnection conn = new XmlDataConnection();
  /// conn.XmlFile = @"c:\data.xml";
  /// report1.Dictionary.Connections.Add(conn);
  /// conn.CreateAllTables();
  /// </code>
  /// </example>
  public partial class XmlDataConnection : DataConnectionBase
  {
    #region Properties
    /// <summary>
    /// Gets or sets the path to .xsd file.
    /// </summary>
    [Category("Data")]
    public string XsdFile
    {
      get 
      { 
        XmlConnectionStringBuilder builder = new XmlConnectionStringBuilder(ConnectionString);
        return builder.XsdFile; 
      }
      set
      {
        XmlConnectionStringBuilder builder = new XmlConnectionStringBuilder(ConnectionString);
        builder.XsdFile = value;
        ConnectionString = builder.ToString();
      }
    }

    /// <summary>
    /// Gets or sets the path to .xml file.
    /// </summary>
    [Category("Data")]
    public string XmlFile
    {
      get 
      {
        XmlConnectionStringBuilder builder = new XmlConnectionStringBuilder(ConnectionString);
        return builder.XmlFile;
      }
      set
      {
        XmlConnectionStringBuilder builder = new XmlConnectionStringBuilder(ConnectionString);
        builder.XmlFile = value;
        ConnectionString = builder.ToString();
      }
    }
    #endregion

    #region Protected Methods
    /// <inheritdoc/>
    protected override DataSet CreateDataSet()
    {
      DataSet dataset = base.CreateDataSet();
      if (!String.IsNullOrEmpty(XsdFile))
        dataset.ReadXmlSchema(XsdFile);
      dataset.ReadXml(XmlFile);
      return dataset;
    }

    /// <inheritdoc/>
    protected override void SetConnectionString(string value)
    {
      DisposeDataSet();
      base.SetConnectionString(value);
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void FillTableSchema(DataTable table, string selectCommand, CommandParameterCollection parameters)
    {
      // do nothing
    }

    /// <inheritdoc/>
    public override void FillTableData(DataTable table, string selectCommand, CommandParameterCollection parameters)
    {
      // do nothing
    }

    /// <inheritdoc/>
    public override void CreateTable(TableDataSource source)
    {
      if (DataSet.Tables.Contains(source.TableName))
      {
        source.Table = DataSet.Tables[source.TableName];
        base.CreateTable(source);
      }
      else
        source.Table = null;
    }

    /// <inheritdoc/>
    public override void DeleteTable(TableDataSource source)
    {
      // do nothing
    }

    /// <inheritdoc/>
    public override string QuoteIdentifier(string value, DbConnection connection)
    {
      return value;
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlDataConnection"/> class with default settings.
    /// </summary>
    public XmlDataConnection()
    {
      IsSqlBased = false;
    }
  }
}
