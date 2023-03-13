using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.MVC.DataBase.Models
{
    [Table("customers")]
    public partial class Customers
    {
        [Column("CustomerID", TypeName = "varchar (150)")]
        public string CustomerId { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string CompanyName { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string ContactName { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string ContactTitle { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string Address { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string City { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string Region { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string PostalCode { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string Country { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string Phone { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string Fax { get; set; }
    }
}
