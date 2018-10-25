using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Format
{
  /// <summary>
  /// Represents a format used to display values with no formatting.
  /// </summary>
  public class GeneralFormat : FormatBase
  {
    #region Public Methods
    /// <inheritdoc/>
    public override FormatBase Clone()
    {
      return new GeneralFormat();
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      GeneralFormat f = obj as GeneralFormat;
      return f != null;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <inheritdoc/>
    public override string FormatValue(object value)
    {
      if (value != null)
        return value.ToString();
      return "";  
    }

    internal override string GetSampleValue()
    {
      return "";
    }
    #endregion
  }
}
