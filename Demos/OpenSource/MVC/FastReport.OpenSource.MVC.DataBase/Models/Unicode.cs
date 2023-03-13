using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.MVC.DataBase.Models
{
    [Table("unicode")]
    public partial class Unicode
    {
        public long Id { get; set; }
        [Column(TypeName = "varchar(150)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(150)")]
        public string UnicodeName { get; set; }
        [Column(TypeName = "varchar(750)")]
        public string Text { get; set; }
        [Column(TypeName = "tinyint(1)")]
        public bool? Rtl { get; set; }
        [Column(TypeName = "tinyint(1)")]
        public bool? Active { get; set; }
    }
}
