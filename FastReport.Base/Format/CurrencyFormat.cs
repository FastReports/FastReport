using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using FastReport.Utils;
using System.Globalization;

namespace FastReport.Format
{
  /// <summary>
  /// Defines how currency values are formatted and displayed.
  /// </summary>
  public class CurrencyFormat : FormatBase
  {
    #region Fields
    private bool useLocale;
    private int decimalDigits;
    private string decimalSeparator;
    private string groupSeparator;
    private string currencySymbol;
    private int positivePattern;
    private int negativePattern;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a value that determines whether to use system locale settings to format a value.
    /// </summary>
    [DefaultValue(true)]
    public bool UseLocale
    {
      get { return useLocale; }
      set { useLocale = value; }
    }

    /// <summary>
    /// Gets or sets the number of decimal places to use in currency values. 
    /// </summary>
    [DefaultValue(2)]
    public int DecimalDigits
    {
      get { return decimalDigits; }
      set { decimalDigits = value; }
    }
    
    /// <summary>
    /// Gets or sets the string to use as the decimal separator in currency values.
    /// </summary>
    public string DecimalSeparator
    {
      get { return decimalSeparator; }
      set { decimalSeparator = value; }
    }

    /// <summary>
    /// Gets or sets the string that separates groups of digits to the left of the decimal in currency values. 
    /// </summary>
    public string GroupSeparator
    {
      get { return groupSeparator; }
      set { groupSeparator = value; }
    }
    
    /// <summary>
    /// Gets or sets the string to use as the currency symbol.
    /// </summary>
    public string CurrencySymbol
    {
      get { return currencySymbol; }
      set { currencySymbol = value; }
    }

    /// <summary>
    /// Gets or sets the format pattern for positive currency values.
    /// </summary>
    /// <remarks>This property can have one of the values in the following table. 
    /// The symbol "$" is the <b>CurrencySymbol</b> and <i>n</i> is a number.
    /// <list type="table">
    ///   <listheader><term>Value</term><description>Associated Pattern</description></listheader>
    ///   <item><term>0</term><description>$n</description></item>
    ///   <item><term>1</term><description>n$</description></item>
    ///   <item><term>2</term><description>$ n</description></item>
    ///   <item><term>3</term><description>n $</description></item>
    /// </list>
    /// </remarks>
    [DefaultValue(0)]
    public int PositivePattern
    {
      get { return positivePattern; }
      set { positivePattern = value; }
    }

    /// <summary>
    /// Gets or sets the format pattern for negative currency values.
    /// </summary>
    /// <remarks>This property can have one of the values in the following table. 
    /// The symbol "$" is the <b>CurrencySymbol</b> and <i>n</i> is a number.
    /// <list type="table">
    ///   <listheader><term>Value</term><description>Associated Pattern</description></listheader>
    ///   <item><term>0</term> <description>($n)</description></item>
    ///   <item><term>1</term> <description>-$n</description></item>
    ///   <item><term>2</term> <description>$-n</description></item>
    ///   <item><term>3</term> <description>$n-</description></item>
    ///   <item><term>4</term> <description>(n$)</description></item>
    ///   <item><term>5</term> <description>-n$</description></item>
    ///   <item><term>6</term> <description>n-$</description></item>
    ///   <item><term>7</term> <description>n$-</description></item>
    ///   <item><term>8</term> <description>-n $</description></item>
    ///   <item><term>9</term> <description>-$ n</description></item>
    ///   <item><term>10</term><description>n $-</description></item>
    ///   <item><term>11</term><description>$ n-</description></item>
    ///   <item><term>12</term><description>$ -n</description></item>
    ///   <item><term>13</term><description>n- $</description></item>
    ///   <item><term>14</term><description>($ n)</description></item>
    ///   <item><term>15</term><description>(n $)</description></item>
    /// </list>
    /// </remarks>
    [DefaultValue(0)]
    public int NegativePattern
    {
      get { return negativePattern; }
      set { negativePattern = value; }
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override FormatBase Clone()
    {
      CurrencyFormat result = new CurrencyFormat();
      result.UseLocale = UseLocale;
      result.DecimalDigits = DecimalDigits;
      result.DecimalSeparator = DecimalSeparator;
      result.GroupSeparator = GroupSeparator;
      result.CurrencySymbol = CurrencySymbol;
      result.PositivePattern = PositivePattern;
      result.NegativePattern = NegativePattern;
      return result;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      CurrencyFormat f = obj as CurrencyFormat;
      return f != null &&
        UseLocale == f.UseLocale &&
        DecimalDigits == f.DecimalDigits &&
        DecimalSeparator == f.DecimalSeparator &&
        GroupSeparator == f.GroupSeparator &&
        CurrencySymbol == f.CurrencySymbol &&
        PositivePattern == f.PositivePattern &&
        NegativePattern == f.NegativePattern;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <inheritdoc/>
    public override string FormatValue(object value)
    {
      if (value is Variant)
        value = ((Variant)value).Value;

      return String.Format(GetNumberFormatInfo(), "{0:c}", new object[] { value });
    }

    internal NumberFormatInfo GetNumberFormatInfo()
    {
    
        NumberFormatInfo info = new NumberFormatInfo();
        if (UseLocale)
        {
            info.CurrencyDecimalDigits = DecimalDigits;
            info.CurrencyDecimalSeparator = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
            info.CurrencyGroupSizes = NumberFormatInfo.CurrentInfo.CurrencyGroupSizes;
            info.CurrencyGroupSeparator = NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator;
            info.CurrencySymbol = NumberFormatInfo.CurrentInfo.CurrencySymbol;
            info.CurrencyPositivePattern = NumberFormatInfo.CurrentInfo.CurrencyPositivePattern;
            info.CurrencyNegativePattern = NumberFormatInfo.CurrentInfo.CurrencyNegativePattern;
        }
        else
        {
            info.CurrencyDecimalDigits = DecimalDigits;
            info.CurrencyDecimalSeparator = DecimalSeparator;
            info.CurrencyGroupSizes = new int[] { 3 };
            info.CurrencyGroupSeparator = GroupSeparator;
            info.CurrencySymbol = CurrencySymbol;
            info.CurrencyPositivePattern = PositivePattern;
            info.CurrencyNegativePattern = NegativePattern;
        }
        return info;
    }

    internal override string GetSampleValue()
    {
      return FormatValue(-12345);
    }

    internal override void Serialize(FRWriter writer, string prefix, FormatBase format)
    {
      base.Serialize(writer, prefix, format);
      CurrencyFormat c = format as CurrencyFormat;
      
      if (c == null || UseLocale != c.UseLocale)
        writer.WriteBool(prefix + "UseLocale", UseLocale);
      if (c == null || DecimalDigits != c.DecimalDigits)
        writer.WriteInt(prefix + "DecimalDigits", DecimalDigits);

      if (!UseLocale)
      {
        if (c == null || DecimalSeparator != c.DecimalSeparator)
          writer.WriteStr(prefix + "DecimalSeparator", DecimalSeparator);
        if (c == null || GroupSeparator != c.GroupSeparator)
          writer.WriteStr(prefix + "GroupSeparator", GroupSeparator);
        if (c == null || CurrencySymbol != c.CurrencySymbol)
          writer.WriteStr(prefix + "CurrencySymbol", CurrencySymbol);
        if (c == null || PositivePattern != c.PositivePattern)
          writer.WriteInt(prefix + "PositivePattern", PositivePattern);
        if (c == null || NegativePattern != c.NegativePattern)
          writer.WriteInt(prefix + "NegativePattern", NegativePattern);
      }
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <b>CurrencyFormat</b> class with default settings. 
    /// </summary>
    public CurrencyFormat()
    {
      UseLocale = true;
      DecimalDigits = 2;
      DecimalSeparator = ".";
      GroupSeparator = ",";
      CurrencySymbol = "$";
    }
  }
}
