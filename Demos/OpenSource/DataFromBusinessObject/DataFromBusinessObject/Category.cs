using System;
using System.Collections.Generic;
using System.Text;

namespace DataFromBusinessObject
{
    public class Category
    {
        private string FName;
        private string FDescription;
        private List<Product> FProducts;

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
