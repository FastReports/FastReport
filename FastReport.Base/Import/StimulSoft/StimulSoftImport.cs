using System;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Table;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FastReport.Barcode;
using FastReport.Utils;
#if MSCHART
using FastReport.MSChart;
using FastReport.DataVisualization.Charting;
#endif
using FastReport.Matrix;
using FastReport.Dialog;
using FastReport.Data;
using FastReport.Data.JsonConnection;

namespace FastReport.Import.StimulSoft
{
    /// <summary>
    /// Represents the StimulSoft import plugin.
    /// </summary>
    public partial class StimulSoftImport : ImportBase
    {
        #region Fields

        private ReportPage page;
        private XmlNode reportNode;
        private PageUnits unitType;
        private float leftOffset;
        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StimulSoftImport"/> class.
        /// </summary>
        public StimulSoftImport() : base() { }

        #endregion // Constructors

        #region Private Methods

        private string RemovePrefix(string value)
        {
            if (!value.Contains("."))
                return value;

            string[] temp = value.Split('.');
            return temp[temp.Length - 1].Replace("Sti", "");
        }

        private string SwapBarackets(string value)
        {
            return value.Replace("{", "[").Replace("}", "]");
        }

        private void LoadImage(ReportComponentBase parent, XmlNode xmlObject, bool useAbsTop)
        {
            if (xmlObject["File"] != null && new FileInfo(xmlObject["File"].InnerText).Extension == ".svg"
                || xmlObject["ImageURL"] != null && xmlObject["ImageURL"].InnerText.EndsWith(".svg"))
            {
                LoadSVGImage(parent, xmlObject, useAbsTop);
            }
            else
            {
                var PictureObject = ComponentsFactory.CreatePictureObject(xmlObject["Name"].InnerText, parent);
                LoadReportComponentBase(PictureObject, xmlObject);

                if (parent is TableCell)
                {
                    RectangleF rect = ParseRectangleF(xmlObject["ClientRectangle"].InnerText);
                    rect.Offset(-rect.Left, -rect.Top);
                    rect.Offset(-rect.Left + 1, -rect.Top + 1);
                    rect.Width -= 2;
                    rect.Height -= 2;
                    PictureObject.Bounds = rect;
                }
                else if (useAbsTop)
                    SetAbsTop(PictureObject, parent);

                if (xmlObject["Margins"] != null)
                    PictureObject.Padding = ParseMargins(xmlObject["Margins"].InnerText);

                if (xmlObject["File"] != null)
                    PictureObject.ImageLocation = xmlObject["File"].InnerText;
                else if (xmlObject["ImageURL"] != null)
                    PictureObject.ImageLocation = xmlObject["ImageURL"].InnerText;
                else if (xmlObject["DataColumn"] != null)
                    PictureObject.DataColumn = xmlObject["DataColumn"].InnerText;
                else if (xmlObject["ImageBytes"] != null)
                {
                    MemoryStream memoryStream = new MemoryStream();
                    StreamWriter streamWriter = new StreamWriter(memoryStream);
                    byte[] arrayimg = Convert.FromBase64String(xmlObject["ImageBytes"].InnerText);
                    PictureObject.Image = ImageHelper.Load(arrayimg);
                    streamWriter.Dispose();
                    memoryStream.Dispose();
                }

                if (xmlObject["AspectRatio"] != null)
                    PictureObject.SizeMode = UnitsConverter.ConvertBool(xmlObject["AspectRatio"].InnerText) ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.Normal;
            }
        }

        partial void LoadSVGImage(ReportComponentBase parent, XmlNode xmlObject, bool useAbsTop);

        private void LoadCheckBox(ReportComponentBase band, XmlNode xmlObject, bool useAbsTop)
        {
            CheckBoxObject checkBox = ComponentsFactory.CreateCheckBoxObject(xmlObject["Name"].InnerText, band);
            LoadReportComponentBase(checkBox, xmlObject);

            if (band is TableCell)
            {
                checkBox.Bounds = new RectangleF(1, 1, Math.Min(checkBox.Width, checkBox.Height) - 1, Math.Min(checkBox.Width, checkBox.Height) - 1);
            }
            else if (useAbsTop)
                SetAbsTop(checkBox, band);

            if (xmlObject["Checked"] != null)
            {
                checkBox.Checked = UnitsConverter.ConvertBool(xmlObject["Checked"].InnerText.Replace("{", "").Replace("}", ""));
                checkBox.DataColumn = xmlObject["Checked"].InnerText.Replace("{true}", "").Replace("{false}", "");
            }

            if (xmlObject["Editable"] != null)
                checkBox.Editable = UnitsConverter.ConvertBool(xmlObject["Editable"].InnerText);

            if (xmlObject["TextBrush"] != null)
                checkBox.CheckColor = UnitsConverter.ConvertBrushToColor(UnitsConverter.ConvertBrush(xmlObject["TextBrush"].InnerText));

            if (xmlObject["CheckStyleForTrue"] != null)
                checkBox.CheckedSymbol = UnitsConverter.ConvertCheckSymbol(xmlObject["CheckStyleForTrue"].InnerText);
        }

        private void LoadShape(BandBase band, XmlNode xmlObject, bool useAbsTop)
        {
            switch (RemovePrefix(xmlObject["ShapeType"].Attributes["type"].Value))
            {
                case "DiagonalUpLineShapeType":
                    {
                        LineObject lineObject = ComponentsFactory.CreateLineObject(xmlObject["Name"].InnerText, band);
                        LoadReportComponentBase(lineObject, xmlObject);
                        lineObject.Diagonal = true;

                        RectangleF rectangle = ParseRectangleF(xmlObject["ClientRectangle"].InnerText);
                        lineObject.Bounds = new RectangleF(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width, -rectangle.Height);

                        if (useAbsTop)
                            SetAbsTop(lineObject, band);

                        if (xmlObject["BorderColor"] != null)
                            lineObject.Border.Color = UnitsConverter.ConvertColor(xmlObject["BorderColor"].InnerText);

                        if (xmlObject["Size"] != null)
                            lineObject.Border.Width = UnitsConverter.ConvertFloat(xmlObject["Size"].InnerText);

                        if (xmlObject["Style"] != null)
                            lineObject.Border.Style = UnitsConverter.ConvertLineStyle(xmlObject["Style"].InnerText);
                        break;
                    }
                case "DiagonalDownLineShapeType":
                    {
                        LineObject lineObject = ComponentsFactory.CreateLineObject(xmlObject["Name"].InnerText, band);
                        LoadReportComponentBase(lineObject, xmlObject);
                        lineObject.Diagonal = true;

                        if (useAbsTop)
                            SetAbsTop(lineObject, band);

                        if (xmlObject["BorderColor"] != null)
                            lineObject.Border.Color = UnitsConverter.ConvertColor(xmlObject["BorderColor"].InnerText);

                        if (xmlObject["Size"] != null)
                            lineObject.Border.Width = UnitsConverter.ConvertFloat(xmlObject["Size"].InnerText);

                        if (xmlObject["Style"] != null)
                            lineObject.Border.Style = UnitsConverter.ConvertLineStyle(xmlObject["Style"].InnerText);
                        break;
                    }
                case "HorizontalLineShapeType":
                case "VerticalLineShapeType":
                    {
                        LineObject lineObject = ComponentsFactory.CreateLineObject(xmlObject["Name"].InnerText, band);
                        LoadReportComponentBase(lineObject, xmlObject);


                        if (useAbsTop)
                            SetAbsTop(lineObject, band);

                        if (xmlObject["BorderColor"] != null)
                            lineObject.Border.Color = UnitsConverter.ConvertColor(xmlObject["BorderColor"].InnerText);

                        if (xmlObject["Size"] != null)
                            lineObject.Border.Width = UnitsConverter.ConvertFloat(xmlObject["Size"].InnerText);

                        if (xmlObject["Style"] != null)
                            lineObject.Border.Style = UnitsConverter.ConvertLineStyle(xmlObject["Style"].InnerText);
                        break;
                    }
            }
        }

        private void LoadZipCode(BandBase band, XmlNode xmlObject, bool useAbsTop)
        {
            ZipCodeObject zipCode = ComponentsFactory.CreateZipCodeObject(xmlObject["Name"].InnerText, band);
            LoadReportComponentBase(zipCode, xmlObject);

            if (useAbsTop)
                SetAbsTop(zipCode, band);

            if (xmlObject["Code"] != null)
            {
                zipCode.Text = xmlObject["Code"].InnerText;
                zipCode.SegmentCount = xmlObject["Code"].InnerText.Length;
            }

            if (xmlObject["SpaceRatio"] != null)
                zipCode.Spacing = UnitsConverter.ConvertFloat(xmlObject["SpaceRatio"].InnerText);
        }

        private void LoadMatrix(BandBase band, XmlNode xmlObject, bool useAbsTop)
        {
            MatrixObject matrix = ComponentsFactory.CreateMatrixObject(xmlObject["Name"].InnerText, band);
            matrix.AutoSize = false;
            LoadReportComponentBase(matrix, xmlObject);

            if (useAbsTop)
                SetAbsTop(matrix, band);

            Dictionary<string, TableCell> totalTemplates = new Dictionary<string, TableCell>();
            foreach (XmlNode node in xmlObject["Components"])
            {
                switch (RemovePrefix(node.Attributes["type"].Value))
                {
                    case "CrossColumnTotal":
                    case "CrossRowTotal":
                        totalTemplates.Add(node["Guid"].InnerText, LoadTableCell(node));
                        break;
                }
            }

            foreach (XmlNode node in xmlObject["Components"])
            {
                switch (RemovePrefix(node.Attributes["type"].Value))
                {
                    case "CrossRow":
                        MatrixHeaderDescriptor row = new MatrixHeaderDescriptor(node["Value"].InnerText.Replace("}", "").Replace("{", ""));
                        matrix.Data.Rows.Add(row);
                        matrix.BuildTemplate();

                        RectangleF rectCellOfRow = ParseRectangleF(node["ClientRectangle"].InnerText);
                        row.TemplateColumn.Width = rectCellOfRow.Width;
                        row.TemplateRow.Height = rectCellOfRow.Height;
                        row.TemplateCell.AssignAll(LoadTableCell(node));
                        row.TemplateCell.Brackets = "[,]";

                        if (node["ShowTotal"] != null)
                            row.Totals = UnitsConverter.ConvertBool(node["ShowTotal"].InnerText);

                        if (node["TotalGuid"] != null)
                            row.TemplateTotalCell = totalTemplates[node["TotalGuid"].InnerText];
                        break;

                    case "CrossColumn":
                        MatrixHeaderDescriptor column = new MatrixHeaderDescriptor(node["Value"].InnerText.Replace("}", "").Replace("{", ""));
                        matrix.Data.Columns.Add(column);
                        matrix.BuildTemplate();

                        RectangleF rectCellOfColumn = ParseRectangleF(node["ClientRectangle"].InnerText);
                        column.TemplateColumn.Width = rectCellOfColumn.Width;
                        column.TemplateRow.Height = rectCellOfColumn.Height;
                        column.TemplateCell.AssignAll(LoadTableCell(node));
                        column.TemplateCell.Brackets = "[,]";

                        if (node["TotalGuid"] != null)
                            column.TemplateTotalCell = totalTemplates[node["TotalGuid"].InnerText];

                        if (node["ShowTotal"] != null)
                            column.Totals = UnitsConverter.ConvertBool(node["ShowTotal"].InnerText);
                        break;

                    case "CrossSummary":
                        MatrixCellDescriptor cell = new MatrixCellDescriptor(
                                node["Value"].InnerText.Replace("}", "").Replace("{", ""),
                                UnitsConverter.GetMatrixAggregateFunction(node)
                            );
                        matrix.Data.Cells.Add(cell);
                        matrix.BuildTemplate();

                        cell.TemplateCell.AssignAll(LoadTableCell(node));
                        cell.TemplateCell.Brackets = "[,]";

                        if (node["Summary"] != null && node["Summary"].InnerText == "Image")
                        {
                            cell.Expression = "";
                            cell.TemplateCell.Text = "";
                            LoadImage(cell.TemplateCell, node, false);
                        }
                        break;
                }
            }
            matrix.BuildTemplate();
        }

        private void LoadTable(BandBase band, XmlNode xmlObject, bool useAbsTop)
        {

            TableObject table = ComponentsFactory.CreateTableObject(xmlObject["Name"].InnerText, band);
            LoadReportComponentBase(table, xmlObject);

            int fixRows = 0;
            DataBand nonHeaderPartOfTab = null;

            RectangleF rectangle = table.Bounds;
            if (useAbsTop)
            {
                rectangle.Offset(0, -band.Bounds.Top);
                table.Bounds = rectangle;
            }

            if (xmlObject["HeaderRowsCount"] != null)
            {
                fixRows = UnitsConverter.ConvertInt(xmlObject["HeaderRowsCount"].InnerText);
                table.FixedRows = fixRows;
            }

            int columnCount = 5;//default value
            if (xmlObject["ColumnCount"] != null)
                columnCount = UnitsConverter.ConvertInt(xmlObject["ColumnCount"].InnerText);

            table.ColumnCount = columnCount;

            int index = 0;
            TableRow row = new TableRow();
            foreach (XmlNode cell in xmlObject["Components"])
            {
                if (index % columnCount == 0)
                {
                    if (table.Rows.Count == fixRows)
                    {
                        SubreportObject subreport = ComponentsFactory.CreateSubreportObject("SubReport_" + xmlObject["Name"].InnerText, band);
                        ReportPage page = ComponentsFactory.CreateReportPage(Report);
                        page.SetName("SubReport_" + xmlObject["Name"].InnerText);
                        subreport.Bounds = rectangle;

                        page.PaperWidth = subreport.Bounds.Width / Units.Millimeters;
                        page.PaperHeight = subreport.Bounds.Height / Units.Millimeters;

                        page.LeftMargin = 0;
                        page.TopMargin = 0;
                        page.RightMargin = 0;
                        page.BottomMargin = 0;
                        subreport.ReportPage = page;
                        nonHeaderPartOfTab = ComponentsFactory.CreateDataBand(page);
                        nonHeaderPartOfTab.CanGrow = true;
                        table = ComponentsFactory.CreateTableObject(xmlObject["Name"].InnerText, nonHeaderPartOfTab);
                        table.ColumnCount = columnCount;
                        index = 0;
                    }
                    row = new TableRow();
                    row.SetReport(Report);
                    row.CreateUniqueName();
                    row.Height = ParseRectangleF(cell["ClientRectangle"].InnerText).Height;
                    rectangle.Height -= row.Height;
                    rectangle.Y += row.Height;
                    table.Rows.Add(row);
                }
                if (index < columnCount)
                {
                    table.Columns[index].Width = ParseRectangleF(cell["ClientRectangle"].InnerText).Width;
                }

                TableCell tableCell = LoadTableCell(cell);
                tableCell.SetReport(Report);
                row[index % columnCount] = tableCell;
                tableCell.CreateUniqueName();

                List<int> joinCellId = new List<int>();
                foreach (XmlNode val in cell["JoinCells"])
                    joinCellId.Add(UnitsConverter.ConvertInt(val.InnerText));

                TableCell cellConteiner = tableCell;
                int rowSpan = 1, colSpan = 1;

                if (cell["JoinHeight"] != null)
                    rowSpan = UnitsConverter.ConvertInt(cell["JoinHeight"].InnerText);
                else
                {
                    rowSpan = GetRowSpan(joinCellId, columnCount);
                    if (rowSpan > table.Rows.Count)
                        rowSpan = 1;
                }

                if (cell["JoinWidth"] != null)
                    colSpan = UnitsConverter.ConvertInt(cell["JoinWidth"].InnerText);
                else
                {
                    colSpan = GetColSpan(joinCellId, columnCount);
                    if (colSpan > columnCount)
                        colSpan = 1;
                }

                if (rowSpan > 1 && colSpan > 1)
                {
                    cellConteiner = table.Rows[table.Rows.Count - rowSpan][index % columnCount - colSpan + 1];
                    cellConteiner.AssignAll(tableCell);
                    table.Rows[table.Rows.Count - rowSpan][index % columnCount - colSpan + 1].RowSpan = rowSpan;
                    table.Rows[table.Rows.Count - rowSpan][index % columnCount - colSpan + 1].ColSpan = colSpan;

                }
                else if (colSpan > 1)
                {
                    cellConteiner = row[index % columnCount - colSpan + 1];
                    cellConteiner.AssignAll(tableCell);
                    row[index % columnCount - colSpan + 1].ColSpan = colSpan;

                }
                else if (rowSpan > 1)
                {
                    cellConteiner = table.Rows[table.Rows.Count - rowSpan][index % columnCount];
                    cellConteiner.AssignAll(tableCell);
                    table.Rows[table.Rows.Count - rowSpan][index % columnCount].RowSpan = rowSpan;

                }

                switch (RemovePrefix(cell.Attributes["type"].Value))
                {
                    case "TableCellRichText":
                        LoadRichText(cellConteiner, cell, false);
                        tableCell.Text = "";
                        break;
                    case "TableCellImage":
                        LoadImage(cellConteiner, cell, false);
                        break;
                    case "TableCellCheckBox":
                        LoadCheckBox(cellConteiner, cell, false);
                        break;
                }
                index++;
            }
            if (nonHeaderPartOfTab != null)
                nonHeaderPartOfTab.CalcHeight();
        }

        private void LoadBarCode(BandBase band, XmlNode xmlObject, bool useAbsTop)
        {
            BarcodeObject barcodeObject = ComponentsFactory.CreateBarcodeObject(xmlObject["Name"].InnerText, band);
            LoadReportComponentBase(barcodeObject, xmlObject);

            barcodeObject.Brackets = "{,}";
            if (xmlObject["Code"] != null)
                barcodeObject.Text = xmlObject["Code"].InnerText;

            UnitsConverter.ConvertBarcodeSymbology(xmlObject["BarCodeType"].Attributes["type"].Value, barcodeObject);

            if (useAbsTop)
                SetAbsTop(barcodeObject, band);

            if (xmlObject["ShowLabelText"] != null)
                barcodeObject.ShowText = UnitsConverter.ConvertBool(xmlObject["ShowLabelText"].InnerText);
        }

        partial void LoadRichText(ReportComponentBase band, XmlNode xmlObject, bool useAbsTop);

        partial void LoadMap(ReportComponentBase band, XmlNode xmlObject, bool useAbsTop);

        private void LoadGauge(BandBase band, XmlNode xmlObject, bool useAbsTop)
        {
            Gauge.GaugeObject gauge;
            if (xmlObject["Type"] != null)
            {
                switch (RemovePrefix(xmlObject["Type"].InnerText))
                {
                    case "Linear":
                    case "HorizontalLinear":
                        gauge = ComponentsFactory.CreateSimpleProgressGauge(xmlObject["Type"].InnerText, band);
                        break;

                    case "Bullet":
                        gauge = ComponentsFactory.CreateSimpleGauge(xmlObject["Name"].InnerText, band);
                        break;

                    default:
                        gauge = ComponentsFactory.CreateRadialGauge(xmlObject["Name"].InnerText, band);
                        break;
                }
            }
            else
            {
                gauge = ComponentsFactory.CreateRadialGauge(xmlObject["Name"].InnerText, band);
            }
            LoadReportComponentBase(gauge, xmlObject);

            if (useAbsTop)
                SetAbsTop(gauge, band);

            if (xmlObject["Maximum"] != null)
                gauge.Maximum = UnitsConverter.ConvertFloat(xmlObject["Maximum"].InnerText);

            if (xmlObject["Minimum"] != null)
                gauge.Minimum = UnitsConverter.ConvertFloat(xmlObject["Minimum"].InnerText);

            gauge.Value = 0;
        }

        partial void LoadSparkLine(BandBase band, XmlNode xmlObject, bool useAbsTop);

        private void LoadTextObjBase(TextObject textObject, XmlNode xmlObject)
        {
            LoadReportComponentBase(textObject, xmlObject);
            textObject.Brackets = "{,}";

            if (xmlObject["Text"] != null)
                textObject.Text = xmlObject["Text"].InnerText;

            if (xmlObject["Margins"] != null)
                textObject.Padding = ParseMargins(xmlObject["Margins"].InnerText);

            if (xmlObject["Font"] != null)
                textObject.Font = ParseFont(xmlObject["Font"].InnerText);

            if (xmlObject["TextBrush"] != null)
                textObject.TextColor = UnitsConverter.ConvertColor(xmlObject["TextBrush"].InnerText);

            if (xmlObject["HorAlignment"] != null)
                textObject.HorzAlign = UnitsConverter.ConvertTextAlignmentToHorzAlign(xmlObject["HorAlignment"].InnerText);

            if (xmlObject["VertAlignment"] != null)
                textObject.VertAlign = UnitsConverter.ConvertTextAlignmentToVertAlign(xmlObject["VertAlignment"].InnerText);

            if (xmlObject["TextFormat"] != null)
                textObject.Format = UnitsConverter.ConvertFormat(xmlObject["TextFormat"]);

            if (xmlObject["TextOptions"] != null)
                ParseTextOptions(textObject, xmlObject["TextOptions"].InnerText);

            if (xmlObject["AllowHtmlTags"] != null)
                textObject.TextRenderType = UnitsConverter.ConvertBool(xmlObject["AllowHtmlTags"].InnerText) ? TextRenderType.HtmlTags : TextRenderType.Default;

            if (xmlObject["Editable"] != null)
                textObject.Editable = UnitsConverter.ConvertBool(xmlObject["Editable"].InnerText);

            foreach (XmlNode xmlHighlight in xmlObject["Conditions"])
            {
                try
                {
                    string[] parametrs = xmlHighlight.InnerText.Split(',');
                    HighlightCondition highlight = new HighlightCondition();
                    highlight.Expression = UnitsConverter.ConvertRTF(parametrs[0]).Replace("value", "Value").Replace("{", "").Replace("}", "");
                    highlight.TextFill = new SolidFill(UnitsConverter.ConvertColor(UnitsConverter.ConvertRTF(parametrs[1])));
                    highlight.Fill = new SolidFill(UnitsConverter.ConvertColor(UnitsConverter.ConvertRTF(parametrs[2])));
                    highlight.Font = ParseFont(UnitsConverter.ConvertRTF(parametrs[3]));
                    highlight.Border.Lines = UnitsConverter.ConvertBorderSides(UnitsConverter.ConvertRTF(parametrs[8]));
                    highlight.ApplyFont = true;
                    highlight.ApplyFill = true;
                    highlight.ApplyTextFill = true;
                    highlight.ApplyBorder = true;
                    textObject.Highlight.Add(highlight);
                }
                catch { }
            }
        }

        private void LoadTextObject(BandBase band, XmlNode xmlObject, bool useAbsTop)
        {
            TextObject textObject = ComponentsFactory.CreateTextObject(xmlObject["Name"].InnerText, band);
            LoadTextObjBase(textObject, xmlObject);

            if (useAbsTop)
                SetAbsTop(textObject, band);
        }

        private void LoadCellularTextObject(BandBase band, XmlNode xmlObject, bool useAbsTop)
        {
            CellularTextObject CellularTextObject = ComponentsFactory.CreateCellularTextObject(xmlObject["Name"].InnerText, band);
            LoadTextObjBase(CellularTextObject, xmlObject);

            if (useAbsTop)
                SetAbsTop(CellularTextObject, band);

            if (xmlObject["CellHeight"] != null)
                CellularTextObject.CellHeight = UnitsConverter.ConvertFloat(xmlObject["CellHeight"].InnerText);

            if (xmlObject["CellWidth"] != null)
                CellularTextObject.CellWidth = UnitsConverter.ConvertFloat(xmlObject["CellWidth"].InnerText);
        }

        private void LoadPrimitiveShape(BandBase band, XmlNode xmlObject, ShapeKind shapeKind, bool useAbsTop)
        {
            ShapeObject shapeObject = ComponentsFactory.CreateShapeObject(xmlObject["Name"].InnerText, band);
            shapeObject.Shape = shapeKind;
            LoadReportComponentBase(shapeObject, xmlObject);

            if (useAbsTop)
                SetAbsTop(shapeObject, band);

            if (xmlObject["Color"] != null)
                shapeObject.Border.Color = UnitsConverter.ConvertColor(xmlObject["Color"].InnerText);

            if (xmlObject["Style"] != null)
                shapeObject.Border.Style = UnitsConverter.ConvertLineStyle(xmlObject["Style"].InnerText);

            if (xmlObject["Size"] != null)
                shapeObject.Border.Width = UnitsConverter.ConvertInt(xmlObject["Size"].InnerText);
        }

        private void LoadPanel(BandBase band, XmlNode xmlObject, bool useAbsTop)
        {
            SubreportObject subreportObject = ComponentsFactory.CreateSubreportObject(xmlObject["Name"].InnerText, band);
            ReportPage page = ComponentsFactory.CreateReportPage(Report);
            LoadReportComponentBase(subreportObject, xmlObject);

            if (useAbsTop)
                SetAbsTop(subreportObject, band);

            page.PaperWidth = subreportObject.Bounds.Width / Units.Millimeters;
            page.PaperHeight = subreportObject.Bounds.Height / Units.Millimeters;

            page.LeftMargin = 0;
            page.TopMargin = 0;
            page.RightMargin = 0;
            page.BottomMargin = 0;

            LoadBands(xmlObject, page);
            LoadChildBands(xmlObject, page);
            subreportObject.ReportPage = page;
        }

        private void LoadLineObject(BandBase band, XmlNode xmlObject, bool useAbsTop)
        {
            LineObject lineObject = ComponentsFactory.CreateLineObject(xmlObject["Name"].InnerText, band);
            LoadReportComponentBase(lineObject, xmlObject);

            if (useAbsTop)
                SetAbsTop(lineObject, band);

            if (xmlObject["EndCap"] != null && xmlObject["EndCap"]["Style"] != null)
                lineObject.EndCap.Style = UnitsConverter.ConvertCapStyle(xmlObject["EndCap"]["Style"].InnerText);

            if (xmlObject["StartCap"] != null && xmlObject["StartCap"]["Style"] != null)
                lineObject.StartCap.Style = UnitsConverter.ConvertCapStyle(xmlObject["StartCap"]["Style"].InnerText);

            if (xmlObject["Color"] != null)
                lineObject.Border.Color = UnitsConverter.ConvertColor(xmlObject["Color"].InnerText);

            if (xmlObject["Size"] != null)
                lineObject.Border.Width = UnitsConverter.ConvertFloat(xmlObject["Size"].InnerText);
        }


        partial void LoadChart(BandBase band, XmlNode xmlObject, bool useAbsTop);

        private void LoadPage(XmlNode node)
        {
            page = ComponentsFactory.CreateReportPage(node["Name"].InnerText, Report);

            if (node["PageWidth"] != null && node["PageHeight"] != null)
            {
                page.PaperWidth = UnitsConverter.ConvertFloat(node["PageWidth"].InnerText) * UnitsConverter.GetPixelsInUnit(unitType) / Units.Millimeters;
                page.PaperHeight = UnitsConverter.ConvertFloat(node["PageHeight"].InnerText) * UnitsConverter.GetPixelsInUnit(unitType) / Units.Millimeters;
            }
            else if (node["PaperSize"] != null)
            {
                UnitsConverter.ConvertPaperSize(node["PaperSize"].InnerText, page);
            }

            if (node["Margins"] != null)
            {
                Padding pagePadding = ParsePageMargins(node["Margins"].InnerText);
                page.LeftMargin = pagePadding.Left / Units.Millimeters;
                page.TopMargin = pagePadding.Top / Units.Millimeters;
                page.RightMargin = pagePadding.Right / Units.Millimeters;
                page.BottomMargin = pagePadding.Bottom / Units.Millimeters;
            }

            if (node["Columns"] != null)
                page.Columns.Count = UnitsConverter.ConvertInt(node["Columns"].InnerText);

            if (node["ColumnWidth"] != null)
                page.Columns.Width = UnitsConverter.ConvertFloat(node["ColumnWidth"].InnerText) * UnitsConverter.GetPixelsInUnit(unitType) / Units.Millimeters;

            if (node["ColumnGaps"] != null)
                page.Columns.Width += UnitsConverter.ConvertFloat(node["ColumnGaps"].InnerText) * UnitsConverter.GetPixelsInUnit(unitType) / Units.Millimeters;

            page.Tag = node["Guid"].InnerText;

            LoadBands(node, page);
            LoadChildBands(node, page);
        }

        private void LoadReportObjects(XmlNode node, BandBase band)
        {
            foreach (XmlNode item in SortNodeByTop(node))
            {
                LoadObject(band, item, false);
            }
        }

        private void LoadBands(XmlNode node, ReportPage page)
        {
            BandBase band;
            foreach (XmlNode item in SortNodeByTop(node))
            {
                band = null;
                switch (RemovePrefix(item.Attributes["type"].Value))
                {
                    case "DataBand":
                        {
                            band = ComponentsFactory.CreateDataBand(page);

                            if (item["DataSourceName"] != null)
                                (band as DataBand).DataSource = Report.GetDataSource(item["DataSourceName"].InnerText);

                            if (item["Columns"] != null)
                                (band as DataBand).Columns.Count = UnitsConverter.ConvertInt(item["Columns"].InnerText);

                            if (item["ColumnWidth"] != null)
                                (band as DataBand).Columns.Width = UnitsConverter.ConvertFloat(item["ColumnWidth"].InnerText) * UnitsConverter.GetPixelsInUnit(unitType);

                            if (item["ColumnGaps"] != null)
                                (band as DataBand).Columns.Width += UnitsConverter.ConvertFloat(item["ColumnGaps"].InnerText) * UnitsConverter.GetPixelsInUnit(unitType);


                            break;
                        }
                    case "ReportTitleBand":
                        {
                            band = ConvertToChildIfDuplicatedBand(typeof(ReportTitleBand));
                            if (band == null)
                                band = ComponentsFactory.CreateReportTitleBand(page);

                            break;
                        }
                    case "ReportSummaryBand":
                        {
                            band = ConvertToChildIfDuplicatedBand(typeof(ReportSummaryBand));
                            if (band == null)
                                band = ComponentsFactory.CreateReportSummaryBand(page);

                            break;
                        }
                    case "OverlayBand":
                        {
                            band = ComponentsFactory.CreateOverlayBand(page);

                            break;
                        }
                    case "ColumnHeaderBand":
                        {
                            band = ConvertToChildIfDuplicatedBand(typeof(ColumnHeaderBand));
                            if (band == null)
                                band = ComponentsFactory.CreateColumnHeaderBand(page);

                            break;
                        }
                    case "ColumnFooterBand":
                        {
                            band = ConvertToChildIfDuplicatedBand(typeof(ColumnFooterBand));
                            if (band == null)
                                band = ComponentsFactory.CreateColumnFooterBand(page);

                            break;
                        }
                    case "PageHeaderBand":
                        {
                            band = ConvertToChildIfDuplicatedBand(typeof(PageHeaderBand));
                            if (band == null)
                                band = ComponentsFactory.CreatePageHeaderBand(page);

                            break;
                        }
                    case "PageFooterBand":
                        {
                            band = ConvertToChildIfDuplicatedBand(typeof(PageFooterBand));
                            if (band == null)
                                band = ComponentsFactory.CreatePageFooterBand(page);

                            break;
                        }
                    case "HierarchicalBand":
                        {
                            band = ComponentsFactory.CreateDataBand(page);

                            if (item["Indent"] != null)
                                (band as DataBand).Indent = UnitsConverter.ConvertFloat(item["Indent"].InnerText);

                            if (item["DataSourceName"] != null)
                            {
                                (band as DataBand).DataSource = Report.GetDataSource(item["DataSourceName"].InnerText);
                                if (item["KeyDataColumn"] != null)
                                    (band as DataBand).IdColumn = item["DataSourceName"].InnerText + "." + item["KeyDataColumn"].InnerText;
                                if (item["MasterKeyDataColumn"] != null)
                                    (band as DataBand).ParentIdColumn = item["DataSourceName"].InnerText + "." + item["MasterKeyDataColumn"].InnerText;
                            }
                            break;
                        }
                    default:
                        {
                            //report object without band
                            if (item["ClientRectangle"] != null)
                            {
                                DataBand dataBand = ComponentsFactory.CreateDataBand(page);
                                RectangleF rect = ParseRectangleF(item["ClientRectangle"].InnerText);
                                dataBand.Bounds = rect;
                                foreach (Base obj in page.AllObjects)
                                {
                                    if (obj is DataBand && obj != dataBand && (obj as DataBand).Bounds.Top + (obj as DataBand).Bounds.Height >= rect.Top)
                                    {
                                        page.RemoveChild(dataBand);
                                        dataBand = (DataBand)obj;
                                        break;
                                    }
                                }

                                LoadObject(dataBand, item, true);
                                if (dataBand.ChildObjects.Count == 0)
                                    page.RemoveChild(dataBand);
                                else
                                {
                                    dataBand.CanGrow = true;
                                    dataBand.CalcHeight();
                                }
                            }
                            break;
                        }
                }
                LoadBandBase(band, item);
            }
        }

        private void LoadObject(BandBase band, XmlNode xmlObject, bool useAbsTop)
        {
            switch (RemovePrefix(xmlObject.Attributes["type"].Value))
            {
                case "Text":
                    {
                        LoadTextObject(band, xmlObject, useAbsTop);
                        break;
                    }
                case "RichText":
                    {
                        LoadRichText(band, xmlObject, useAbsTop);
                        break;
                    }
                case "TextInCells":
                    {
                        LoadCellularTextObject(band, xmlObject, useAbsTop);
                        break;
                    }
                case "Image":
                    {
                        LoadImage(band, xmlObject, useAbsTop);
                        break;
                    }
                case "Chart":
                    {
                        LoadChart(band, xmlObject, useAbsTop);
                        break;
                    }
                case "Shape":
                    {
                        LoadShape(band, xmlObject, useAbsTop);
                        break;
                    }
                case "CheckBox":
                    {
                        LoadCheckBox(band, xmlObject, useAbsTop);
                        break;
                    }
                case "VerticalLinePrimitive":
                case "HorizontalLinePrimitive":
                    {
                        LoadLineObject(band, xmlObject, useAbsTop);
                        break;
                    }
                case "RectanglePrimitive":
                    {
                        LoadPrimitiveShape(band, xmlObject, ShapeKind.Rectangle, useAbsTop);
                        break;
                    }
                case "RoundedRectanglePrimitive":
                    {
                        LoadPrimitiveShape(band, xmlObject, ShapeKind.RoundRectangle, useAbsTop);
                        break;
                    }
                case "BarCode":
                    {
                        LoadBarCode(band, xmlObject, useAbsTop);
                        break;
                    }
                case "Panel":
                    {
                        LoadPanel(band, xmlObject, useAbsTop);
                        break;
                    }
                case "SubReport":
                    {
                        SubreportObject subreportObject = ComponentsFactory.CreateSubreportObject(xmlObject["Name"].InnerText, band);
                        LoadReportComponentBase(subreportObject, xmlObject);

                        if (useAbsTop)
                            SetAbsTop(subreportObject, band);

                        subreportObject.Tag = xmlObject["SubReportPageGuid"].InnerText;
                        break;
                    }
                case "Sparkline":
                    {
                        LoadSparkLine(band, xmlObject, useAbsTop);
                        break;
                    }
                case "Table":
                    {
                        LoadTable(band, xmlObject, useAbsTop);
                        break;
                    }
                case "Map":
                    {
                        LoadMap(band, xmlObject, useAbsTop);
                        break;
                    }
                case "Gauge":
                    {
                        LoadGauge(band, xmlObject, useAbsTop);
                        break;
                    }
                case "ZipCode":
                    {
                        LoadZipCode(band, xmlObject, useAbsTop);
                        break;
                    }
                case "CrossTab":
                    {
                        LoadMatrix(band, xmlObject, useAbsTop);
                        break;
                    }
                case "CrossGroupFooterBand":
                case "CrossHeaderBand":
                case "CrossFooterBand":
                case "CrossDataBand":
                case "CrossGroupHeaderBand":
                    {
                        RectangleF rectangle = ParseRectangleF(xmlObject["ClientRectangle"].InnerText);
                        leftOffset = rectangle.X;
                        LoadReportObjects(xmlObject, band);
                        leftOffset = 0;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void SetAbsTop(ReportComponentBase obj, ReportComponentBase parent)
        {
            RectangleF rect = obj.Bounds;
            rect.Offset(0, -parent.Bounds.Top);
            obj.Bounds = rect;
        }

        private TableCell LoadTableCell(XmlNode node)
        {
            TableCell result = new TableCell();
            result.SetReport(Report);
            LoadTextObjBase(result, node);

            if (node["HorAlignment"] == null)
                result.HorzAlign = HorzAlign.Center;

            if (node["VertAlignment"] == null)
                result.VertAlign = VertAlign.Center;

            return result;
        }

        private int GetColSpan(List<int> arr, int columnCount)
        {
            int result = 0;
            foreach (int i in arr)
                if (i % columnCount == arr[0])
                    result++;
            return result;
        }

        private int GetRowSpan(List<int> arr, int columnCount)
        {
            int result = 0;
            foreach (int i in arr)
                if (i < columnCount)
                    result++;
            return result;
        }

        private void LoadChildBands(XmlNode node, ReportPage page)
        {
            foreach (XmlNode item in SortNodeByTop(node))
            {
                switch (RemovePrefix(item.Attributes["type"].Value))
                {
                    case "HeaderBand":
                        {
                            BandBase parent = GetParentDataBand(ParseRectangleF(item["ClientRectangle"].InnerText), page, true);
                            BandBase newBand;
                            if (parent is DataBand)
                                newBand = ComponentsFactory.CreateDataHeaderBand((DataBand)parent);
                            else
                                newBand = ComponentsFactory.CreateChildBand(parent);
                            LoadBandBase(newBand, item);
                            break;
                        }
                    case "FooterBand":
                        {
                            BandBase parent = GetParentDataBand(ParseRectangleF(item["ClientRectangle"].InnerText), page, false);
                            BandBase newBand;
                            if (parent is DataBand)
                                newBand = ComponentsFactory.CreateDataFooterBand((DataBand)parent);
                            else
                                newBand = ComponentsFactory.CreateChildBand(parent);
                            LoadBandBase(newBand, item);
                            break;
                        }
                    case "GroupHeaderBand":
                        {
                            GroupHeaderBand band = ComponentsFactory.CreateGroupHeaderBand(page);
                            band.Data = GetParentDataBand(ParseRectangleF(item["ClientRectangle"].InnerText), page);
                            band.Condition = item["Condition"].InnerText;

                            LoadBandBase(band, item);
                            break;
                        }
                    case "GroupFooterBand":
                        {
                            BandBase band = ComponentsFactory.CreateGroupFooterBand(GetParentGroupHeaderBand(ParseRectangleF(item["ClientRectangle"].InnerText), page));
                            LoadBandBase(band, item);
                            break;
                        }
                    case "ChildBand":
                        {
                            ChildBand childBand = ComponentsFactory.CreateChildBand(GetParentBandBase(ParseRectangleF(item["ClientRectangle"].InnerText), page));
                            LoadBandBase(childBand, item);
                            break;
                        }
                }
            }
        }

        private void LoadBandBase(BandBase band, XmlNode node)
        {
            if (band == null)
                return;

            band.Bounds = ParseRectangleF(node["ClientRectangle"].InnerText);

            if (node["Border"] != null)
                band.Border = UnitsConverter.ConvertBorder(node["Border"].InnerText);
            if (node["Brush"] != null)
                band.Fill = UnitsConverter.ConvertBrush(node["Brush"].InnerText);
            if (node["CanBreak"] != null)
                band.CanBreak = UnitsConverter.ConvertBool(node["CanBreak"].InnerText);
            if (node["CanGrow"] != null)
                band.CanGrow = UnitsConverter.ConvertBool(node["CanGrow"].InnerText);
            else
                band.CanGrow = true; // theirs default value
            if (node["CanShrink"] != null)
                band.CanShrink = UnitsConverter.ConvertBool(node["CanShrink"].InnerText);

            LoadReportObjects(node, band);
        }

        private void LoadReportComponentBase(ReportComponentBase obj, XmlNode node)
        {
            if (node["ClientRectangle"] != null)
            {
                RectangleF rect = ParseRectangleF(node["ClientRectangle"].InnerText);
                rect.X += leftOffset;
                obj.Bounds = rect;
            }

            if (node["ComponentStyle"] != null)
                ApplyStyleByName(obj, node["ComponentStyle"].InnerText);

            if (node["Border"] != null)
                obj.Border = UnitsConverter.ConvertBorder(node["Border"].InnerText);

            if (node["Brush"] != null)
                obj.Fill = UnitsConverter.ConvertBrush(node["Brush"].InnerText);

            if (node["CanGrow"] != null)
                obj.CanGrow = UnitsConverter.ConvertBool(node["CanGrow"].InnerText);

            if (node["CanShrink"] != null)
                obj.CanShrink = UnitsConverter.ConvertBool(node["CanShrink"].InnerText);

            if (node["Interaction"] != null)
            {
                foreach (XmlNode hyperlink in node["Interaction"])
                    if (hyperlink.Attributes["type"] != null)
                        switch (RemovePrefix(hyperlink.Attributes["type"].Value))
                        {
                            case "DrillDownParameter":
                                obj.Hyperlink.Expression = hyperlink["Expression"].InnerText;
                                break;
                        }
            }

            if (node["Hyperlink"] != null && !node["Hyperlink"].InnerText.StartsWith("mailto"))
            {
                if (node["Hyperlink"].InnerText.StartsWith("#"))
                {
                    obj.Hyperlink.Expression = SwapBarackets(node["Hyperlink"].InnerText.Replace("#", ""));
                    obj.Hyperlink.Kind = HyperlinkKind.Bookmark;
                }
                if (node["Hyperlink"].InnerText.StartsWith("http"))
                {
                    obj.Hyperlink.Value = SwapBarackets(node["Hyperlink"].InnerText);
                    obj.Hyperlink.Kind = HyperlinkKind.URL;
                }
            }

            if (obj is BreakableComponent)
                if (node["CanBreak"] != null)
                    (obj as BreakableComponent).CanBreak = UnitsConverter.ConvertBool(node["CanBreak"].InnerText);
        }

        private BandBase ConvertToChildIfDuplicatedBand(Type type)
        {
            BandBase band = null;
            foreach (Base obj in page.AllObjects)
            {
                if (obj.GetType() == type)
                {
                    band = ComponentsFactory.CreateChildBand(GetLastChild((BandBase)obj));
                    break;
                }
            }
            return band;
        }

        private BandBase GetLastChild(BandBase band)
        {
            if (band.Child != null)
                return GetLastChild(band.Child);
            else
                return band;
        }

        private BandBase GetParentBandBase(RectangleF rect, ReportPage page)
        {
            BandBase result = null;
            float offset = 10000;

            foreach (ReportComponentBase obj in page.AllObjects)
                if (obj is BandBase && obj.Bottom - rect.Top > 0 && obj.Bottom - rect.Top < offset)
                {
                    result = (BandBase)obj;
                    offset = obj.Bottom - rect.Top;
                }
            return result;
        }

        private GroupHeaderBand GetParentGroupHeaderBand(RectangleF rect, ReportPage page)
        {
            GroupHeaderBand result = null;
            float offset = 10000;

            foreach (ReportComponentBase obj in page.AllObjects)
                if (obj is GroupHeaderBand && rect.Top - obj.Bottom > 0 && rect.Top - obj.Bottom < offset)
                {
                    result = (GroupHeaderBand)obj;
                    offset = obj.Bottom - rect.Top;
                }
            return result;
        }

        private BandBase GetParentDataBand(RectangleF rect, ReportPage page, bool isTopBand)
        {
            BandBase result = null;
            float offset = 10000;

            foreach (ReportComponentBase obj in page.AllObjects)
                if (isTopBand)
                {
                    if (obj is DataBand && obj.Top - rect.Bottom >= 0 && obj.Top - rect.Bottom < offset)
                    {
                        if ((obj as DataBand).Header != null)
                            result = (obj as DataBand).Header;
                        else
                            result = (DataBand)obj;

                        offset = obj.Top - rect.Bottom;
                    }
                }
                else
                {
                    if (obj is DataBand && rect.Top - obj.Bottom >= 0 && rect.Top - obj.Bottom < offset)
                    {
                        if ((obj as DataBand).Footer != null)
                            result = (obj as DataBand).Footer;
                        else
                            result = (DataBand)obj;

                        offset = rect.Top - obj.Bottom;
                    }
                }
            return result;
        }

        private DataBand GetParentDataBand(RectangleF rect, ReportPage page)
        {
            DataBand result = null;
            float offset = 10000;

            foreach (ReportComponentBase obj in page.AllObjects)
                if (obj is DataBand && obj.Top - rect.Bottom >= 0 && obj.Top - rect.Bottom < offset)
                {
                    result = (DataBand)obj;
                    offset = obj.Top - rect.Bottom;
                }

            return result;
        }

        private List<XmlNode> SortNodeByTop(XmlNode node)
        {
            List<XmlNode> xmlNodeList = new List<XmlNode>();
            if (node["Components"] != null)
                foreach (XmlNode childNode in node["Components"].ChildNodes)
                    xmlNodeList.Add(childNode);

            xmlNodeList = xmlNodeList.OrderBy(x => ParseRectangleF(x["ClientRectangle"].InnerText).Top).ToList();

            return xmlNodeList;
        }

        private Font ParseFont(string font)
        {
            FontStyle fontStyle = FontStyle.Regular;
            string[] defFontParts = font.Split(',');
            if (font.Contains("Bold"))
            {
                fontStyle |= FontStyle.Bold;
            }
            if (font.Contains("Italic"))
            {
                fontStyle |= FontStyle.Italic;
            }
            if (font.Contains("Underline"))
            {
                fontStyle |= FontStyle.Underline;
            }
            if (font.Contains("Strikeout"))
            {
                fontStyle |= FontStyle.Strikeout;
            }

            return new Font(defFontParts[0], UnitsConverter.ConvertFloat(defFontParts[1]), fontStyle);
        }

        private void ParseTextOptions(TextObject textObject, string data)
        {
            if (data == null)
            {
                return;
            }

            string[] props = data.Split(',');

            if (props[2].Contains("="))
                textObject.RightToLeft = UnitsConverter.ConvertBool(props[2].Split('=')[1]);

            if (props[4].Contains("="))
                textObject.WordWrap = UnitsConverter.ConvertBool(props[4].Split('=')[1]);

            int angle = 0;
            if (props[5].Contains("="))
                int.TryParse(props[5].Split('=')[1], System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out angle);

            textObject.Angle = -angle;
        }

        private Padding ParsePageMargins(string margins)
        {
            int[] marg = new int[4];
            int index = 0;
            foreach (var item in margins.Split(','))
            {
                marg[index] = (int)(UnitsConverter.ConvertFloat(item) * UnitsConverter.GetPixelsInUnit(unitType));
                index++;
            }

            return new Padding(marg[0], marg[1], marg[2], marg[3]);
        }

        private Padding ParseMargins(string margins)
        {
            int[] marg = new int[4];
            int index = 0;
            foreach (var item in margins.Split(','))
            {
                marg[index] = UnitsConverter.ConvertInt(item);
                index++;
            }

            return new Padding(marg[0], marg[1], marg[2], marg[3]);
        }

        private RectangleF ParseRectangleF(string rect)
        {
            float[] marg = new float[4];
            int index = 0;
            foreach (var item in rect.Split(','))
            {
                float.TryParse(item, System.Globalization.NumberStyles.Float, System.Globalization.NumberFormatInfo.InvariantInfo, out marg[index]);
                marg[index] *= UnitsConverter.GetPixelsInUnit(unitType);
                index++;
            }

            return new RectangleF(marg[0], marg[1], marg[2], marg[3]);
        }

        private void LoadReferencedAssemblies(XmlNode references)
        {
            foreach (XmlNode reference in references.ChildNodes)
            {
                bool exists = false;

                if (String.IsNullOrEmpty(reference.InnerText))
                    continue;

                // Strip off any trailing ".dll" ".exe" if present.
                string dllName = reference.InnerText;
                if (string.Compare(reference.InnerText.Substring(reference.InnerText.Length - 4), ".dll", true) == 0 ||
                  string.Compare(reference.InnerText.Substring(reference.InnerText.Length - 4), ".exe", true) == 0)
                    dllName = reference.InnerText.Substring(0, reference.InnerText.Length - 4);

                // See if the required assembly is already present in our current AppDomain
                foreach (Assembly currAssembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (string.Compare(currAssembly.GetName().Name, dllName, true) == 0)
                    {
                        // Found it, return the location as the full reference.
                        exists = true;
                    }
                }

                // See if the required assembly is present in the ReferencedAssemblies but not yet loaded
                foreach (AssemblyName assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                {
                    if (string.Compare(assemblyName.Name, dllName, true) == 0)
                    {
                        // Found it, try to load assembly and return the location as the full reference.
                        try
                        {
                            exists = true;
                        }
                        catch { }
                    }
                }

                // Search assembly local and in current folder.
                if (File.Exists(Path.Combine(Config.ApplicationFolder, reference.InnerText)) ||
                    File.Exists(reference.InnerText))
                {
                    exists = true;
                }

                if (exists)
                {
                    Report.AddReferencedAssembly(reference.InnerText);
                }
            }
        }

        private void LoadReportInfo()
        {
            unitType = UnitsConverter.ConverPageUnits(reportNode["ReportUnit"].InnerText);
            Report.SetName(reportNode["ReportName"].InnerText);
            Report.ScriptLanguage = reportNode["ScriptLanguage"].InnerText == "CSharp" ? Language.CSharp : Language.Vb;
            Report.ScriptText += "/*" + reportNode["Script"].InnerText + "*/";
            Report.Alias = reportNode["ReportAlias"].InnerText;

            if (reportNode["ReportDescription"] != null)
                Report.ReportInfo.Description = reportNode["ReportDescription"].InnerText;

            LoadReferencedAssemblies(reportNode["ReferencedAssemblies"]);
        }

        private void LoadStyles()
        {
            foreach (XmlNode style in reportNode["Styles"])
            {
                Style newStyle = ComponentsFactory.CreateStyle(style["Name"].InnerText, Report);

                if (style["Border"] != null)
                {
                    newStyle.Border = UnitsConverter.ConvertBorder(style["Border"].InnerText);
                    newStyle.ApplyBorder = true;
                }
                if (style["TextBrush"] != null)
                {
                    newStyle.TextFill = UnitsConverter.ConvertBrush(style["TextBrush"].InnerText);
                    newStyle.ApplyTextFill = true;
                }
                if (style["Brush"] != null)
                {
                    newStyle.Fill = UnitsConverter.ConvertBrush(style["Brush"].InnerText);
                    newStyle.ApplyFill = true;
                }
                if (style["Font"] != null)
                {
                    newStyle.Font = ParseFont(style["Font"].InnerText);
                    newStyle.ApplyFont = true;
                }
            }
        }

        private void ApplyStyleByName(ReportComponentBase component, string styleName)
        {
            foreach (Style subStyle in Report.Styles)
            {
                if (subStyle.Name == styleName)
                {
                    component.Style = styleName;
                    component.ApplyStyle(subStyle);
                    return;
                }
            }
        }

        private void LoadReport()
        {
            LoadReportInfo();
            LoadDataSource();
            LoadStyles();
            LoadPages();
        }

        private void LoadDataSource()
        {
            foreach (XmlNode db in reportNode["Dictionary"]["Databases"])
            {
                switch (RemovePrefix(db.Attributes["type"].Value))
                {
                    case "XmlDatabase":
                        {
                            XmlDataConnection connection = new XmlDataConnection();
                            XmlConnectionStringBuilder stringBuilder = new XmlConnectionStringBuilder();

                            if (db["PathSchema"].InnerText != "")
                                stringBuilder.XsdFile = db["PathSchema"].InnerText;
                            stringBuilder.XmlFile = db["PathData"].InnerText;

                            connection.Name = db["Name"].InnerText;
                            connection.Alias = db["Alias"].InnerText;
                            connection.ConnectionString = stringBuilder.ToString();
                            connection.CreateAllTables();
                            connection.Enabled = true;

                            Report.Dictionary.Connections.Add(connection);

                            foreach (TableDataSource table in connection.Tables)
                            {
                                table.Enabled = true;
                            }
                            break;
                        }
                    case "JsonDatabase":
                        {
                            JsonDataSourceConnection connection = new JsonDataSourceConnection();
                            JsonDataSourceConnectionStringBuilder stringBuilder = new JsonDataSourceConnectionStringBuilder();

                            if (db["PathData"].InnerText.StartsWith("http"))
                                stringBuilder.Json = db["PathData"].InnerText;
                            else
                                stringBuilder.Json = new StreamReader(db["PathData"].InnerText).ReadToEnd();

                            connection.Name = db["Name"].InnerText;
                            connection.Alias = db["Alias"].InnerText;
                            connection.ConnectionString = stringBuilder.ToString();
                            connection.CreateAllTables();
                            connection.Enabled = true;

                            Report.Dictionary.Connections.Add(connection);

                            foreach (TableDataSource table in connection.Tables)
                            {
                                table.Enabled = true;
                            }
                            break;
                        }
                    case "CsvDatabase":
                        {
                            if (!File.Exists(db["PathData"].InnerText))
                                break;
                            CsvDataConnection connection = new CsvDataConnection();
                            CsvConnectionStringBuilder stringBuilder = new CsvConnectionStringBuilder();

                            stringBuilder.CsvFile = db["PathData"].InnerText;
                            stringBuilder.Codepage = UnitsConverter.ConvertInt(db["CodePage"].InnerText);
                            stringBuilder.Separator = db["CodePage"].InnerText;

                            connection.Name = db["Name"].InnerText;
                            connection.Alias = db["Alias"].InnerText;
                            connection.ConnectionString = stringBuilder.ToString();
                            connection.CreateAllTables();
                            connection.Enabled = true;

                            Report.Dictionary.Connections.Add(connection);

                            foreach (TableDataSource table in connection.Tables)
                            {
                                table.Enabled = true;
                            }
                            break;
                        }
                }
            }
        }

        private void LoadPages()
        {
            foreach (XmlNode node in reportNode["Pages"].ChildNodes)
            {
                if (RemovePrefix(node.Attributes["type"].Value) == "Form")
                    LoadDialogPage(node);
                else
                    LoadPage(node);
            }

            UpdateSubReportPage();
        }

        partial void LoadDialogPage(XmlNode node);

        private void UpdateSubReportPage()
        {
            foreach (Base obj in Report.AllObjects)
            {
                if (obj is SubreportObject)
                {
                    foreach (ReportPage page in Report.Pages)
                    {
                        if (page.Tag == (obj as SubreportObject).Tag)
                            (obj as SubreportObject).ReportPage = page;
                    }
                }
            }
        }

        #endregion // Private Methods

        #region Public Methods
        /// <inheritdoc/>
        public override void LoadReport(Report report, string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                LoadReport(report, stream);
            }
        }
        /// <inheritdoc/>
        public override void LoadReport(Report report, Stream stream)
        {
            Report = report;
            Report.Clear();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(stream);
            reportNode = doc.LastChild;
            page = null;
            leftOffset = 0;
            LoadReport();
        }

        #endregion // Public Methods
    }
}
