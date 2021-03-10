using System.Text;

namespace ClickHouse.Client.Utility
{
    public static class StringExtensions
    {
        public static string Escape(this string str) => "'" + str.Replace("\\", "\\\\").Replace("\'", "\\\'") + "'";

        /// <summary>
        /// Encloses column name in backticks (`). Escapes ` symbol if met inside name
        /// Does nothing if column is already enclosed
        /// </summary>
        /// <param name="str">Column name</param>
        /// <returns>Backticked column name</returns>
        public static string EncloseColumnName(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            if (str[0] == '`' && str[str.Length - 1] == '`')
                return str; // Early return if already enclosed

            var builder = new StringBuilder();
            builder.Append('`');
            builder.Append(str.Replace("`", "\\`"));
            builder.Append('`');
            return builder.ToString();
        }
    }
}
