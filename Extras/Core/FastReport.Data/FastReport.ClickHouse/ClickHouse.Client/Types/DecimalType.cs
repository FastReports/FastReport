using System;
using ClickHouse.Client.Types.Grammar;
using ClickHouse.Client.Utility;

namespace ClickHouse.Client.Types
{
    internal class DecimalType : ParameterizedType
    {
        private int scale;

        public virtual int Precision { get; set; }

        /// <summary>
        /// Gets or sets the decimal 'scale' (precision) in ClickHouse
        /// </summary>
        public int Scale
        {
            get => scale;
            set
            {
                scale = value;
                Exponent = MathUtils.ToPower(10, value);
            }
        }

        /// <summary>
        /// Gets decimal exponent value based on Scale
        /// </summary>
        public long Exponent { get; private set; }

        public override string Name => "Decimal";

        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.Decimal;

        /// <summary>
        /// Gets size of type in bytes
        /// </summary>
        public virtual int Size => GetSizeFromPrecision(Precision);

        public override Type FrameworkType => typeof(decimal);

        public override ParameterizedType Parse(SyntaxTreeNode node, Func<SyntaxTreeNode, ClickHouseType> parseClickHouseTypeFunc)
        {
            var precision = int.Parse(node.ChildNodes[0].Value);
            var scale = int.Parse(node.ChildNodes[1].Value);

            var size = GetSizeFromPrecision(precision);

            switch (size)
            {
                case 4:
                    return new Decimal32Type { Precision = precision, Scale = scale };
                case 8:
                    return new Decimal64Type { Precision = precision, Scale = scale };
                case 16:
                    return new Decimal128Type { Precision = precision, Scale = scale };
                default:
                    return new DecimalType { Precision = precision, Scale = scale };
            }
        }

        public override string ToString() => $"{Name}({Precision}, {Scale})";

        private int GetSizeFromPrecision(int precision) => precision switch
        {
            int p when p >= 1 && p < 10 => 4,
            int p when p >= 10 && p < 19 => 8,
            int p when p >= 19 && p < 39 => 16,
            _ => throw new ArgumentOutOfRangeException(nameof(Precision)),
        };

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);
    }
}
