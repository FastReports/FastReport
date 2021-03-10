namespace ClickHouse.Client.Types
{
    internal class Enum8Type : EnumType
    {
        public override string Name => "Enum8";

        public override object AcceptRead(ISerializationTypeVisitorReader reader) => reader.Read(this);

        public override void AcceptWrite(ISerializationTypeVisitorWriter writer, object value) => writer.Write(this, value);
    }
}
