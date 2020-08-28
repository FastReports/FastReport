using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using FastReport.Utils;
using System.Drawing.Design;

namespace FastReport
{
  /// <summary>
  /// Represents a report page.
  /// </summary>
  /// <remarks>
  /// To get/set a paper size and orientation, use the <see cref="PaperWidth"/>, <see cref="PaperHeight"/>
  /// and <see cref="Landscape"/> properties. Note that paper size is measured in millimeters.
  /// <para/>Report page can contain one or several bands with report objects. Use the <see cref="ReportTitle"/>, 
  /// <see cref="ReportSummary"/>, <see cref="PageHeader"/>, <see cref="PageFooter"/>, 
  /// <see cref="ColumnHeader"/>, <see cref="ColumnFooter"/>, <see cref="Overlay"/> properties
  /// to get/set the page bands. The <see cref="Bands"/> property holds the list of data bands or groups. 
  /// Thus you may add several databands to this property to create master-master reports, for example.
  /// <note type="caution">
  /// Report page can contain bands only. You cannot place report objects such as <b>TextObject</b> on a page.
  /// </note>
  /// </remarks>
  /// <example>
  /// This example shows how to create a page with one <b>ReportTitleBand</b> and <b>DataBand</b> bands and add
  /// it to the report.
  /// <code>
  /// ReportPage page = new ReportPage();
  /// // set the paper in millimeters
  /// page.PaperWidth = 210;
  /// page.PaperHeight = 297;
  /// // create report title
  /// page.ReportTitle = new ReportTitleBand();
  /// page.ReportTitle.Name = "ReportTitle1";
  /// page.ReportTitle.Height = Units.Millimeters * 10;
  /// // create data band
  /// DataBand data = new DataBand();
  /// data.Name = "Data1";
  /// data.Height = Units.Millimeters * 10;
  /// // add data band to the page
  /// page.Bands.Add(data);
  /// // add page to the report
  /// report.Pages.Add(page);
  /// </code>
  /// </example>
  public partial class ReportPage : PageBase, IParent
  {
    #region Constants
    
    private const float MAX_PAPER_SIZE_MM = 2000000000;

    #endregion // Constants
    
    #region Fields
    private string exportAlias;
    private float paperWidth;
    private float paperHeight;
    private int rawPaperSize;
    private bool landscape;
    private float leftMargin;
    private float topMargin;
    private float rightMargin;
    private float bottomMargin;
    private bool mirrorMargins;
    private PageColumns columns;
    private FloatCollection guides;
    private Border border;
    private FillBase fill;
    private Watermark watermark;
    private bool titleBeforeHeader;
    private string outlineExpression;
    private bool printOnPreviousPage;
    private bool resetPageNumber;
    private bool extraDesignWidth;
    private bool startOnOddPage;
    private bool backPage;
    private SubreportObject subreport;
    private PageHeaderBand pageHeader;
    private ReportTitleBand reportTitle;
    private ColumnHeaderBand columnHeader;
    private BandCollection bands;
    private ReportSummaryBand reportSummary;
    private ColumnFooterBand columnFooter;
    private PageFooterBand pageFooter;
    private OverlayBand overlay;
    private string startPageEvent;
    private string finishPageEvent;
    private string manualBuildEvent;

    private bool unlimitedHeight;
    private bool printOnRollPaper;
    private bool unlimitedWidth;
    private float unlimitedHeightValue;
    private float unlimitedWidthValue;

    #endregion

    #region Properties
    /// <summary>
    /// This event occurs when the report engine starts this page.
    /// </summary>
    public event EventHandler StartPage;
    
    /// <summary>
    /// This event occurs when the report engine finished this page.
    /// </summary>
    public event EventHandler FinishPage;

    /// <summary>
    /// This event occurs when the report engine is about to print databands in this page.
    /// </summary>
    public event EventHandler ManualBuild;

    /// <summary>
    /// Gets or sets a width of the paper, in millimeters.
    /// </summary>
    [Category("Paper")]
    [TypeConverter("FastReport.TypeConverters.PaperConverter, FastReport")]
    public float PaperWidth
    {
      get { return paperWidth; }
      set { paperWidth = value; }
    }

    /// <summary>
    /// Gets or sets the page name on export
    /// </summary>
    [Category("Paper")]
    public string ExportAlias
    {
      get { return exportAlias; }
      set { exportAlias = value; }
    }

    /// <summary>
    /// Gets or sets a height of the paper, in millimeters.
    /// </summary>
    [Category("Paper")]
    [TypeConverter("FastReport.TypeConverters.PaperConverter, FastReport")]
    public float PaperHeight
    {
      get { return paperHeight; }
      set { paperHeight = value; }
    }

    /// <summary>
    /// Gets or sets the raw index of a paper size.
    /// </summary>
    /// <remarks>
    /// This property stores the RawKind value of a selected papersize. It is used to distiguish
    /// between several papers with the same size (for ex. "A3" and "A3 with no margins") used in some
    /// printer drivers. 
    /// <para/>It is not obligatory to set this property. FastReport will select the
    /// necessary paper using the <b>PaperWidth</b> and <b>PaperHeight</b> values.
    /// </remarks>
    [Category("Paper")]
    [DefaultValue(0)]
    public int RawPaperSize
    {
      get { return rawPaperSize; }
      set { rawPaperSize = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the page has unlimited height.
    /// </summary>
    [DefaultValue(false)]
    [Category("Paper")]
    public bool UnlimitedHeight
    {
        get { return unlimitedHeight; }
        set
        {
            unlimitedHeight = value;
            if (!unlimitedHeight)
                printOnRollPaper = false;
        }
    }

    /// <summary>
    /// Gets or sets the value indicating whether the unlimited page should be printed on roll paper.
    /// </summary>
    [DefaultValue(false)]
    [Category("Paper")]
    public bool PrintOnRollPaper
    {
        get { return printOnRollPaper; }
        set
        {
            if (unlimitedHeight)
                printOnRollPaper = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the page has unlimited width.
    /// </summary>
    [DefaultValue(false)]
    [Category("Paper")]
    public bool UnlimitedWidth
    {
        get { return unlimitedWidth; }
        set { unlimitedWidth = value; }
    }

    /// <summary>
    /// Get or set the current height of unlimited page.
    /// </summary>
    [Browsable(false)]
    public float UnlimitedHeightValue
    {
        get { return unlimitedHeightValue; }
        set
        {
            unlimitedHeightValue = value;
            if (printOnRollPaper)
                PaperHeight = unlimitedHeightValue / Units.Millimeters;
        }
    }

    /// <summary>
    /// Get or set the current width of unlimited page.
    /// </summary>
    [Browsable(false)]
    public float UnlimitedWidthValue
    {
        get { return unlimitedWidthValue; }
        set { unlimitedWidthValue = value; }
    }

    /// <summary>
    /// Gets the current page height in pixels.
    /// </summary>
    [Browsable(false)]
    public float HeightInPixels
    {
        get
        {
            return UnlimitedHeight ? UnlimitedHeightValue : PaperHeight * Units.Millimeters;
        }
    }

    /// <summary>
    /// Gets the current page width in pixels.
    /// </summary>
    [Browsable(false)]
    public float WidthInPixels
    {
        get
        {
            if (UnlimitedWidth)
            {
                if (!IsDesigning)
                {
                    return UnlimitedWidthValue;
                }
            }      
            return PaperWidth * Units.Millimeters;
            
        }
    }

    /// <summary>
    /// Gets or sets a value indicating that page should be in landscape orientation.
    /// </summary>
    /// <remarks>
    /// When you change this property, it will automatically swap paper width and height, as well as paper margins.
    /// </remarks>
    [DefaultValue(false)]
    [Category("Paper")]
    public bool Landscape
    {
      get { return landscape; }
      set 
      {
        if (landscape != value)
        {
          float e = paperWidth;
          paperWidth = paperHeight;
          paperHeight = e;

          float m1 = leftMargin;   //     m3
          float m2 = rightMargin;  //  m1    m2
          float m3 = topMargin;    //     m4
          float m4 = bottomMargin; //

          if (value)
          {
            leftMargin = m3;       // rotate counter-clockwise
            rightMargin = m4;
            topMargin = m2;
            bottomMargin = m1;
          }
          else
          {
            leftMargin = m4;       // rotate clockwise
            rightMargin = m3;
            topMargin = m1;
            bottomMargin = m2;
          }
        }
        landscape = value;
      }
    }

    /// <summary>
    /// Gets or sets the left page margin, in millimeters.
    /// </summary>
    [Category("Paper")]
    [TypeConverter("FastReport.TypeConverters.PaperConverter, FastReport")]
    public float LeftMargin
    {
      get { return leftMargin; }
      set { leftMargin = value; }
    }

    /// <summary>
    /// Gets or sets the top page margin, in millimeters.
    /// </summary>
    [Category("Paper")]
    [TypeConverter("FastReport.TypeConverters.PaperConverter, FastReport")]
    public float TopMargin
    {
      get { return topMargin; }
      set { topMargin = value; }
    }

    /// <summary>
    /// Gets or sets the right page margin, in millimeters.
    /// </summary>
    [Category("Paper")]
    [TypeConverter("FastReport.TypeConverters.PaperConverter, FastReport")]
    public float RightMargin
    {
      get { return rightMargin; }
      set { rightMargin = value; }
    }

    /// <summary>
    /// Gets or sets the bottom page margin, in millimeters.
    /// </summary>
    [Category("Paper")]
    [TypeConverter("FastReport.TypeConverters.PaperConverter, FastReport")]
    public float BottomMargin
    {
      get { return bottomMargin; }
      set { bottomMargin = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating that even pages should swap its left and right margins when
    /// previewed or printed.
    /// </summary>
    [DefaultValue(false)]
    [Category("Behavior")]
    public bool MirrorMargins
    {
      get { return mirrorMargins; }
      set { mirrorMargins = value; }
    }

    /// <summary>
    /// Gets the page columns settings.
    /// </summary>
    [Category("Appearance")]
    public PageColumns Columns
    {
      get { return columns; }
    }
    
    /// <summary>
    /// Gets or sets the page border that will be printed inside the page printing area.
    /// </summary>
    [Category("Appearance")]
    public Border Border
    {
      get { return border; }
      set { border = value; }
    }

    
    /// <summary>
    /// Gets or sets the page background fill.
    /// </summary>
    [Category("Appearance")]
    [Editor("FastReport.TypeEditors.FillEditor, FastReport", typeof(UITypeEditor))]
    public FillBase Fill
    {
      get { return fill; }
      set
      {
        if (value == null)
          throw new ArgumentNullException("Fill");
        fill = value;
      }
    }

    /// <summary>
    /// Gets or sets the page watermark.
    /// </summary>
    /// <remarks>
    /// To enabled watermark, set its <b>Enabled</b> property to <b>true</b>.
    /// </remarks>
    [Category("Appearance")]
    public Watermark Watermark
    {
      get { return watermark; }
      set 
      { 
        if (watermark != value)
          if (watermark != null)
            watermark.Dispose();
        watermark = value; 
      }
    }
    
    /// <summary>
    /// Gets or sets a value indicating that <b>ReportTitle</b> band should be printed before the 
    /// <b>PageHeader</b> band.
    /// </summary>
    [DefaultValue(true)]
    [Category("Behavior")]
    public bool TitleBeforeHeader
    {
      get { return titleBeforeHeader; }
      set { titleBeforeHeader = value; }
    }

    /// <summary>
    /// Gets or sets an outline expression.
    /// </summary>
    /// <remarks>
    /// For more information, see <see cref="BandBase.OutlineExpression"/> property.
    /// </remarks>
    [Category("Data")]
    [Editor("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
    public string OutlineExpression
    {
      get { return outlineExpression; }
      set { outlineExpression = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to start to print this page on a free space of the previous page.
    /// </summary>
    /// <remarks>
    /// This property can be used if you have two or more pages in the report template.
    /// </remarks>
    [DefaultValue(false)]
    [Category("Behavior")]
    public bool PrintOnPreviousPage
    {
      get { return printOnPreviousPage; }
      set { printOnPreviousPage = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating that FastReport engine must reset page numbers before printing this page.
    /// </summary>
    /// <remarks>
    /// This property can be used if you have two or more pages in the report template.
    /// </remarks>
    [DefaultValue(false)]
    [Category("Behavior")]
    public bool ResetPageNumber
    {
      get { return resetPageNumber; }
      set { resetPageNumber = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the page has extra width in the report designer.
    /// </summary>
    /// <remarks>
    /// This property may be useful if you work with such objects as Matrix and Table.
    /// </remarks>
    [DefaultValue(false)]
    [Category("Design")]
    public bool ExtraDesignWidth
    {
      get { return extraDesignWidth; }
      set { extraDesignWidth = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this page will start on an odd page only.
    /// </summary>
    /// <remarks>
    /// This property is useful to print booklet-type reports. Setting this property to <b>true</b>
    /// means that this page will start to print on an odd page only. If necessary, an empty page
    /// will be added to the prepared report before this page will be printed.
    /// </remarks>
    [DefaultValue(false)]
    [Category("Behavior")]
    public bool StartOnOddPage
    {
      get { return startOnOddPage; }
      set { startOnOddPage = value; }
    }

    /// <summary>
    /// Uses this page as a back page for previously printed pages.
    /// </summary>
    [DefaultValue(false)]
    [Category("Behavior")]
    public bool BackPage
    {
      get { return backPage; }
      set { backPage = value; }
    }

    /// <summary>
    /// Gets or sets a report title band.
    /// </summary>
    [Browsable(false)]
    public ReportTitleBand ReportTitle
    {
      get { return reportTitle; }
      set
      {
        SetProp(reportTitle, value);
        reportTitle = value;
      }
    }

    /// <summary>
    /// Gets or sets a report summary band.
    /// </summary>
    [Browsable(false)]
    public ReportSummaryBand ReportSummary
    {
      get { return reportSummary; }
      set
      {
        SetProp(reportSummary, value);
        reportSummary = value;
      }
    }

    /// <summary>
    /// Gets or sets a page header band.
    /// </summary>
    [Browsable(false)]
    public PageHeaderBand PageHeader
    {
      get { return pageHeader; }
      set
      {
        SetProp(pageHeader, value);
        pageHeader = value;
      }
    }

    /// <summary>
    /// Gets or sets a page footer band.
    /// </summary>
    [Browsable(false)]
    public PageFooterBand PageFooter
    {
      get { return pageFooter; }
      set
      {
        SetProp(pageFooter, value);
        pageFooter = value;
      }
    }

    /// <summary>
    /// Gets or sets a column header band.
    /// </summary>
    [Browsable(false)]
    public ColumnHeaderBand ColumnHeader
    {
      get { return columnHeader; }
      set
      {
        SetProp(columnHeader, value);
        columnHeader = value;
      }
    }

    /// <summary>
    /// Gets or sets a column footer band.
    /// </summary>
    [Browsable(false)]
    public ColumnFooterBand ColumnFooter
    {
      get { return columnFooter; }
      set
      {
        SetProp(columnFooter, value);
        columnFooter = value;
      }
    }

    /// <summary>
    /// Gets or sets an overlay band.
    /// </summary>
    [Browsable(false)]
    public OverlayBand Overlay
    {
      get { return overlay; }
      set
      {
        SetProp(overlay, value);
        overlay = value;
      }
    }

    /// <summary>
    /// Gets the collection of data bands or group header bands.
    /// </summary>
    /// <remarks>
    /// The <b>Bands</b> property holds the list of data bands or group headers. 
    /// Thus you may add several databands to this property to create master-master reports, for example.
    /// </remarks>
    [Browsable(false)]
    public BandCollection Bands
    {
      get { return bands; }
    }

    /// <summary>
    /// Gets or sets the page guidelines.
    /// </summary>
    /// <remarks>
    /// This property hold all vertical guidelines. The horizontal guidelines are owned by the bands (see
    /// <see cref="BandBase.Guides"/> property).
    /// </remarks>
    [Browsable(false)]
    public FloatCollection Guides
    {
      get { return guides; } 
      set { guides = value; }
    }

    /// <summary>
    /// Gets or sets the reference to a parent <b>SubreportObject</b> that owns this page.
    /// </summary>
    /// <remarks>
    /// This property is <b>null</b> for regular report pages. See the <see cref="SubreportObject"/> for details.
    /// </remarks>
    [Browsable(false)]
    public SubreportObject Subreport
    {
      get { return subreport; }
      set { subreport = value; }
    }

    /// <summary>
    /// Gets or sets a script event name that will be fired when the report engine starts this page.
    /// </summary>
    [Category("Build")]
    public string StartPageEvent
    {
      get { return startPageEvent; }
      set { startPageEvent = value; }
    }

    /// <summary>
    /// Gets or sets a script event name that will be fired when the report engine finished this page.
    /// </summary>
    [Category("Build")]
    public string FinishPageEvent
    {
      get { return finishPageEvent; }
      set { finishPageEvent = value; }
    }

    /// <summary>
    /// Gets or sets a script event name that will be fired when the report engine is about 
    /// to print databands in this page.
    /// </summary>
    [Category("Build")]
    public string ManualBuildEvent
    {
      get { return manualBuildEvent; }
      set { manualBuildEvent = value; }
    }

    internal bool IsManualBuild
    {
      get { return !String.IsNullOrEmpty(manualBuildEvent) || ManualBuild != null; }
    }
    #endregion

    #region Private Methods
    private void DrawBackground(FRPaintEventArgs e, RectangleF rect)
    {
      rect.Width *= e.ScaleX;
      rect.Height *= e.ScaleY;
      Brush brush = null;
      if (Fill is SolidFill)
        brush = e.Cache.GetBrush((Fill as SolidFill).Color);
      else
        brush = Fill.CreateBrush(rect, e.ScaleX, e.ScaleY);

      e.Graphics.FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
      if (!(Fill is SolidFill))
        brush.Dispose();
    }
    #endregion

    #region Protected Methods
    
    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
                if (Subreport != null)
                {
                    Subreport.ReportPage = null;
                }
                if (Watermark != null)
                {
                    Watermark.Dispose();
                    Watermark = null;
                }
        }
        base.Dispose(disposing);
    }

    #endregion

    #region IParent
    /// <inheritdoc/>
    public virtual void GetChildObjects(ObjectCollection list)
    {
      if (TitleBeforeHeader)
      {
        list.Add(reportTitle);
        list.Add(pageHeader);
      }
      else
      {
        list.Add(pageHeader);
        list.Add(reportTitle);
      }
      list.Add(columnHeader);
      foreach (BandBase band in bands)
      {
        list.Add(band);
      }
      list.Add(reportSummary);
      list.Add(columnFooter);
      list.Add(pageFooter);
      list.Add(overlay);
    }

    /// <inheritdoc/>
    public virtual bool CanContain(Base child)
    {
      if (IsRunning)
        return child is BandBase;
      return (child is PageHeaderBand || child is ReportTitleBand || child is ColumnHeaderBand ||
        child is DataBand || child is GroupHeaderBand || child is ColumnFooterBand ||
        child is ReportSummaryBand || child is PageFooterBand || child is OverlayBand);
    }

    /// <inheritdoc/>
    public virtual void AddChild(Base child)
    {
      if (IsRunning)
      {
        bands.Add(child as BandBase);
        return;
      }
      if (child is PageHeaderBand)
        PageHeader = child as PageHeaderBand;
      if (child is ReportTitleBand)
        ReportTitle = child as ReportTitleBand;
      if (child is ColumnHeaderBand)
        ColumnHeader = child as ColumnHeaderBand;
      if (child is DataBand || child is GroupHeaderBand)
        bands.Add(child as BandBase);
      if (child is ReportSummaryBand)
        ReportSummary = child as ReportSummaryBand;
      if (child is ColumnFooterBand)
        ColumnFooter = child as ColumnFooterBand;
      if (child is PageFooterBand)
        PageFooter = child as PageFooterBand;
      if (child is OverlayBand)
        Overlay = child as OverlayBand;
    }

    /// <inheritdoc/>
    public virtual void RemoveChild(Base child)
    {
      if (IsRunning)
      {
        bands.Remove(child as BandBase);
        return;
      }
      if (child is PageHeaderBand && pageHeader == child as PageHeaderBand)
        PageHeader = null;
      if (child is ReportTitleBand && reportTitle == child as ReportTitleBand)
        ReportTitle = null;
      if (child is ColumnHeaderBand && columnHeader == child as ColumnHeaderBand)
        ColumnHeader = null;
      if (child is DataBand || child is GroupHeaderBand)
        bands.Remove(child as BandBase);
      if (child is ReportSummaryBand && reportSummary == child as ReportSummaryBand)
        ReportSummary = null;
      if (child is ColumnFooterBand && columnFooter == child as ColumnFooterBand)
        ColumnFooter = null;
      if (child is PageFooterBand && pageFooter == child as PageFooterBand)
        PageFooter = null;
      if (child is OverlayBand && overlay == child as OverlayBand)
        Overlay = null;
    }

    /// <inheritdoc/>
    public virtual int GetChildOrder(Base child)
    {
      return bands.IndexOf(child as BandBase);
    }

    /// <inheritdoc/>
    public virtual void SetChildOrder(Base child, int order)
    {
      if (order > bands.Count)
        order = bands.Count;
      int oldOrder = child.ZOrder;
      if (oldOrder != -1 && order != -1 && oldOrder != order)
      {
        if (oldOrder <= order)
          order--;
        bands.Remove(child as BandBase);
        bands.Insert(order, child as BandBase);
      }
    }

    /// <inheritdoc/>
    public virtual void UpdateLayout(float dx, float dy)
    {
      // do nothing
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Assign(Base source)
    {
      base.Assign(source);
      
      ReportPage src = source as ReportPage;
      ExportAlias = src.ExportAlias;
      Landscape = src.Landscape;
      PaperWidth = src.PaperWidth;
      PaperHeight = src.PaperHeight;
      RawPaperSize = src.RawPaperSize;
      LeftMargin = src.LeftMargin;
      TopMargin = src.TopMargin;
      RightMargin = src.RightMargin;
      BottomMargin = src.BottomMargin;
      MirrorMargins = src.MirrorMargins;
      AssignPreview(src);
      Columns.Assign(src.Columns);
      Guides.Assign(src.Guides);
      Border = src.Border.Clone();
      Fill = src.Fill.Clone();
      Watermark.Assign(src.Watermark);
      TitleBeforeHeader = src.TitleBeforeHeader;
      OutlineExpression = src.OutlineExpression;
      PrintOnPreviousPage = src.PrintOnPreviousPage;
      ResetPageNumber = src.ResetPageNumber;
      ExtraDesignWidth = src.ExtraDesignWidth;
      BackPage = src.BackPage;
      StartOnOddPage = src.StartOnOddPage;
      StartPageEvent = src.StartPageEvent;
      FinishPageEvent = src.FinishPageEvent;
      ManualBuildEvent = src.ManualBuildEvent;
      UnlimitedHeight = src.UnlimitedHeight;
      PrintOnRollPaper = src.PrintOnRollPaper;
      UnlimitedWidth = src.UnlimitedWidth;
      UnlimitedHeightValue = src.UnlimitedHeightValue;
      UnlimitedWidthValue = src.UnlimitedWidthValue;
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      ReportPage c = writer.DiffObject as ReportPage;
      base.Serialize(writer);
      if (ExportAlias != c.ExportAlias)
        writer.WriteStr("ExportAlias", ExportAlias);
      if (Landscape != c.Landscape)
        writer.WriteBool("Landscape", Landscape);
      if (FloatDiff(PaperWidth, c.PaperWidth))
        writer.WriteFloat("PaperWidth", PaperWidth);
      if (FloatDiff(PaperHeight, c.PaperHeight))
        writer.WriteFloat("PaperHeight", PaperHeight);
      if (RawPaperSize != c.RawPaperSize)
        writer.WriteInt("RawPaperSize", RawPaperSize);
      if (FloatDiff(LeftMargin, c.LeftMargin))
        writer.WriteFloat("LeftMargin", LeftMargin);
      if (FloatDiff(TopMargin, c.TopMargin))
        writer.WriteFloat("TopMargin", TopMargin);
      if (FloatDiff(RightMargin, c.RightMargin))
        writer.WriteFloat("RightMargin", RightMargin);
      if (FloatDiff(BottomMargin, c.BottomMargin))
        writer.WriteFloat("BottomMargin", BottomMargin);
      if (MirrorMargins != c.MirrorMargins)
        writer.WriteBool("MirrorMargins", MirrorMargins);
      WritePreview(writer,c);
      Columns.Serialize(writer, c.Columns);
      if (Guides.Count > 0)
        writer.WriteValue("Guides", Guides);
      Border.Serialize(writer, "Border", c.Border);
      Fill.Serialize(writer, "Fill", c.Fill);
      Watermark.Serialize(writer, "Watermark", c.Watermark);
      if (TitleBeforeHeader != c.TitleBeforeHeader)
        writer.WriteBool("TitleBeforeHeader", TitleBeforeHeader);
      if (OutlineExpression != c.OutlineExpression)
        writer.WriteStr("OutlineExpression", OutlineExpression);
      if (PrintOnPreviousPage != c.PrintOnPreviousPage)
        writer.WriteBool("PrintOnPreviousPage", PrintOnPreviousPage);
      if (ResetPageNumber != c.ResetPageNumber)
        writer.WriteBool("ResetPageNumber", ResetPageNumber);
      if (ExtraDesignWidth != c.ExtraDesignWidth)
        writer.WriteBool("ExtraDesignWidth", ExtraDesignWidth);
      if (StartOnOddPage != c.StartOnOddPage)
        writer.WriteBool("StartOnOddPage", StartOnOddPage);
      if (BackPage != c.BackPage)
        writer.WriteBool("BackPage", BackPage);
      if (StartPageEvent != c.StartPageEvent)
        writer.WriteStr("StartPageEvent", StartPageEvent);
      if (FinishPageEvent != c.FinishPageEvent)
        writer.WriteStr("FinishPageEvent", FinishPageEvent);
      if (ManualBuildEvent != c.ManualBuildEvent)
        writer.WriteStr("ManualBuildEvent", ManualBuildEvent);
      if (UnlimitedHeight != c.UnlimitedHeight)
        writer.WriteBool("UnlimitedHeight", UnlimitedHeight);
      if (PrintOnRollPaper != c.PrintOnRollPaper)
        writer.WriteBool("PrintOnRollPaper", PrintOnRollPaper);
      if (UnlimitedWidth != c.UnlimitedWidth)
        writer.WriteBool("UnlimitedWidth", UnlimitedWidth);
      if (FloatDiff(UnlimitedHeightValue, c.UnlimitedHeightValue))
        writer.WriteFloat("UnlimitedHeightValue", UnlimitedHeightValue);
      if (FloatDiff(UnlimitedWidthValue, c.UnlimitedWidthValue))
        writer.WriteFloat("UnlimitedWidthValue", UnlimitedWidthValue);
    }

    /// <inheritdoc/>
    public override void Draw(FRPaintEventArgs e)
    {
      if (IsDesigning)
        return;
      
      Graphics g = e.Graphics;
      RectangleF pageRect = new RectangleF(0, 0,
        WidthInPixels - 1 / e.ScaleX, HeightInPixels - 1 / e.ScaleY);
      RectangleF printableRect = new RectangleF(
        LeftMargin * Units.Millimeters,
        TopMargin * Units.Millimeters,
        (PaperWidth - LeftMargin - RightMargin) * Units.Millimeters,
        (PaperHeight - TopMargin - BottomMargin) * Units.Millimeters);

      DrawBackground(e, pageRect);
      Border.Draw(e, printableRect);
      if (Watermark.Enabled)
      {
        if (!Watermark.ShowImageOnTop)
          Watermark.DrawImage(e, pageRect, Report, IsPrinting);
        if (!Watermark.ShowTextOnTop)
          Watermark.DrawText(e, pageRect, Report, IsPrinting);
      }
      
      float leftMargin = (int)Math.Round(LeftMargin * Units.Millimeters * e.ScaleX);
      float topMargin = (int)Math.Round(TopMargin * Units.Millimeters * e.ScaleY);
      g.TranslateTransform(leftMargin, topMargin);
      
      try
      {
        foreach (Base c in AllObjects)
        {
          if (c is ReportComponentBase && c.HasFlag(Flags.CanDraw))
          {
            ReportComponentBase obj = c as ReportComponentBase;
            if (!IsPrinting)
            {
#if !MONO
              if (!obj.IsVisible(e))
                continue;
#endif
            }
            else
            {
              if (!obj.Printable)
                continue;
              else if (obj.Parent is BandBase && !(obj.Parent as BandBase).Printable)
                continue;
            }
            obj.SetDesigning(false);
            obj.SetPrinting(IsPrinting);
            obj.Draw(e);
            obj.SetPrinting(false);
          }
        }
      }
      finally
      {
        g.TranslateTransform(-leftMargin, -topMargin);
      }

      if (Watermark.Enabled)
      {
        if (Watermark.ShowImageOnTop)
          Watermark.DrawImage(e, pageRect, Report, IsPrinting);
        if (Watermark.ShowTextOnTop)
          Watermark.DrawText(e, pageRect, Report, IsPrinting);
      }
    }

    internal void InitializeComponents()
    {
      ObjectCollection allObjects = AllObjects;
      foreach (Base obj in allObjects)
      {
        if (obj is ReportComponentBase)
          (obj as ReportComponentBase).InitializeComponent();
      }
    }

    internal void FinalizeComponents()
    {
      ObjectCollection allObjects = AllObjects;
      foreach (Base obj in allObjects)
      {
        if (obj is ReportComponentBase)
          (obj as ReportComponentBase).FinalizeComponent();
      }
    }

    /// <inheritdoc/>
    public override string[] GetExpressions()
    {
      List<string> expressions = new List<string>();

      if (!String.IsNullOrEmpty(OutlineExpression))
        expressions.Add(OutlineExpression);

      return expressions.ToArray();
    }

    /// <inheritdoc/>
    public override void ExtractMacros()
    {
      Watermark.Text = ExtractDefaultMacros(Watermark.Text);
    }

    /// <summary>
    /// This method fires the <b>StartPage</b> event and the script code connected to the <b>StartPageEvent</b>.
    /// </summary>
    public void OnStartPage(EventArgs e)
    {
      if (StartPage != null)
        StartPage(this, e);
      InvokeEvent(StartPageEvent, e);
    }

    /// <summary>
    /// This method fires the <b>FinishPage</b> event and the script code connected to the <b>FinishPageEvent</b>.
    /// </summary>
    public void OnFinishPage(EventArgs e)
    {
      if (FinishPage != null)
        FinishPage(this, e);
      InvokeEvent(FinishPageEvent, e);
    }

    /// <summary>
    /// This method fires the <b>ManualBuild</b> event and the script code connected to the <b>ManualBuildEvent</b>.
    /// </summary>
    public void OnManualBuild(EventArgs e)
    {
      if (ManualBuild != null)
        ManualBuild(this, e);
      InvokeEvent(ManualBuildEvent, e);
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportPage"/> class with default settings.
    /// </summary>
    public ReportPage()
    {
      paperWidth = 210;
      paperHeight = 297;
      leftMargin = 10;
      topMargin = 10;
      rightMargin = 10;
      bottomMargin = 10;
      InitPreview();
      bands = new BandCollection(this);
      guides = new FloatCollection();
      columns = new PageColumns(this);
      border = new Border();
      fill = new SolidFill(SystemColors.Window);
      watermark = new Watermark();
      titleBeforeHeader = true;
      startPageEvent = "";
      finishPageEvent = "";
      manualBuildEvent = "";
      BaseName = "Page";
      unlimitedHeight = false;
      printOnRollPaper = false;
      unlimitedWidth = false;
      unlimitedHeightValue = MAX_PAPER_SIZE_MM * Units.Millimeters;
      unlimitedWidthValue = MAX_PAPER_SIZE_MM * Units.Millimeters;
    }
  }
}