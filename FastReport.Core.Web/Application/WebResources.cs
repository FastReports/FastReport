using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FastReport.Web.Application
{
    internal interface IWebRes : IDisposable
    {
        void LoadLocale(string fileName);

        void LoadLocale(Stream stream);

        string Get(string id);

        string GetInternal(string id);

        void Root(string section);
    }

    internal sealed class WebRes : IWebRes
    {
        //private XmlItem root;
        private string[] categories;
        private const string badResult = "NOT LOCALIZED!";
        private XmlDocument locale;

        private static readonly XmlDocument builtinLocale;

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

        public string GetInternal(string id)
        {
            return Get(id, locale);
        }

        private string Get(string id, XmlDocument locale)
        {
            XmlItem xi = locale.Root;
            int i;
            foreach (string category in categories)
            {
                i = xi.Find(category);
                if (i == -1)
                    return null;
                xi = xi[i];
            }

            // find 'id'
            if(!string.IsNullOrEmpty(id))
            {
                i = xi.Find(id);
                if (i == -1)
                    return null;
                xi = xi[i];
            }

            return xi.GetProp("Text");
        }


        public void Root(string section)
        {
            categories = section.Split(',');
        }

        public void Dispose()
        {
            if (locale != builtinLocale)
                locale.Dispose();
        }

        public WebRes(string section = "")
        {
            locale = builtinLocale;
             
            Root(section);
        }

        static WebRes()
        {
            builtinLocale = new XmlDocument();
            using (Stream stream = ResourceLoader.GetStream("en.xml"))
            {
                builtinLocale.Load(stream);
            }
        }
    }
}
