using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Data;
using FastReport.Export;
using System.Data.Common;
using System.ComponentModel;

namespace FastReport
{
  /// <summary>
  /// Provides data for the <see cref="FastReport.Report.LoadBaseReport"/> event.
  /// </summary>
  public class CustomLoadEventArgs : EventArgs
  {
    private string fileName;
    private Report report;

    /// <summary>
    /// Gets a name of the file to load the report from.
    /// </summary>
    public string FileName
    {
      get { return fileName; }
    }

    /// <summary>
    /// The reference to a report.
    /// </summary>
    public Report Report
    {
      get { return report; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomLoadEventArgs"/> class using the specified
    /// file name and the report.
    /// </summary>
    /// <param name="fileName">The name of the file to load the report from.</param>
    /// <param name="report">The report.</param>
    public CustomLoadEventArgs(string fileName, Report report)
    {
            this.fileName = fileName;
            this.report = report;
    }
  }

  /// <summary>
  /// Provides data for the <see cref="FastReport.Report.CustomCalc"/> event.
  /// </summary>
  public class CustomCalcEventArgs : EventArgs
  {
    private string expr;
    private object @object;
    private Report report;

    /// <summary>
    /// Gets an expression.
    /// </summary>
    public string Expression
    {
      get { return expr; }
    }

    /// <summary>
    /// Gets or sets a object.
    /// </summary>
    public object CalculatedObject
    {
      get { return @object; }
      set { @object = value; }
    }

    /// <summary>
    /// The reference to a report.
    /// </summary>
    public Report Report
    {
      get { return report; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomLoadEventArgs"/> class using the specified
    /// file name and the report.
    /// </summary>
    /// <param name="expression">The text of expression.</param>
    /// <param name="Object">The name of the file to load the report from.</param>
    /// <param name="report">The report.</param>
    public CustomCalcEventArgs(string expression, object Object, Report report)
    {
      expr = expression;
      @object = Object;
            this.report = report;
    }
  }

  /// <summary>
  /// Represents the method that will handle the <see cref="Report.LoadBaseReport"/> event.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void CustomLoadEventHandler(object sender, CustomLoadEventArgs e);

  /// <summary>
  /// Represents the method that will handle the event.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void CustomCalcEventHandler(object sender, CustomCalcEventArgs e);

  /// <summary>
  /// Provides data for the Progress event.
  /// </summary>
  public class ProgressEventArgs
  {
    private string message;
    private int progress;
    private int total;
    
    /// <summary>
    /// Gets a progress message.
    /// </summary>
    public string Message
    {
      get { return message; }
    }
    
    /// <summary>
    /// Gets the current page number.
    /// </summary>
    public int Progress
    {
      get { return progress; }
    }
    
    /// <summary>
    /// Gets the number of total pages.
    /// </summary>
    public int Total
    {
      get { return total; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressEventArgs"/> class using the specified
    /// message, page number and total number of pages.
    /// </summary>
    /// <param name="message">The progress message.</param>
    /// <param name="progress">Current page number.</param>
    /// <param name="total">Number of total pages.</param>
    public ProgressEventArgs(string message, int progress, int total)
    {
            this.message = message;
            this.progress = progress;
            this.total = total;
    }
  }

  /// <summary>
  /// Represents the method that will handle the Progress event.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);


  /// <summary>
  /// Provides data for the DatabaseLogin event.
  /// </summary>
  public class DatabaseLoginEventArgs
  {
    private string connectionString;
    private string userName;
    private string password;
    
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string ConnectionString
    {
      get { return connectionString; }
      set { connectionString = value; }
    }
    
    /// <summary>
    /// Gets or sets an user name.
    /// </summary>
    public string UserName
    {
      get { return userName; }
      set { userName = value; }
    }
    
    /// <summary>
    /// Gets or sets a password.
    /// </summary>
    public string Password
    {
      get { return password; }
      set { password = value; }
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseLoginEventArgs"/> class using the specified
    /// connection string.
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    public DatabaseLoginEventArgs(string connectionString)
    {
            this.connectionString = connectionString;
      userName = "";
      password = "";
    }
  }


  /// <summary>
  /// Represents the method that will handle the DatabaseLogin event.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void DatabaseLoginEventHandler(object sender, DatabaseLoginEventArgs e);


  /// <summary>
  /// Provides data for the AfterDatabaseLogin event.
  /// </summary>
  public class AfterDatabaseLoginEventArgs
  {
    private DbConnection connection;
    
    /// <summary>
    /// Gets the <b>DbConnection</b> object.
    /// </summary>
    public DbConnection Connection
    {
      get { return connection; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AfterDatabaseLoginEventArgs"/> class using 
    /// the specified connection.
    /// </summary>
    /// <param name="connection">The connection object.</param>
    public AfterDatabaseLoginEventArgs(DbConnection connection)
    {
            this.connection = connection;
    }
  }

  /// <summary>
  /// Represents the method that will handle the AfterDatabaseLogin event.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void AfterDatabaseLoginEventHandler(object sender, AfterDatabaseLoginEventArgs e);


  /// <summary>
  /// Provides data for the FilterProperties event.
  /// </summary>
  public class FilterPropertiesEventArgs
  {
    private PropertyDescriptor property;
    private bool skip;

    /// <summary>
    /// Gets the property descriptor.
    /// </summary>
    public PropertyDescriptor Property
    {
      get { return property; }
      set { property = value; }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether this property should be skipped.
    /// </summary>
    public bool Skip
    {
      get { return skip; }
      set { skip = value; }
    }

    internal FilterPropertiesEventArgs(PropertyDescriptor property)
    {
            this.property = property;
      skip = false;
    }
  }

  /// <summary>
  /// Represents the method that will handle the FilterProperties event.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void FilterPropertiesEventHandler(object sender, FilterPropertiesEventArgs e);


  /// <summary>
  /// Provides data for the GetPropertyKind event.
  /// </summary>
  public class GetPropertyKindEventArgs
  {
    private string propertyName;
    private Type propertyType;
    private PropertyKind propertyKind;

    /// <summary>
    /// Gets the property name.
    /// </summary>
    public string PropertyName
    {
      get { return propertyName; }
    }

    /// <summary>
    /// Gets the property type.
    /// </summary>
    public Type PropertyType
    {
      get { return propertyType; }
    }

    /// <summary>
    /// Gets or sets the kind of property.
    /// </summary>
    public PropertyKind PropertyKind
    {
      get { return propertyKind; }
      set { propertyKind = value; }
    }

    internal GetPropertyKindEventArgs(string propertyName, Type propertyType, PropertyKind propertyKind)
    {
            this.propertyName = propertyName;
            this.propertyType = propertyType;
            this.propertyKind = propertyKind;
    }
  }

  /// <summary>
  /// Represents the method that will handle the GetPropertyKind event.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void GetPropertyKindEventHandler(object sender, GetPropertyKindEventArgs e);


  /// <summary>
  /// Provides data for the GetTypeInstance event.
  /// </summary>
  public class GetTypeInstanceEventArgs
  {
    private Type type;
    private object instance;

    /// <summary>
    /// Gets the type.
    /// </summary>
    public Type Type
    {
      get { return type; }
    }

    /// <summary>
    /// Gets or sets the instance of type.
    /// </summary>
    public object Instance
    {
      get { return instance; }
      set { instance = value; }
    }

    internal GetTypeInstanceEventArgs(Type type)
    {
            this.type = type;
    }
  }

  /// <summary>
  /// Represents the method that will handle the GetPropertyKind event.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void GetTypeInstanceEventHandler(object sender, GetTypeInstanceEventArgs e);

    /// <summary>
    /// Event arguments for custom Export parameters
    /// </summary>
    public class ExportParametersEventArgs : EventArgs
    {
        /// <summary>
        /// Used to set custom export parameters
        /// </summary>
        public readonly ExportBase Export;

        public ExportParametersEventArgs(ExportBase export)
        {
            this.Export = export;
        }
    }
}
