using System.Collections.Generic;

namespace DataFromBusinessObject
{
    public class Category
    {
        private readonly string FName;
        private readonly string FDescription;
        private readonly List<Product> FProducts;

        public string Name
        {
            get { return FName; }
        }

        public string Description
        {
            get { return FDescription; }
        }

        public List<Product> Products
        {
            get { return FProducts; }
        }

        public Category(string name, string description)
        {
            FName = name;
            FDescription = description;
            FProducts = new List<Product>();
        }
    }
}
