using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;

namespace FastReport.Data
{
  internal static class CubeHelper
  {
    public static CubeSourceBase GetCubeSource(Dictionary dictionary, string complexName)
    {
      if (String.IsNullOrEmpty(complexName))
        return null;
      string[] names = complexName.Split(new char[] { '.' });
      return dictionary.FindByAlias(names[0]) as CubeSourceBase;
    }
  }
}
