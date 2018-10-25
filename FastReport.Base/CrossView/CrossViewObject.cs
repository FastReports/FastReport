using System;
using System.ComponentModel;
using FastReport.Matrix;
using FastReport.Utils;
using FastReport.Data;
using FastReport.Table;
using System.Drawing.Design;

namespace FastReport.CrossView
{
  /// <summary>
  /// Represents the crossview object that is used to print cube slice or slicegrid.
  /// </summary>
  public partial class CrossViewObject : TableBase
  {
    #region Fields
    //private FastCubeSource fastCubeSource;
    private CubeSourceBase cubeSource;
    private bool showTitle;
    private bool showXAxisFieldsCaption;
    private bool showYAxisFieldsCaption;
    private string style;
    private CrossViewData data;
    private string modifyResultEvent;
    private CrossViewHelper helper;
    private bool saveVisible;
    private MatrixStyleSheet styleSheet;
    #endregion

    #region Properties
    /// <summary>
    /// Allows to modify the prepared matrix elements such as cells, rows, columns.
    /// </summary>
    public event EventHandler ModifyResult;

    /// <summary>
    /// Gets or sets a value indicating whether to show a title row.
    /// </summary>
    [DefaultValue(false)]
    [Category("Behavior")]
    public bool ShowTitle
    {
      get { return showTitle; }
      set
      {
        showTitle = value;
        if (IsDesigning)
        {
          //Data.CreateDescriptors();
          //FHelper.CreateOtherDescriptor();
          BuildTemplate();
        }
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether to show a X Axis fields Caption.
    /// </summary>
    [DefaultValue(false)]
    [Category("Behavior")]
    public bool ShowXAxisFieldsCaption
    {
      get { return showXAxisFieldsCaption; }
      set
      {
        showXAxisFieldsCaption = value;
        if (IsDesigning)
        {
          //Data.CreateDescriptors();
          //FHelper.CreateOtherDescriptor();
          BuildTemplate();
        }
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether to show a Y Axis fields Caption.
    /// </summary>
    [DefaultValue(false)]
    [Category("Behavior")]
    public bool ShowYAxisFieldsCaption
    {
      get { return showYAxisFieldsCaption; }
      set
      {
        showYAxisFieldsCaption = value;
        if (IsDesigning)
        {
          //Data.CreateDescriptors();
          //FHelper.CreateOtherDescriptor();
          BuildTemplate();
        }
      }
    }

    /// <summary>
    /// Gets or sets a matrix style.
    /// </summary>
    [Category("Appearance")]
    [Editor("FastReport.TypeEditors.CrossViewStyleEditor, FastReport", typeof(UITypeEditor))]
    public new string Style
    {
      get { return style; }
      set
      {
        style = value;
        Helper.UpdateStyle();
      }
    }

    /// <summary>
    /// Gets or sets a script method name that will be used to handle the 
    /// <see cref="ModifyResult"/> event.
    /// </summary>
    /// <remarks>
    /// See the <see cref="ModifyResult"/> event for more details.
    /// </remarks>
    [Category("Build")]
    public string ModifyResultEvent
    {
      get { return modifyResultEvent; }
      set { modifyResultEvent = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Browsable(false)]
    public string ColumnDescriptorsIndexes
    {
      get { return Data.ColumnDescriptorsIndexes; }
      set { if (!IsDesigning) Data.ColumnDescriptorsIndexes = value; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Browsable(false)]
    public string RowDescriptorsIndexes
    {
      get { return Data.RowDescriptorsIndexes; }
      set { if (!IsDesigning) Data.RowDescriptorsIndexes = value; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Browsable(false)]
    public string ColumnTerminalIndexes
    {
      get { return Data.ColumnTerminalIndexes; }
      set { if (!IsDesigning) Data.ColumnTerminalIndexes = value; }
    }
    /// <summary>
    /// 
    /// </summary>
    [Browsable(false)]
    public string RowTerminalIndexes
    {
      get { return Data.RowTerminalIndexes; }
      set { if (!IsDesigning) Data.RowTerminalIndexes = value; }
    }

    /// <summary>
    /// Gets or sets a cube source.
    /// </summary>
    [Category("Data")]
    public CubeSourceBase CubeSource
    {
      get { return cubeSource; }
      set
      {
        if (cubeSource != value)
        {
          if (cubeSource != null)
          {
            cubeSource.Disposed -= new EventHandler(CubeSource_Disposed);
            cubeSource.OnChanged -= new EventHandler(CubeSource_OnChanged);
          }
          if (value != null)
          {
            value.Disposed += new EventHandler(CubeSource_Disposed);
            value.OnChanged += new EventHandler(CubeSource_OnChanged);
          }
          cubeSource = value;
          Data.CubeSource = value;
          if (IsDesigning)
          {
            Data.CreateDescriptors();
            helper.CreateOtherDescriptor();
            BuildTemplate();
          }
        }
      }
    }

    /// <summary>
    /// Gets the object that holds data of Cube
    /// </summary>
    /// <remarks>
    /// See the <see cref="CrossViewData"/> class for more details.
    /// </remarks>
    [Browsable(false)]
    public CrossViewData Data
    {
      get { return data; }
    }

    internal MatrixStyleSheet StyleSheet
    {
      get { return styleSheet; }
    }

    private CrossViewHelper Helper
    {
      get { return helper; }
    }

    private bool IsResultCrossView
    {
      get { return !IsDesigning /*&& Data.Columns.Count == 0 && Data.Rows.Count == 0 */; }
    }

    private BandBase ParentBand
    {
      get
      {
        BandBase parentBand = this.Band;
        if (parentBand is ChildBand)
          parentBand = (parentBand as ChildBand).GetTopParentBand;
        return parentBand;
      }
    }

    private DataBand FootersDataBand
    {
      get
      {
        DataBand dataBand = null;
        if (ParentBand is GroupFooterBand)
          dataBand = ((ParentBand as GroupFooterBand).Parent as GroupHeaderBand).GroupDataBand;
        else if (ParentBand is DataFooterBand)
          dataBand = ParentBand.Parent as DataBand;
        return dataBand;
      }
    }

    private bool IsOnFooter
    {
      get
      {
        DataBand dataBand = FootersDataBand;
        if (dataBand != null)
        {
          //                    return DataSource == dataBand.DataSource;
        }
        return false;
      }
    }
    #endregion

    #region Private Methods
    private void CreateResultTable()
    {
      SetResultTable(new TableResult());
      // assign properties from this object. Do not use Assign method: TableResult is incompatible with MatrixObject.
      ResultTable.OriginalComponent = OriginalComponent;
      ResultTable.Alias = Alias;
      ResultTable.Border = Border.Clone();
      ResultTable.Fill = Fill.Clone();
      ResultTable.Bounds = Bounds;
      ResultTable.RepeatHeaders = RepeatHeaders;
      ResultTable.RepeatRowHeaders = RepeatRowHeaders;
      ResultTable.RepeatColumnHeaders = RepeatColumnHeaders;
      ResultTable.Layout = Layout;
      ResultTable.WrappedGap = WrappedGap;
      ResultTable.AdjustSpannedCellsWidth = AdjustSpannedCellsWidth;
      ResultTable.SetReport(Report);
      ResultTable.AfterData += new EventHandler(ResultTable_AfterData);
    }

    private void DisposeResultTable()
    {
      ResultTable.Dispose();
      SetResultTable(null);
    }

    private void ResultTable_AfterData(object sender, EventArgs e)
    {
      OnModifyResult(e);
    }

    private void CubeSource_Disposed(object sender, EventArgs e)
    {
      Data.CubeSource = null;
    }

    private void CubeSource_OnChanged(object sender, EventArgs e)
    {
      Data.CreateDescriptors();
      helper.CreateOtherDescriptor();
      BuildTemplate();
    }

    private void WireEvents(bool wire)
    {
      if (IsOnFooter)
      {
        DataBand dataBand = FootersDataBand;
        if (wire)
          dataBand.BeforePrint += new EventHandler(dataBand_BeforePrint);
        else
          dataBand.BeforePrint -= new EventHandler(dataBand_BeforePrint);
      }
    }

    private void dataBand_BeforePrint(object sender, EventArgs e)
    {
      /*
                  bool firstRow = (sender as DataBand).IsFirstRow;
                  if (firstRow)
                      Helper.StartPrint();
                  Helper.AddDataRow();
      */
    }
    #endregion

    #region Protected Methods
    /// <inheritdoc/>
    protected override void DeserializeSubItems(FRReader reader)
    {
      if (String.Compare(reader.ItemName, "CrossViewColumns", true) == 0)
        reader.Read(Data.Columns);
      else if (String.Compare(reader.ItemName, "CrossViewRows", true) == 0)
        reader.Read(Data.Rows);
      else if (String.Compare(reader.ItemName, "CrossViewCells", true) == 0)
        reader.Read(Data.Cells);
      else
        base.DeserializeSubItems(reader);
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Assign(Base source)
    {
      base.Assign(source);

      CrossViewObject src = source as CrossViewObject;
      CubeSource = src.CubeSource;
      ShowTitle = src.ShowTitle;
      ShowXAxisFieldsCaption = src.ShowXAxisFieldsCaption;
      ShowYAxisFieldsCaption = src.ShowYAxisFieldsCaption;
      Style = src.Style;
      //            MatrixEvenStylePriority = src.MatrixEvenStylePriority;
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      if (writer.SerializeTo != SerializeTo.SourcePages)
      {
        writer.Write(Data.Columns);
        writer.Write(Data.Rows);
        writer.Write(Data.Cells);
      }
      else
        RefreshTemplate(true);

      base.Serialize(writer);
      CrossViewObject c = writer.DiffObject as CrossViewObject;

      if (CubeSource != c.CubeSource)
        writer.WriteRef("CubeSource", CubeSource);
      if (ColumnDescriptorsIndexes != c.ColumnDescriptorsIndexes)
        writer.WriteStr("ColumnDescriptorsIndexes", ColumnDescriptorsIndexes);
      if (RowDescriptorsIndexes != c.RowDescriptorsIndexes)
        writer.WriteStr("RowDescriptorsIndexes", RowDescriptorsIndexes);
      if (ColumnTerminalIndexes != c.ColumnTerminalIndexes)
        writer.WriteStr("ColumnTerminalIndexes", ColumnTerminalIndexes);
      if (RowTerminalIndexes != c.RowTerminalIndexes)
        writer.WriteStr("RowTerminalIndexes", RowTerminalIndexes);
      if (ShowTitle != c.ShowTitle)
        writer.WriteBool("ShowTitle", ShowTitle);
      if (ShowXAxisFieldsCaption != c.ShowXAxisFieldsCaption)
        writer.WriteBool("ShowXAxisFieldsCaption", ShowXAxisFieldsCaption);
      if (ShowYAxisFieldsCaption != c.ShowYAxisFieldsCaption)
        writer.WriteBool("ShowYAxisFieldsCaption", ShowYAxisFieldsCaption);
      if (Style != c.Style)
        writer.WriteStr("Style", Style);
      //            if (MatrixEvenStylePriority != c.MatrixEvenStylePriority)
      //                writer.WriteValue("MatrixEvenStylePriority", MatrixEvenStylePriority);
      if (ModifyResultEvent != c.ModifyResultEvent)
        writer.WriteStr("ModifyResultEvent", ModifyResultEvent);
    }

    /// <summary>
    /// Creates or updates the matrix template.
    /// </summary>
    /// <remarks>
    /// Call this method after you modify the matrix descriptors using the <see cref="Data"/>
    /// object's properties. 
    /// </remarks>
    public void BuildTemplate()
    {
      Helper.BuildTemplate();
    }
    #endregion

    #region Report Engine
    /// <inheritdoc/>
    public override void InitializeComponent()
    {
      base.InitializeComponent();
      WireEvents(true);
    }

    /// <inheritdoc/>
    public override void FinalizeComponent()
    {
      base.FinalizeComponent();
      WireEvents(false);
    }

    /// <inheritdoc/>
    public override void SaveState()
    {
      saveVisible = Visible;
      BandBase parent = Parent as BandBase;
      if (!Visible || (parent != null && !parent.Visible))
        return;

      // create the result table that will be rendered in the preview
      CreateResultTable();
      Visible = false;

      if (parent != null)
      {
        parent.Height = Top;
        parent.CanGrow = false;
        parent.CanShrink = false;
        parent.AfterPrint += new EventHandler(ResultTable.GeneratePages);
      }
    }

    /// <inheritdoc/>
    public override void GetData()
    {

      base.GetData();
      if (Data.SourceAssigned)
      {

        //      if (!IsOnFooter)
        //      {
        Helper.StartPrint();
        Helper.AddData();
//      }

        Helper.FinishPrint();
      }
    }

    /// <inheritdoc/>
    public override void RestoreState()
    {
      BandBase parent = Parent as BandBase;
      if (!saveVisible || (parent != null && !parent.Visible))
        return;

      if (parent != null)
        parent.AfterPrint -= new EventHandler(ResultTable.GeneratePages);

      DisposeResultTable();
      Visible = saveVisible;
    }

    /// <summary>
    /// This method fires the <b>ModifyResult</b> event and the script code connected to the <b>ModifyResultEvent</b>.
    /// </summary>
    /// <param name="e">Event data.</param>
    public void OnModifyResult(EventArgs e)
    {
      if (ModifyResult != null)
        ModifyResult(this, e);
      InvokeEvent(ModifyResultEvent, e);
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossViewObject"/> class.
    /// </summary>
    public CrossViewObject()
    {
      //FAutoSize = true;
      showXAxisFieldsCaption = true;
      showYAxisFieldsCaption = true;
      data = new CrossViewData();
      helper = new CrossViewHelper(this);
      styleSheet = new MatrixStyleSheet();
      styleSheet.Load(ResourceLoader.GetStream("cross.frss"));
      style = "";
      RepeatHeaders = false;
      RepeatColumnHeaders = true;
      RepeatRowHeaders = true;
      //FFilter = "";
    }
  }
}
