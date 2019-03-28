using System;
using System.ComponentModel;
using System.Drawing.Design;
using FastReport.Utils;


namespace FastReport
{
  /// <summary>
  /// Base class for report components that can break across pages.
  /// </summary>
  public class BreakableComponent : ReportComponentBase
  {
    #region Fields
    private bool canBreak;
    private BreakableComponent breakTo;
    #endregion
    
    #region Properties
    /// <summary>
    /// Gets or sets a value that determines if the component can break its contents across pages.
    /// </summary>
    [DefaultValue(true)]
    [Category("Behavior")]
    public bool CanBreak
    {
      get { return canBreak; }
      set { canBreak = value; }
    }
    
    /// <summary>
    /// Gets or sets a reference to another similar object that will be used for displaying the
    /// text that not fit in this object.
    /// </summary>
    [Category("Behavior")]
    [TypeConverter(typeof(FastReport.TypeConverters.ComponentRefConverter))]
    [Editor("FastReport.TypeEditors.BandComponentRefEditor, FastReport", typeof(UITypeEditor))]
    public BreakableComponent BreakTo
    {
      get { return breakTo; }
      set 
      {
        if (breakTo != value)
        {
          if (breakTo != null)
            breakTo.Disposed -= new EventHandler(BreakTo_Disposed);
          if (value != null)
            value.Disposed += new EventHandler(BreakTo_Disposed);
        }
        breakTo = value;
      }
    }
    #endregion

    #region Private Methods
    private void BreakTo_Disposed(object sender, EventArgs e)
    {
      breakTo = null;
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Assign(Base source)
    {
      base.Assign(source);
      
      BreakableComponent src = source as BreakableComponent;
      CanBreak = src.CanBreak;
      BreakTo = src.BreakTo;
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      BreakableComponent c = writer.DiffObject as BreakableComponent;
      base.Serialize(writer);
      
      if (CanBreak != c.CanBreak)
        writer.WriteBool("CanBreak", CanBreak);
      if (writer.SerializeTo != SerializeTo.Preview && writer.SerializeTo != SerializeTo.SourcePages && 
        BreakTo != c.BreakTo)
        writer.WriteRef("BreakTo", BreakTo);  
    }
    
    /// <summary>
    /// Breaks the contents of the object.
    /// </summary>
    /// <param name="breakTo">Object to put the part of content to that does not fit in this object. These two 
    /// objects must have the same type.</param>
    /// <returns><b>true</b> if there is enough space in this object to display at least one text line.</returns>
    /// <remarks>
    /// <para>
    /// Do not call this method directly, it is used by the report engine. You should override it if
    /// you are writing a new FastReport object.
    /// </para>
    /// <para>
    /// This method must break the contents of the object. The part of content that fit in current object's
    /// bounds should remain in this object, the part that does not fit should be transferred to <b>breakTo</b>
    /// object. 
    /// </para>
    /// </remarks>
    public virtual bool Break(BreakableComponent breakTo)
    {
      return false;
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <b>BreakableComponent</b> class with default settings. 
    /// </summary>
    public BreakableComponent()
    {
      CanBreak = true;
    }
  }
}
