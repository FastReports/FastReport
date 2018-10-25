using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport
{
  /// <summary>
  /// Base class for headers and footers which support the "Keep With Data" and "Repeat on Every Page" features.
  /// </summary>
  public partial class HeaderFooterBandBase : BandBase
  {
    #region Fields
    private bool keepWithData;
    private bool repeatOnEveryPage;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a value indicating that the band should be printed together with data band.
    /// </summary>
    [DefaultValue(false)]
    [Category("Behavior")]
    public bool KeepWithData
    {
      get { return keepWithData; }
      set { keepWithData = value; }
    }

    /// <summary>
    /// Gets or sets a value that determines whether to repeat this band on every page.
    /// </summary>
    /// <remarks>
    /// When band is repeated, its <see cref="BandBase.Repeated"/> property is set to <b>true</b>. You can use
    /// it to show any additional information on the band. To do this,
    /// use the <see cref="ReportComponentBase.PrintOn"/> property which
    /// can be set to "Rpeeated". In that case the object will be printed
    /// only on the repeated band.
    /// </remarks>
    [DefaultValue(false)]
    [Category("Behavior")]
    public bool RepeatOnEveryPage
    {
      get { return repeatOnEveryPage; }
      set { repeatOnEveryPage = value; }
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Assign(Base source)
    {
      base.Assign(source);
      HeaderFooterBandBase src = source as HeaderFooterBandBase;
      KeepWithData = src.KeepWithData;
      RepeatOnEveryPage = src.RepeatOnEveryPage;
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      HeaderFooterBandBase c = writer.DiffObject as HeaderFooterBandBase;
      base.Serialize(writer);
      
      if (KeepWithData != c.KeepWithData)
        writer.WriteBool("KeepWithData", KeepWithData);
      if (RepeatOnEveryPage != c.RepeatOnEveryPage)
        writer.WriteBool("RepeatOnEveryPage", RepeatOnEveryPage);
    }
    #endregion
  }
}
