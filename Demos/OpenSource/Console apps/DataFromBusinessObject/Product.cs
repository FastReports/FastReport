namespace DataFromBusinessObject
{
    public class Product
    {
        private readonly string FName;
        private readonly decimal FUnitPrice;

        public string Name
        {
            get { return FName; }
        }

        public decimal UnitPrice
        {
            get { return FUnitPrice; }
        }

        public Product(string name, decimal unitPrice)
        {
            FName = name;
            FUnitPrice = unitPrice;
        }
    }
}
