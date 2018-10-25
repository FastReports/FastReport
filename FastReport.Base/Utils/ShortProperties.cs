using System;
using System.Collections;
using System.Collections.Generic;

namespace FastReport.Utils
{
  internal static class ShortProperties
  {
    private static SortedList<string, string> FProps = new SortedList<string, string>();

    public static void Add(string shortName, string name)
    {
      if (!FProps.ContainsKey(shortName))
        FProps.Add(shortName, name);
    }

    public static string GetFullName(string name)
    {
      int i = FProps.IndexOfKey(name);
      return i == -1 ? name : FProps.Values[i];
    }

    public static string GetShortName(string name)
    {
      int i = FProps.IndexOfValue(name);
      if (i != -1)
        return FProps.Keys[i];
      else
        return name;
    }
    
    static ShortProperties()
    {
      Add("l", "Left");
      Add("t", "Top");
      Add("w", "Width");
      Add("h", "Height");
      Add("x", "Text");
    }
  }
}