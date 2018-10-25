using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using FastReport.Utils;
using FastReport.Data;

namespace FastReport.Table
{
  /// <summary>
  /// Represents a table object that consists of several rows and columns.
  /// </summary>
  /// <remarks>
  /// <para/>To add/remove columns, use the <see cref="TableBase.Columns"/> collection. To add/remove 
  /// rows, use the <see cref="TableBase.Rows"/> collection. To initialize a table with specified number of
  /// columns and rows, use <see cref="ColumnCount"/> and <see cref="RowCount"/> properties.
  /// <para/>To print a table in code, use the <see cref="ManualBuild"/> event. In the manual build
  /// mode, you can use aggregate functions. The following functions available:
  /// <list type="table">
  ///   <listheader>
  ///     <term>Aggregate function</term>
  ///     <description>Description</description>
  ///   </listheader>
  ///   <item>
  ///     <term>Sum(cell)</term>
  ///     <description>Calculates the sum of values in specified table cell.</description>
  ///   </item>
  ///   <item>
  ///     <term>Min(cell)</term>
  ///     <description>Calculates the minimum of values in specified table cell.</description>
  ///   </item>
  ///   <item>
  ///     <term>Max(cell)</term>
  ///     <description>Calculates the maximum of values in specified table cell.</description>
  ///   </item>
  ///   <item>
  ///     <term>Avg(cell)</term>
  ///     <description>Calculates the average of values in specified table cell.</description>
  ///   </item>
  ///   <item>
  ///     <term>Count(cell)</term>
  ///     <description>Calculates the number of repeats of a specified table cell.</description>
  ///   </item>
  /// </list>
  /// <para/>To print aggregate value, place the aggregate function call in the table cell:
  /// <c>[Count(Cell2)]</c>. 
  /// </remarks>
  public partial class TableObject : TableBase
  {
    #region Fields
    private string manualBuildEvent;
    private TableHelper helper;
    private bool saveVisible;
    private bool saveStateSkipped;
    private bool manualBuildAutoSpans;
    #endregion

    #region Properties
    /// <summary>
    /// Allows to print table rows/columns dynamically.
    /// </summary>
    /// <remarks>
    /// This event is used to handle the table print process in a code. Using special methods
    /// like <see cref="PrintRow"/>, <see cref="PrintColumn"/> you can print specified rows/columns.
    /// 
    /// <para/>First way is to repeat specified row(s) to get a table that will grow downwards. 
    /// To do this, you have to call the <b>PrintRow</b> method followed by the <b>PrintColumns</b> method.
    /// 
    /// <para/>Another way is to repeat the specified column(s) to get a table that grows sidewards.
    /// To do this, call the <b>PrintColumn</b> method followed by the <b>PrintRows</b> method.
    /// 
    /// <para/>Finally, the third way is to repeat rows and columns. The table will grow downwards and
    /// sidewards. To do this, call the <b>PrintRow</b> method followed by the <b>PrintColumn</b>
    /// method (or vice versa).
    /// 
    /// <para/>
    /// <note type="caution">
    /// When you print a table row-by-row, you must call one of the <b>PrintColumn</b>,
    /// <b>PrintColumns</b> methods right after the <b>PrintRow</b> method. 
    /// In the same manner, when you print a table column-by-column, call one of the 
    /// <b>PrintRow</b>, <b>PrintRows</b> methods right after the <b>PrintColumn</b> method. 
    /// If you ignore this rule you will get an exception.
    /// </note>
    /// </remarks>
    /// <example>
    /// In this example, we will consider all three ways to print a table which has 3 rows and 3 columns.
    /// <para/>Case 1: print a table downwards.
    /// <code>
    /// // print table header (the first row)
    /// Table1.PrintRow(0);
    /// Table1.PrintColumns();
    /// // print table body (the second row)
    /// for (int i = 0; i &lt; 10; i++)
    /// {
    ///   Table1.PrintRow(1);
    ///   Table1.PrintColumns();
    /// }
    /// // print table footer (the third row)
    /// Table1.PrintRow(2);
    /// Table1.PrintColumns();
    /// </code>
    /// 
    /// <para/>Case 2: print a table sidewards.
    /// <code>
    /// // print table header (the first column)
    /// Table1.PrintColumn(0);
    /// Table1.PrintRows();
    /// // print table body (the second column)
    /// for (int i = 0; i &lt; 10; i++)
    /// {
    ///   Table1.PrintColumn(1);
    ///   Table1.PrintRows();
    /// }
    /// // print table footer (the third column)
    /// Table1.PrintColumn(2);
    /// Table1.PrintRows();
    /// </code>
    /// 
    /// <para/>Case 3: print a table downwards and sidewards.
    /// <code>
    /// // print the first row with all its columns
    /// Table1.PrintRow(0);
    /// // print header column
    /// Table1.PrintColumn(0);
    /// // print 10 data columns
    /// for (int i = 0; i &lt; 10; i++)
    /// {
    ///   Table1.PrintColumn(1);
    /// }
    /// // print footer column
    /// Table1.PrintColumn(2);
    /// 
    /// // print table body (the second row)
    /// for (int i = 0; i &lt; 10; i++)
    /// {
    ///   // print data row with all its columns
    ///   Table1.PrintRow(1);
    ///   Table1.PrintColumn(0);
    ///   for (int j = 0; j &lt; 10; j++)
    ///   {
    ///     Table1.PrintColumn(1);
    ///   }
    ///   Table1.PrintColumn(2);
    /// }
    /// 
    /// // print table footer (the third row)
    /// Table1.PrintRow(2);
    /// // again print all columns in the table footer
    /// Table1.PrintColumn(0);
    /// for (int i = 0; i &lt; 10; i++)
    /// {
    ///   Table1.PrintColumn(1);
    /// }
    /// Table1.PrintColumn(2);
    /// </code>
    /// </example>
    public event EventHandler ManualBuild;

    /// <summary>
    /// Gets or sets a script method name that will be used to handle the 
    /// <see cref="ManualBuild"/> event.
    /// </summary>
    /// <remarks>
    /// If you use this event, you must handle the table print process manually.
    /// See the <see cref="ManualBuild"/> event for details.
    /// </remarks>
    [Category("Build")]
    public string ManualBuildEvent
    {
      get { return manualBuildEvent; }
      set { manualBuildEvent = value; }
    }

    /// <summary>
    /// Determines whether to manage cell spans automatically during manual build.
    /// </summary>
    /// <remarks>
    /// The default value for this property is <b>true</b>. If you set it to <b>false</b>, you need to manage
    /// spans in your ManualBuild event handler.
    /// </remarks>
    [Category("Build")]
    [DefaultValue(true)]
    public bool ManualBuildAutoSpans
    {
      get { return manualBuildAutoSpans; }
      set { manualBuildAutoSpans = value; }
    }

    /// <inheritdoc/>
    public override int ColumnCount
    {
      get { return base.ColumnCount; }
      set
      {
        base.ColumnCount = value;
        CreateUniqueNames();
      }
    }

    /// <inheritdoc/>
    public override int RowCount
    {
      get { return base.RowCount; }
      set
      {
        base.RowCount = value;
        CreateUniqueNames();
      }
    }

    internal bool IsManualBuild
    {
      get { return !String.IsNullOrEmpty(ManualBuildEvent) || ManualBuild != null; }
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Assign(Base source)
    {
      base.Assign(source);

      TableObject src = source as TableObject;
      ManualBuildEvent = src.ManualBuildEvent;
      ManualBuildAutoSpans = src.ManualBuildAutoSpans;
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      TableObject c = writer.DiffObject as TableObject;
      base.Serialize(writer);

      if (ManualBuildEvent != c.ManualBuildEvent)
        writer.WriteStr("ManualBuildEvent", ManualBuildEvent);
      if (ManualBuildAutoSpans != c.ManualBuildAutoSpans)
        writer.WriteBool("ManualBuildAutoSpans", ManualBuildAutoSpans);
    }
    #endregion

    #region Report Engine
    private string GetFunctionCode(string function)
    {
      string result = "";

      switch (Report.ScriptLanguage)
      {
        case Language.CSharp:
          result =
            "    private object " + function + "(TableCell cell)\r\n" +
            "    {\r\n" +
            "      return cell.Table." + function + "(cell);\r\n" +
            "    }\r\n\r\n";
          break;

        case Language.Vb:
          result =
            "    Private Function " + function + "(ByVal cell As TableCell) As Object\r\n" +
            "      Return cell.Table." + function + "(cell)\r\n" +
            "    End Function\r\n\r\n";
          break;
      }

      return result;
    }

    /// <inheritdoc/>
    public override string GetCustomScript()
    {
      string result = "";
      string[] functions = new string[] { "Sum", "Min", "Max", "Avg", "Count" };
      
      foreach (string function in functions)
      {
        result += GetFunctionCode(function);
      }
      
      return result;
    }

    /// <inheritdoc/>
    public override void SaveState()
    {
      saveVisible = Visible;
      BandBase parent = Parent as BandBase;
      saveStateSkipped = !Visible || (parent != null && !parent.Visible);
      if (saveStateSkipped)
        return;

      if (!IsManualBuild)
      {
        base.SaveState();
      }
      else
      {
        // create the result table that will be rendered in the preview
        SetResultTable(new TableResult());
        ResultTable.Assign(this);
        ResultTable.SetReport(Report);
        helper = new TableHelper(this, ResultTable);

        Visible = false;

        if (parent != null)
        {
          parent.Height = Top;
          parent.CanGrow = false;
          parent.CanShrink = false;
          parent.AfterPrint += new EventHandler(ResultTable.GeneratePages);
        }
        
        OnManualBuild(EventArgs.Empty);
      }
    }

    /// <inheritdoc/>
    public override void RestoreState()
    {
      BandBase parent = Parent as BandBase;
      // SaveState was skipped, there is nothing to restore
      if (saveStateSkipped)
        return;

      if (!IsManualBuild)
      {
        base.RestoreState();
      }
      else
      {
        if (parent != null)
          parent.AfterPrint -= new EventHandler(ResultTable.GeneratePages);

        helper = null;
        ResultTable.Dispose();
        SetResultTable(null);
        Visible = saveVisible;
      }
    }

    /// <inheritdoc/>
    public override void GetData()
    {
      base.GetData();
      
      if (!IsManualBuild)
      {
        for (int y = 0; y < Rows.Count; y++)
        {
          for (int x = 0; x < Columns.Count; x++)
          {
            this[x, y].GetData();
          }
        }
      }
    }

    /// <summary>
    /// This method fires the <b>ManualBuild</b> event and the script code connected to the <b>ManualBuildEvent</b>.
    /// </summary>
    /// <param name="e">Event data.</param>
    public void OnManualBuild(EventArgs e)
    {
      if (ManualBuild != null)
        ManualBuild(this, e);
      InvokeEvent(ManualBuildEvent, e);
    }

    /// <summary>
    /// Prints a row with specified index.
    /// </summary>
    /// <param name="index">Index of a row to print.</param>
    /// <remarks>
    /// See the <see cref="ManualBuild"/> event for more details.
    /// </remarks>
    public void PrintRow(int index)
    {
      if (!IsManualBuild)
        throw new TableManualBuildException();
      helper.PrintRow(index);
    }
    
    /// <summary>
    /// Prints rows with specified indices.
    /// </summary>
    /// <param name="indices">Indices of rows to print.</param>
    /// <remarks>
    /// See the <see cref="ManualBuild"/> event for more details.
    /// </remarks>
    public void PrintRows(int[] indices)
    {
      foreach (int index in indices)
      {
        PrintRow(index);
      }
    }

    /// <summary>
    /// Prints all rows.
    /// </summary>
    /// <remarks>
    /// See the <see cref="ManualBuild"/> event for more details.
    /// </remarks>
    public void PrintRows()
    {
      for (int i = 0; i < Rows.Count; i++)
      {
        PrintRow(i);
      }
    }

    /// <summary>
    /// Prints a column with specified index.
    /// </summary>
    /// <param name="index">Index of a column to print.</param>
    /// <remarks>
    /// See the <see cref="ManualBuild"/> event for more details.
    /// </remarks>
    public void PrintColumn(int index)
    {
      if (!IsManualBuild)
        throw new TableManualBuildException();
      helper.PrintColumn(index);
    }

    /// <summary>
    /// Prints columns with specified indices.
    /// </summary>
    /// <param name="indices">Indices of columns to print.</param>
    /// <remarks>
    /// See the <see cref="ManualBuild"/> event for more details.
    /// </remarks>
    public void PrintColumns(int[] indices)
    {
      foreach (int index in indices)
      {
        PrintColumn(index);
      }
    }

    /// <summary>
    /// Prints all columns.
    /// </summary>
    /// <remarks>
    /// See the <see cref="ManualBuild"/> event for more details.
    /// </remarks>
    public void PrintColumns()
    {
      for (int i = 0; i < Columns.Count; i++)
      {
        PrintColumn(i);
      }
    }
    
    /// <summary>
    /// Adds a page before rows or columns.
    /// </summary>
    /// <remarks>
    /// Call this method to insert a page break before the next row or column that you intend to print
    /// using <b>PrintRow(s)</b> or <b>PrintColumn(s)</b> methods.
    /// See the <see cref="ManualBuild"/> event for more details.
    /// </remarks>
    public void PageBreak()
    {
      if (!IsManualBuild)
        throw new TableManualBuildException();
      if (!Report.Engine.UnlimitedHeight && !Report.Engine.UnlimitedWidth)
        helper.PageBreak();
    }
    #endregion
    
    /// <summary>
    /// Initializes a new instance of the <see cref="TableObject"/> class.
    /// </summary>
    public TableObject()
    {
      manualBuildEvent = "";
      manualBuildAutoSpans = true;
    }
  }
}
