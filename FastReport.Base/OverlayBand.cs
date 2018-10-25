using System;
using System.Collections;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport
{
  /// <summary>
  /// Represents an overlay band.
  /// </summary>
  public class OverlayBand : BandBase
  {
    #region Properties
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new bool StartNewPage
    {
      get { return base.StartNewPage; }
      set { base.StartNewPage = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new bool PrintOnBottom
    {
      get { return base.PrintOnBottom; }
      set { base.PrintOnBottom = value; }
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="OverlayBand"/> class with default settings.
    /// </summary>
    public OverlayBand()
    {
      FlagUseStartNewPage = false;
    }
  }
}