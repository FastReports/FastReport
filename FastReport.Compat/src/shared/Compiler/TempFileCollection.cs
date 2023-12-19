#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP

using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Code.CodeDom.Compiler
{
    public class TempFileCollection
    {
        public string tempFolder;
        public bool v;

        public TempFileCollection(string tempFolder, bool v)
        {
            this.tempFolder = tempFolder;
            this.v = v;
        }
    }
}

#endif