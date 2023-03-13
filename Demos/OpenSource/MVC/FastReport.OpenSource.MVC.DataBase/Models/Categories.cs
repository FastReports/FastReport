using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.MVC.DataBase.Models
{
    [Table("categories")]
    public partial class Categories
    {
        [Column("CategoryID", TypeName = "int (11)")]
        public int CategoryId { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string CategoryName { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string Description { get; set; }
        [Column(TypeName = "mediumblob")]
        public byte[] Picture { get; set; }
    }
}
