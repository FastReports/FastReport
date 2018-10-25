using System;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport
{
  /// <summary>
  /// This class represents a child band.
  /// </summary>
  /// <remarks>
  /// Typical use of child band is to print several objects that can grow or shrink. It also can be done
  /// using the shift feature (via <see cref="ShiftMode"/> property), but in some cases it's not possible.
  /// </remarks>
  public partial class ChildBand : BandBase
  {
    private bool fillUnusedSpace;
    private int completeToNRows;
    private bool printIfDatabandEmpty;
    
    /// <summary>
    /// Gets or sets a value indicating that band will be used to fill unused space on a page.
    /// </summary>
    /// <remarks>
    /// If you set this property to <b>true</b>, the band will be printed several times to fill 
    /// unused space on a report page.
    /// </remarks>
    [Category("Behavior")]
    [DefaultValue(false)]
    public bool FillUnusedSpace
    {
      get { return fillUnusedSpace; }
      set { fillUnusedSpace = value; }
    }

    /// <summary>
    /// Gets or sets a value that determines the overall number of data rows printed by the data band.
    /// </summary>
    /// <remarks>
    /// Using this property, you may complete the data band upto N data rows.
    /// If the data band has less number of rows, this band will be used to print empty rows.
    /// </remarks>
    [Category("Behavior")]
    [DefaultValue(0)]
    public int CompleteToNRows
    {
      get { return completeToNRows; }
      set { completeToNRows = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating that the band will be printed if its parent databand is empty.
    /// </summary>
    /// <remarks>
    /// The child band with this property set to true, connected to a databand can be used to print "No data" 
    /// text if the databand has no rows.
    /// </remarks>
    [Category("Behavior")]
    [DefaultValue(false)]
    public bool PrintIfDatabandEmpty
    {
      get { return printIfDatabandEmpty; }
      set { printIfDatabandEmpty = value; }
    }

    internal BandBase GetTopParentBand
    {
      get
      {
        BandBase band = this;
        while (band is ChildBand)
        {
          band = band.Parent as BandBase;
        }
        
        return band;
      }
    }

    /// <inheritdoc/>
    public override void Assign(Base source)
    {
      base.Assign(source);
      ChildBand src = source as ChildBand;
      FillUnusedSpace = src.FillUnusedSpace;
      CompleteToNRows = src.CompleteToNRows;
      PrintIfDatabandEmpty = src.PrintIfDatabandEmpty;
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      ChildBand c = writer.DiffObject as ChildBand;
      base.Serialize(writer);

      if (FillUnusedSpace != c.FillUnusedSpace)
        writer.WriteBool("FillUnusedSpace", FillUnusedSpace);
      if (CompleteToNRows != c.CompleteToNRows)
        writer.WriteInt("CompleteToNRows", CompleteToNRows);
      if (PrintIfDatabandEmpty != c.PrintIfDatabandEmpty)
        writer.WriteBool("PrintIfDatabandEmpty", PrintIfDatabandEmpty);
    }
  }
}