using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace FastReport.Code
{
  internal class AssemblyCollection : CollectionBase
  {
    public AssemblyDescriptor this[int index]
    {
      get { return List[index] as AssemblyDescriptor; }
      set { List[index] = value; }
    }

    public void AddRange(AssemblyDescriptor[] range)
    {
      foreach (AssemblyDescriptor t in range)
      {
        Add(t);
      }
    }

    public int Add(AssemblyDescriptor value)
    {
      if (value == null)
        return -1;
      return List.Add(value);
    }

    public void Insert(int index, AssemblyDescriptor value)
    {
      if (value != null)
        List.Insert(index, value);
    }

    public void Remove(AssemblyDescriptor value)
    {
      List.Remove(value);
    }

    public int IndexOf(AssemblyDescriptor value)
    {
      return List.IndexOf(value);
    }

    public bool Contains(AssemblyDescriptor value)
    {
      return List.Contains(value);
    }
  }
}
