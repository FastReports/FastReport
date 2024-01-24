using System;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Data;
using FastReport.Table;

namespace FastReport.Import.RDL
{
    /// <summary>
    /// Represents the RDL import plugin.
    /// </summary>
    public partial class RDLImport : ImportBase
    {
        #region Fields

        private ReportPage page;
        private ComponentBase component;
        private Base parent;
        private string defaultFontFamily;
        private BandBase curBand;
        private string dataSetName;
        private bool firstRun;
        private XmlNode reportNode;
        private string filename;
        private float sectionWidth = 0.0f;

        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RDLImport"/> class.
        /// </summary>
        public RDLImport() : base()
        {
        }

        #endregion // Constructors

        #region Private Methods

        private void LoadBorderColor(XmlNode borderColorNode, string border)
        {
            XmlNodeList nodeList = borderColorNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (border == "Default")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.Color = UnitsConverter.ConvertColor(node.InnerText);
                    }
                }
                else if (border == "Top")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.TopLine.Color = UnitsConverter.ConvertColor(node.InnerText);
                    }
                }
                else if (border == "Left")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.LeftLine.Color = UnitsConverter.ConvertColor(node.InnerText);
                    }
                }
                else if (border == "Right")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.RightLine.Color = UnitsConverter.ConvertColor(node.InnerText);
                    }
                }
                else if (border == "Bottom")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.BottomLine.Color = UnitsConverter.ConvertColor(node.InnerText);
                    }
                }
            }
        }

        private void LoadBorder(XmlNode borderNode)
        {
            XmlNodeList nodeList = borderNode.ChildNodes;
            string border = "";

            if (borderNode.Name == "TopBorder")
            {
                border = "Top";
            }
            else if (borderNode.Name == "BottomBorder")
            {
                border = "Bottom";
            }
            else if (borderNode.Name == "LeftBorder")
            {
                border = "Left";
            }
            else if (borderNode.Name == "RightBorder")
            {
                border = "Right";
            }
            else
            {
                border = "default";
            }

            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Color")
                {
                    LoadBorderColor(node, border);
                }
                else if (node.Name == "Style")
                {
                    LoadBorderStyle(node, border);
                }
                else if (node.Name == "Width")
                {
                    LoadBorderWidth(node, border);
                }
            }
        }

        private void LoadBorderStyle(XmlNode borderStyleNode, string border)
        {
            XmlNodeList nodeList = borderStyleNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.InnerText != "None")
                {
                    if (border == "Default")
                    {
                        if (component is ReportComponentBase)
                        {
                            (component as ReportComponentBase).Border.Lines = BorderLines.All;
                            (component as ReportComponentBase).Border.Style = UnitsConverter.ConvertBorderStyle(node.InnerText);
                        }
                    }
                    else if (border == "Top")
                    {
                        if (component is ReportComponentBase)
                        {
                            (component as ReportComponentBase).Border.Lines |= BorderLines.Top;
                            (component as ReportComponentBase).Border.TopLine.Style = UnitsConverter.ConvertBorderStyle(node.InnerText);
                        }
                    }
                    else if (border == "Left")
                    {
                        if (component is ReportComponentBase)
                        {
                            (component as ReportComponentBase).Border.Lines |= BorderLines.Left;
                            (component as ReportComponentBase).Border.LeftLine.Style = UnitsConverter.ConvertBorderStyle(node.InnerText);
                        }
                    }
                    else if (border == "Right")
                    {
                        if (component is ReportComponentBase)
                        {
                            (component as ReportComponentBase).Border.Lines |= BorderLines.Right;
                            (component as ReportComponentBase).Border.RightLine.Style = UnitsConverter.ConvertBorderStyle(node.InnerText);
                        }
                    }
                    else if (border == "Bottom")
                    {
                        if (component is ReportComponentBase)
                        {
                            (component as ReportComponentBase).Border.Lines |= BorderLines.Bottom;
                            (component as ReportComponentBase).Border.BottomLine.Style = UnitsConverter.ConvertBorderStyle(node.InnerText);
                        }
                    }
                }
            }
        }

        private void LoadBorderWidth(XmlNode borderWidthNode, string border)
        {
            XmlNodeList nodeList = borderWidthNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (border == "Default")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.Width = UnitsConverter.SizeToPixels(node.InnerText);
                    }
                }
                else if (border == "Top")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.TopLine.Width = UnitsConverter.SizeToPixels(node.InnerText);
                    }
                }
                else if (border == "Left")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.LeftLine.Width = UnitsConverter.SizeToPixels(node.InnerText);
                    }
                }
                else if (border == "Right")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.RightLine.Width = UnitsConverter.SizeToPixels(node.InnerText);
                    }
                }
                else if (border == "Bottom")
                {
                    if (component is ReportComponentBase)
                    {
                        (component as ReportComponentBase).Border.BottomLine.Width = UnitsConverter.SizeToPixels(node.InnerText);
                    }
                }
            }
        }

        private void LoadStyle(XmlNode styleNode)
        {
            FontStyle fontStyle = FontStyle.Regular;
            string fontFamily = "Arial";
            float fontSize = 10.0f;
            int paddingTop = 0;
            int paddingLeft = 2;
            int paddingRight = 2;
            int paddingBottom = 0;
            XmlNodeList nodeList = styleNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name.EndsWith("Border"))
                {
                    LoadBorder(node);
                }
                else if (node.Name == "BackgroundColor")
                {
                    if (component is ShapeObject)
                    {
                        (component as ShapeObject).FillColor = UnitsConverter.ConvertColor(node.InnerText);
                    }
                    else if (component is TableObject)
                    {
                        (component as TableObject).FillColor = UnitsConverter.ConvertColor(node.InnerText);
                    }
                }
                else if (node.Name == "FontStyle")
                {
                    fontStyle = UnitsConverter.ConvertFontStyle(node.InnerText);
                }
                else if (node.Name == "FontFamily")
                {
                    fontFamily = node.InnerText;
                }
                else if (node.Name == "FontSize")
                {
                    fontSize = Convert.ToSingle(UnitsConverter.ConvertFontSize(node.InnerText));
                }
                else if (node.Name == "TextAlign")
                {
                    if (component is TextObject)
                    {
                        (component as TextObject).HorzAlign = UnitsConverter.ConvertTextAlign(node.InnerText);
                    }
                }
                else if (node.Name == "VerticalAlign")
                {
                    if (component is TextObject)
                    {
                        (component as TextObject).VertAlign = UnitsConverter.ConvertVerticalAlign(node.InnerText);
                    }
                }
                else if (node.Name == "WritingMode")
                {
                    if (component is TextObject)
                    {
                        (component as TextObject).Angle = UnitsConverter.ConvertWritingMode(node.InnerText);
                    }
                }
                else if (node.Name == "Color")
                {
                    if (component is TextObject)
                    {
                        (component as TextObject).TextColor = UnitsConverter.ConvertColor(node.InnerText);
                    }
                }
                else if (node.Name == "PaddingLeft")
                {
                    paddingLeft = UnitsConverter.SizeToInt(node.InnerText, SizeUnits.Point);
                }
                else if (node.Name == "PaddingRight")
                {
                    paddingRight = UnitsConverter.SizeToInt(node.InnerText, SizeUnits.Point);
                }
                else if (node.Name == "PaddingTop")
                {
                    paddingTop = UnitsConverter.SizeToInt(node.InnerText, SizeUnits.Point);
                }
                else if (node.Name == "PaddingBottom")
                {
                    paddingBottom = UnitsConverter.SizeToInt(node.InnerText, SizeUnits.Point);
                }
            }
            if (component is TextObject)
            {
                (component as TextObject).Font = new Font(fontFamily, fontSize, fontStyle);
                (component as TextObject).Padding = new Padding(paddingLeft, paddingTop, paddingRight, paddingBottom);
            }
            else if (component is PictureObject)
            {
                (component as PictureObject).Padding = new Padding(paddingLeft, paddingTop, paddingRight, paddingBottom);
            }
        }

        private void LoadVisibility(XmlNode visibilityNode)
        {
            XmlNodeList nodeList = visibilityNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Hidden")
                {
                    component.Visible = !UnitsConverter.BooleanToBool(node.InnerText);
                }
            }
        }

        private void LoadPlotArea(XmlNode plotAreaNode)
        {
            XmlNodeList nodeList = plotAreaNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Style")
                {
                    LoadStyle(node);
                }
            }
        }

        private void LoadSubtotal(XmlNode subtotalNode)
        {
            XmlNodeList nodeList = subtotalNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "ReportItems")
                {
                    //LoadReportItems(node);
                }
                else if (node.Name == "Style")
                {
                    LoadStyle(node);
                }
            }
        }

        private void LoadReportItem(XmlNodeList nodeList)
        {
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Top")
                {
                    component.Top = UnitsConverter.SizeToPixels(node.InnerText);
                }
                else if (node.Name == "Left")
                {
                    component.Left = UnitsConverter.SizeToPixels(node.InnerText);
                }
                else if (node.Name == "Height")
                {
                    component.Height = UnitsConverter.SizeToPixels(node.InnerText);
                }
                else if (node.Name == "Width")
                {
                    component.Width = UnitsConverter.SizeToPixels(node.InnerText);
                }
                else if (node.Name == "Visibility")
                {
                    LoadVisibility(node);
                }
                else if (node.Name == "Style")
                {
                    LoadStyle(node);
                }
            }
            if (parent is TableCell)
            {
                component.Width = (parent as TableCell).Width;
                component.Height = (parent as TableCell).Height;
            }
        }

        private void LoadLine(XmlNode lineNode)
        {
            component = ComponentsFactory.CreateLineObject(lineNode.Attributes["Name"].Value, parent);
            XmlNodeList nodeList = lineNode.ChildNodes;
            LoadReportItem(nodeList);
        }

        private void LoadRectangle(XmlNode rectangleNode)
        {
            XmlNodeList nodeList = rectangleNode.ChildNodes;
            if (RectangleExistReportItem(nodeList))
            {
                LoadContainerRectangle(rectangleNode);
                return;
            }
            component = ComponentsFactory.CreateShapeObject(rectangleNode.Attributes["Name"].Value, parent);
            (component as ShapeObject).Shape = ShapeKind.Rectangle;
            LoadReportItem(nodeList);
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "ReportItems")
                {
                    LoadReportItems(node);
                }
            }
        }
        private void LoadContainerRectangle(XmlNode rectangleNode)
        {
            Base tempParent = parent;
            component = ComponentsFactory.CreateContainerObject(rectangleNode.Attributes["Name"].Value, parent);
            parent = component;
            XmlNodeList nodeList = rectangleNode.ChildNodes;
            (component as ContainerObject).Border.Lines = BorderLines.All;
            (component as ContainerObject).Border.Color = Color.Black;
            LoadReportItem(nodeList);
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "ReportItems")
                {
                    LoadReportItems(node);
                }
            }
            parent = tempParent;
        }

        private bool RectangleExistReportItem(XmlNodeList reportItemsNode)
        {
            foreach (XmlNode node in reportItemsNode)
            {
                if (node.Name == "ReportItems")
                    foreach (XmlNode itemNode in node)
                    {
                        switch (itemNode.Name)
                        {
                            case "Line":
                            case "Rectangle":
                            case "Textbox":
                            case "Image":
                            case "Subreport":
                            case "Chart":
                            case "Table":
                            case "Matrix":
                                return true;
                        }
                    }
            }
            return false;
        }

        private void LoadParagraphs(XmlNode paragraphs)
        {
            firstRun = true;
            foreach (XmlNode paragraph in paragraphs.ChildNodes)
            {
                if (paragraph.Name == "Paragraph")
                {
                    foreach (XmlNode parChild in paragraph.ChildNodes)
                    {
                        if (parChild.Name == "TextRuns")
                        {
                            foreach (XmlNode runsChild in parChild.ChildNodes)
                            {
                                ParseTextRun(runsChild);
                            }
                        }
                    }
                }
                if (firstRun)
                    firstRun = false;
            }
        }

        private void ParseTextRun(XmlNode runsChild)
        {
            if (runsChild.Name == "TextRun")
            {
                foreach (XmlNode runChild in runsChild.ChildNodes)
                {
                    if (runChild.Name == "Value")
                        ParseTextBoxValue(runChild);
                    else if (runChild.Name == "Style")
                        ParseTextBoxStyle(runChild);
                }
            }
        }

        private void ParseTextBoxValue(XmlNode runChild)
        {
            if (!firstRun)
                (component as TextObject).Text += "\r\n";
            (component as TextObject).Text += GetValue(runChild.InnerText);
        }

        private void ParseTextBoxStyle(XmlNode runChild)
        {
            FontStyle style = FontStyle.Regular;
            Color textBoxForeColor = Color.Black;
            string fontFamily = String.Empty;
            int fontSize = 0;
            foreach (XmlNode styleChild in runChild.ChildNodes)
            {
                if (styleChild.Name == "FontFamily")
                    fontFamily = styleChild.InnerText;
                else if (styleChild.Name == "FontSize")
                    int.TryParse(styleChild.InnerText.Replace("pt", ""), out fontSize);
                else if (styleChild.Name == "FontWeight" && styleChild.InnerText == "Bold")
                    style = style | FontStyle.Bold;
                else if (styleChild.Name == "FontStyle" && styleChild.InnerText == "Italic")
                    style = style | FontStyle.Italic;
                else if (styleChild.Name == "TextDecoration" && styleChild.InnerText == "Underline")
                    style = style | FontStyle.Underline;
                else if (styleChild.Name == "Color")
                    textBoxForeColor = ColorTranslator.FromHtml(styleChild.InnerText);

            }
            if (fontFamily == string.Empty)
                fontFamily = defaultFontFamily;
            if (fontFamily == string.Empty && fontSize == 0)
                (component as TextObject).Font = new Font((component as TextObject).Font, style);
            else if (fontFamily == string.Empty)
                (component as TextObject).Font = new Font((component as TextObject).Font.FontFamily, fontSize, style);
            else if (fontSize == 0)
                (component as TextObject).Font = new Font(fontFamily, (component as TextObject).Font.Size, style);
            else
                (component as TextObject).Font = new Font(fontFamily, fontSize, style);
            (component as TextObject).TextColor = textBoxForeColor;
        }

        private string GetValue(string rdlValue)
        {
            //=Fields!VATIdentifierCaption.Value
            string frExpression = "[";
            if (!string.IsNullOrEmpty(rdlValue) && rdlValue[0] == '=') //is expression
            {
                if (rdlValue.IndexOf("Fields") == 1) //is sumple data source
                {
                    frExpression += dataSetName + ".";

                    int fieldStart = rdlValue.IndexOf("!") + 1;
                    int fieldEnd = rdlValue.Substring(fieldStart).IndexOf(".") - 1;
                    frExpression += rdlValue.Substring(fieldStart, fieldEnd + 1);
                    frExpression += "]";
                    return frExpression;
                }
            }
            return rdlValue;
        }

        private void LoadTextbox(XmlNode textboxNode)
        {
            if (parent is TableCell)
                component = (TableCell)parent;
            else
                component = ComponentsFactory.CreateTextObject(textboxNode.Attributes["Name"].Value, parent);
            XmlNodeList nodeList = textboxNode.ChildNodes;
            LoadReportItem(nodeList);
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "CanGrow")
                {
                    (component as TextObject).CanGrow = UnitsConverter.BooleanToBool(node.InnerText);
                }
                else if (node.Name == "CanShrink")
                {
                    (component as TextObject).CanShrink = UnitsConverter.BooleanToBool(node.InnerText);
                }
                else if (node.Name == "HideDuplicates")
                {
                    (component as TextObject).Duplicates = Duplicates.Hide;
                }
                else if (node.Name == "Value")
                {
                    (component as TextObject).Text = node.InnerText;
                }
                else if (node.Name == "Paragraphs")
                {
                    LoadParagraphs(node);
                }
            }
        }

        private void LoadImage(XmlNode imageNode)
        {
            component = ComponentsFactory.CreatePictureObject(imageNode.Attributes["Name"].Value, parent);
            XmlNodeList nodeList = imageNode.ChildNodes;
            LoadReportItem(nodeList);
            string name = String.Empty;
            foreach (XmlNode node in nodeList)
            {
                //if (node.Name == "Source")
                //{
                //}
                /*else */
                if (node.Name == "Value")
                {
                    if (File.Exists(node.InnerText))
                    {
                        (component as PictureObject).ImageLocation = node.InnerText;
                    }
                }
                else if (node.Name == "Sizing")
                {
                    (component as PictureObject).SizeMode = UnitsConverter.ConvertSizing(node.InnerText);
                }
            }
        }

        private void LoadSubreport(XmlNode subreportNode)
        {
            component = ComponentsFactory.CreateSubreportObject(subreportNode.Attributes["Name"].Value, parent);
            ReportPage subPage = ComponentsFactory.CreateReportPage(Report);
            (component as SubreportObject).ReportPage = subPage;
            XmlNodeList nodeList = subreportNode.ChildNodes;

            string path = Path.GetDirectoryName(filename);
            string reportName = String.Empty;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "ReportName")
                    reportName = node.InnerText;
            }
            string subFilename = Path.Combine(path, reportName + ".rdl");
            if (!File.Exists(subFilename))
            {
                subFilename = Path.Combine(path, reportName + ".rdlc");
                if (!File.Exists(subFilename))
                    subFilename = String.Empty;
            }

            if (!String.IsNullOrEmpty(subFilename))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(subFilename);
                reportNode = doc.LastChild;
                ReportPage tempPage = page;
                page = subPage;
                LoadReport(reportNode);
                page = tempPage;
            }
            else
            {
                DataBand subBand = ComponentsFactory.CreateDataBand(subPage);
                subBand.Height = 2.0f * Utils.Units.Centimeters;
                LoadReportItem(nodeList);
            }
        }

        partial void LoadChart(XmlNode chartNode);

        private void LoadReportItems(XmlNode reportItemsNode)
        {
            XmlNodeList nodeList = reportItemsNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Line")
                {
                    LoadLine(node);
                }
                else if (node.Name == "Rectangle")
                {
                    LoadRectangle(node);
                }
                else if (node.Name == "Textbox")
                {
                    LoadTextbox(node);
                }
                else if (node.Name == "Image")
                {
                    LoadImage(node);
                }
                else if (node.Name == "Subreport")
                {
                    LoadSubreport(node);
                }
                else if (node.Name == "Chart")
                {
                    LoadChart(node);
                }
                else if (node.Name == "Table" || (node.Name == "Tablix" && !IsTablixMatrix(node)))
                {
                    LoadTable(node);
                }
                else if (node.Name == "Matrix" || (node.Name == "Tablix" && IsTablixMatrix(node)))
                {
                    LoadMatrix(node);
                }
            }
        }

        private void LoadBody(XmlNode bodyNode)
        {
            parent = ComponentsFactory.CreateDataBand(page);
            curBand = (BandBase)parent;
            XmlNodeList nodeList = bodyNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "ReportItems")
                {
                    LoadReportItems(node);
                }
                else if (node.Name == "Height")
                {
                    (parent as DataBand).Height = UnitsConverter.SizeToPixels(node.InnerText);
                }
            }
        }

        private void LoadPageSection(XmlNode pageSectionNode)
        {
            if (pageSectionNode.Name == "PageHeader")
            {
                page.PageHeader = new PageHeaderBand();
                page.PageHeader.CreateUniqueName();
                page.PageHeader.PrintOn = PrintOn.EvenPages | PrintOn.OddPages | PrintOn.RepeatedBand;
                XmlNodeList nodeList = pageSectionNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name == "Height")
                    {
                        page.PageHeader.Height = UnitsConverter.SizeToPixels(node.InnerText);
                    }
                    else if (node.Name == "PrintOnFirstPage")
                    {
                        if (node.InnerText == "true")
                        {
                            page.PageHeader.PrintOn |= PrintOn.FirstPage;
                        }
                    }
                    else if (node.Name == "PrintOnLastPage")
                    {
                        if (node.InnerText == "true")
                        {
                            page.PageHeader.PrintOn |= PrintOn.LastPage;
                        }
                    }
                    else if (node.Name == "ReportItems")
                    {
                        parent = page.PageHeader;
                        LoadReportItems(node);
                    }
                }
            }
            else if (pageSectionNode.Name == "PageFooter")
            {
                page.PageFooter = new PageFooterBand();
                page.PageFooter.CreateUniqueName();
                page.PageFooter.PrintOn = PrintOn.EvenPages | PrintOn.OddPages | PrintOn.RepeatedBand;
                XmlNodeList nodeList = pageSectionNode.ChildNodes;
                foreach (XmlNode node in nodeList)
                {
                    if (node.Name == "Height")
                    {
                        page.PageFooter.Height = UnitsConverter.SizeToPixels(node.InnerText);
                    }
                    else if (node.Name == "PrintOnFirstPage")
                    {
                        if (node.InnerText == "true")
                        {
                            page.PageFooter.PrintOn |= PrintOn.FirstPage;
                        }
                    }
                    else if (node.Name == "PrintOnLastPage")
                    {
                        if (node.InnerText == "true")
                        {
                            page.PageFooter.PrintOn |= PrintOn.LastPage;
                        }
                    }
                    else if (node.Name == "ReportItems")
                    {
                        parent = page.PageFooter;
                        LoadReportItems(node);
                    }
                }
            }
        }

        private void LoadPage(XmlNode pageNode)
        {
            XmlNodeList nodeList = pageNode.ChildNodes;
            bool pageWidthLoaded = false;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "PageHeader")
                {
                    LoadPageSection(node);
                }
                else if (node.Name == "PageFooter")
                {
                    LoadPageSection(node);
                }
                else if (node.Name == "PageHeight")
                {
                    page.PaperHeight = UnitsConverter.SizeToMillimeters(node.InnerText);
                }
                else if (node.Name == "PageWidth")
                {
                    page.PaperWidth = UnitsConverter.SizeToMillimeters(node.InnerText);
                    pageWidthLoaded = true;
                }
                else if (node.Name == "LeftMargin")
                {
                    page.LeftMargin = UnitsConverter.SizeToMillimeters(node.InnerText);
                }
                else if (node.Name == "RightMargin")
                {
                    page.RightMargin = UnitsConverter.SizeToMillimeters(node.InnerText);
                }
                else if (node.Name == "TopMargin")
                {
                    page.TopMargin = UnitsConverter.SizeToMillimeters(node.InnerText);
                }
                else if (node.Name == "BottomMargin")
                {
                    page.BottomMargin = UnitsConverter.SizeToMillimeters(node.InnerText);
                }
                else if (node.Name == "Style")
                {
                    LoadStyle(node);
                }
            }

            if (!pageWidthLoaded && sectionWidth > 0)
            {
                page.PaperWidth = page.LeftMargin + sectionWidth + page.RightMargin;
            }
        }

        private void LoadParameters(XmlNode parametersNode)
        {
            foreach (XmlNode node in parametersNode.ChildNodes)
            {
                Parameter parameter = ComponentsFactory.CreateParameter(node.Attributes["Name"].Value, Report);
                foreach (XmlNode subNode in node.ChildNodes)
                {
                    if (subNode.Name == "DataType")
                    {
                        Type type = Type.GetType("System." + subNode.InnerText);
                        if (type != null)
                            parameter.DataType = type;
                        else if (subNode.InnerText == "Integer")
                            parameter.DataType = typeof(Int32);
                        else if (subNode.InnerText == "Float")
                            parameter.DataType = typeof(float);
                    }
                    else if (subNode.Name == "Prompt")
                    {
                        parameter.Description = subNode.InnerText;
                    }
                    else if (subNode.Name == "DefaultValue")
                    {
                    }
                }
            }
        }

        private XmlNode GetEmbededImageNode(string objectName)
        {
            foreach (XmlNode node in reportNode.ChildNodes)
            {
                if (node.Name == "EmbeddedImages")
                {
                    foreach (XmlNode embededImage in node.ChildNodes)
                    {
                        if (embededImage.Attributes[0].InnerText == objectName)
                            return embededImage;
                    }
                }
            }
            return null;
        }

        private void LoadReport(XmlNode reportNode)
        {
            int pageNbr = 0;
            XmlNodeList nodeList = reportNode.ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "Description")
                {
                    Report.ReportInfo.Description = node.InnerText;
                }
                else if (node.Name == "Author")
                {
                    Report.ReportInfo.Author = node.InnerText;
                }
                else if (node.Name == "Body")
                {
                    if (page == null)
                        page = ComponentsFactory.CreateReportPage(Report);
                    LoadBody(node);
                }
                else if (node.Name == "Page")
                {
                    if (pageNbr > 0)
                        page = ComponentsFactory.CreateReportPage(Report);
                    LoadPage(node);
                    pageNbr++;
                }
                else if (node.Name == "ReportSections")
                {
                    if (node.HasChildNodes && node.FirstChild.Name == "ReportSection")
                        foreach (XmlNode sectionItem in node.FirstChild)
                        {
                            if (sectionItem.Name == "Body")
                            {
                                if (page == null)
                                    page = ComponentsFactory.CreateReportPage(Report);
                                LoadBody(sectionItem);
                            }
                            else if (sectionItem.Name == "Page")
                            {
                                if (pageNbr > 0)
                                    page = ComponentsFactory.CreateReportPage(Report);
                                LoadPage(sectionItem);
                                pageNbr++;
                            }
                            else if (sectionItem.Name == "Width")
                            {
                                sectionWidth = UnitsConverter.SizeToMillimeters(sectionItem.InnerText);
                            }
                        }
                }
                else if (node.Name == "df:DefaultFontFamily")
                {
                    defaultFontFamily = node.InnerText;
                }
                else if (node.Name == "DataSets")
                {
                    if (node.HasChildNodes)
                        dataSetName = node.FirstChild.Attributes["Name"].Value;
                }
                else if (node.Name == "ReportParameters")
                {
                    if (node.HasChildNodes)
                        LoadParameters(node);
                }
            }
            if (page == null)
            {
                page = ComponentsFactory.CreateReportPage(Report);
                LoadPage(reportNode);
            }
        }

        #endregion // Private Methods

        #region Public Methods

        /// <inheritdoc/>
        public override void LoadReport(Report report, string filename)
        {
            this.filename = filename;
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                LoadReport(report, fs);
            }
        }
        /// <inheritdoc />
        public override void LoadReport(Report report, Stream content)
        {
            Report = report;
            Report.Clear();
            XmlDocument document = new XmlDocument();
            document.Load(content);
            reportNode = document.LastChild;
            defaultFontFamily = "";
            page = null;
            LoadReport(reportNode);
        }

        #endregion // Public Methods
    }
}
