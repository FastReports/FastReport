using System;
using System.Collections.Generic;
using System.Data.Common;
using SpreadsheetLight;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;
using FastReport.Utils;
using System.ComponentModel;

namespace FastReport.Data
{
    public partial class ExcelDataConnection : DataConnectionBase
    {

        #region Properties
        /// <summary>
        /// Gets or sets the path to .xlsx file.
        /// </summary>
        [Category("Data")]
        public string ExcelFile
        {
            get
            {
                ExcelConnectionStringBuilder builder = new ExcelConnectionStringBuilder(ConnectionString);
                return builder.ExcelFile;
            }
            set
            {
                ExcelConnectionStringBuilder builder = new ExcelConnectionStringBuilder(ConnectionString);
                builder.ExcelFile = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the value indicating that field names should be loaded from the first string of the file.
        /// </summary>
        [Category("Data")]
        public bool FieldNamesInFirstString
        {
            get
            {
                ExcelConnectionStringBuilder builder = new ExcelConnectionStringBuilder(ConnectionString);
                return builder.FieldNamesInFirstString;
            }
            set
            {
                ExcelConnectionStringBuilder builder = new ExcelConnectionStringBuilder(ConnectionString);
                builder.FieldNamesInFirstString = value;
                ConnectionString = builder.ToString();
            }
        }
        #endregion Properties

        SLDocument document;

        /// <inheritdoc/>
        public override string[] GetTableNames()
        {
            return document.GetSheetNames().ToArray();
        }

        /// <inheritdoc/>
        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return value;
        }

        /// <inheritdoc/>
        public override void CreateAllTables(bool initSchema)
        {
            if (document == null)
                InitConnection();

            bool found = false;
            foreach (Base b in Tables)
            {
                if (b.Parent is ExcelDataConnection)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                foreach (string name in GetTableNames())
                {
                    TableDataSource dataTable = new TableDataSource();
                    string fixedTableName = name.Replace(".", "_").Replace("[", "").Replace("]", "").Replace("\"", "");
                    dataTable.TableName = fixedTableName;

                    if (Report != null)
                    {
                        dataTable.Name = Report.Dictionary.CreateUniqueName(fixedTableName);
                        dataTable.Alias = Report.Dictionary.CreateUniqueAlias(dataTable.Alias);
                    }
                    else
                        dataTable.Name = fixedTableName;

                    dataTable.Parent = this;
                    dataTable.Enabled = false;

                    CreateTable(dataTable);
                    Tables.Add(dataTable);
                }
            }
        }

        /// <inheritdoc/>
        public override void CreateTable(TableDataSource source)
        {
            if (document == null)
                InitConnection();

            document.SelectWorksheet(source.TableName);

            base.CreateTable(source);
        }

        /// <inheritdoc/>
        public override void FillTableSchema(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            document.SelectWorksheet(table.TableName);
            string columnName;

            for (int i = 1; i <= document.GetWorksheetStatistics().EndColumnIndex; i++)
            {
                if (FieldNamesInFirstString)
                    columnName = document.GetCellValueAsString(1, i);
                else
                    columnName = IndexToName(i - 1);
                table.Columns.Add(columnName, GetTypeColumn(i));
            }
        }

        /// <inheritdoc/>
        public override void FillTableData(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            object value = null;
            document.SelectWorksheet(table.TableName);

            for (int y = 1; y <= document.GetWorksheetStatistics().EndRowIndex; y++)
            {
                if (FieldNamesInFirstString)
                {
                    y = 2;
                    FieldNamesInFirstString = false;
                } 
                DataRow row = table.NewRow();
                List<object> rowItems = new List<object>();
                for (int x = 1; x <= document.GetWorksheetStatistics().EndColumnIndex; x++)
                {

                    if (table.Columns[x - 1].DataType == typeof(string))
                    {
                        value = document.GetCellValueAsString(y, x);
                    }
                    else if (table.Columns[x - 1].DataType == typeof(decimal))
                    {
                        value = document.GetCellValueAsDecimal(y, x);
                    }
                    else if (table.Columns[x - 1].DataType == typeof(bool))
                    {
                        value = document.GetCellValueAsBoolean(y, x);
                    }
                    else if (table.Columns[x - 1].DataType == typeof(DataTable))
                    {
                        value = document.GetCellValueAsDateTime(y, x);
                    }
                    else
                        value = null;

                    rowItems.Add(value);
                }

                row.ItemArray = rowItems.ToArray();
                table.Rows.Add(row);
            }
        }


        /// <summary>
        /// Get type of data in column.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Type GetTypeColumn(int index)
        {
            SLCell cell;
            try
            {
                cell = document.GetCells()[1][index];
            }
            catch
            {
                return typeof(string);
            }

            switch (cell.DataType)
            {
                case CellValues.Date:
                    return typeof(DateTime);
                case CellValues.Boolean:
                    return typeof(bool);
                case CellValues.Number:
                    return typeof(decimal);
                case CellValues.InlineString:
                case CellValues.SharedString:
                case CellValues.String:
                    return typeof(string);
                case CellValues.Error:
                default:
                    return typeof(string);
            }
        }

        /// <summary>
        /// Convert index to Excel column name.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Column name</returns>
        private string IndexToName(int index)
        {

            bool firstSymbol = false;
            string result = "";

            for (; index > 0; index /= 26)
            {
                if (index < 26 && result != "")
                    firstSymbol = true;

                var x = index % 26;
                result = (char)(x + 'A' + (firstSymbol ? -1 : 0)) + result;
            }

            if (result == "")
                result = "A";

            return result;
        }

        public ExcelDataConnection()
        {
            IsSqlBased = false;
        }

        private void InitConnection()
        {
            //document = new SLDocument(ConnectionString);
            document = new SLDocument(ExcelFile);
        }
    }
}
