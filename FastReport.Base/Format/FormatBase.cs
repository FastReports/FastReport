using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.Format
{
  /// <summary>
  /// Base class for all formats.
  /// </summary>
  /// <remarks>
  /// The format is used to format expression value in a <see cref="TextObject"/> object. 
  /// </remarks>
  [TypeConverter(typeof(FastReport.TypeConverters.FormatConverter))]
  public abstract class FormatBase : IFRSerializable
  {
    #region Properties
    /// <summary>
    /// Gets the short format name (e.g. without a "Format" suffix).
    /// </summary>
    [Browsable(false)]
    public string Name
    {
      get { return GetType().Name.Replace("Format", ""); }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Creates exact copy of this format.
    /// </summary>
    /// <returns>The copy of this format.</returns>
    public abstract FormatBase Clone();
    
    /// <summary>
    /// Formats the specified value.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The string that represents the formatted value.</returns>
    public abstract string FormatValue(object value);

    internal abstract string GetSampleValue();

    internal virtual void Serialize(FRWriter writer, string prefix, FormatBase format)
    {
      if (format.GetType() != GetType())
        writer.WriteStr("Format", Name);
    }

    /// <inheritdoc/>
    public void Serialize(FRWriter writer)
    {
      writer.ItemName = GetType().Name;
      Serialize(writer, "", writer.DiffObject as FormatBase);
    }

    /// <inheritdoc/>
    public void Deserialize(FRReader reader)
    {
      reader.ReadProperties(this);
    }
    #endregion
  }
}
