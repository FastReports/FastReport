using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FastReport.Web.Application
{
    internal class WebRes
    {
        private string category;
        private bool localeLoaded = false;
        private string badResult = "NOT LOCALIZED!";
        private XmlDocument locale;
        private XmlDocument builtinLocale;

        public void LoadLocale(string fileName)
        {
            if (!localeLoaded)
                LoadBuiltinLocale();

            if (File.Exists(fileName))
            {
                locale = new XmlDocument();
                locale.Load(fileName);
            }
            else
                locale = builtinLocale;
        }

        public string Get(string id)
        {
            if (!String.IsNullOrEmpty(category))
            {
                if (id != "")
                    return InternalGet(category + "," + id);
                else
                    return InternalGet(category);
            }
            else
                return InternalGet(id);
        }

        private string InternalGet(string id)
        {
            if (!localeLoaded)
                LoadBuiltinLocale();

            string result = Get(id, locale);
            // if no item found, try built-in (english) locale
            if (result.IndexOf(badResult) != -1 && locale != builtinLocale)
                result = Get(id, builtinLocale);
            return result;
        }

        private string Get(string id, XmlDocument locale)
        {
            string[] categories = id.Split(new char[] { ',' });
            XmlItem xi = locale.Root;
            foreach (string category in categories)
            {
                int i = xi.Find(category);
                if (i == -1)
                    return id + " " + badResult;
                xi = xi[i];
            }
            string result = xi.GetProp("Text");
            if (result == "")
                result = id + " " + badResult;
            return result;
        }


        public void Root(string Section)
        {
            category = Section;
        }

        private void LoadBuiltinLocale()
        {
            locale = new XmlDocument();
            builtinLocale = locale;
            using (Stream stream = ResourceLoader.GetStream("en.xml"))
            {
                locale.Load(stream);
            }
            localeLoaded = true;
        }

        public WebRes(string Section)
        {
            category = Section;
        }

        public WebRes()
        {
            category = "";
        }
    }
}
