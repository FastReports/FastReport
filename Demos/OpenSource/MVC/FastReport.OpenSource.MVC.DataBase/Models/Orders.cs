using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.MVC.DataBase.Models
{
    [Table("orders")]
    public partial class Orders
    {
        [Column("OrderID")]
        [Key]
        public int OrderId { get; set; }
        [Column("CustomerID", TypeName = "varchar KEY")]
        public string CustomerId { get; set; }
        [Column("EmployeeID", TypeName = "INTEGER KEY")]
        public int? EmployeeId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? OrderDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RequiredDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ShippedDate { get; set; }
        [Column(TypeName = "INTEGER KEY")]
        public int? ShipVia { get; set; }
        [Column(TypeName = "decimal (10, 0)")]
        public decimal? Freight { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string ShipName { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string ShipAddress { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string ShipCity { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string ShipRegion { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string ShipPostalCode { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string ShipCountry { get; set; }
        [Column(TypeName = "double")]
        public double? Latitude { get; set; }
        [Column(TypeName = "double")]
        public double? Longitude { get; set; }
    }
}
