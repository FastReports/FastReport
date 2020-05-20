using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
#if NETSTANDARD || NETCOREAPP
using FastReport.Code.CodeDom.Compiler;
using FastReport.Code.VisualBasic;
#else
using System.CodeDom.Compiler;
using Microsoft.VisualBasic;
#endif
using FastReport.Utils;
using FastReport.Data;

namespace FastReport.Code
{
    internal partial class VbCodeHelper : CodeHelperBase
    {
        #region Private Methods
        private string GetEquivalentKeyword(string s)
        {
            if (s.EndsWith("[]"))
                return GetEquivalentKeyword1(s.Substring(0, s.Length - 2)) + "()";
            return GetEquivalentKeyword1(s);
        }

        private string GetEquivalentKeyword1(string s)
        {
            switch (s)
            {
                case "DateTime":
                    return "Date";
                case "Int16":
                    return "Short";
                case "UInt16":
                    return "UShort";
                case "Int32":
                    return "Integer";
                case "UInt32":
                    return "UInteger";
                case "Int64":
                    return "Long";
                case "UInt64":
                    return "ULong";
            }
            return s;
        }
        #endregion

        #region Protected Methods
        protected override string GetTypeDeclaration(Type type)
        {
            if (type.IsGenericType)
            {
                string result = type.Name;
                result = result.Substring(0, result.IndexOf('`'));
                result += "(Of ";

                foreach (Type elementType in type.GetGenericArguments())
                {
                    result += GetTypeDeclaration(elementType) + ",";
                }

                result = result.Substring(0, result.Length - 1) + ")";
                return result;
            }
            else
            {
                string typeName = type.Name;
                typeName = typeName.Replace("[]", "()");
                return typeName;
            }
        }
        #endregion

        #region Public Methods
        public override string EmptyScript()
        {
            return "Imports System\r\nImports System.Collections\r\nImports System.Collections.Generic\r\n" +
              "Imports System.ComponentModel\r\nImports System.Windows.Forms\r\nImports System.Drawing\r\n" +
              "Imports Microsoft.VisualBasic\r\n" +
              "Imports FastReport\r\nImports FastReport.Data\r\nImports FastReport.Dialog\r\nImports FastReport.Table\r\n" +
              "Imports FastReport.Barcode\r\nImports FastReport.Utils\r\n\r\nNamespace FastReport\r\n" +
              "  Public Class ReportScript\r\n\r\n  End Class\r\nEnd Namespace\r\n";
        }

        public override int GetPositionToInsertOwnItems(string scriptText)
        {
            int pos = scriptText.IndexOf("Public Class ReportScript");
            if (pos == -1)
                return -1;
            return scriptText.IndexOf('\n', pos) + 1;
        }

        public override string AddField(Type type, string name)
        {
            name = name.Replace(" ", "_");
            return "    Public " + name + " as Global." + type.FullName + "\r\n";
        }

        public override string BeginCalcExpression()
        {
            return "    Private Function CalcExpression(ByVal expression As String, ByVal Value as Global.FastReport.Variant) As Object\r\n      ";
        }

        public override string AddExpression(string expr, string value)
        {
            expr = expr.Replace("\"", "\"\"");
            return "If expression = \"" + expr + "\" Then\r\n        Return " + value + "\r\n      End If\r\n      ";
        }

        public override string EndCalcExpression()
        {
            return "Return Nothing\r\n    End Function\r\n\r\n";
        }

        public override string ReplaceColumnName(string name, Type type)
        {
            string typeName = GetTypeDeclaration(type);
            string result = "CType(Report.GetColumnValue(\"" + name + "\"";
            result += "), " + typeName + ")";
            return result;
        }

        public override string ReplaceParameterName(Parameter parameter)
        {
            string typeName = GetTypeDeclaration(parameter.DataType);
            return "CType(Report.GetParameterValue(\"" + parameter.FullName + "\"), " + typeName + ")";
        }

        public override string ReplaceVariableName(Parameter parameter)
        {
            string typeName = GetTypeDeclaration(parameter.DataType);
            return "CType(Report.GetVariableValue(\"" + parameter.FullName + "\"), " + typeName + ")";
        }

        public override string ReplaceTotalName(string name)
        {
            return "Report.GetTotalValue(\"" + name + "\")";
        }

        public override string GenerateInitializeMethod()
        {
            Hashtable events = new Hashtable();
            string reportString = StripEventHandlers(events);
            string result = "    Private Sub InitializeComponent\r\n      ";

            // form the reportString
            result += "Dim reportString As String = _\r\n        ";

            int totalLength = 0;
            while (reportString.Length > 0)
            {
                string part = "";
                if (reportString.Length > 80)
                {
                    part = reportString.Substring(0, 80);
                    reportString = reportString.Substring(80);
                }
                else
                {
                    part = reportString;
                    reportString = "";
                }

                part = "\"" + part.Replace("\"", "\"\"").Replace("\u201c", "\"\"").Replace("\u201d", "\"\"") + "\"";
                part = part.Replace("\r\n", "\" + ChrW(13) + ChrW(10) + \"");
                part = part.Replace("\r", "\" + ChrW(13) + \"");
                part = part.Replace("\n", "\" + ChrW(10) + \"");
                result += part;
                if (reportString != "")
                {
                    if (totalLength > 1024)
                    {
                        totalLength = 0;
                        result += "\r\n      reportString = reportString + ";
                    }
                    else
                        result += " + _\r\n        ";
                    totalLength += part.Length;
                }
                else
                {
                    result += "\r\n      ";
                }
            }

            result += "LoadFromString(reportString)\r\n      ";
            result += "InternalInit()\r\n      ";

            // form objects' event handlers
            foreach (DictionaryEntry de in events)
            {
                result += "AddHandler " + de.Key.ToString() + ", AddressOf " +
                  de.Value.ToString() + "\r\n      ";
            }

            result += "\r\n    End Sub\r\n\r\n";
            result += "    Public Sub New()\r\n      InitializeComponent()\r\n    End Sub\r\n";
            return result;
        }

        public override string ReplaceClassName(string scriptText, string className)
        {
            // replace the first occurence of "ReportScript"
            string replace = "Class ReportScript";
            int index = scriptText.IndexOf(replace);
            scriptText = scriptText.Remove(index, replace.Length);
            scriptText = scriptText.Insert(index, "Class " + className + "\r\n    Inherits Report");
            // replace other items
            return scriptText.Replace("Private Function CalcExpression", "Protected Overrides Function CalcExpression");
        }

        public override string GetMethodSignature(MethodInfo info, bool fullForm)
        {
            string result = info.Name + "(";
            string fontBegin = "<font color=\"Blue\">";
            string fontEnd = "</font>";

            System.Reflection.ParameterInfo[] pars = info.GetParameters();
            foreach (System.Reflection.ParameterInfo par in pars)
            {
                // special case - skip "thisReport" parameter
                if (par.Name == "thisReport")
                    continue;

                string modifier = "ByVal";
                if (par.IsOptional)
                    modifier = "Optional " + modifier;
                object[] attr = par.GetCustomAttributes(typeof(ParamArrayAttribute), false);
                if (attr.Length > 0)
                    modifier += " ParamArray";
                result += fullForm ? fontBegin + modifier + fontEnd + " " + par.Name + " " + fontBegin + "As" + fontEnd + " " : "";
                result += (fullForm ? fontBegin : "") + GetEquivalentKeyword(par.ParameterType.Name) + (fullForm ? fontEnd : "");
#if DOTNET_4
                if (par.IsOptional && fullForm)
                    result += CodeUtils.GetOptionalParameter(par, CodeUtils.Language.Vb);
#endif
                result += ", ";
            }

            if (result.EndsWith(", "))
                result = result.Substring(0, result.Length - 2);

            result += ")";
            if (fullForm)
                result += " " + fontBegin + "As " + info.ReturnType.Name + fontEnd;
            return result;
        }

        public override string GetMethodSignatureAndBody(MethodInfo info)
        {
            string result = info.Name + "(";
            result = "    Private Function " + result;

            System.Reflection.ParameterInfo[] pars = info.GetParameters();
            foreach (System.Reflection.ParameterInfo par in pars)
            {
                // special case - skip "thisReport" parameter
                if (par.Name == "thisReport")
                    continue;

                string parName = "_" + par.Name;
                string modifier = "ByVal";
                if (par.IsOptional)
                    modifier = "Optional " + modifier;
                object[] attr = par.GetCustomAttributes(typeof(ParamArrayAttribute), false);
                if (attr.Length > 0)
                    modifier += " ParamArray";
                result += modifier + " " + parName + " As ";
                result += GetTypeDeclaration(par.ParameterType);
#if DOTNET_4
                if (par.IsOptional)
                    result += CodeUtils.GetOptionalParameter(par, CodeUtils.Language.Vb);
#endif
                result += ", ";
            }

            if (result.EndsWith(", "))
                result = result.Substring(0, result.Length - 2);
            result += ")";

            result += " As " + GetTypeDeclaration(info.ReturnType);
            result += "\r\n";
            result += "      Return Global." + info.ReflectedType.Namespace + "." +
              info.ReflectedType.Name + "." + info.Name + "(";

            foreach (System.Reflection.ParameterInfo par in pars)
            {
                string parName = "_" + par.Name;
                // special case - handle "thisReport" parameter
                if (parName == "_thisReport")
                    parName = "Report";

                result += parName + ", ";
            }

            if (result.EndsWith(", "))
                result = result.Substring(0, result.Length - 2);
            result += ")\r\n";
            result += "    End Function\r\n";
            result += "\r\n";

            return result;
        }

        public override CodeDomProvider GetCodeProvider()
        {
            return new VBCodeProvider();
        }
        #endregion

        public VbCodeHelper(Report report) : base(report)
        {
        }
    }

}