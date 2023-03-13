using System.IO;
using System.Reflection;

namespace ConsoleAppsUtils
{
    public static class Utils
    {
        private static string GetDirectory(string subPath)
        {
            if (subPath == null)
                return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            return Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), subPath);
        }

        public static string FindDirectory(string directory1)
        {
            string thisFolder = GetDirectory(null);
            try
            {
                for (int i = 0; i < 50; i++)
                {

                    string dir = Path.Combine(thisFolder, directory1);
                    if (Directory.Exists(dir))
                        return dir;
                    thisFolder = Directory.GetParent(thisFolder).FullName;

                }
            }
            catch { return ""; }
            return "";
        }
    }
}
