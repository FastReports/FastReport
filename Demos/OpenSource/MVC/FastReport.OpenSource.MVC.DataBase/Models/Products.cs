using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.MVC.DataBase.Models
{
    [Table("products")]
    public partial class Products
    {
        [Column("ProductID", TypeName = "INTEGER KEY")]
        public int ProductId { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string ProductName { get; set; }
        [Column("SupplierID", TypeName = "INTEGER KEY")]
        public int? SupplierId { get; set; }
        [Column("CategoryID", TypeName = "INTEGER KEY")]
        public int? CategoryId { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string QuantityPerUnit { get; set; }
        [Column(TypeName = "decimal (10, 0)")]
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        [Column(TypeName = "tinyint (1)")]
        public bool? Discontinued { get; set; }
        [Column("EAN13", TypeName = "varchar (150)")]
        public string Ean13 { get; set; }
    }
}
