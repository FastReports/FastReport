using System;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Table;
using FastReport.Barcode;
using FastReport.Utils;
using FastReport.Matrix;

namespace FastReport.Import.JasperReports
{
    /// <summary>
    /// Represents the JasperReports import plugin.
    /// </summary>
    public partial class JasperReportsImport : ImportBase
    {
        #region Fields

        private ReportPage page;
        private XmlNode reportNode;
        private GroupHeaderBand lastNestedGroup;
        private string FolderPath;

        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JasperReportsImport"/> class.
        /// </summary>
        public JasperReportsImport() : base() { }

        #endregion // Constructors

        #region Private Methods

        private string SwapBarackets(string value)
        {
            return value.Replace("{", "[").Replace("}", "]");
        }

        private ReportComponentBase LoadImage(IParent parent, XmlNode xmlObject)
        {
            if (xmlObject["imageExpression"] != null && (xmlObject["imageExpression"].InnerText).EndsWith(".svg"))
            {
                LoadSVGImage((ReportComponentBase)parent, xmlObject);
                return GetLastReportCompnent(parent);
            }
            else
            {
                var PictureObject = ComponentsFactory.CreatePictureObject("", (Base)parent);
                LoadReportComponentBase(PictureObject, xmlObject);
                LoadPadding(PictureObject, xmlObject);

                if (xmlObject.Attributes["rotation"] != null)
                {
                    PictureObject.Angle = UnitsConverter.ConvertRotation(xmlObject.Attributes["rotation"].Value);
                }

                if (xmlObject.Attributes["scaleImage"] != null)
                {
                    PictureObject.SizeMode = UnitsConverter.ConvertImageSizeMode(xmlObject.Attributes["scaleImage"].Value);
                }

                if (xmlObject["imageExpression"] != null)
                {
                    if (!xmlObject["imageExpression"].InnerText.StartsWith("http") && xmlObject["imageExpression"].InnerText.Contains("."))
                        PictureObject.ImageLocation = FolderPath + "\\" + (xmlObject["imageExpression"].InnerText).Replace("\"", "");
                    else
                        PictureObject.ImageLocation = (xmlObject["imageExpression"].InnerText).Replace("\"", "");
                }

                return PictureObject;
            }
        }

        partial void LoadSVGImage(ReportComponentBase parent, XmlNode xmlObject);

        private MatrixObject LoadMatrix(IParent band, XmlNode xmlObject)
        {
            MatrixObject matrix = ComponentsFactory.CreateMatrixObject("", (Base)band);
            LoadReportComponentBase(matrix, xmlObject);

            int countCell = 0;

            foreach (XmlNode node in xmlObject)
            {
                switch ((node.Name))
                {
                    case "rowGroup":
                        MatrixHeaderDescriptor row = new MatrixHeaderDescriptor((node["bucket"]["bucketExpression"].InnerText).Replace("}", "").Replace("{", ""));
                        matrix.Data.Rows.Add(row);
                        matrix.BuildTemplate();

                        row.TemplateCell.AssignAll(LoadTableCell(node["crosstabRowHeader"]["cellContents"]["textField"]));
                        row.TemplateCell.Brackets = "[,]";

                        if (node["crosstabTotalRowHeader"]["cellContents"]["staticText"] != null)
                        {
                            row.Totals = true;
                            row.TemplateTotalCell = LoadTableCell(node["crosstabTotalRowHeader"]["cellContents"]["staticText"]);
                        }

                        break;

                    case "columnGroup":
                        MatrixHeaderDescriptor column = new MatrixHeaderDescriptor((node["bucket"]["bucketExpression"].InnerText).Replace("}", "").Replace("{", ""));
                        matrix.Data.Columns.Add(column);
                        matrix.BuildTemplate();

                        column.TemplateCell.AssignAll(LoadTableCell(node["crosstabColumnHeader"]["cellContents"]["textField"]));
                        column.TemplateCell.Brackets = "[,]";

                        if (node["crosstabTotalColumnHeader"]["cellContents"]["staticText"] != null)
                        {
                            column.Totals = true;
                            column.TemplateTotalCell = LoadTableCell(node["crosstabTotalColumnHeader"]["cellContents"]["staticText"]);
                        }

                        break;

                    case "crosstabCell":
                        if (countCell > 0)
                            break;

                        countCell++;
                        MatrixCellDescriptor cell = new MatrixCellDescriptor(
                                node["cellContents"]["textField"]["textFieldExpression"].InnerText.Replace("}", "").Replace("{", ""),
                                MatrixAggregateFunction.Sum
                            );
                        matrix.Data.Cells.Add(cell);
                        matrix.BuildTemplate();

                        cell.TemplateCell.Brackets = "[,]";

                        break;
                }
            }
            matrix.BuildTemplate();

            return matrix;
        }

        private SubreportObject LoadTable(IParent band, XmlNode xmlObject)
        {
            SubreportObject table = ComponentsFactory.CreateSubreportObject("", (Base)band);
            table.BaseName += "_Table";
            LoadReportComponentBase(table, xmlObject);
            ReportPage page = ComponentsFactory.CreateReportPage(Report);
            page.CreateUniqueName();

            page.PaperWidth = table.Bounds.Width / Units.Millimeters;
            page.PaperHeight = table.Bounds.Height / Units.Millimeters;
            page.LeftMargin = 0;
            page.TopMargin = 0;
            page.RightMargin = 0;
            page.BottomMargin = 0;
            table.ReportPage = page;

            float xHeader = 0, yHeader = 0, xData = 0, xFooter = 0;
            DataBand data = ComponentsFactory.CreateDataBand(page);
            DataHeaderBand groupHeader = ComponentsFactory.CreateDataHeaderBand(data);
            DataFooterBand groupFooter = ComponentsFactory.CreateDataFooterBand(data);

            groupHeader.Border = table.Border.Clone();
            groupHeader.Border.Lines &= ~BorderLines.Bottom;

            data.Border = table.Border.Clone();
            data.Border.Lines &= ~BorderLines.Top;
            data.Border.Lines &= ~BorderLines.Bottom;

            groupFooter.Border = table.Border.Clone();
            groupFooter.Border.Lines &= ~BorderLines.Top;

            groupFooter.RepeatOnEveryPage = true;
            groupFooter.CanGrow = true;
            groupHeader.CanGrow = true;
            data.CanGrow = true;

            foreach (XmlNode node in xmlObject.ChildNodes)
                if (node.Name.Contains("table"))
                    foreach (XmlNode column in node.ChildNodes)
                    {
                        LoadColumn(column, data, groupHeader, groupFooter, ref xHeader, ref yHeader, ref xData, ref xFooter);
                    }

            data.CalcHeight();
            groupHeader.CalcHeight();
            groupFooter.CalcHeight();

            return table;
        }

        private void LoadColumn(XmlNode column, DataBand data, DataHeaderBand groupHeader, DataFooterBand groupFooter, ref float xHeader, ref float yHeader, ref float xData, ref float xFooter)
        {
            bool isColumnGroup = false;
            ReportComponentBase obj;

            if (column.Name.Contains("columnGroup"))
                isColumnGroup = true;

            foreach (XmlNode cell in column.ChildNodes)
            {
                if (cell.Name.Contains("detailCell"))
                {
                    obj = LoadObject(data, cell.LastChild);

                    if (obj is PictureObjectBase)
                        LoadPadding(obj as PictureObjectBase, cell, true);
                    else if (obj is TextObjectBase)
                        LoadPadding(obj as TextObjectBase, cell, true);

                    if (cell.Attributes["style"] != null)
                        ApplyStyleByName(obj, cell.Attributes["style"].Value);

                    obj.Border = UnitsConverter.ConvertBorder(cell.FirstChild);

                    obj.Left += xData;
                    xData += obj.Width;

                }
                else if (cell.Name.Contains("columnHeader"))
                {
                    obj = LoadObject(groupHeader, cell.LastChild);

                    if (obj is PictureObjectBase)
                        LoadPadding(obj as PictureObjectBase, cell, true);
                    else if (obj is TextObjectBase)
                        LoadPadding(obj as TextObjectBase, cell, true);

                    if (obj == null)
                        continue;

                    if (cell.Attributes["style"] != null)
                        ApplyStyleByName(obj, cell.Attributes["style"].Value);

                    obj.Border = UnitsConverter.ConvertBorder(cell.FirstChild);

                    obj.Left += xHeader;
                    obj.Top += yHeader;
                    xHeader += obj.Width;

                    if (isColumnGroup)
                    {
                        yHeader += obj.Height;
                        xHeader -= obj.Width;
                        isColumnGroup = false;
                    }
                }
                else if (cell.Name.Contains("columnFooter"))
                {
                    obj = LoadObject(groupFooter, cell.LastChild);

                    if (obj is PictureObjectBase)
                        LoadPadding(obj as PictureObjectBase, cell, true);
                    else if (obj is TextObjectBase)
                        LoadPadding(obj as TextObjectBase, cell, true);

                    if (cell.Attributes["style"] != null)
                        ApplyStyleByName(obj, cell.Attributes["style"].Value);

                    obj.Border = UnitsConverter.ConvertBorder(cell.FirstChild);

                    obj.Left += xFooter;
                    xFooter += obj.Width;
                }
                if (cell.Name.EndsWith("column"))
                {
                    LoadColumn(cell, data, groupHeader, groupFooter, ref xHeader, ref yHeader, ref xData, ref xFooter);
                }
            }
        }

        private BarcodeObject LoadBarCode(IParent band, XmlNode xmlObject)
        {
            BarcodeObject barcodeObject = null;
            foreach (XmlNode node in xmlObject.ChildNodes)
            {
                // it's need for skip other users component
                switch (node.Name.ToLower().Remove(0, node.Name.IndexOf(':') + 1))
                {
                    case "ean8":
                    case "ean128":
                    case "pdf417":
                    case "ean13":
                    case "usps":
                    case "code128a":
                    case "code128b":
                    case "code128c":
                    case "code128":
                    case "code39":
                    case "code39 (extended)":
                    case "codabar":
                    case "upca":
                    case "upce":
                    case "interleaved2of5":
                    case "int2of5":
                    case "std2of5":
                    case "uspsintelligentmail":
                    case "postnet":
                    case "datamatrix":
                        {
                            barcodeObject = ComponentsFactory.CreateBarcodeObject("", (Base)band);
                            barcodeObject.AutoSize = false;
                            LoadReportComponentBase(barcodeObject, xmlObject);
                            UnitsConverter.ConvertBarcodeSymbology(node.Name.ToLower().Remove(0, node.Name.IndexOf(':') + 1), barcodeObject);

                            if (node.Attributes["orientation"] != null)
                                barcodeObject.Angle = UnitsConverter.ConvertRotation(node.Attributes["orientation"].Value);
                            break;
                        }
                    default:
                        if (node.Name.Contains("barbecue"))
                        {
                            barcodeObject = ComponentsFactory.CreateBarcodeObject("", (Base)band);
                            LoadReportComponentBase(barcodeObject, xmlObject);
                            if (node.Attributes["type"] != null)
                                UnitsConverter.ConvertBarcodeSymbology(node.Attributes["type"].Value.ToLower(), barcodeObject);

                            if (node.Attributes["drawText"] != null)
                                barcodeObject.ShowText = UnitsConverter.ConvertBool(node.Attributes["drawText"].Value);

                            if (node.Attributes["rotation"] != null)
                                barcodeObject.Angle = UnitsConverter.ConvertRotation(node.Attributes["rotation"].Value);
                        }
                        if (node.Name.Contains("QRCode"))
                        {
                            barcodeObject = ComponentsFactory.CreateBarcodeObject("", (Base)band);
                            LoadReportComponentBase(barcodeObject, xmlObject);

                            barcodeObject.Barcode = new BarcodeQR();

                            if (node.Attributes["errorCorrectionLevel"] != null)
                                (barcodeObject.Barcode as BarcodeQR).ErrorCorrection = UnitsConverter.ConvertErrorCorrection(node.Attributes["errorCorrectionLevel"].Value);
                        }
                        break;
                }
            }

            return barcodeObject;
        }

        partial void LoadRichText(IParent band, XmlNode xmlObject);

        partial void LoadMap(IParent band, XmlNode xmlObject);

        partial void LoadChart(IParent band, XmlNode xmlObject);

        private TextObjectBase LoadTextObject(IParent band, XmlNode xmlObject)
        {
            if (xmlObject["textElement"] != null && xmlObject["textElement"].Attributes["markup"] != null && xmlObject["textElement"].Attributes["markup"].Value == "rtf")
            {
                LoadRichText(band, xmlObject);
                return (TextObjectBase)(band as ReportComponentBase).ChildObjects[(band as ReportComponentBase).ChildObjects.Count - 1];
            }
            TextObject textObject = ComponentsFactory.CreateTextObject("", band as Base);
            textObject.CreateUniqueName();
            LoadReportComponentBase(textObject, xmlObject);
            LoadPadding(textObject, xmlObject);

            if (xmlObject["text"] != null)
                textObject.Text = xmlObject["text"].InnerText;

            if (xmlObject["textFieldExpression"] != null)
                textObject.Text = xmlObject["textFieldExpression"].InnerText;

            if (xmlObject["reportElement"] != null && xmlObject["reportElement"].Attributes["forecolor"] != null)
                textObject.TextColor = UnitsConverter.ConvertColor(xmlObject["reportElement"].Attributes["forecolor"].Value);

            if (xmlObject["textElement"] != null)
            {
                if (xmlObject["textElement"].Attributes["textAlignment"] != null)
                    textObject.HorzAlign = UnitsConverter.ConvertTextAlignmentToHorzAlign(xmlObject["textElement"].Attributes["textAlignment"].Value);

                if (xmlObject["textElement"].Attributes["verticalAlignment"] != null)
                    textObject.VertAlign = UnitsConverter.ConvertTextAlignmentToVertAlign(xmlObject["textElement"].Attributes["verticalAlignment"].Value);

                if (xmlObject["textElement"]["font"] != null)
                {
                    textObject.Font = ParseFont(xmlObject["textElement"]["font"]);
                }

                if (xmlObject["textElement"].Attributes["rotation"] != null)
                {
                    textObject.Angle = UnitsConverter.ConvertRotation(xmlObject["textElement"].Attributes["rotation"].Value);
                }

                if (xmlObject["textElement"]["paragraph"] != null)
                {
                    if (xmlObject["textElement"]["paragraph"].Attributes["leftIndent"] != null)
                        textObject.ParagraphOffset = UnitsConverter.ConvertInt(xmlObject["textElement"]["paragraph"].Attributes["leftIndent"].Value);

                    if (xmlObject["textElement"]["paragraph"].HasChildNodes)
                    {
                        foreach (XmlNode node2 in xmlObject["textElement"]["paragraph"].ChildNodes)
                            if (node2.Attributes["position"] != null)
                                textObject.TabPositions.Add(UnitsConverter.ConvertInt(node2.Attributes["position"].Value));
                    }
                }
            }

            return textObject;
        }

        private ShapeObject LoadPrimitiveShape(IParent band, XmlNode xmlObject, ShapeKind shapeKind)
        {
            ShapeObject shapeObject = ComponentsFactory.CreateShapeObject("", (Base)band);
            shapeObject.Shape = shapeKind;
            LoadReportComponentBase(shapeObject, xmlObject);

            return shapeObject;
        }

        private LineObject LoadLineObject(IParent band, XmlNode xmlObject)
        {
            LineObject lineObject = ComponentsFactory.CreateLineObject("", (Base)band);
            LoadReportComponentBase(lineObject, xmlObject);

            lineObject.Diagonal = true;

            return lineObject;
        }

        partial void LoadChart(BandBase band, XmlNode xmlObject);

        private void LoadOverlayBand(XmlNode node)
        {
            OverlayBand band = null;
            if (node["band"].Attributes["height"] != null)
                band = ComponentsFactory.CreateOverlayBand(page);

            LoadBandBase(band, node);
        }

        private void LoadReportObjects(XmlNode node, IParent band, int topOffset = 0)
        {
            ReportComponentBase obj;
            foreach (XmlNode item in node.ChildNodes)
            {
                obj = LoadObject(band, item);
                if (obj != null)
                    obj.Top += topOffset;
            }
        }

        private void LoadBands(XmlNode node, ReportPage page)
        {
            BandBase band;
            foreach (XmlNode item in node.ChildNodes)
            {
                band = null;
                switch ((item.Name))
                {
                    case "detail":
                        {
                            if (item["band"].Attributes["height"] != null)
                                band = ComponentsFactory.CreateDataBand(page);

                            break;
                        }
                    case "title":
                        {
                            band = CheckDuplicatedBand(typeof(ReportTitleBand));
                            if (band == null && item["band"].Attributes["height"] != null && !(reportNode.Attributes["isTitleNewPage"] != null && UnitsConverter.ConvertBool(reportNode.Attributes["isTitleNewPage"].Value)))
                                band = ComponentsFactory.CreateReportTitleBand(page);

                            break;
                        }
                    case "columnHeader":
                        {
                            band = CheckDuplicatedBand(typeof(ColumnHeaderBand));
                            if (band == null && item["band"].Attributes["height"] != null)
                                band = ComponentsFactory.CreateColumnHeaderBand(page);

                            break;
                        }
                    case "columnFooter":
                        {
                            band = CheckDuplicatedBand(typeof(ColumnFooterBand));
                            if (band == null && item["band"].Attributes["height"] != null)
                                band = ComponentsFactory.CreateColumnFooterBand(page);

                            break;
                        }
                    case "pageHeader":
                        {
                            band = CheckDuplicatedBand(typeof(PageHeaderBand));
                            if (band == null && item["band"].Attributes["height"] != null)
                                band = ComponentsFactory.CreatePageHeaderBand(page);

                            break;
                        }
                    case "pageFooter":
                        {
                            band = CheckDuplicatedBand(typeof(PageFooterBand));
                            if (band == null && item["band"].Attributes["height"] != null)
                                band = ComponentsFactory.CreatePageFooterBand(page);

                            break;
                        }
                    case "lastPageFooter":
                        {
                            band = CheckDuplicatedBand(typeof(ReportSummaryBand));
                            if (band == null && item["band"].Attributes["height"] != null)
                                band = ComponentsFactory.CreateReportSummaryBand(page);

                            Report.DoublePass = true;
                            PageFooterBand footerBand = (PageFooterBand)CheckDuplicatedBand(typeof(PageFooterBand));

                            if (footerBand != null)
                                footerBand.PrintOn &= ~PrintOn.LastPage;

                            break;
                        }
                    case "noData":
                        {
                            if (item["band"].Attributes["height"] != null)
                                band = ComponentsFactory.CreateDataBand(page);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                LoadBandBase(band, item);
            }
        }

        private ReportComponentBase LoadObject(IParent band, XmlNode xmlObject)
        {
            switch ((xmlObject.Name))
            {
                case "textField":
                case "staticText":
                    {
                        return LoadTextObject(band, xmlObject);
                    }
                case "frame":
                    {
                        ContainerObject containerObject = ComponentsFactory.CreateContainerObject("", band as Base);
                        LoadReportComponentBase(containerObject, xmlObject);
                        LoadReportObjects(xmlObject, containerObject);
                        return containerObject;
                    }
                case "rectangle":
                    {
                        if (xmlObject.Attributes["radius"] != null && UnitsConverter.ConvertInt(xmlObject.Attributes["radius"].Value) > 0)
                            return LoadPrimitiveShape(band, xmlObject, ShapeKind.RoundRectangle);
                        else
                            return LoadPrimitiveShape(band, xmlObject, ShapeKind.Rectangle);
                    }
                case "image":
                    {
                        return LoadImage(band, xmlObject);
                    }
                case "pie3DChart":
                case "pieChart":
                case "stackedBar3DChart":
                case "stackedBarChart":
                case "xyBarChart":
                case "bar3DChart":
                case "barChart":
                case "scatterChart":
                case "xyLineChart":
                case "lineChart":
                case "timeSeriesChart":
                case "xyAreaChart":
                case "areaChart":
                case "highLowChart":
                case "candlestickChart":
                case "stackedAreaChart":
                case "bubbleChart":
                case "thermometerChart":
                case "meterChart":
                case "ganttChart":
                case "multiAxisPlot":
                    {
                        LoadChart(band, xmlObject);
                        return GetLastReportCompnent(band);
                    }
                case "line":
                    {
                        return LoadLineObject(band, xmlObject);
                    }
                case "ellipse":
                    {
                        return LoadPrimitiveShape(band, xmlObject, ShapeKind.Ellipse);
                    }
                case "componentElement":
                    {
                        foreach (XmlNode childNode in xmlObject.ChildNodes)
                        {
                            if (childNode.Name == "reportElement")
                                continue;
                            if (childNode.Name.Contains("table"))
                                return LoadTable(band, xmlObject);
                            else if (childNode.Name.Contains("Chart"))
                            {
                                LoadChart(band, childNode);
                                LoadReportComponentBase(GetLastReportCompnent(band), xmlObject);
                                return GetLastReportCompnent(band);
                            }
                            else if (childNode.Name.Contains("map"))
                            {
                                LoadMap(band, childNode);
                                LoadReportComponentBase(GetLastReportCompnent(band), xmlObject);
                                return GetLastReportCompnent(band);
                            }
                            else
                                return LoadBarCode(band, xmlObject);
                        }
                        return null;
                    }
                case "subreport":
                    {
                        SubreportObject subreportObject = ComponentsFactory.CreateSubreportObject("", (Base)band);
                        LoadReportComponentBase(subreportObject, xmlObject);

                        if (FolderPath == null)
                            return null;
                        try
                        {
                            Report report = new Report();
                            JasperReportsImport reportsImport = new JasperReportsImport();
                            var path = FolderPath + "\\" + (xmlObject["subreportExpression"].InnerText).Replace(".jasper", ".jrxml").Replace("\"", "");
                            reportsImport.LoadReport(report, path);
                            foreach (PageBase page in report.Pages)
                            {
                                page.Parent = Report;
                                page.CreateUniqueName();
                                subreportObject.ReportPage = (ReportPage)page;
                                Report.Pages.Add(page);
                            }
                        }
                        catch { }

                        return subreportObject;
                    }
                case "crosstab":
                    {
                        return LoadMatrix(band, xmlObject);
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        private TableCell LoadTableCell(XmlNode node)
        {
            TableCell result = new TableCell();
            result.SetReport(Report);
            LoadReportComponentBase(result, node);

            if (node.Attributes["rotation"] != null)
            {
                result.Angle = UnitsConverter.ConvertRotation(node.Attributes["rotation"].Value);
            }

            if (node["textElement"] != null)
            {
                if (node["textElement"].Attributes["textAlignment"] != null)
                    result.HorzAlign = UnitsConverter.ConvertTextAlignmentToHorzAlign(node["textElement"].Attributes["textAlignment"].Value);

                if (node["textElement"].Attributes["verticalAlignment"] != null)
                    result.VertAlign = UnitsConverter.ConvertTextAlignmentToVertAlign(node["textElement"].Attributes["verticalAlignment"].Value);

                if (node["textElement"]["font"] != null)
                {
                    result.Font = ParseFont(node["textElement"]["font"]);
                }

                if (node["textElement"]["paragraph"] != null)
                {
                    if (node["textElement"]["paragraph"].Attributes["leftIndent"] != null)
                        result.ParagraphOffset = UnitsConverter.ConvertInt(node["textElement"]["paragraph"].Attributes["leftIndent"].Value);

                    if (node["textElement"]["paragraph"].HasChildNodes)
                    {
                        foreach (XmlNode node2 in node["textElement"]["paragraph"].ChildNodes)
                            if (node2.Attributes["position"] != null)
                                result.TabPositions.Add(UnitsConverter.ConvertInt(node2.Attributes["position"].Value));
                    }
                }
            }

            return result;
        }

        private void LoadChildBands(XmlNode node, ReportPage page)
        {
            foreach (XmlNode item in (node))
                if (item.Name == "group")
                {
                    foreach (XmlNode xmlBand in (item))
                    {
                        switch ((xmlBand.Name))
                        {
                            case "groupHeader":
                                {
                                    GroupHeaderBand band = ComponentsFactory.CreateGroupHeaderBand(page);
                                    LoadBandBase(band, xmlBand);
                                    band.Data = GetParentDataBand(page);
                                    if (lastNestedGroup != null)
                                        lastNestedGroup.NestedGroup = band;
                                    lastNestedGroup = band;

                                    break;
                                }
                            case "groupFooter":
                                {
                                    if (lastNestedGroup == null)
                                        break;

                                    GroupFooterBand band = ComponentsFactory.CreateGroupFooterBand(lastNestedGroup);
                                    LoadBandBase(band, xmlBand);
                                    break;
                                }
                        }
                    }
                }
                else if (item.Name == "summary")
                {
                    PageFooterBand band = (PageFooterBand)CheckDuplicatedBand(typeof(PageFooterBand));
                    if (item["band"].Attributes["height"] != null && !(reportNode.Attributes["isSummaryNewPage"] != null && UnitsConverter.ConvertBool(reportNode.Attributes["isSummaryNewPage"].Value)))
                    {
                        if (band == null)
                            band = ComponentsFactory.CreatePageFooterBand(page);

                        Report.DoublePass = true;

                        ChildBand childBand = ComponentsFactory.CreateChildBand(band);
                        LoadBandBase(childBand, item);
                        childBand.PrintOn = PrintOn.LastPage | PrintOn.SinglePage;
                    }
                }
                else if (item.Name == "background")
                {
                    LoadOverlayBand(item);
                }
        }

        private void LoadBandBase(BandBase band, XmlNode node)
        {
            if (band == null)
                return;

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                if (node.ChildNodes[i].Name == "band")
                {
                    if (i == 0)
                    {
                        if (node.ChildNodes[i].Attributes["height"] != null)
                            band.Height = UnitsConverter.ConvertInt(node.ChildNodes[i].Attributes["height"].Value) * DrawUtils.ScreenDpi / 96f;
                        LoadReportObjects(node.ChildNodes[i], band);
                    }
                    else
                    {
                        ChildBand childBand = ComponentsFactory.CreateChildBand(GetLastChild(band));
                        if (node.ChildNodes[i].Attributes["height"] != null)
                            childBand.Height = UnitsConverter.ConvertInt(node.ChildNodes[i].Attributes["height"].Value) * DrawUtils.ScreenDpi / 96f;
                        LoadReportObjects(node.ChildNodes[i], childBand);
                    }
                }
            }
        }

        private void LoadReportComponentBase(ReportComponentBase obj, XmlNode node)
        {
            obj.CreateUniqueName();

            if (node["reportElement"] != null)
            {
                if (node["reportElement"].Attributes["x"] != null && node["reportElement"].Attributes["y"] != null &&
                    node["reportElement"].Attributes["width"] != null && node["reportElement"].Attributes["height"] != null)
                {
                    RectangleF rect = new RectangleF(
                        (float)Math.Ceiling(UnitsConverter.ConvertFloat(node["reportElement"].Attributes["x"].Value) * DrawUtils.ScreenDpi / 96f),
                        (float)Math.Ceiling(UnitsConverter.ConvertFloat(node["reportElement"].Attributes["y"].Value) * DrawUtils.ScreenDpi / 96f),
                        (float)Math.Floor(UnitsConverter.ConvertFloat(node["reportElement"].Attributes["width"].Value) * DrawUtils.ScreenDpi / 96f),
                        (float)Math.Floor(UnitsConverter.ConvertFloat(node["reportElement"].Attributes["height"].Value) * DrawUtils.ScreenDpi / 96f));
                    obj.Bounds = rect;
                }

                if (node["reportElement"].Attributes["backcolor"] != null)
                    obj.FillColor = UnitsConverter.ConvertColor(node["reportElement"].Attributes["backcolor"].Value);

                if (node["reportElement"].Attributes["style"] != null)
                    ApplyStyleByName(obj, node["reportElement"].Attributes["style"].Value);
            }

            if (node["box"] != null)
            {
                obj.Border = UnitsConverter.ConvertBorder(node["box"]);
            }

            if (node.Attributes["hyperlinkType"] != null)
            {
                string hyperlink = "";
                if (node["hyperlinkAnchorExpression"] != null)
                    hyperlink = SwapBarackets((node["hyperlinkAnchorExpression"].InnerText));
                if (node["hyperlinkReferenceExpression"] != null)
                    hyperlink = SwapBarackets((node["hyperlinkReferenceExpression"].InnerText));
                if (node["hyperlinkPageExpression"] != null)
                    hyperlink = SwapBarackets((node["hyperlinkPageExpression"].InnerText));

                obj.Hyperlink.Kind = UnitsConverter.ConvertHyperlinkType(node);

                if (hyperlink.StartsWith("["))
                    obj.Hyperlink.Expression = hyperlink;
                else
                    obj.Hyperlink.Value = hyperlink;
            }

            if (node["anchorNameExpression"] != null)
            {
                obj.Bookmark = SwapBarackets((node["anchorNameExpression"].InnerText));
            }
        }

        private void LoadPadding(PictureObjectBase obj, XmlNode node, bool margin = false)
        {
            Padding padding = new Padding();
            if (node["box"] != null)
            {
                if (node["box"].Attributes["padding"] != null)
                {
                    padding.Top = (int)Math.Round(UnitsConverter.ConvertInt(node["box"].Attributes["padding"].Value) * DrawUtils.ScreenDpi / 96f);
                    padding.Left = padding.Top;
                    padding.Right = padding.Top;
                    padding.Bottom = padding.Top;
                    if (margin)
                    {
                        obj.Height += padding.Top * 2;
                        obj.Width += padding.Top * 2;
                    }
                }
                if (node["box"].Attributes["topPadding"] != null)
                {
                    padding.Top = (int)Math.Round(UnitsConverter.ConvertInt(node["box"].Attributes["topPadding"].Value) * DrawUtils.ScreenDpi / 96f);
                    if (margin)
                        obj.Height += padding.Top;
                }
                if (node["box"].Attributes["leftPadding"] != null)
                {
                    padding.Left = (int)Math.Round(UnitsConverter.ConvertInt(node["box"].Attributes["leftPadding"].Value) * DrawUtils.ScreenDpi / 96f);
                    if (margin)
                        obj.Width += padding.Left;
                }
                if (node["box"].Attributes["bottomPadding"] != null)
                {
                    padding.Bottom = (int)Math.Round(UnitsConverter.ConvertInt(node["box"].Attributes["bottomPadding"].Value) * DrawUtils.ScreenDpi / 96f);
                    if (margin)
                        obj.Height += padding.Bottom;
                }
                if (node["box"].Attributes["rightPadding"] != null)
                {
                    padding.Right = (int)Math.Round(UnitsConverter.ConvertInt(node["box"].Attributes["rightPadding"].Value) * DrawUtils.ScreenDpi / 96f);
                    if (margin)
                        obj.Width += padding.Right;
                }
            }

            obj.Padding = padding;
        }

        private void LoadPadding(TextObjectBase obj, XmlNode node, bool margin = false)
        {
            Padding padding = new Padding();
            if (node["box"] != null)
            {
                if (node["box"].Attributes["padding"] != null)
                {
                    padding.Top = (int)Math.Round(UnitsConverter.ConvertInt(node["box"].Attributes["padding"].Value) * DrawUtils.ScreenDpi / 96f);
                    padding.Left = padding.Top;
                    padding.Right = padding.Top;
                    padding.Bottom = padding.Top;
                    if (margin)
                    {
                        obj.Height += padding.Top * 2;
                        obj.Width += padding.Top * 2;
                    }
                }
                if (node["box"].Attributes["topPadding"] != null)
                {
                    padding.Top = (int)Math.Round(UnitsConverter.ConvertInt(node["box"].Attributes["topPadding"].Value) * DrawUtils.ScreenDpi / 96f);
                    if (margin)
                        obj.Height += padding.Top;
                }
                if (node["box"].Attributes["leftPadding"] != null)
                {
                    padding.Left = (int)Math.Round(UnitsConverter.ConvertInt(node["box"].Attributes["leftPadding"].Value) * DrawUtils.ScreenDpi / 96f);
                    if (margin)
                        obj.Width += padding.Left;
                }
                if (node["box"].Attributes["bottomPadding"] != null)
                {
                    padding.Bottom = (int)Math.Round(UnitsConverter.ConvertInt(node["box"].Attributes["bottomPadding"].Value) * DrawUtils.ScreenDpi / 96f);
                    if (margin)
                        obj.Height += padding.Bottom;
                }
                if (node["box"].Attributes["rightPadding"] != null)
                {
                    padding.Right = (int)Math.Round(UnitsConverter.ConvertInt(node["box"].Attributes["rightPadding"].Value) * DrawUtils.ScreenDpi / 96f);
                    if (margin)
                        obj.Width += padding.Right;
                }
            }

            obj.Padding = padding;
        }

        private BandBase CheckDuplicatedBand(Type type)
        {
            BandBase band = null;
            foreach (Base obj in page.AllObjects)
            {
                if (obj.GetType() == type)
                {
                    return (BandBase)obj;
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

        private DataBand GetParentDataBand(ReportPage page)
        {
            DataBand result = null;

            foreach (Base obj in page.AllObjects)
                if (obj is DataBand)
                {
                    result = (DataBand)obj;
                }

            return result;
        }

        private ReportComponentBase GetLastReportCompnent(IParent band)
        {
            return (ReportComponentBase)(band as ReportComponentBase).ChildObjects[(band as ReportComponentBase).ChildObjects.Count - 1];
        }

        private Font ParseFont(XmlNode font)
        {
            float size = 10;
            if (font.Attributes["size"] != null)
                size = UnitsConverter.ConvertFloat(font.Attributes["size"].Value);

            FontStyle fontStyle = FontStyle.Regular;
            if (font.Attributes["isBold"] != null && UnitsConverter.ConvertBool(font.Attributes["isBold"].Value))
            {
                fontStyle |= FontStyle.Bold;
            }
            if (font.Attributes["isItalic"] != null && UnitsConverter.ConvertBool(font.Attributes["isItalic"].Value))
            {
                fontStyle |= FontStyle.Italic;
            }
            if (font.Attributes["isUnderline"] != null && UnitsConverter.ConvertBool(font.Attributes["isUnderline"].Value))
            {
                fontStyle |= FontStyle.Underline;
            }
            if (font.Attributes["isStrikeThrough"] != null && UnitsConverter.ConvertBool(font.Attributes["isStrikeThrough"].Value))
            {
                fontStyle |= FontStyle.Strikeout;
            }

            if (font.Attributes["fontName"] == null)
                return new Font(DrawUtils.DefaultReportFont.FontFamily, size, fontStyle);

            return new Font(font.Attributes["fontName"].Value, size, fontStyle);
        }

        private void LoadStyles()
        {
            foreach (XmlNode node in reportNode)
            {
                if (node.Name == "style")
                {
                    Style newStyle = ComponentsFactory.CreateStyle(node.Attributes["name"].Value, Report);

                    newStyle.ApplyFont = false;
                    newStyle.ApplyBorder = false;
                    newStyle.ApplyFill = false;
                    newStyle.ApplyTextFill = false;

                    if (node["box"] != null)
                    {
                        newStyle.Border = UnitsConverter.ConvertBorder(node["box"]);
                        newStyle.ApplyBorder = true;
                    }
                    if (node.Attributes["forecolor"] != null)
                    {
                        newStyle.TextFill = new SolidFill(UnitsConverter.ConvertColor(node.Attributes["forecolor"].Value));
                        newStyle.ApplyTextFill = true;
                    }
                    if (node.Attributes["backcolor"] != null)
                    {
                        newStyle.Fill = new SolidFill(UnitsConverter.ConvertColor(node.Attributes["backcolor"].Value));
                        newStyle.ApplyFill = true;
                    }
                    if (node.Attributes["fontName"] != null || node.Attributes["size"] != null || node.Attributes["isBold"] != null
                        || node.Attributes["isItalic"] != null || node.Attributes["isUnderline"] != null || node.Attributes["isStrikeThrough"] != null)
                    {
                        newStyle.Font = ParseFont(node);
                        newStyle.ApplyFont = true;
                    }
                }
            }
        }

        private void ApplyStyleByName(ReportComponentBase component, string styleName)
        {
            foreach (Style style in Report.Styles)
            {
                if (style.Name == styleName)
                {
                    component.Style = styleName;
                    component.ApplyStyle(style);
                    return;
                }
            }
        }

        private void LoadReport()
        {
            lastNestedGroup = null;
            page = null;

            if (reportNode.Attributes["name"] != null)
                Report.SetName(reportNode.Attributes["name"].InnerText);

            LoadStyles();

            CreatePage();
            if (reportNode.Attributes["isTitleNewPage"] != null)
            {
                if (UnitsConverter.ConvertBool(reportNode.Attributes["isTitleNewPage"].Value))
                {
                    page.Name = "Tittle";
                    foreach (XmlNode node in reportNode.ChildNodes)
                    {
                        if (node.Name == "title")
                        {
                            ReportTitleBand band = (ReportTitleBand)CheckDuplicatedBand(typeof(ReportTitleBand));
                            if (band == null && node["band"].Attributes["height"] != null)
                                band = ComponentsFactory.CreateReportTitleBand(page);
                            LoadBandBase(band, node);
                        }

                        if (node.Name == "background")
                        {
                            LoadOverlayBand(node);
                        }
                    }
                    CreatePage();
                }
            }

            LoadBands(reportNode, page);
            LoadChildBands(reportNode, page);

            if (reportNode.Attributes["isSummaryNewPage"] != null)
            {
                if (UnitsConverter.ConvertBool(reportNode.Attributes["isSummaryNewPage"].Value))
                {
                    CreatePage();
                    page.Name = "Summary";
                    foreach (XmlNode node in reportNode.ChildNodes)
                    {
                        if (node.Name == "summary")
                        {
                            PageFooterBand band = (PageFooterBand)CheckDuplicatedBand(typeof(PageFooterBand));
                            if (node["band"].Attributes["height"] != null)
                            {
                                if (band == null)
                                    band = ComponentsFactory.CreatePageFooterBand(page);

                                Report.DoublePass = true;

                                ChildBand childBand = ComponentsFactory.CreateChildBand(band);
                                LoadBandBase(childBand, node);
                                childBand.PrintOn = PrintOn.LastPage | PrintOn.SinglePage;
                            }
                        }

                        if (node.Name == "background")
                        {
                            LoadOverlayBand(node);
                        }
                    }

                }
            }
        }

        private void CreatePage()
        {
            page = ComponentsFactory.CreateReportPage("", Report);

            if (reportNode.Attributes["orientation"] != null && reportNode.Attributes["orientation"].Value == "Landscape")
                page.Landscape = true;

            if (reportNode.Attributes["pageWidth"] != null && reportNode.Attributes["pageHeight"] != null)
            {
                page.PaperWidth = UnitsConverter.ConvertFloat(reportNode.Attributes["pageWidth"].Value) * DrawUtils.ScreenDpi / 96f / Units.Millimeters;
                page.PaperHeight = UnitsConverter.ConvertFloat(reportNode.Attributes["pageHeight"].Value) * DrawUtils.ScreenDpi / 96f / Units.Millimeters;
            }

            if (reportNode.Attributes["leftMargin"] != null)
                page.LeftMargin = UnitsConverter.ConvertFloat(reportNode.Attributes["leftMargin"].Value) * DrawUtils.ScreenDpi / 96f / Units.Millimeters;

            if (reportNode.Attributes["topMargin"] != null)
                page.TopMargin = UnitsConverter.ConvertFloat(reportNode.Attributes["topMargin"].Value) * DrawUtils.ScreenDpi / 96f / Units.Millimeters;

            if (reportNode.Attributes["rightMargin"] != null)
                page.RightMargin = UnitsConverter.ConvertFloat(reportNode.Attributes["rightMargin"].Value) * DrawUtils.ScreenDpi / 96f / Units.Millimeters;

            if (reportNode.Attributes["bottomMargin"] != null)
                page.BottomMargin = UnitsConverter.ConvertFloat(reportNode.Attributes["bottomMargin"].Value) * DrawUtils.ScreenDpi / 96f / Units.Millimeters;
        }

        #endregion // Private Methods

        #region Public Methods

        /// <inheritdoc/>
        public override void LoadReport(Report report, string filename)
        {
            FolderPath = Path.GetDirectoryName(filename);
            Report = report;
            Report.Clear();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(filename);
            reportNode = doc.LastChild;
            LoadReport();
        }

        /// <inheritdoc/>
        public override void LoadReport(Report report, Stream content)
        {
            base.LoadReport(report, content);
            Report = report;
            Report.Clear();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(content);
            reportNode = doc.LastChild;
            LoadReport();
        }

        #endregion // Public Methods
    }
}
