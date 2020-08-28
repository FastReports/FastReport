using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastReport.Utils;
using System.Text.RegularExpressions;

namespace FastReport.Web
{
    partial class WebReport
    {
        /// <summary>
        /// Sets custom class for checking the report script.
        /// </summary>
        /// <param name="scriptChecker"></param>
        public static void SetScriptSecurity(IScriptChecker scriptChecker)
        {
            ScriptSecurity.Dispose();
            ScriptSecurity = new ScriptSecurity(scriptChecker);
        }

    }

    internal class ScriptSecurity : IDisposable
    {
        private IScriptChecker ScriptChecker;

        internal ScriptSecurity(IScriptChecker checker)
        {
            ScriptChecker = checker;
            Config.ScriptCompile += Config_ScriptCompile;
        }

        internal void Config_ScriptCompile(object sender, ScriptSecurityEventArgs e)
        {
            if(Config.EnableScriptSecurity)
                e.IsValid = ScriptChecker.IsValid(e.ReportLanguage, e.ReportScript, e.References, e.Report);
        }

        public void Dispose()
        {
            Config.ScriptCompile -= Config_ScriptCompile;
        }
    }

    /// <summary>
    /// Interface for overriding the standard check of the report script
    /// <see cref="IsValid(Language, string, string[], Report)"/>
    /// </summary>
    public interface IScriptChecker
    {
        /// <summary>
        /// Method for checking the report script
        /// </summary>
        /// <param name="lang">Report script language</param>
        /// <param name="reportScript">Report script</param>
        /// <param name="references">Referenced assemblies</param>
        /// <param name="report">Report</param>
        /// <returns>Returns true if the report passed the validation check</returns>
        bool IsValid(Language lang, string reportScript, string[] references, Report report);
    }

    internal class ScriptChecker : IScriptChecker
    {
        public bool IsValid(Language lang, string reportScript, string[] references, Report report)
        {
            // LOGIC
            foreach(string reference in references)
            {
                // in .Net Core need to add reference
                if (reference.IndexOf("System.IO.FileSystem") != -1)
                    return false;

                if (reference.IndexOf("Microsoft.AspNetCore") != -1)
                    return false;

                if(reference.IndexOf("System.Net") != -1)
                    return false;
            }

            foreach (string pattern in Config.ScriptSecurityProps.StopList)
            {
                if (reportScript.IndexOf(pattern) != -1)
                    return false;

                //regex = new Regex(pattern);
                //if (regex.IsMatch(reportScript))
            }

            return true;
        }
    }
}
