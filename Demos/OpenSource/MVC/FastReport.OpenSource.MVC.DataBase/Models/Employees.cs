using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.MVC.DataBase.Models
{
    [Table("employees")]
    public partial class Employees
    {
        [Column("EmployeeID", TypeName = "int (11)")]
        public int EmployeeId { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string LastName { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string FirstName { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string Title { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string TitleOfCourtesy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? BirthDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? HireDate { get; set; }
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
        public string HomePhone { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string Extension { get; set; }
        [Column(TypeName = "mediumblob")]
        public byte[] Photo { get; set; }
        [Column(TypeName = "varchar (150)")]
        public string Notes { get; set; }
        [Column(TypeName = "int (11)")]
        public int? ReportsTo { get; set; }
    }
}
