using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Utils;
using System.Linq;

namespace FastReport.Import.ListAndLabel
{
    /// <summary>
    /// Represents the List and Label import plugin.
    /// </summary>
    public class ListAndLabelImport : ImportBase
    {
        #region Fields

        private ReportPage page;
        private string textLL;
        private Font defaultFont;
        private Color defaultTextColor;
        private bool isListAndLabelReport;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets the value indicating is the report List and Label template after trying to load it. 
        /// </summary>
        public bool IsListAndLabelReport
        {
            get { return isListAndLabelReport; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ListAndLabelImport"/> class.
        /// </summary>
        public ListAndLabelImport() : base()
        {
            textLL = "";
            defaultFont = new Font("Arial", 10.0f, FontStyle.Regular);
            defaultTextColor = Color.Black;
            isListAndLabelReport = true;
        }

        #endregion // Constructors

        #region Private Methods

        private string GetValueLL(string str)
        {
            int index = textLL.IndexOf(str) + str.Length + 1;
            int length = textLL.IndexOf("\r\n", index) - index;
            return textLL.Substring(index, length);
        }

        private string GetValueLL(string str, int startIndex)
        {
            int index = textLL.IndexOf(str, startIndex) + str.Length + 1;
            int length = textLL.IndexOf("\r\n", index) - index;
            return textLL.Substring(index, length);
        }

        private string RemoveQuotes(string str)
        {
            return str.Replace("\"", "");
        }

        private void LoadReportInfo()
        {
            int index = textLL.IndexOf("Text=", textLL.IndexOf("[Description]")) + 5;
            int length = textLL.IndexOf("\r\n", index) - index;
            Report.ReportInfo.Description = textLL.Substring(index, length);
        }

        private void LoadPageSettings()
        {
            page.PaperWidth = UnitsConverter.LLUnitsToMillimeters(GetValueLL("PaperFormat.cx"));
            page.PaperHeight = UnitsConverter.LLUnitsToMillimeters(GetValueLL("PaperFormat.cy"));
            page.Landscape = UnitsConverter.ConvertPaperOrientation(GetValueLL("PaperFormat.Orientation"));
            page.TopMargin = page.LeftMargin = page.RightMargin = page.BottomMargin = 0.0f;
        }

        private void LoadDefaultFont()
        {
            string defFontStr = GetValueLL("DefFont=");
            string[] defFontParts = defFontStr.Split(',');
            defFontParts[0] = defFontParts[0][1].ToString();
            defFontParts[2] = defFontParts[2][0].ToString();
            defaultTextColor = Color.FromArgb(int.Parse(defFontParts[0]), int.Parse(defFontParts[1]), int.Parse(defFontParts[2]));
            float fontsize = Convert.ToSingle(defFontParts[3].Replace('.', ','));
            defaultFont = new Font(defFontParts.Last().Trim('}'), fontsize, FontStyle.Regular);
            return;
            if (UnitsConverter.ConvertBool(GetValueLL("DefaultFont/Default")))
            {

                string fontFamily = GetValueLL("DefaultFont/FaceName");
                float fontSize = DrawUtils.DefaultReportFont.Size;
                try
                {
                    fontSize = Convert.ToSingle(GetValueLL("DefaultFont/Size"));
                }
                catch
                {
                    fontSize = DrawUtils.DefaultReportFont.Size;
                }
                FontStyle fontStyle = FontStyle.Regular;
                if (UnitsConverter.ConvertBool(GetValueLL("DefaultFont/Bold")))
                {
                    fontStyle |= FontStyle.Bold;
                }
                if (UnitsConverter.ConvertBool(GetValueLL("DefaultFont/Italic")))
                {
                    fontStyle |= FontStyle.Italic;
                }
                if (UnitsConverter.ConvertBool(GetValueLL("DefaultFont/Underline")))
                {
                    fontStyle |= FontStyle.Underline;
                }
                if (UnitsConverter.ConvertBool(GetValueLL("DefaultFont/Strikeout")))
                {
                    fontStyle |= FontStyle.Strikeout;
                }
                defaultFont = new Font(fontFamily, fontSize, fontStyle);
                defaultTextColor = Color.FromName(GetValueLL("DefaultFont/Color=LL.Color"));
            }
        }

        private List<int> GetAllObjectsLL()
        {
            List<int> list = new List<int>();
            int firstIndex = textLL.IndexOf("[Object]");
            int lastIndex = textLL.LastIndexOf("[Object]");
            int currentIndex = firstIndex;
            if (currentIndex >= 0)
            {
                do
                {
                    list.Add(currentIndex);
                    currentIndex = textLL.IndexOf("[Object]", currentIndex + 1);
                }
                while (currentIndex < lastIndex);
            }
            if (firstIndex != lastIndex)
            {
                list.Add(lastIndex);
            }
            return list;
        }

        private void LoadComponent(int startIndex, ComponentBase comp)
        {
            try
            {
                comp.Name = GetValueLL("Identifier", startIndex);
                if (String.IsNullOrEmpty(comp.Name))
                {
                    comp.CreateUniqueName();
                }
            }
            catch (DuplicateNameException)
            {
                comp.CreateUniqueName();
            }

            comp.Left = UnitsConverter.LLUnitsToPixels(GetValueLL("Position/Left", startIndex));
            comp.Top = UnitsConverter.LLUnitsToPixels(GetValueLL("Position/Top", startIndex));
            comp.Width = UnitsConverter.LLUnitsToPixels(GetValueLL("Position/Width", startIndex));
            comp.Height = UnitsConverter.LLUnitsToPixels(GetValueLL("Position/Height", startIndex));
        }

        private Font LoadFont(int startIndex)
        {
            int index = textLL.IndexOf("[Font]", startIndex);
            //if (!UnitsConverter.ConvertBool(GetValueLL("Default", index)))
            //{
            string fontFamily = RemoveQuotes(GetValueLL("FaceName", index));
            float fontSize = defaultFont.Size;
            if (GetValueLL("Size", index) != "Null()")
                fontSize = Convert.ToSingle(GetValueLL("Size", index).Replace('.', ','));
            FontStyle fontStyle = FontStyle.Regular;
            if (UnitsConverter.ConvertBool(GetValueLL("Bold", index)))
            {
                fontStyle |= FontStyle.Bold;
            }
            if (UnitsConverter.ConvertBool(GetValueLL("Italic", index)))
            {
                fontStyle |= FontStyle.Italic;
            }
            if (UnitsConverter.ConvertBool(GetValueLL("Underline", index)))
            {
                fontStyle |= FontStyle.Underline;
            }
            if (UnitsConverter.ConvertBool(GetValueLL("Strikeout", index)))
            {
                fontStyle |= FontStyle.Strikeout;
            }
            return new Font(fontFamily == "Null()" ? defaultFont.FontFamily.Name : fontFamily, fontSize, fontStyle);
            //}
        }

        private void LoadBorder(int startIndex, Border border)
        {
            if (UnitsConverter.ConvertBool(GetValueLL("Frame/Left/Line", startIndex)))
            {
                border.Lines |= BorderLines.Left;
                border.LeftLine.Color = Color.FromName(GetValueLL("Frame/Left/Line/Color=LL.Color", startIndex));
                border.LeftLine.Style = UnitsConverter.ConvertLineType(GetValueLL("Frame/Left/Line/LineType", startIndex));
                border.LeftLine.Width = UnitsConverter.LLUnitsToPixels(GetValueLL("Frame/Left/LineWidth", startIndex));
            }
            if (UnitsConverter.ConvertBool(GetValueLL("Frame/Top/Line", startIndex)))
            {
                border.Lines |= BorderLines.Top;
                border.TopLine.Color = Color.FromName(GetValueLL("Frame/Top/Line/Color=LL.Color", startIndex));
                border.TopLine.Style = UnitsConverter.ConvertLineType(GetValueLL("Frame/Top/Line/LineType", startIndex));
                border.TopLine.Width = UnitsConverter.LLUnitsToPixels(GetValueLL("Frame/Top/LineWidth", startIndex));
            }
            if (UnitsConverter.ConvertBool(GetValueLL("Frame/Right/Line", startIndex)))
            {
                border.Lines |= BorderLines.Right;
                border.RightLine.Color = Color.FromName(GetValueLL("Frame/Right/Line/Color=LL.Color", startIndex));
                border.RightLine.Style = UnitsConverter.ConvertLineType(GetValueLL("Frame/Right/Line/LineType", startIndex));
                border.RightLine.Width = UnitsConverter.LLUnitsToPixels(GetValueLL("Frame/Right/LineWidth", startIndex));
            }
            if (UnitsConverter.ConvertBool(GetValueLL("Frame/Bottom/Line", startIndex)))
            {
                border.Lines |= BorderLines.Bottom;
                border.BottomLine.Color = Color.FromName(GetValueLL("Frame/Bottom/Line/Color=LL.Color", startIndex));
                border.BottomLine.Style = UnitsConverter.ConvertLineType(GetValueLL("Frame/Bottom/Line/LineType", startIndex));
                border.BottomLine.Width = UnitsConverter.LLUnitsToPixels(GetValueLL("Frame/Bottom/LineWidth", startIndex));
            }
        }

        private void LoadTextObject(int startIndex, TextObject textObj)
        {
            // It can be an object without font and text. In list and labels it looks like a gray background
            if (textLL.IndexOf("[Font]", startIndex) == -1)
                return;
            LoadComponent(startIndex, textObj);
            textObj.Font = LoadFont(startIndex);
            textObj.TextColor = defaultTextColor;
            int fontIndex = textLL.IndexOf("[Font]", startIndex);
            if (GetValueLL("Color", fontIndex) != "Null()")
            {
                textObj.TextColor = Color.FromName(GetValueLL("Color=LL.Color", fontIndex));
            }
            //if (!UnitsConverter.ConvertBool(GetValueLL("Default", fontIndex)))
            //{
            //    textObj.TextColor = Color.FromName(GetValueLL("Color=LL.Color.", fontIndex));
            //}
            textObj.HorzAlign = UnitsConverter.ConvertTextAlign(GetValueLL("Align", fontIndex));
            textObj.Text = RemoveQuotes(GetValueLL("Text", fontIndex));
            LoadBorder(startIndex, textObj.Border);
        }

        private void LoadLineObject(int startIndex, LineObject lineObj)
        {
            LoadComponent(startIndex, lineObj);
            int colorIndex = textLL.IndexOf("FgColor", startIndex);
            if (colorIndex >= 0)
            {
                lineObj.Border.Color = Color.FromName(GetValueLL("FgColor=LL.Color", colorIndex));
                lineObj.Border.Style = UnitsConverter.ConvertLineType(GetValueLL("LineType", colorIndex));
                lineObj.Border.Width = UnitsConverter.LLUnitsToPixels(GetValueLL("Width", colorIndex));
            }
            if (lineObj.Width > 0 || lineObj.Height > 0)
            {
                lineObj.Diagonal = true;
            }
            LoadBorder(startIndex, lineObj.Border);
        }

        private void LoadShapeObject(int startIndex, ShapeObject shapeObj)
        {
            LoadComponent(startIndex, shapeObj);
            int colorIndex = textLL.IndexOf("FgColor", startIndex);
            if (colorIndex >= 0)
            {
                shapeObj.Border.Color = GetColorForShapeObject("FgColor", colorIndex);//Color.FromName(GetValueLL("FgColor=LL.Color", colorIndex));
                shapeObj.Border.Width = UnitsConverter.LLUnitsToPixels(GetValueLL("Width", colorIndex));
                shapeObj.Border.Style = UnitsConverter.ConvertLineType(GetValueLL("LineType", colorIndex));
                shapeObj.FillColor = GetColorForShapeObject("BkColor", colorIndex);
            }
        }

        private Color GetColorForShapeObject(string colorString, int colorIndex)
        {
            string colorName = GetValueLL(colorString + "=LL.Color", colorIndex);
            Color color = Color.FromName(colorName);
            if (color.IsNamedColor)
                return color;
            colorName = GetValueLL(colorString, colorIndex);
            string[] colors = colorName.Replace("RGB", "").Replace("(", "").Replace(")", "").Split(',');
            if (colors.Length != 3)
                return Color.Transparent;
            color = Color.FromArgb(int.Parse(colors[0]), int.Parse(colors[1]), int.Parse(colors[2]));
            return color;
        }

        private void LoadRectangle(int startIndex, ShapeObject shapeObj)
        {
            LoadShapeObject(startIndex, shapeObj);
            float curve = UnitsConverter.ConvertRounding(GetValueLL("Rounding", startIndex));
            if (curve == 0)
            {
                shapeObj.Shape = ShapeKind.Rectangle;
            }
            else
            {
                shapeObj.Shape = ShapeKind.RoundRectangle;
                shapeObj.Curve = curve;
            }
        }

        private void LoadEllipse(int startIndex, ShapeObject shapeObj)
        {
            LoadShapeObject(startIndex, shapeObj);
            shapeObj.Shape = ShapeKind.Ellipse;
        }

        private void LoadPictureObject(int startIndex, PictureObject pictureObj)
        {
            LoadComponent(startIndex, pictureObj);
            if (UnitsConverter.ConvertBool(GetValueLL("OriginalSize", startIndex)))
            {
                pictureObj.SizeMode = PictureBoxSizeMode.Normal;
            }
            if (Convert.ToInt32(GetValueLL("Alignment", startIndex)) == 0)
            {
                pictureObj.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            if (UnitsConverter.ConvertBool(GetValueLL("bIsotropic", startIndex)))
            {
                pictureObj.SizeMode = PictureBoxSizeMode.AutoSize;
            }
            else
            {
                pictureObj.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            string filename = GetValueLL("Filename", startIndex);
            if (filename.Equals("<embedded>"))
            {
                // cant find an encoding that use l&l for store images
            }
            else if (!String.IsNullOrEmpty(filename))
            {
                pictureObj.ImageLocation = filename;
            }
            LoadBorder(startIndex, pictureObj.Border);
        }

        private void LoadObjects()
        {
            DataBand band = ComponentsFactory.CreateDataBand(page);
            band.Height = page.PaperHeight * Units.Millimeters;
            List<int> objects = GetAllObjectsLL();
            foreach (int index in objects)
            {
                string objectName = GetValueLL("ObjectName", index);
                switch (objectName)
                {
                    case "Text":
                        TextObject textObj = ComponentsFactory.CreateTextObject("", band);
                        LoadTextObject(index, textObj);
                        break;
                    case "Line":
                        LineObject lineObj = ComponentsFactory.CreateLineObject("", band);
                        LoadLineObject(index, lineObj);
                        break;
                    case "Rectangle":
                        ShapeObject rectangle = ComponentsFactory.CreateShapeObject("", band);
                        LoadRectangle(index, rectangle);
                        break;
                    case "Ellipse":
                        ShapeObject ellipse = ComponentsFactory.CreateShapeObject("", band);
                        LoadEllipse(index, ellipse);
                        break;
                    case "Picture":
                        PictureObject pictureObj = ComponentsFactory.CreatePictureObject("", band);
                        LoadPictureObject(index, pictureObj);
                        break;
                }
            }
        }

        private bool CheckIsListAndLabelReport()
        {
            if (!String.IsNullOrEmpty(textLL) && textLL.IndexOf("[Description]") != -1)
            {
                return true;
            }
            return false;
        }

        private void LoadReport()
        {
            page = ComponentsFactory.CreateReportPage(Report);
            LoadReportInfo();
            LoadPageSettings();
            LoadDefaultFont();
            LoadObjects();
        }

        #endregion // Private Methods

        #region Public Methods

        ///<inheritdoc/>
        public override void LoadReport(Report report, string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                LoadReport(report, fs);
            }
        }

        /// <inheritdoc />
        public override void LoadReport(Report report, Stream content)
        {
            using (var sr = new StreamReader(content))
            {
                textLL = sr.ReadToEnd();
            }
            isListAndLabelReport = CheckIsListAndLabelReport();
            if (isListAndLabelReport)
            {
                Report = report;
                Report.Clear();
                LoadReport();
            }
            page = null;
        }

        #endregion // Public Methods
    }
}
