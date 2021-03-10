using System.IO;
using System.Text;

namespace ClickHouse.Client.Formats
{
    public class ExtendedBinaryWriter : BinaryWriter
    {
        public ExtendedBinaryWriter(Stream stream)
            : base(stream, Encoding.UTF8, false) { }

        public new void Write7BitEncodedInt(int i) => base.Write7BitEncodedInt(i);
    }
}
