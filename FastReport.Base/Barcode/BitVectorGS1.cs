using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastReport.Barcode
{
    public sealed class BitVectorGS1
    {
        private const int DefaultSize = 32;
        private int sizeInBits;
        private byte[] array;

        /// <summary>Bit count.</summary>
        public int SizeInBits => sizeInBits;

        /// <summary>Bit count.</summary>
        public int SizeInBytes => (sizeInBits + 7) >> 3;

        /// <summary>Buffer capacity.</summary>
        public int Capacity => array.Length;

        /// <summary>Default constructor.</summary>
        public BitVectorGS1() => array = new byte[DefaultSize];

        /// <summary>Constructor with size.</summary>
        public BitVectorGS1(int size) => array = new byte[size];

        /// <summary>Returns bytes.</summary>
        public byte[] ToByteArray()
        {
            var copy = new byte[SizeInBytes];
            Array.Copy(array, copy, copy.Length);
            return copy;
        }

        /// <summary>Gets bit.</summary>
        private byte GetBit(int index)
        {
            if ((uint)index >= sizeInBits)
                return 0;

            return (byte)((array[index >> 3] >> (7 - (index & 7))) & 1);
        }

        /// <summary>Sets bit.</summary>
        private void SetBit(int index, byte value)
        {
            if ((uint)index >= sizeInBits || (value & ~1) != 0)
                return;

            int shift = 7 - (index & 7);
            if (value == 1)
                array[index >> 3] |= (byte)(1 << shift);
            else
                array[index >> 3] &= (byte)~(1 << shift);
        }

        /// <summary>Adds one bit.</summary>
        public void AppendBit(byte value)
        {
            if ((value & ~1) != 0)
                return;

            if ((sizeInBits & 7) == 0)
            {
                if ((sizeInBits >> 3) == array.Length)
                    Array.Resize(ref array, array.Length << 1);

                array[sizeInBits >> 3] = 0;
            }

            array[sizeInBits >> 3] |= (byte)(value << (7 - (sizeInBits & 7)));
            sizeInBits++;
        }

        /// <summary>Adds multiple bits.</summary>
        public void AppendBits(int value, int count)
        {
            if ((uint)count > 32)
                return;

            while (count > 0)
            {
                if ((sizeInBits & 7) == 0 && count >= 8)
                {
                    AppendByte((byte)(value >> (count - 8)));
                    count -= 8;
                }
                else
                {
                    AppendBit((byte)((value >> (--count)) & 1));
                }
            }
        }

        /// <summary>Adds byte.</summary>
        private void AppendByte(byte value)
        {
            if ((sizeInBits >> 3) == array.Length)
                Array.Resize(ref array, array.Length << 1);

            array[sizeInBits >> 3] = value;
            sizeInBits += 8;
        }

        /// <summary>Inserts bit.</summary>
        public void Insert(int index, byte value)
        {
            AppendBit(0);
            for (int i = sizeInBits - 1; i > index; i--)
                SetBit(i, GetBit(i - 1));

            SetBit(index, value);
        }

        /// <summary>Removes bits.</summary>
        public void RemoveBits(int index, int count)
        {
            if (index + count > sizeInBits)
                return;

            for (int i = index; i < sizeInBits - count; i++)
                SetBit(i, GetBit(i + count));

            sizeInBits -= count;
        }

        /// <summary>Clears vector.</summary>
        public void Clear()
        {
            Array.Clear(array, 0, SizeInBytes);
            sizeInBits = 0;
        }

        /// <summary>Appends vector.</summary>
        public void AppendBitVector(BitVectorGS1 other)
        {
            for (int i = 0; i < other.sizeInBits; i++)
                AppendBit(other.GetBit(i));
        }

        /// <summary>XOR with another vector.</summary>
        public void Xor(BitVectorGS1 other)
        {
            if (sizeInBits != other.sizeInBits)
                return;

            for (int i = 0; i < SizeInBytes; i++)
                array[i] ^= other.array[i];
        }

        /// <summary>Bit indexer.</summary>
        public byte this[int index]
        {
            get => GetBit(index);
            set => SetBit(index, value);
        }

        /// <summary>Binary string.</summary>
        public override string ToString()
        {
            var sb = new StringBuilder(sizeInBits);
            for (int i = 0; i < sizeInBits; i++)
                sb.Append(GetBit(i) == 1 ? '1' : '0');

            return sb.ToString();
        }
    }
    public class BitList
    {
        private List<bool> bits = new List<bool>();

        /// <summary>Adds value as bits.</summary>
        public void Add(long value, int length)
        {
            for (int i = length - 1; i >= 0; i--)
                bits.Add(((value >> i) & 1) == 1);
        }

        /// <summary>Adds single bit.</summary>
        public void AddBit(bool value)
        {
            bits.Add(value);
        }

        /// <summary>Returns bits.</summary>
        public List<bool> GetBits() => bits;

        /// <summary>Bit count.</summary>
        public int Count => bits.Count;
    }
    public sealed class SymbolData
    {
        private float height;
        private byte[] data;

        /// <summary>Row height.</summary>
        public float RowHeight
        {
            get { return height; }
        }

        /// <summary>Row length.</summary>
        public int RowCount
        {
            get { return data.Length; }
        }

        /// <summary>Sets row data.</summary>
        public byte[] RowData
        {
            set { data = value; }
        }

        /// <summary>Gets row copy.</summary>
        public byte[] GetRowData()
        {
            return (byte[])data.Clone();
        }

        /// <summary>Constructor with height.</summary>
        public SymbolData(byte[] data, float height)
        {
            this.data = data;
            this.height = height;
        }

        /// <summary>Constructor.</summary>
        public SymbolData(byte[] data)
        {
            this.data = data;
            this.height = 0.0f; 
        }
    }
}
