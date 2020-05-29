using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
#if NETSTANDARD || NETCOREAPP
using FastReport.Code.CodeDom.Compiler;
using FastReport.Code.CSharp;
#else
using System.CodeDom.Compiler;
using Microsoft.CSharp;
#endif
using FastReport.Utils;
using FastReport.Data;

namespace FastReport.Code
{
    internal partial class CsCodeHelper : CodeHelperBase
    {
        #region Private Methods
        private string GetEquivalentKeyword(string s)
        {
            if (s.EndsWith("[]"))
                return GetEquivalentKeyword1(s.Substring(0, s.Length - 2)) + "[]";
            return GetEquivalentKeyword1(s);
        }

        private string GetEquivalentKeyword1(string s)
        {
            switch (s)
            {
                case "Object":
                    return "object";
                case "String":
                    return "string";
                case "Char":
                    return "char";
                case "Byte":
                    return "byte";
                case "SByte":
                    return "sbyte";
                case "Int16":
                    return "short";
                case "UInt16":
                    return "ushort";
                case "Int32":
                    return "int";
                case "UInt32":
                    return "uint";
                case "Int64":
                    return "long";
                case "UInt64":
                    return "ulong";
                case "Single":
                    return "float";
                case "Double":
                    return "double";
                case "Decimal":
                    return "decimal";
                case "Boolean":
                    return "bool";
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
                result += "<";

                foreach (Type elementType in type.GetGenericArguments())
                {
                    result += GetTypeDeclaration(elementType) + ",";
                }

                result = result.Substring(0, result.Length - 1) + ">";
                return result;
            }
            else
                return type.Name;
        }
        #endregion

        #region Public Methods
        public override string EmptyScript()
        {
            return "using System;\r\nusing System.Collections;\r\nusing System.Collections.Generic;\r\n" +
              "using System.ComponentModel;\r\nusing System.Windows.Forms;\r\nusing System.Drawing;\r\n" +
              "using System.Data;\r\nusing FastReport;\r\nusing FastReport.Data;\r\nusing FastReport.Dialog;\r\n" +
              "using FastReport.Barcode;\r\nusing FastReport.Table;\r\nusing FastReport.Utils;\r\n\r\n" +
              "namespace FastReport\r\n{\r\n  public class ReportScript\r\n  {\r\n  }\r\n}\r\n";
        }

        public override int GetPositionToInsertOwnItems(string scriptText)
        {
            int pos = scriptText.IndexOf("public class ReportScript");
            if (pos == -1)
                return -1;
            return scriptText.IndexOf('{', pos) + 3;
        }

        public override string AddField(Type type, string name)
        {
            name = name.Replace(" ", "_");
            return "    public " + type.FullName + " " + name + ";\r\n";
        }

        public override string BeginCalcExpression()
        {
            return "    private object CalcExpression(string expression, Variant Value)\r\n    {\r\n      ";
        }

        public override string AddExpression(string expr, string value)
        {
            expr = expr.Replace("\\", "\\\\");
            expr = expr.Replace("\"", "\\\"");
            return "if (expression == \"" + expr + "\")\r\n        return " + value + ";\r\n      ";
        }

        public override string EndCalcExpression()
        {
            return "return null;\r\n    }\r\n\r\n";
        }

        public override string ReplaceColumnName(string name, Type type)
        {
            string typeName = GetTypeDeclaration(type);
            string result = "((" + typeName + ")Report.GetColumnValue(\"" + name + "\"";
            result += "))";
            return result;
        }

        public override string ReplaceParameterName(Parameter parameter)
        {
            string typeName = GetTypeDeclaration(parameter.DataType);
            return "((" + typeName + ")Report.GetParameterValue(\"" + parameter.FullName + "\"))";
        }

        public override string ReplaceVariableName(Parameter parameter)
        {
            string typeName = GetTypeDeclaration(parameter.DataType);
            return "((" + typeName + ")Report.GetVariableValue(\"" + parameter.FullName + "\"))";
        }

        public override string ReplaceTotalName(string name)
        {
            return "Report.GetTotalValue(\"" + name + "\")";
        }

        public override string GenerateInitializeMethod()
        {
            Hashtable events = new Hashtable();
            string reportString = StripEventHandlers(events);
            string result = "";

            // form the InitializeComponent method
            result += "    private void InitializeComponent()\r\n    {\r\n      ";

            // form the reportString
            result += "string reportString = \r\n        ";

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

                part = part.Replace("\\", "\\\\");
                part = part.Replace("\"", "\\\"");
                part = part.Replace("\r", "\\r");
                part = part.Replace("\n", "\\n");
                result += "\"" + part + "\"";
                if (reportString != "")
                {
                    if (totalLength > 1024)
                    {
                        totalLength = 0;
                        result += ";\r\n      reportString += ";
                    }
                    else
                        result += " +\r\n        ";
                    totalLength += part.Length;
                }
                else
                {
                    result += ";\r\n      ";
                }
            }

            result += "LoadFromString(reportString);\r\n      ";
            result += "InternalInit();\r\n      ";

            // form objects' event handlers
            foreach (DictionaryEntry de in events)
            {
                result += de.Key.ToString() + " += " + de.Value.ToString() + ";\r\n      ";
            }

            result += "\r\n    }\r\n\r\n";
            result += "    public ReportScript()\r\n    {\r\n      InitializeComponent();\r\n    }\r\n";
            return result;
        }

        public override string ReplaceClassName(string scriptText, string className)
        {
            return scriptText.Replace("class ReportScript", "class " + className + " : Report").Replace(
              "public ReportScript()", "public " + className + "()").Replace(
              "private object CalcExpression", "protected override object CalcExpression");
        }

        public override string GetMethodSignature(MethodInfo info, bool fullForm)
        {
            string result = info.Name + "(";
            string fontBegin = "<font color=\"Blue\">";
            string fontEnd = "</font>";
            if (fullForm)
                result = fontBegin + GetEquivalentKeyword(info.ReturnType.Name) + fontEnd + " " + result;

            System.Reflection.ParameterInfo[] pars = info.GetParameters();
            foreach (System.Reflection.ParameterInfo par in pars)
            {
                // special case - skip "thisReport" parameter
                if (par.Name == "thisReport")
                    continue;
                string paramType = "";
                object[] attr = par.GetCustomAttributes(typeof(ParamArrayAttribute), false);
                if (attr.Length > 0)
                    paramType = "params ";
                paramType += GetEquivalentKeyword(par.ParameterType.Name);
                result += (fullForm ? fontBegin : "") + paramType + (fullForm ? fontEnd : "");
                result += (fullForm ? " " + par.Name : "");
#if DOTNET_4
                if (par.IsOptional && fullForm)
                    result += CodeUtils.GetOptionalParameter(par, CodeUtils.Language.Cs);
#endif
                result += ", ";
            }

            if (result.EndsWith(", "))
                result = result.Substring(0, result.Length - 2);

            result += ")";
            return result;
        }

        public override string GetMethodSignatureAndBody(MethodInfo info)
        {
            string result = info.Name + "(";
            result = "    private " + GetTypeDeclaration(info.ReturnType) + " " + result;

            System.Reflection.ParameterInfo[] pars = info.GetParameters();
            foreach (System.Reflection.ParameterInfo par in pars)
            {
                // special case - skip "thisReport" parameter
                if (par.Name == "thisReport")
                    continue;

                string paramType = "";
                object[] attr = par.GetCustomAttributes(typeof(ParamArrayAttribute), false);
                if (attr.Length > 0)
                    paramType = "params ";
                paramType += GetTypeDeclaration(par.ParameterType);
                result += paramType;
                result += " " + par.Name;
#if DOTNET_4
                if (par.IsOptional)
                    result += CodeUtils.GetOptionalParameter(par, CodeUtils.Language.Cs);
#endif
                result += ", ";
            }

            if (result.EndsWith(", "))
                result = result.Substring(0, result.Length - 2);
            result += ")";

            result += "\r\n";
            result += "    {\r\n";
            result += "      return " + info.ReflectedType.Namespace + "." +
              info.ReflectedType.Name + "." + info.Name + "(";

            foreach (System.Reflection.ParameterInfo par in pars)
            {
                string parName = par.Name;
                // special case - handle "thisReport" parameter
                if (parName == "thisReport")
                    parName = "Report";
                result += parName + ", ";
            }

            if (result.EndsWith(", "))
                result = result.Substring(0, result.Length - 2);
            result += ");\r\n";
            result += "    }\r\n";
            result += "\r\n";

            return result;
        }


        public override CodeDomProvider GetCodeProvider()
        {
            return new CSharpCodeProvider();
        }

        #endregion

        public CsCodeHelper(Report report) : base(report)
        {
        }
    }

}