using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DataFromBusinessObject
{
    public class Product
    {
        private string FName;
        private decimal FUnitPrice;

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
