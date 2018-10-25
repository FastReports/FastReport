using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace FastReport.Data
{
  /// <summary>
  /// Represents the collection of system variables.
  /// </summary>
  public class SystemVariables : ParameterCollection
  {
    internal SystemVariables(Base owner) : base(owner)
    {
      Add(new DateVariable());
      Add(new PageVariable());
      Add(new TotalPagesVariable());
      Add(new PageNVariable());
      Add(new PageNofMVariable());
      Add(new RowVariable());
      Add(new AbsRowVariable());
      Add(new PageMacroVariable());
      Add(new TotalPagesMacroVariable());
      Add(new CopyNameMacroVariable());
      Add(new HierarchyLevelVariable());
      Add(new HierarchyRowNoVariable());
    }
  }


  /// <summary>
  /// Represents the base class for system variables.
  /// </summary>
  public class SystemVariable : Parameter
  {
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new string Expression
    {
      get { return base.Expression; }
      set { base.Expression = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new string Description
    {
      get { return base.Description; }
      set { base.Description = value; }
    }

    /// <inheritdoc/>
    public override bool CanContain(Base child)
    {
      return false;
    }
  }

  /// <summary>
  /// Returns date and time of the report's start.
  /// </summary>
  public class DateVariable : SystemVariable
  {
    /// <inheritdoc/>
    public override object Value
    {
      get { return Report.Engine.Date; }
    }

    internal DateVariable()
    {
      Name = "Date";
      DataType = typeof(DateTime);
    }
  }

  /// <summary>
  /// Returns current page number.
  /// </summary>
  public class PageVariable : SystemVariable
  {
    /// <inheritdoc/>
    public override object Value
    {
      get { return Report.Engine.PageNo; }
    }

    internal PageVariable()
    {
      Name = "Page";
      DataType = typeof(int);
    }
  }

  /// <summary>
  /// Returns total number of pages in the report. To use this variable, you need 
  /// to enable the report's double pass.
  /// </summary>
  public class TotalPagesVariable : SystemVariable
  {
    /// <inheritdoc/>
    public override object Value
    {
      get { return Report.Engine.TotalPages; }
    }

    internal TotalPagesVariable()
    {
      Name = "TotalPages";
      DataType = typeof(int);
    }
  }

  /// <summary>
  /// Returns a string containing the current page number in a form "Page N".
  /// </summary>
  public class PageNVariable : SystemVariable
  {
    /// <inheritdoc/>
    public override object Value
    {
      get { return Report.Engine.PageN; }
    }

    internal PageNVariable()
    {
      Name = "PageN";
      DataType = typeof(string);
    }
  }

  /// <summary>
  /// Returns a string containing the current page number and total pages in a form "Page N of M".
  /// To use this variable, you need to enable the report's double pass.
  /// </summary>
  public class PageNofMVariable : SystemVariable
  {
    /// <inheritdoc/>
    public override object Value
    {
      get { return Report.Engine.PageNofM; }
    }

    internal PageNofMVariable()
    {
      Name = "PageNofM";
      DataType = typeof(string);
    }
  }

  /// <summary>
  /// Returns data row number inside the group. This value is reset at the start of a new group. 
  /// </summary>
  public class RowVariable : SystemVariable
  {
    /// <inheritdoc/>
    public override object Value
    {
      get { return Report.Engine.RowNo; }
    }

    internal RowVariable()
    {
      Name = "Row#";
      DataType = typeof(int);
    }
  }

  /// <summary>
  /// Returns absolute number of data row. This value is never reset at the start of a new group.
  /// </summary>
  public class AbsRowVariable : SystemVariable
  {
    /// <inheritdoc/>
    public override object Value
    {
      get { return Report.Engine.AbsRowNo; }
    }

    internal AbsRowVariable()
    {
      Name = "AbsRow#";
      DataType = typeof(int);
    }
  }

  /// <summary>
  /// Returns current page number.
  /// <para/>This variable is actually a macro. Its value is substituted when the component is viewed in 
  /// the preview window. That means you cannot use it in an expression.
  /// </summary>
  public class PageMacroVariable : SystemVariable
  {
    /// <inheritdoc/>
    public override object Value
    {
      get { return "[PAGE#]"; }
    }

    internal PageMacroVariable()
    {
      Name = "Page#";
      DataType = typeof(int);
    }
  }

  /// <summary>
  /// Returns the number of total pages in the report.
  /// <para/>This variable is actually a macro. Its value is substituted when the component is viewed in 
  /// the preview window. That means you cannot use it in an expression.
  /// </summary>
  public class TotalPagesMacroVariable : SystemVariable
  {
    /// <inheritdoc/>
    public override object Value
    {
      get { return "[TOTALPAGES#]"; }
    }

    internal TotalPagesMacroVariable()
    {
      Name = "TotalPages#";
      DataType = typeof(int);
    }
  }

  /// <summary>
  /// Returns the name of the printed copy.
  /// <para/>This variable is actually a macro. Its value is substituted when the component is viewed in 
  /// the preview window. That means you cannot use it in an expression.
  /// </summary>
  public class CopyNameMacroVariable : SystemVariable
  {
    /// <inheritdoc/>
    public override object Value
    {
      get { return "[COPYNAME#]"; }
    }

    internal CopyNameMacroVariable()
    {
      Name = "CopyName#";
      DataType = typeof(string);
    }
  }

  /// <summary>
  /// Returns a level of hierarchy in the hierarchical report.
  /// </summary>
  public class HierarchyLevelVariable : SystemVariable
  {
    /// <inheritdoc/>
    public override object Value
    {
      get { return Report.Engine.HierarchyLevel; }
    }

    internal HierarchyLevelVariable()
    {
      Name = "HierarchyLevel";
      DataType = typeof(int);
    }
  }

  /// <summary>
  /// Returns the row number like "1.2.1" in the hierarchical report.
  /// </summary>
  public class HierarchyRowNoVariable : SystemVariable
  {
    /// <inheritdoc/>
    public override object Value
    {
      get { return Report.Engine.HierarchyRowNo; }
    }

    internal HierarchyRowNoVariable()
    {
      Name = "HierarchyRow#";
      DataType = typeof(string);
    }
  }
}
