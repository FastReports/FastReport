using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Format
{
  /// <summary>
  /// Defines how date values are formatted and displayed.
  /// </summary>
  public class DateFormat : CustomFormat
  {
    #region Public Methods
    /// <inheritdoc/>
    public override FormatBase Clone()
    {
      DateFormat result = new DateFormat();
      result.Format = Format;
      return result;
    }

    internal override string GetSampleValue()
    {
      return FormatValue(new DateTime(2007, 11, 30, 13, 30, 0));
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <b>DateFormat</b> class with default settings. 
    /// </summary>
    public DateFormat()
    {
      Format = "d";
    }
  }
}
