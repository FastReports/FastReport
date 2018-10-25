using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FastReport.Utils
{
  internal static class FileUtils
  {
    public static string GetRelativePath(string absPath, string baseDirectoryPath)
    {
      char[] separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
      baseDirectoryPath = Path.GetFullPath(baseDirectoryPath);
      absPath = Path.GetFullPath(absPath);
      baseDirectoryPath = baseDirectoryPath.TrimEnd(separators);
      
      string[] bPath = baseDirectoryPath.Split(separators);
      string[] aPath = absPath.Split(separators);
      int indx = 0;
      while (indx < Math.Min(bPath.Length, aPath.Length))
      {
        if (String.Compare(aPath[indx], bPath[indx], true) != 0)
          break;
        indx++;  
      }
      // no matches, return absPath
      if (indx == 0)
        return absPath;

      string result = "";
      for (int i = indx; i < bPath.Length; i++)
      {
        result += ".." + Path.DirectorySeparatorChar;
      }
      result += String.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length - indx);
      return result;
    }

    public static void CopyStream(Stream input, Stream output)
    {
        byte[] buffer = new byte[32768];
        int read;
        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            output.Write(buffer, 0, read);
    }
  }
}
