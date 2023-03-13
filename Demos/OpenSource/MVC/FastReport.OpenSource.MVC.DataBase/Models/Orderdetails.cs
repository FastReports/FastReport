using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.MVC.DataBase.Models
{
    [Table("orderdetails")]
    public partial class Orderdetails
    {
        public long Id { get; set; }
        [Column("OrderID", TypeName = "INTEGER KEY")]
        public int? OrderId { get; set; }
        [Column("ProductID", TypeName = "INTEGER KEY")]
        public int? ProductId { get; set; }
        [Column(TypeName = "decimal (10, 0)")]
        public decimal? UnitPrice { get; set; }
        public short? Quantity { get; set; }
        [Column(TypeName = "float")]
        public float? Discount { get; set; }
    }
}
