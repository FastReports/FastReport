using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FastReport.Web.Application
{
    internal class WebRes
    {
        //private XmlItem root;
        private string category;
        private const string badResult = "NOT LOCALIZED!";
        private XmlDocument locale;

        private readonly XmlDocument builtinLocale;

        public void LoadLocale(string fileName)
        {
            if (File.Exists(fileName))
            {
                locale = new XmlDocument();
                locale.Load(fileName);
            }
            else
                locale = builtinLocale;
        }

        public void LoadLocale(Stream stream)
        {
            locale = new XmlDocument();
            locale.Load(stream);
        }

        public string Get(string id)
        {
            if (!String.IsNullOrEmpty(category))
            {
                return InternalGet(category + "," + id);
            }
            else
                return InternalGet(id);
        }

        private string InternalGet(string id)
        {
            string result = Get(id, locale);
            // if no item found, try built-in (english) locale
            if (string.IsNullOrEmpty(result))
            {
                if (locale != builtinLocale)
                    result = Get(id, builtinLocale);

                if (string.IsNullOrEmpty(result))
                    result = id + " " + badResult;
            }
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
                    return null;
                xi = xi[i];
            }
            return xi.GetProp("Text");
        }


        public void Root(string Section)
        {
            category = Section;
        }

        public WebRes(string Section = "")
        {
            locale = new XmlDocument();
            builtinLocale = locale;
            using (Stream stream = ResourceLoader.GetStream("en.xml"))
            {
                locale.Load(stream);
            }

            category = Section;
        }
    }
}
