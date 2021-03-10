using System.IO;
using System.Text;

namespace ClickHouse.Client.Formats
{
    internal class ExtendedBinaryReader : BinaryReader
    {
        private readonly PeekableStreamWrapper streamWrapper;

        public ExtendedBinaryReader(Stream stream)
            : base(new PeekableStreamWrapper(stream), Encoding.UTF8, false)
        {
            streamWrapper = (PeekableStreamWrapper)BaseStream;
        }

        public new int Read7BitEncodedInt() => base.Read7BitEncodedInt();

        public override byte[] ReadBytes(int count)
        {
            var buffer = new byte[count];
            var bytesRead = base.Read(buffer, 0, count);
            if (bytesRead < count)
            {
                throw new EndOfStreamException($"Expected to read {count} bytes, got {bytesRead}");
            }

            return buffer;
        }

        public override int Read(byte[] buffer, int index, int count)
        {
            var bytesRead = base.Read(buffer, index, count);
            if (bytesRead < count)
            {
                throw new EndOfStreamException($"Expected to read {count} bytes, got {bytesRead}");
            }

            return bytesRead;
        }

        public override int PeekChar() => streamWrapper.Peek();
    }
}
