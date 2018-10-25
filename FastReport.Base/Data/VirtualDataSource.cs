using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace FastReport.Data
{
  internal class VirtualDataSource : DataSourceBase
  {
    private int virtualRowsCount;

    public int VirtualRowsCount
    {
      get { return virtualRowsCount; }
      set { virtualRowsCount = value; }
    }

    #region Protected Methods
    /// <inheritdoc/>
    protected override object GetValue(Column column)
    {
      return null;
    }
    #endregion

    #region Public Methods
    public override void InitSchema()
    {
     // do nothing
    }

    public override void LoadData(ArrayList rows)
    {
      rows.Clear();
      for (int i = 0; i < virtualRowsCount; i++)
      {
        rows.Add(0);
      }
    }
    #endregion
  }
}
