using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Data;
using FastReport.Engine;
using FastReport.Utils;

namespace FastReport.Code
{
    /// <summary>
    /// This class is used to pass find arguments to some methods of the <b>CodeUtils</b> class.
    /// </summary>
    public class FindTextArgs
    {
        
        private int startIndex;
        private int endIndex;
        private string openBracket;
        private string closeBracket;
        private FastString text;
        private string foundText;

        /// <summary>
        /// The start position of the search. After the search, this property points to
        /// the begin of an expression.
        /// </summary>
        public int StartIndex
        {
            get { return startIndex; }
            set { startIndex = value; }
        }

        /// <summary>
        /// After the search, this property points to the end of an expression.
        /// </summary>
        public int EndIndex
        {
            get { return endIndex; }
            set { endIndex = value; }
        }

        /// <summary>
        /// The char sequence used to find the expression's begin.
        /// </summary>
        public string OpenBracket
        {
            get { return openBracket; }
            set { openBracket = value; }
        }

        /// <summary>
        /// The char sequence used to find the expression's end.
        /// </summary>
        public string CloseBracket
        {
            get { return closeBracket; }
            set { closeBracket = value; }
        }

        /// <summary>
        /// The text with embedded expressions.
        /// </summary>
        public FastString Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// The last found expression.
        /// </summary>
        public string FoundText
        {
            get { return foundText;  }
            set { foundText = value; }
        }
     
    }

    /// <summary>
    /// This static class contains methods that may be used to find expressions embedded 
    /// in the object's text.
    /// </summary>
    public static class CodeUtils
    {
        internal enum Language
        {
            Cs,
            Vb
        }
        private static bool isTypeSuffixesInitialized = false;
        private static Dictionary<Type, string[]> typeSuffixes;

        #region Private Methods
        private static bool TypeHasSuffix(Type type)
        {
            return typeSuffixes.ContainsKey(type);
        }
        private static string GetTypeSuffix(Type type, Language lang)
        {
            string[] suffix;
            typeSuffixes.TryGetValue(type, out suffix);
            return suffix[(int)lang];
        }
        private static void InitializeTypeSuffixes()
        {
            typeSuffixes = new Dictionary<Type, string[]>();
            //c# and vb type prefixes
            typeSuffixes.Add(typeof(float), new string[] { "F", "F" });
            typeSuffixes.Add(typeof(double), new string[] { "D", "R" });
            typeSuffixes.Add(typeof(uint), new string[] { "U", "" });
            typeSuffixes.Add(typeof(long), new string[] { "L", "L" });
            typeSuffixes.Add(typeof(ulong), new string[] { "UL", "UL" });
            typeSuffixes.Add(typeof(decimal), new string[] { "M", "D" });
            isTypeSuffixesInitialized = true;
        }
        // adjusts StartIndex to the next char after end of string. Returns true if string is correct.
        private static bool SkipString(FindTextArgs args)
        {
            if (args.Text[args.StartIndex] == '"')
                args.StartIndex++;
            else
                return true;

            while (args.StartIndex < args.Text.Length)
            {
                if (args.Text[args.StartIndex] == '"')
                {
                    if (args.Text[args.StartIndex - 1] != '\\')
                    {
                        args.StartIndex++;
                        return true;
                    }
                }
                args.StartIndex++;
            }
            return false;
        }

        // find matching open and close brackets starting from StartIndex. Takes strings into account.
        // Returns true if matching brackets found. Also returns FoundText with text inside brackets, 
        // StartIndex pointing to the OpenBracket and EndIndex pointing to the next char after CloseBracket.
        private static bool FindMatchingBrackets(FindTextArgs args, bool skipLeadingStrings)
        {
            if (!skipLeadingStrings)
            {
                args.StartIndex = args.Text.IndexOf(args.OpenBracket, args.StartIndex);
                if (args.StartIndex == -1)
                    return false;
            }

            int saveStartIndex = 0;
            int brCount = 0;

            while (args.StartIndex < args.Text.Length)
            {
                if (!SkipString(args))
                    return false;
                if (args.StartIndex + args.OpenBracket.Length > args.Text.Length)
                    return false;
                if (args.Text.SubstringCompare(args.StartIndex, args.OpenBracket))
                {
                    if (brCount == 0)
                        saveStartIndex = args.StartIndex;
                    brCount++;
                }
                else if (args.Text.SubstringCompare(args.StartIndex, args.CloseBracket))
                {
                    brCount--;
                    if (brCount == 0)
                    {
                        args.EndIndex = args.StartIndex + args.CloseBracket.Length;
                        args.StartIndex = saveStartIndex;
                        args.FoundText = args.Text.Substring(args.StartIndex + args.OpenBracket.Length,
                          args.EndIndex - args.StartIndex - args.OpenBracket.Length - args.CloseBracket.Length);
                        return true;
                    }
                }
                args.StartIndex++;
            }
            return false;
        }
        #endregion

        #region Internal Methods
        // determines whether given index is inside brackets, or is after OpenBracket
        internal static bool IndexInsideBrackets(FindTextArgs args)
        {
            int pos = args.StartIndex;
            args.StartIndex = 0;

            while (args.StartIndex < pos)
            {
                // find open bracket
                args.StartIndex = args.Text.IndexOf(args.OpenBracket, args.StartIndex);
                if (args.StartIndex == -1)
                    return false;

                // missing close bracket
                if (!FindMatchingBrackets(args, false))
                    return true;
                // pos is inside brackets
                if (args.StartIndex < pos && args.EndIndex > pos)
                    return true;
                args.StartIndex = args.EndIndex;
            }
            return false;
        }

        internal static string GetOptionalParameter(System.Reflection.ParameterInfo par, Language lang)
        {
            if(!isTypeSuffixesInitialized)
                InitializeTypeSuffixes();
            string optionalParamString = " = ";
            if (par.DefaultValue.GetType().IsEnum)
                optionalParamString += par.DefaultValue.GetType().Name + ".";
            optionalParamString += par.DefaultValue.ToString();
            if (TypeHasSuffix(par.ParameterType))
                optionalParamString += GetTypeSuffix(par.ParameterType, lang);
            return optionalParamString;
        }

        internal static string FixExpressionWithBrackets(string expression)
        {
            string result = expression;
            if (expression.StartsWith("[") && expression.EndsWith("]"))
            {
                string tempExpression = expression.Substring(1, expression.Length - 2);
                int firstOpen = tempExpression.IndexOf("[");
                int firstClose = tempExpression.IndexOf("]");
                int lastOpen = tempExpression.LastIndexOf("[");
                int lastClose = tempExpression.LastIndexOf("]");
                if ((firstOpen < 0 && firstClose >= 0) || (lastOpen >= 0 && lastClose < 0)
                    || (firstOpen >= 0 && firstClose >= 0 && firstClose < firstOpen)
                    || (lastOpen >= 0 && lastClose >= 0 && lastOpen > lastClose))
                {
                    result = expression;
                }
                else
                {
                    result = tempExpression;
                }
            }
            else
            {
                result = expression;
            }
            return result;
        }

        #endregion

        /// <summary>
        /// Returns expressions found in the text.
        /// </summary>
        /// <param name="text">Text that may contain expressions.</param>
        /// <param name="openBracket">The char sequence used to find the start of expression.</param>
        /// <param name="closeBracket">The char sequence used to find the end of expression.</param>
        /// <returns>Array of expressions if found; otherwise return an empty array.</returns>
        public static string[] GetExpressions(string text, string openBracket, string closeBracket)
        {
            List<string> expressions = new List<string>();
            FindTextArgs args = new FindTextArgs();
            args.Text = new FastString(text);
            args.OpenBracket = openBracket;
            args.CloseBracket = closeBracket;

            while (args.StartIndex < args.Text.Length) //text.Length
            {
                if (!FindMatchingBrackets(args, false))
                    break;
                expressions.Add(args.FoundText);
                args.StartIndex = args.EndIndex;
            }

            return expressions.ToArray();
        }

        /// <summary>
        /// Gets first expression found in the text. 
        /// </summary>
        /// <param name="args">Object with find arguments.</param>
        /// <param name="skipStrings">Indicates whether to skip strings.</param>
        /// <returns>The expression if found; otherwise, returns an empty string.</returns>
        public static string GetExpression(FindTextArgs args, bool skipStrings)
        {
            if (args.StartIndex < args.Text.Length)
            {
                if (FindMatchingBrackets(args, skipStrings))
                    return args.FoundText;
            }
            return "";
        }
    }
}
