using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace System.Drawing
{
    internal class ColorTranslator
    {

        private static Hashtable htmlSysColorTable;


        public static string ToHtml(Color c)
        {
            string colorString = String.Empty;

            if (c.IsEmpty)
                return colorString;
            bool flag = true;
            if (ColorExt.IsKnownColor(c))
            {
                flag = false;
                switch (ColorExt.ToKnownColor(c))
                {
                    case KnownColor.ActiveBorder: colorString = "activeborder"; break;
                    case KnownColor.GradientActiveCaption:
                    case KnownColor.ActiveCaption: colorString = "activecaption"; break;
                    case KnownColor.AppWorkspace: colorString = "appworkspace"; break;
                    case KnownColor.Desktop: colorString = "background"; break;
                    case KnownColor.Control: colorString = "buttonface"; break;
                    case KnownColor.ControlLight: colorString = "buttonface"; break;
                    case KnownColor.ControlDark: colorString = "buttonshadow"; break;
                    case KnownColor.ControlText: colorString = "buttontext"; break;
                    case KnownColor.ActiveCaptionText: colorString = "captiontext"; break;
                    case KnownColor.GrayText: colorString = "graytext"; break;
                    case KnownColor.HotTrack:
                    case KnownColor.Highlight: colorString = "highlight"; break;
                    case KnownColor.MenuHighlight:
                    case KnownColor.HighlightText: colorString = "highlighttext"; break;
                    case KnownColor.InactiveBorder: colorString = "inactiveborder"; break;
                    case KnownColor.GradientInactiveCaption:
                    case KnownColor.InactiveCaption: colorString = "inactivecaption"; break;
                    case KnownColor.InactiveCaptionText: colorString = "inactivecaptiontext"; break;
                    case KnownColor.Info: colorString = "infobackground"; break;
                    case KnownColor.InfoText: colorString = "infotext"; break;
                    case KnownColor.MenuBar:
                    case KnownColor.Menu: colorString = "menu"; break;
                    case KnownColor.MenuText: colorString = "menutext"; break;
                    case KnownColor.ScrollBar: colorString = "scrollbar"; break;
                    case KnownColor.ControlDarkDark: colorString = "threeddarkshadow"; break;
                    case KnownColor.ControlLightLight: colorString = "buttonhighlight"; break;
                    case KnownColor.Window: colorString = "window"; break;
                    case KnownColor.WindowFrame: colorString = "windowframe"; break;
                    case KnownColor.WindowText: colorString = "windowtext"; break;
                    default: flag = true; break;
                }
            }
            if (flag)
            {
                if (c.IsNamedColor)
                {
                    if (c == Color.LightGray)
                    {
                        // special case due to mismatch between Html and enum spelling
                        colorString = "LightGrey";
                    }
                    else
                    {
                        colorString = c.Name;
                    }
                }
                else
                {
                    colorString = "#" + c.R.ToString("X2", null) +
                                        c.G.ToString("X2", null) +
                                        c.B.ToString("X2", null);
                }
            }
            return colorString;
        }

            public static Color FromHtml(string htmlColor)
            {
                Color c = Color.Empty;

                // empty color
                if ((htmlColor == null) || (htmlColor.Length == 0))
                    return c;

                // #RRGGBB or #RGB
                if ((htmlColor[0] == '#') &&
                    ((htmlColor.Length == 7) || (htmlColor.Length == 4)))
                {

                    if (htmlColor.Length == 7)
                    {
                        c = Color.FromArgb(Convert.ToInt32(htmlColor.Substring(1, 2), 16),
                                           Convert.ToInt32(htmlColor.Substring(3, 2), 16),
                                           Convert.ToInt32(htmlColor.Substring(5, 2), 16));
                    }
                    else
                    {
                        string r = Char.ToString(htmlColor[1]);
                        string g = Char.ToString(htmlColor[2]);
                        string b = Char.ToString(htmlColor[3]);

                        c = Color.FromArgb(Convert.ToInt32(r + r, 16),
                                           Convert.ToInt32(g + g, 16),
                                           Convert.ToInt32(b + b, 16));
                    }
                }

                // special case. Html requires LightGrey, but .NET uses LightGray
                if (c.IsEmpty && String.Equals(htmlColor, "LightGrey", StringComparison.OrdinalIgnoreCase))
                {
                    c = Color.LightGray;
                }

                // System color
                if (c.IsEmpty)
                {
                    if (htmlSysColorTable == null)
                    {
                        InitializeHtmlSysColorTable();
                    }

                    object o = htmlSysColorTable[htmlColor.ToLower(CultureInfo.InvariantCulture)];
                    if (o != null)
                    {
                        c = (Color)o;
                    }
                }

                // resort to type converter which will handle named colors
                if (c.IsEmpty)
                {
                    c = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(htmlColor);
                }

                return c;
            }

        private static void InitializeHtmlSysColorTable()
        {
            htmlSysColorTable = new Hashtable(26);
            htmlSysColorTable["activeborder"] = ColorExt.FromKnownColor(KnownColor.ActiveBorder);
            htmlSysColorTable["activecaption"] = ColorExt.FromKnownColor(KnownColor.ActiveCaption);
            htmlSysColorTable["appworkspace"] = ColorExt.FromKnownColor(KnownColor.AppWorkspace);
            htmlSysColorTable["background"] = ColorExt.FromKnownColor(KnownColor.Desktop);
            htmlSysColorTable["buttonface"] = ColorExt.FromKnownColor(KnownColor.Control);
            htmlSysColorTable["buttonhighlight"] = ColorExt.FromKnownColor(KnownColor.ControlLightLight);
            htmlSysColorTable["buttonshadow"] = ColorExt.FromKnownColor(KnownColor.ControlDark);
            htmlSysColorTable["buttontext"] = ColorExt.FromKnownColor(KnownColor.ControlText);
            htmlSysColorTable["captiontext"] = ColorExt.FromKnownColor(KnownColor.ActiveCaptionText);
            htmlSysColorTable["graytext"] = ColorExt.FromKnownColor(KnownColor.GrayText);
            htmlSysColorTable["highlight"] = ColorExt.FromKnownColor(KnownColor.Highlight);
            htmlSysColorTable["highlighttext"] = ColorExt.FromKnownColor(KnownColor.HighlightText);
            htmlSysColorTable["inactiveborder"] = ColorExt.FromKnownColor(KnownColor.InactiveBorder);
            htmlSysColorTable["inactivecaption"] = ColorExt.FromKnownColor(KnownColor.InactiveCaption);
            htmlSysColorTable["inactivecaptiontext"] = ColorExt.FromKnownColor(KnownColor.InactiveCaptionText);
            htmlSysColorTable["infobackground"] = ColorExt.FromKnownColor(KnownColor.Info);
            htmlSysColorTable["infotext"] = ColorExt.FromKnownColor(KnownColor.InfoText);
            htmlSysColorTable["menu"] = ColorExt.FromKnownColor(KnownColor.Menu);
            htmlSysColorTable["menutext"] = ColorExt.FromKnownColor(KnownColor.MenuText);
            htmlSysColorTable["scrollbar"] = ColorExt.FromKnownColor(KnownColor.ScrollBar);
            htmlSysColorTable["threeddarkshadow"] = ColorExt.FromKnownColor(KnownColor.ControlDarkDark);
            htmlSysColorTable["threedface"] = ColorExt.FromKnownColor(KnownColor.Control);
            htmlSysColorTable["threedhighlight"] = ColorExt.FromKnownColor(KnownColor.ControlLight);
            htmlSysColorTable["threedlightshadow"] = ColorExt.FromKnownColor(KnownColor.ControlLightLight);
            htmlSysColorTable["window"] = ColorExt.FromKnownColor(KnownColor.Window);
            htmlSysColorTable["windowframe"] = ColorExt.FromKnownColor(KnownColor.WindowFrame);
            htmlSysColorTable["windowtext"] = ColorExt.FromKnownColor(KnownColor.WindowText);
        }
    }
}
