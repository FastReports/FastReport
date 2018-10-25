using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace FastReport.Functions
{
  /// <summary>
  /// Based on code of Stefan Böther,  xprocs@hotmail.de
  /// </summary>
  internal static class Roman
  {
    private static int MAX = 3998;
    private static string[,] romanDigits = new string[,] {
        {"M",      "C",    "X",    "I"    },
        {"MM",     "CC",   "XX",   "II"   },
        {"MMM",    "CCC",  "XXX",  "III"  },
        {null,     "CD",   "XL",   "IV"   },
        {null,     "D",    "L",    "V"    },
        {null,     "DC",   "LX",   "VI"   },
        {null,     "DCC",  "LXX",  "VII"  },
        {null,     "DCCC", "LXXX", "VIII" },
        {null,     "CM",   "XC",   "IX"   }};

    public static string Convert(int value)
    {
      if (value > MAX)
        throw new ArgumentOutOfRangeException("value");

      StringBuilder result = new StringBuilder(15);

      for (int index = 0; index < 4; index++)
      {
        int power = (int)Math.Pow(10, 3 - index);
        int digit = value / power;
        value -= digit * power;
        if (digit > 0)
          result.Append(romanDigits[digit - 1, index]);
      }

      return result.ToString();
    }
  }
}