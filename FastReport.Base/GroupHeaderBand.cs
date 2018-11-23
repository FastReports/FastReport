using System.ComponentModel;
using FastReport.Utils;
using FastReport.Data;
using System.Drawing.Design;

namespace FastReport
{
  /// <summary>
  /// Specifies a sort order.
  /// </summary>
  /// <remarks>
  /// This enumeration is used in the group header and in the "Matrix" object.
  /// </remarks>
  public enum SortOrder 
  { 
    /// <summary>
    /// Specifies no sort (natural order).
    /// </summary>
    None, 
    
    /// <summary>
    /// Specifies an ascending sort order.
    /// </summary>
    Ascending,

    /// <summary>
    /// Specifies a descending sort order.
    /// </summary>
    Descending
  }

  /// <summary>
  /// Represents a group header band.
  /// </summary>
  /// <remarks>
  /// A simple group consists of one <b>GroupHeaderBand</b> and the <b>DataBand</b> that is set 
  /// to the <see cref="Data"/> property. To create the nested groups, use the <see cref="NestedGroup"/> property.
  /// <note type="caution">
  /// Only the last nested group can have data band.
  /// </note>
  /// <para/>Use the <see cref="Condition"/> property to set the group condition. The <see cref="SortOrder"/>
  /// property can be used to set the sort order for group's data rows. You can also use the <b>Sort</b>
  /// property of the group's <b>DataBand</b> to specify additional sort.
  /// </remarks>
  /// <example>This example shows how to create nested groups.
  /// <code>
  /// ReportPage page = report.Pages[0] as ReportPage;
  /// 
  /// // create the main group
  /// GroupHeaderBand mainGroup = new GroupHeaderBand();
  /// mainGroup.Height = Units.Millimeters * 10;
  /// mainGroup.Name = "MainGroup";
  /// mainGroup.Condition = "[Orders.CustomerName]";
  /// // add a group to the page
  /// page.Bands.Add(mainGroup);
  /// 
  /// // create the nested group
  /// GroupHeaderBand nestedGroup = new GroupHeaderBand();
  /// nestedGroup.Height = Units.Millimeters * 10;
  /// nestedGroup.Name = "NestedGroup";
  /// nestedGroup.Condition = "[Orders.OrderDate]";
  /// // add it to the main group
  /// mainGroup.NestedGroup = nestedGroup;
  /// 
  /// // create a data band
  /// DataBand dataBand = new DataBand();
  /// dataBand.Height = Units.Millimeters * 10;
  /// dataBand.Name = "GroupData";
  /// dataBand.DataSource = report.GetDataSource("Orders");
  /// // connect the databand to the nested group
  /// nestedGroup.Data = dataBand;
  /// </code>
  /// </example>
  public partial class GroupHeaderBand : HeaderFooterBandBase
  {
    #region Fields
    private GroupHeaderBand nestedGroup;
    private DataBand data;
    private GroupFooterBand groupFooter;
    private DataHeaderBand header;
    private DataFooterBand footer;
    private string condition;
    private SortOrder sortOrder;
    private bool keepTogether;
    private bool resetPageNumber;
    private object groupValue;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a nested group.
    /// </summary>
    /// <remarks>
    /// Use this property to create nested groups.
    /// <note type="caution">
    /// Only the last nested group can have data band.
    /// </note>
    /// </remarks>
    /// <example>
    /// This example demonstrates how to create a group with nested group.
    /// <code>
    /// ReportPage page;
    /// GroupHeaderBand group = new GroupHeaderBand();
    /// group.NestedGroup = new GroupHeaderBand();
    /// group.NestedGroup.Data = new DataBand();
    /// page.Bands.Add(group);
    /// </code>
    /// </example>
    [Browsable(false)]
    public GroupHeaderBand NestedGroup
    {
      get { return nestedGroup; }
      set 
      {
        SetProp(nestedGroup, value);
        nestedGroup = value;
      }
    }

    /// <summary>
    /// Gets or sets the group data band.
    /// </summary>
    /// <remarks>
    /// Use this property to add a data band to a group. Note: only the last nested group can have Data band.
    /// </remarks>
    /// <example>
    /// This example demonstrates how to add a data band to a group.
    /// <code>
    /// ReportPage page;
    /// GroupHeaderBand group = new GroupHeaderBand();
    /// group.Data = new DataBand();
    /// page.Bands.Add(group);
    /// </code>
    /// </example>
    [Browsable(false)]
    public DataBand Data
    {
      get { return data; }
      set
      {
        SetProp(data, value);
        data = value;
      }
    }

    /// <summary>
    /// Gets or sets a group footer.
    /// </summary>
    [Browsable(false)]
    public GroupFooterBand GroupFooter
    {
      get { return groupFooter; }
      set
      {
        SetProp(groupFooter, value);
        groupFooter = value;
      }
    }

    /// <summary>
    /// Gets or sets a header band.
    /// </summary>
    [Browsable(false)]
    public DataHeaderBand Header
    {
      get { return header; }
      set
      {
        SetProp(header, value);
        header = value;
      }
    }

    /// <summary>
    /// Gets or sets a footer band.
    /// </summary>
    /// <remarks>
    /// To access a group footer band, use the <see cref="GroupFooter"/> property.
    /// </remarks>
    [Browsable(false)]
    public DataFooterBand Footer
    {
      get { return footer; }
      set
      {
        SetProp(footer, value);
        footer = value;
      }
    }

    /// <summary>
    /// Gets or sets the group condition.
    /// </summary>
    /// <remarks>
    /// This property can contain any valid expression. When running a report, this expression is calculated 
    /// for each data row. When the value of this condition is changed, FastReport starts a new group.
    /// </remarks>
    [Category("Data")]
    [Editor("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
    public string Condition
    {
      get { return condition; }
      set { condition = value; }
    }
    
    /// <summary>
    /// Gets or sets the sort order.
    /// </summary>
    /// <remarks>
    /// FastReport can sort data rows automatically using the <see cref="Condition"/> value.
    /// </remarks>
    [DefaultValue(SortOrder.Ascending)]
    [Category("Behavior")]
    public SortOrder SortOrder
    {
      get { return sortOrder; }
      set { sortOrder = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating that the group should be printed together on one page.
    /// </summary>
    [DefaultValue(false)]
    [Category("Behavior")]
    public bool KeepTogether
    {
      get { return keepTogether; }
      set { keepTogether = value; }
    }

    /// <summary>
    /// Gets or sets a value that determines whether to reset the page numbers when this group starts print.
    /// </summary>
    /// <remarks>
    /// Typically you should set the <see cref="BandBase.StartNewPage"/> property to <b>true</b> as well.
    /// </remarks>
    [DefaultValue(false)]
    [Category("Behavior")]
    public bool ResetPageNumber
    {
      get { return resetPageNumber; }
      set { resetPageNumber = value; }
    }

    internal DataSourceBase DataSource
    {
      get 
      { 
        DataBand dataBand = GroupDataBand;
        return dataBand == null ? null : dataBand.DataSource;
      }  
    }
    
    internal DataBand GroupDataBand
    {
      get
      {
        GroupHeaderBand group = this;
        while (group != null)
        {
          if (group.Data != null)
            return group.Data;
          group = group.NestedGroup;
        }
        return null;
      }  
    }
    #endregion

    #region IParent
    /// <inheritdoc/>
    public override void GetChildObjects(ObjectCollection list)
    {
      base.GetChildObjects(list);
      if (!IsRunning)
      {
        list.Add(header);
        list.Add(nestedGroup);
        list.Add(data);
        list.Add(groupFooter);
        list.Add(footer);
      }
    }

    /// <inheritdoc/>
    public override bool CanContain(Base child)
    {
      return base.CanContain(child) || 
        (child is DataBand && nestedGroup == null && data == null) || 
        (child is GroupHeaderBand && nestedGroup == null && data == null) || 
        child is GroupFooterBand || child is DataHeaderBand || child is DataFooterBand;
    }

    /// <inheritdoc/>
    public override void AddChild(Base child)
    {
      if (IsRunning)
      {
        base.AddChild(child);
        return;
      }
      
      if (child is GroupHeaderBand)
        NestedGroup = child as GroupHeaderBand;
      else if (child is DataBand)
        Data = child as DataBand;
      else if (child is GroupFooterBand)
        GroupFooter = child as GroupFooterBand;
      else if (child is DataHeaderBand)
        Header = child as DataHeaderBand;
      else if (child is DataFooterBand)
        Footer = child as DataFooterBand;
      else
        base.AddChild(child);
    }

    /// <inheritdoc/>
    public override void RemoveChild(Base child)
    {
      base.RemoveChild(child);
      if (IsRunning)
        return;

      if (child is GroupHeaderBand && nestedGroup == child)
        NestedGroup = null;
      if (child is DataBand && data == child as DataBand)
        Data = null;
      if (child is GroupFooterBand && groupFooter == child)
        GroupFooter = null;
      if (child is DataHeaderBand && header == child)
        Header = null;
      if (child is DataFooterBand && footer == child)
        Footer = null;
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Assign(Base source)
    {
      base.Assign(source);
      
      GroupHeaderBand src = source as GroupHeaderBand;
      Condition = src.Condition;
      SortOrder = src.SortOrder;
      KeepTogether = src.KeepTogether;
      ResetPageNumber = src.ResetPageNumber;
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      GroupHeaderBand c = writer.DiffObject as GroupHeaderBand;
      base.Serialize(writer);
      if (writer.SerializeTo == SerializeTo.Preview)
        return;
      
      if (Condition != c.Condition)
        writer.WriteStr("Condition", Condition);
      if (SortOrder != c.SortOrder)
        writer.WriteValue("SortOrder", SortOrder);
      if (KeepTogether != c.KeepTogether)
        writer.WriteBool("KeepTogether", KeepTogether);
      if (ResetPageNumber != c.ResetPageNumber)
        writer.WriteBool("ResetPageNumber", ResetPageNumber);
    }

    /// <inheritdoc/>
    public override string[] GetExpressions()
    {
      return new string[] { Condition };
    }

    internal override bool IsEmpty()
    {
      if (NestedGroup != null)
        return NestedGroup.IsEmpty();
      else if (Data != null)
        return Data.IsEmpty();
      return base.IsEmpty();
    }
    
    internal void InitDataSource()
    {
      DataBand dataBand = GroupDataBand;
      GroupHeaderBand group = this;
      int index = 0;
      // insert group sort to the databand
      while (group != null)
      {
        if (group.SortOrder != SortOrder.None)
        {
          dataBand.Sort.Insert(index, new Sort(group.Condition, group.SortOrder == SortOrder.Descending));
          index++;
        }
        group = group.NestedGroup;
      }
      
      dataBand.InitDataSource();
    }
    
    internal void FinalizeDataSource()
    {
      DataBand dataBand = GroupDataBand;
      GroupHeaderBand group = this;
      // remove group sort from the databand
      while (group != null)
      {
        if (group.SortOrder != SortOrder.None)
          dataBand.Sort.RemoveAt(0);
        group = group.NestedGroup;
      }
    }
    
    internal void ResetGroupValue()
    {
      if (!string.IsNullOrEmpty(Condition))
      {
        groupValue = Report.Calc(Condition);
      }
      else
      {
        throw new GroupHeaderHasNoGroupCondition(Name);
      }
    }

    internal bool GroupValueChanged()
    {
      object value = null;
      if (!string.IsNullOrEmpty(Condition))
      { 
        value = Report.Calc(Condition);
      }
      else
      {
        throw new GroupHeaderHasNoGroupCondition(Name);
      }
      if (groupValue == null)
      {
        if (value == null)
          return false;
        return true;  
      }
      return !groupValue.Equals(value);
    }
    #endregion
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupHeaderBand"/> class with default settings.
    /// </summary>
    public GroupHeaderBand()
    {
      condition = "";
      sortOrder = SortOrder.Ascending;
    }
  }
}