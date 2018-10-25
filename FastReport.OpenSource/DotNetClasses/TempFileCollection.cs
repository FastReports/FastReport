using System;
using System.Collections.Generic;
using System.Text;

namespace System.CodeDom.Compiler
{
    internal class TempFileCollection
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
