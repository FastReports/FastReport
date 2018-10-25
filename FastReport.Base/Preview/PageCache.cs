using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Preview
{
    internal partial class PageCache : IDisposable
    {
        private List<CacheItem> pages;
        private PreparedPages preparedPages;

        private int ItemIndex(int index)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                if (pages[i].index == index)
                    return i;
            }
            return -1;
        }

        private void RemoveAt(int index)
        {
            CacheItem item = pages[index];
            item.Dispose();
            item = null;
            pages.RemoveAt(index);
        }

        public ReportPage Get(int index)
        {
            int i = ItemIndex(index);
            if (i != -1)
            {
                // page exists. Put it on the top of list.
                CacheItem item = pages[i];
                if (i != 0)
                {
                    pages.RemoveAt(i);
                    pages.Insert(0, item);
                }
                return item.page;
            }

            // add new page on the top of list.
            ReportPage page = preparedPages.GetPage(index);

            if (preparedPages.Count > pages.Count)
            {
                pages.Insert(0, new CacheItem(index, page));

                // remove least used pages.
                while (pages.Count > GetPageLimit())
                {
                    RemoveAt(pages.Count - 1);
                }
            }
            return page;
        }

        public void Remove(int index)
        {
            int i = ItemIndex(index);
            if (i != -1)
                RemoveAt(i);
        }

        public void Clear()
        {
            while (pages.Count > 0)
            {
                RemoveAt(0);
            }
        }

        public void Dispose()
        {
            Clear();
        }

        public PageCache(PreparedPages preparedPages)
        {
            this.preparedPages = preparedPages;
            pages = new List<CacheItem>();
        }


        private class CacheItem : IDisposable
        {
            public int index;
            public ReportPage page;

            public void Dispose()
            {
                page.Dispose();
                page = null;
            }

            public CacheItem(int index, ReportPage page)
            {
                this.index = index;
                this.page = page;
            }
        }
    }
}
