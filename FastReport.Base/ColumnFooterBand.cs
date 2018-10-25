using System;
using System.Collections;
using System.ComponentModel;

namespace FastReport
{
  /// <summary>
  /// This class represents a column footer band.
  /// </summary>
  public class ColumnFooterBand : BandBase
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
    /// Initializes a new instance of the <see cref="ColumnFooterBand"/> class with default settings.
    /// </summary>
    public ColumnFooterBand()
    {
      FlagUseStartNewPage = false;
    }
  }
}