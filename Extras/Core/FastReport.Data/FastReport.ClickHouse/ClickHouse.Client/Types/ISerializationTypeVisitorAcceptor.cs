namespace ClickHouse.Client.Types
{
    internal interface ISerializationTypeVisitorAcceptor
    {
        void AcceptWrite(ISerializationTypeVisitorWriter writer, object value);

        object AcceptRead(ISerializationTypeVisitorReader reader);
    }
}
