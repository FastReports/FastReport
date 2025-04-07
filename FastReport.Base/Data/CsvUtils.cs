using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Net;
using FastReport.Utils;
using System.Globalization;
using System.Collections;

namespace FastReport.Data
{
    internal static class CsvUtils
    {
        /// <summary>
        /// The default field name.
        /// </summary>
        public const string DEFAULT_FIELD_NAME = "Field";

        private static void DetermineTypes(List<string[]> lines, DataTable table, NumberFormatInfo numberInfo, NumberFormatInfo currencyInfo, DateTimeFormatInfo dateTimeInfo)
        {
            int intTemp;
            double doubleTemp;
            decimal decimalTemp;
            DateTime dateTemp;

            for (int i = 0; i < table.Columns.Count; i++)
            {
                // gather types here
                Dictionary<Type, int> types = new Dictionary<Type, int>();

                // check all values in the column
                for (int j = 0; j < lines.Count; j++)
                {
                    if (i >= lines[j].Length)
                    {
                        // number of values is less than number of table columns. Reasons: wrong separator or bad-formed csv file?
                        // just skip this line
                    }
                    else
                    {
                        string value = lines[j][i];
                        if (!String.IsNullOrEmpty(value))
                        {
                            if (Int32.TryParse(value, out intTemp))
                            {
                                types[typeof(Int32)] = 1;
                            }
                            else if (value.Contains(currencyInfo.CurrencySymbol) && Decimal.TryParse(value, NumberStyles.Currency, currencyInfo, out decimalTemp))
                            {
                                types[typeof(Decimal)] = 1;
                            }
                            else if (Double.TryParse(value, NumberStyles.Number, numberInfo, out doubleTemp))
                            {
                                types[typeof(Double)] = 1;
                            }
                            else if (DateTime.TryParse(value, dateTimeInfo, DateTimeStyles.NoCurrentDateDefault, out dateTemp))
                            {
                                types[typeof(DateTime)] = 1;
                            }
                            else
                            {
                                types[typeof(String)] = 1;
                                break;
                            }
                        }
                    }
                }

                // cases allowed:
                // - single type -> the type
                // - mix of ints and doubles -> double
                // - all others should not be mixed -> string
                Type guessType = typeof(String);
                if (types.Count == 1)
                {
                    // get a single value this way
                    foreach (Type t in types.Keys)
                    {
                        guessType = t;
                    }
                }
                else if (types.Count == 2 && types.ContainsKey(typeof(Int32)) && types.ContainsKey(typeof(Double)))
                {
                    guessType = typeof(Double);
                }

                table.Columns[i].DataType = guessType;
            }
        }

        internal static List<string> ReadLines(CsvConnectionStringBuilder builder, int maxLines = 0)
        {
            if (String.IsNullOrEmpty(builder.CsvFile) || String.IsNullOrEmpty(builder.Separator))
                return null;

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            WebRequest request;
            WebResponse response = null;
            Uri uri = new Uri(builder.CsvFile);

            try
            {
                // fix for datafile in current folder
                if (File.Exists(builder.CsvFile))
                {
                    builder.CsvFile = Path.GetFullPath(builder.CsvFile);

                    if (uri.IsFile)
                    {
                        if (Config.ForbidLocalData)
                            throw new Exception(Res.Get("ConnectionEditors,Common,OnlyUrlException"));
                        request = (FileWebRequest)WebRequest.Create(uri);
                        request.Timeout = 5000;
                        response = (FileWebResponse)request.GetResponse();
                    }
                }

                else if (uri.OriginalString.StartsWith("http"))
                {
                    request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Timeout = 5000;
                    response = (HttpWebResponse)request.GetResponse();
                }
                else if (uri.OriginalString.StartsWith("ftp"))
                {
                    request = (FtpWebRequest)WebRequest.Create(uri);
                    request.Timeout = 5000;
                    response = (FtpWebResponse)request.GetResponse();
                }
                else
                {
                    throw new NullReferenceException(Res.Get("ConnectionEditors,Common,ErrorUrlException"));
                }
            }

            catch (NullReferenceException ex)
            {
                throw ex;
            }

            catch (Exception e)
            {
                throw e;
            }

            if (response == null)
                return null;

            List<string> lines = new List<string>();
            if (maxLines == 0)
                maxLines = int.MaxValue;

            // read lines
            Encoding encoding;
#if !NETFRAMEWORK
            encoding = CodePagesEncodingProvider.Instance.GetEncoding(builder.Codepage);
#else
            encoding = Encoding.GetEncoding(builder.Codepage);
#endif
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
            {
                for (int i = 0; i < maxLines; i++)
                {
                    string line = reader.ReadLine();
                    // end of stream reached
                    if (line == null)
                        break;

                    // skip empty lines
                    if (!String.IsNullOrEmpty(line))
                        lines.Add(line);
                }
            }
            return lines;
        }

        internal static DataTable CreateDataTable(CsvConnectionStringBuilder builder, List<string> rawLines)
        {
            if (rawLines == null)
                return null;

            // split each line to array of values
            List<string[]> lines = new List<string[]>();
            for (int i = 0; i < rawLines.Count; i++)
            {
                string line = rawLines[i];
                string[] values = line.Split(builder.Separator.ToCharArray());
                if (builder.RemoveQuotationMarks)
                {
                    for (int j = 0; j < values.Length; j++)
                    {
                        values[j] = values[j].Trim("\"".ToCharArray());
                    }
                }
                lines.Add(values);
            }

            if (lines.Count == 0)
                return null;

            NumberFormatInfo numberInfo = CultureInfo.GetCultureInfo(builder.NumberFormat)?.NumberFormat ?? CultureInfo.CurrentCulture.NumberFormat;
            NumberFormatInfo currencyInfo = CultureInfo.GetCultureInfo(builder.CurrencyFormat)?.NumberFormat ?? CultureInfo.CurrentCulture.NumberFormat;
            DateTimeFormatInfo dateTimeInfo = CultureInfo.GetCultureInfo(builder.DateTimeFormat)?.DateTimeFormat ?? CultureInfo.CurrentCulture.DateTimeFormat;

            // get table name from file name
            string tableName = Path.GetFileNameWithoutExtension(builder.CsvFile).Replace(".", "_");
            if (String.IsNullOrEmpty(tableName))
            {
                tableName = "Table";
            }

            DataTable table = new DataTable(tableName);

            string[] fields = lines[0];

            // create table columns
            for (int i = 0; i < fields.Length; i++)
            {
                DataColumn column = new DataColumn();
                column.DataType = typeof(string);

                // get field names from first string if needed
                string fieldName = fields[i].Replace("\t", "");
                if (builder.FieldNamesInFirstString && !table.Columns.Contains(fieldName))
                {
                    column.ColumnName = fieldName;
                    column.Caption = column.ColumnName;
                }
                else
                {
                    column.ColumnName = DEFAULT_FIELD_NAME + i.ToString();
                    column.Caption = column.ColumnName;
                }

                table.Columns.Add(column);
            }

            int startIndex = builder.FieldNamesInFirstString ? 1 : 0;
            // cast types of fields if needed
            if (builder.ConvertFieldTypes)
            {
                int number = lines.Count - startIndex;
                DetermineTypes(lines.GetRange(startIndex, number), table, numberInfo, currencyInfo, dateTimeInfo);
            }

            // add table rows
            for (int i = startIndex; i < lines.Count; i++)
            {
                if (lines[i].Length > 0)
                {
                    // get values from the string
                    fields = lines[i];

                    // add a new row
                    DataRow row = table.NewRow();
                    int valuesCount = fields.Length < table.Columns.Count ? fields.Length : table.Columns.Count;
                    for (int j = 0; j < valuesCount; j++)
                    {
                        string value = fields[j];
                        if (!String.IsNullOrEmpty(value))
                        {
                            if (table.Columns[j].DataType == typeof(String))
                            {
                                row[j] = value;
                            }
                            else if (table.Columns[j].DataType == typeof(Int32))
                            {
                                row[j] = Int32.Parse(value);
                            }
                            else if (table.Columns[j].DataType == typeof(Decimal))
                            {
                                row[j] = Decimal.Parse(value, NumberStyles.Currency, currencyInfo);
                            }
                            else if (table.Columns[j].DataType == typeof(Double))
                            {
                                row[j] = Double.Parse(value, NumberStyles.Number, numberInfo);
                            }
                            else if (table.Columns[j].DataType == typeof(DateTime))
                            {
                                row[j] = DateTime.Parse(value, dateTimeInfo);
                            }
                        }
                    }
                    table.Rows.Add(row);
                }
            }

            return table;
        }


    }
}
