using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Export.Html
{
    /// <summary>
    /// Represents the HTML export templates.
    /// </summary>
    public class HtmlTemplates
    {
        #region private fields
        private string pageTemplateTitle;
        private string pageTemplateFooter;
        private string navigatorTemplate;
        private string outlineTemplate;
        private string indexTemplate;
        private FastString capacitor;
        #endregion

        #region private methods
        private void NewCapacitor()
        {
            capacitor = new FastString(512);
        }
        private void Part(string str)
        {
            capacitor.AppendLine(str);
        }
        private string Capacitor()
        {
            return capacitor.ToString();
        }
        #endregion

        #region public properties

        /// <summary>
        /// Page Template Title
        /// </summary>
        public string PageTemplateTitle
        {
            get { return pageTemplateTitle; }
            set { pageTemplateTitle = value; }
        }

        /// <summary>
        /// Page Template Footer
        /// </summary>
        public string PageTemplateFooter
        {
            get { return pageTemplateFooter; }
            set { pageTemplateFooter = value; }
        }

        /// <summary>
        /// Navigator Template
        /// </summary>
        public string NavigatorTemplate
        {
            get { return navigatorTemplate; }
            set { navigatorTemplate = value; }
        }
        /// <summary>
        /// OutlineTemplate
        /// </summary>
        public string OutlineTemplate
        {
            get { return outlineTemplate; }
            set { outlineTemplate = value; }
        }

        /// <summary>
        /// Index Template
        /// </summary>
        public string IndexTemplate
        {
            get { return indexTemplate; }
            set { outlineTemplate = value; }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlTemplates"/> class.
        /// </summary>
        public HtmlTemplates()
        {
            #region fill page template
            // {0} - title
            NewCapacitor();
            Part("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\">");
            Part("<html><head>");
            Part("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            Part("<meta name=Generator content=\"FastReport http://www.fast-report.com\">");
            Part("<title>{0}</title>");
            pageTemplateTitle = Capacitor();

            NewCapacitor();
            Part("</html>");
            pageTemplateFooter = Capacitor();
            #endregion

            #region fill navigator template
            //  {0} - pages count {1} - name of report {2} multipage document {3} prefix of pages
            //  {4} first caption {5} previous caption {6} next caption {7} last caption
            //  {8} total caption
            NewCapacitor();
            Part("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\">");
            Part("<html><head>");
            Part("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            Part("<meta name=Generator content=\"FastReport http://www.fast-report.com\">");
            Part("<title></title><style type=\"text/css\"><!--");
            Part("body,input,select {{ font-family:\"Lucida Grande\",Calibri,Arial,sans-serif; font-size: 8px; font-weight: bold; font-style: normal; text-align: center; vertical-align: middle; }}");
            Part("input {{text-align: center}}");
            Part(".nav {{ font-size : 9pt; color : #283e66; font-weight : bold; text-decoration : none;}}");
            Part("--></style><script language=\"javascript\" type=\"text/javascript\"><!--");
            Part("  var frPgCnt = {0}; var frRepName = \"{1}\"; var frMultipage = {2}; var frPrefix=\"{3}\";");
            Part("  function DoPage(PgN) {{");
            Part("    if ((PgN > 0) && (PgN <= frPgCnt) && (PgN != parent.frCurPage)) {{");
            Part("      if (frMultipage > 0)  parent.mainFrame.location = frPrefix + PgN + \".html\";");
            Part("      else parent.mainFrame.location = frPrefix + \".main.html#PageN\" + PgN;");
            Part("      UpdateNav(PgN); }} else document.PgForm.PgEdit.value = parent.frCurPage; }}");
            Part("  function UpdateNav(PgN) {{");
            Part("    parent.frCurPage = PgN; document.PgForm.PgEdit.value = PgN;");
            Part("    if (PgN == 1) {{ document.PgForm.bFirst.disabled = 1; document.PgForm.bPrev.disabled = 1; }}");
            Part("    else {{ document.PgForm.bFirst.disabled = 0; document.PgForm.bPrev.disabled = 0; }}");
            Part("    if (PgN == frPgCnt) {{ document.PgForm.bNext.disabled = 1; document.PgForm.bLast.disabled = 1; }}");
            Part("    else {{ document.PgForm.bNext.disabled = 0; document.PgForm.bLast.disabled = 0; }} }}");
            Part("--></script></head>");
            Part("<body bgcolor=\"#DDDDDD\" text=\"#000000\" leftmargin=\"0\" topmargin=\"4\" onload=\"UpdateNav(parent.frCurPage)\">");
            Part("<form name=\"PgForm\" onsubmit=\"DoPage(document.forms[0].PgEdit.value); return false;\" action=\"\">");
            Part("<table cellspacing=\"0\" align=\"left\" cellpadding=\"0\" border=\"0\" width=\"100%\">");
            Part("<tr valign=\"middle\">");
            Part("<td width=\"60\" align=\"center\"><button name=\"bFirst\" class=\"nav\" type=\"button\" onclick=\"DoPage(1); return false;\"><b>{4}</b></button></td>");
            Part("<td width=\"60\" align=\"center\"><button name=\"bPrev\" class=\"nav\" type=\"button\" onclick=\"DoPage(Math.max(parent.frCurPage - 1, 1)); return false;\"><b>{5}</b></button></td>");
            Part("<td width=\"100\" align=\"center\"><input type=\"text\" class=\"nav\" name=\"PgEdit\" value=\"parent.frCurPage\" size=\"4\"></td>");
            Part("<td width=\"60\" align=\"center\"><button name=\"bNext\" class=\"nav\" type=\"button\" onclick=\"DoPage(parent.frCurPage + 1); return false;\"><b>{6}</b></button></td>");
            Part("<td width=\"60\" align=\"center\"><button name=\"bLast\" class=\"nav\" type=\"button\" onclick=\"DoPage(frPgCnt); return false;\"><b>{7}</b></button></td>");
            Part("<td width=\"20\">&nbsp;</td>\r\n");
            Part("<td align=\"right\">{8}: <script language=\"javascript\" type=\"text/javascript\"> document.write(frPgCnt);</script></td>");
            Part("<td width=\"10\">&nbsp;</td>");
            Part("</tr></table></form></body></html>");
            navigatorTemplate = Capacitor();
            #endregion

            #region fill outline template
            // under construction
            outlineTemplate = String.Empty;
            #endregion

            #region fill index template
            // {0} - title, {1} - navigator frame, {2} - main frame
            NewCapacitor();
            Part("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Frameset//EN\" \"http://www.w3.org/TR/html4/frameset.dtd\"");
            Part("<html><head>");
            Part("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            Part("<meta name=Generator content=\"FastReport http://www.fast-report.com\">");
            Part("<title>{0}</title>");
            Part("<script language=\"javascript\" type=\"text/javascript\"> var frCurPage = 1;</script></head>");
            Part("<frameset rows=\"36,*\" cols=\"*\">");
            Part("<frame name=\"topFrame\" src=\"{1}\" noresize frameborder=\"0\" scrolling=\"no\">");
            Part("<frame name=\"mainFrame\" src=\"{2}\" frameborder=\"0\">");
            Part("</frameset>");
            Part("</html>");
            indexTemplate = Capacitor();
            #endregion
        }
    }
}
