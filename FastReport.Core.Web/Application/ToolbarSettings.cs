using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Drawing;
using System.Globalization;

namespace FastReport.Web
{
    public class ToolbarSettings
    {

        public static ToolbarSettings Default => new ToolbarSettings();

        public bool Show { get; set; } = true;

#if DIALOGS
        public bool ShowOnDialogPage { get; set; } = true;
#endif


        [Obsolete("Please, use Position")]
        public bool ShowBottomToolbar
        {
            get => Position == Positions.Bottom;
            set
            {
                if (value)
                {
                    Position = Positions.Bottom;
                }
                else
                    Position = default;
            }
        }

        /// <summary>
        /// ExportMenu settings
        /// </summary>
        public ExportMenuSettings Exports { get; set; } = ExportMenuSettings.Default;

        public bool ShowPrevButton { get; set; } = true;
        public bool ShowNextButton { get; set; } = true;
        public bool ShowFirstButton { get; set; } = true;
        public bool ShowLastButton { get; set; } = true;
        public bool ShowRefreshButton { get; set; } = true;
        public bool ShowZoomButton { get; set; } = true;
        public bool ShowPrint { get; set; } = true;
        public bool PrintInHtml { get; set; } = true;
#if !OPENSOURCE
        public bool PrintInPdf { get; set; } = true;
#endif
        /// <summary>
        /// Use to change ToolbarColor,
        /// Default value Color.LightGray
        /// </summary>
        public Color Color { get; set; } = Color.LightGray;
        /// <summary>
        /// Use to change Toolbar DropDownMenuColor,
        /// Default value Color.White
        /// </summary>
        public Color DropDownMenuColor { get; set; } = Color.White;
        /// <summary>
        /// Use to change Toolbar DropDownMenuText Color,
        /// Default value Color.Black
        /// </summary>
        public Color DropDownMenuTextColor { get; set; } = Color.Black;
        /// <summary>
        /// Use to change Toolbar Position in report,
        /// Default value Position.Top
        /// </summary>
        public Positions Position { get; set; } = Positions.Top;
        /// <summary>
        /// Use to add Roundness to Toolbar,
        /// Default value RoundnessEnum.None
        /// </summary>
        public RoundnessEnum Roundness { get; set; } = RoundnessEnum.None;
        /// <summary>
        /// Use to change content position in Toolbar,
        /// Default value ContentPositions.Left
        /// </summary>
        public ContentPositions ContentPosition { get; set; } = ContentPositions.Left;
        /// <summary>
        /// Use to change Icons color in Toolbar,
        /// Default value IconColors.Black
        /// </summary>
        public IconColors IconColor { get; set; } = IconColors.Black;
        /// <summary>
        /// Use to add Transparency in icon Toolbar,
        /// Default value IconTransparencyEnum.Default
        /// </summary>
        public IconTransparencyEnum IconTransperency { get; set; } = IconTransparencyEnum.Default;
        /// <summary>
        /// Use to change Font in Toolbar,
        /// Default value null
        /// <para>Example syntax : new Font("Arial", 14 , FontStyle.Bold)</para>
        /// </summary>
        public Font FontSettings { get; set; } = null;

        public int Height { get; set; } = 40;

        internal int ToolbarSlash
        {
            get
            {
                switch (Position)
                {
                    case Positions.Left:
                    case Positions.Right:
                        return 90;
                    default:
                        return 20;
                }
            }
        }
        internal int ToolbarNarrow
        {
            get
            {
                switch (Position)
                {
                    case Positions.Left:
                    case Positions.Right:
                        return 90;
                    default:
                        return 0;
                }
            }
        }

        internal string UserFontSettings
        {
            get
            {
                if (FontSettings != null)
                {
                    return FontSettings.Size + "em " + FontSettings.OriginalFontName + " " + FontSettings.Style;
                }
                else
                    return "15em Verdana,Arial sans-serif Regular";
            }
        }

        
        internal string VerticalToolbarHeight
        {
            get
            {
                switch (Position)
                {
                    case Positions.Left:
                    case Positions.Right:
                        return (Height * 11f).ToString("0.##", CultureInfo.InvariantCulture);
                    default:
                        return Height.ToString();
                }
            }
        }
        
        internal int TopOrBottom
        {
            get
            {
                switch (Position)
                {
                    case Positions.Bottom:
                        return 1;
                    default:
                        return -1;
                }
            }
        }
        internal string Vertical
        {
            get
            {

                switch (Position)
                {
                    case Positions.Right:
                        return "row-reverse";
                    case Positions.Left:
                        return "row";
                    default:
                        return "column";
                }
            }

        }
        internal int DropDownMenuPositionLeft
        {
            get
            {
                switch (Position)
                {
                    case Positions.Left:
                    case Positions.Right:
                        return Height+10;
                    default:
                        return 0;
                }
            }
        }
        internal int DropDownMenuPositionTops
        {
            get
            {
                switch (Position)
                {
                    case Positions.Right:
                    case Positions.Left:
                        return 7;
                    default:
                        return 0;
                }
            }
        }
        internal string RowOrColumn
        {
            get
            {
                switch (Position)
                {
                    case Positions.Left:
                    case Positions.Right:
                        return "column";
                    default:
                        return "row";
                }
            }
        }
        internal string Content
        {
            get
            {
                switch (ContentPosition)
                {
                    case ContentPositions.Center:
                        return "center";
                    case ContentPositions.Right:
                        return "flex-end";
                    default:
                        return "flex-start";
                }
            }
        }
        internal string DropDownMenuPosition
        {
            get
            {
                switch (Position)
                {
                    case Positions.Left:
                        return "left:" + Height + "px;" + "text-align:left;" + "top:8px;";
                    case Positions.Right:
                        return "right:" + Height + "px;" + "text-align:right;" + "top:8px;";
                    case Positions.Bottom:
                        return "bottom:" + Height + "px;";
                    default:
                        return "top:" + Height + "px;";
                }
            }
        }
        internal string TabsPositionSettings
        {
            get
            {
                switch (Position)
                {
                    case Positions.Left:
                        return "179px";
                    case Positions.Right:
                        return "179px";
                    case Positions.Bottom:
                        return "order:1;";
                    default:
                        return "auto";
                }
            }
        }
        internal int ToolbarRoundness
        {
            get
            {
                switch (Roundness)
                {
                    case RoundnessEnum.High:
                        return 10;
                    case RoundnessEnum.Medium:
                        return 5;
                    case RoundnessEnum.Low:
                        return 3;
                    case RoundnessEnum.None:
                    default:
                        return 0;
                }
            }
        }
        internal int ColorIcon
        {
            get
            {
                switch (IconColor)
                {
                    case IconColors.White:
                        return 1;
                    default:
                        return 0;
                }
            }
        }
        internal string TransparencyIcon
        {
            get
            {
                switch (IconTransperency)
                {
                    case IconTransparencyEnum.Low:
                        return "0.9";
                    case IconTransparencyEnum.Medium:
                        return "0.7";
                    case IconTransparencyEnum.Default:
                        return "0.5";
                    default:
                        return "0.5";
                }
            }
        }
       
    }
    public enum IconColors
    {
        White, Black
    }
    public enum IconTransparencyEnum
    {
        Low, Medium, High, Default
    }
    public enum Positions
    {
        Top = 0,
        Bottom,
        Right,
        Left,
    }
    public enum ContentPositions
    {
        Left,
        Right,
        Center,
    }
    public enum RoundnessEnum
    {
        Low, Medium, High, None
    }

}
