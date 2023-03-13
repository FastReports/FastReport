using Demo.MVC.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection;

namespace Demo.MVC.DataBase
{
    public partial class FastreportContext : DbContext
    {
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<Matrixdemo> Matrixdemo { get; set; }
        public virtual DbSet<Orderdetails> Orderdetails { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<Shippers> Shippers { get; set; }
        public virtual DbSet<Suppliers> Suppliers { get; set; }
        public virtual DbSet<Unicode> Unicode { get; set; }

        static DataTable GetTable<TEntity>(DbSet<TEntity> table, string name) where TEntity : class
        {
            DataTable result = new DataTable(name);
            PropertyInfo[] infos = typeof(TEntity).GetProperties();
            foreach (PropertyInfo info in infos)
            {
                if (info.PropertyType.IsGenericType
                    && info.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    result.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType)));
                else
                    result.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (var el in table)
            {
                DataRow row = result.NewRow();
                foreach (PropertyInfo info in infos)
                    if (info.PropertyType.IsGenericType
                    && info.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        object t = info.GetValue(el);
                        if (t == null)
                            t = Activator.CreateInstance(Nullable.GetUnderlyingType(info.PropertyType));
                        row[info.Name] = t;
                    }
                    else
                        row[info.Name] = info.GetValue(el);
                result.Rows.Add(row);
            }
            return result;
        }

        public DataSet GetDataSet(string name)
        {
            DataSet set = new DataSet(name);
            set.Tables.Add(GetTable(Employees, "Employees"));
            set.Tables.Add(GetTable(Categories, "Categories"));
            set.Tables.Add(GetTable(Customers, "Customers"));
            set.Tables.Add(GetTable(Matrixdemo, "MatrixDemo"));
            set.Tables.Add(GetTable(Orderdetails, "Order Details"));
            set.Tables.Add(GetTable(Orders, "Orders"));
            set.Tables.Add(GetTable(Products, "Products"));
            set.Tables.Add(GetTable(Shippers, "Shippers"));
            set.Tables.Add(GetTable(Suppliers, "Suppliers"));
            set.Tables.Add(GetTable(Unicode, "Unicode"));
            return set;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            optionsBuilder.UseSqlite(@"data source=" + "fastreport.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categories>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("sqlite_autoindex_categories_1");
                entity.Property(e => e.CategoryName).HasDefaultValueSql("NULL");
                entity.Property(e => e.Description).HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Customers>(entity =>
            {
                entity.HasKey(e => e.CustomerId)
                    .HasName("sqlite_autoindex_customers_1");
                entity.Property(e => e.Address).HasDefaultValueSql("NULL");
                entity.Property(e => e.City).HasDefaultValueSql("NULL");
                entity.Property(e => e.CompanyName).HasDefaultValueSql("NULL");
                entity.Property(e => e.ContactName).HasDefaultValueSql("NULL");
                entity.Property(e => e.ContactTitle).HasDefaultValueSql("NULL");
                entity.Property(e => e.Country).HasDefaultValueSql("NULL");
                entity.Property(e => e.Fax).HasDefaultValueSql("NULL");
                entity.Property(e => e.Phone).HasDefaultValueSql("NULL");
                entity.Property(e => e.PostalCode).HasDefaultValueSql("NULL");
                entity.Property(e => e.Region).HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Employees>(entity =>
            {
                entity.HasKey(e => e.EmployeeId)
                    .HasName("sqlite_autoindex_employees_1");

                entity.Property(e => e.Address).HasDefaultValueSql("NULL");
                entity.Property(e => e.BirthDate).HasDefaultValueSql("NULL");
                entity.Property(e => e.City).HasDefaultValueSql("NULL");
                entity.Property(e => e.Country).HasDefaultValueSql("NULL");
                entity.Property(e => e.Extension).HasDefaultValueSql("NULL");
                entity.Property(e => e.FirstName).HasDefaultValueSql("NULL");
                entity.Property(e => e.HireDate).HasDefaultValueSql("NULL");
                entity.Property(e => e.HomePhone).HasDefaultValueSql("NULL");
                entity.Property(e => e.LastName).HasDefaultValueSql("NULL");
                entity.Property(e => e.Notes).HasDefaultValueSql("NULL");
                entity.Property(e => e.PostalCode).HasDefaultValueSql("NULL");
                entity.Property(e => e.Region).HasDefaultValueSql("NULL");
                entity.Property(e => e.ReportsTo).HasDefaultValueSql("NULL");
                entity.Property(e => e.Title).HasDefaultValueSql("NULL");
                entity.Property(e => e.TitleOfCourtesy).HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Matrixdemo>(entity =>
            {
                entity.Property(e => e.ItemsSold).HasDefaultValueSql("NULL");
                entity.Property(e => e.Month).HasDefaultValueSql("NULL");
                entity.Property(e => e.Name).HasDefaultValueSql("NULL");
                entity.Property(e => e.Revenue).HasDefaultValueSql("NULL");
                entity.Property(e => e.Year).HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Orderdetails>(entity =>
            {
                entity.Property(e => e.Discount).HasDefaultValueSql("NULL");
                entity.Property(e => e.OrderId).HasDefaultValueSql("NULL");
                entity.Property(e => e.ProductId).HasDefaultValueSql("NULL");
                entity.Property(e => e.Quantity).HasDefaultValueSql("NULL");
                entity.Property(e => e.UnitPrice).HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.Property(e => e.CustomerId).HasDefaultValueSql("NULL");
                entity.Property(e => e.EmployeeId).HasDefaultValueSql("NULL");
                entity.Property(e => e.Freight).HasDefaultValueSql("NULL");
                entity.Property(e => e.Latitude).HasDefaultValueSql("NULL");
                entity.Property(e => e.Longitude).HasDefaultValueSql("NULL");
                entity.Property(e => e.OrderDate).HasDefaultValueSql("NULL");
                entity.Property(e => e.RequiredDate).HasDefaultValueSql("NULL");
                entity.Property(e => e.ShipAddress).HasDefaultValueSql("NULL");
                entity.Property(e => e.ShipCity).HasDefaultValueSql("NULL");
                entity.Property(e => e.ShipCountry).HasDefaultValueSql("NULL");
                entity.Property(e => e.ShipName).HasDefaultValueSql("NULL");
                entity.Property(e => e.ShipPostalCode).HasDefaultValueSql("NULL");
                entity.Property(e => e.ShipRegion).HasDefaultValueSql("NULL");
                entity.Property(e => e.ShipVia).HasDefaultValueSql("NULL");
                entity.Property(e => e.ShippedDate).HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasKey(e => e.ProductId)
                    .HasName("sqlite_autoindex_products_1");
                entity.Property(e => e.CategoryId).HasDefaultValueSql("NULL");
                entity.Property(e => e.Discontinued).HasDefaultValueSql("NULL");
                entity.Property(e => e.Ean13).HasDefaultValueSql("NULL");
                entity.Property(e => e.ProductName).HasDefaultValueSql("NULL");
                entity.Property(e => e.QuantityPerUnit).HasDefaultValueSql("NULL");
                entity.Property(e => e.ReorderLevel).HasDefaultValueSql("NULL");
                entity.Property(e => e.SupplierId).HasDefaultValueSql("NULL");
                entity.Property(e => e.UnitPrice).HasDefaultValueSql("NULL");
                entity.Property(e => e.UnitsInStock).HasDefaultValueSql("NULL");
                entity.Property(e => e.UnitsOnOrder).HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Shippers>(entity =>
            {
                entity.Property(e => e.CompanyName).HasDefaultValueSql("NULL");
                entity.Property(e => e.Phone).HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Suppliers>(entity =>
            {
                entity.Property(e => e.Address).HasDefaultValueSql("NULL");
                entity.Property(e => e.City).HasDefaultValueSql("NULL");
                entity.Property(e => e.CompanyName).HasDefaultValueSql("NULL");
                entity.Property(e => e.ContactName).HasDefaultValueSql("NULL");
                entity.Property(e => e.ContactTitle).HasDefaultValueSql("NULL");
                entity.Property(e => e.Country).HasDefaultValueSql("NULL");
                entity.Property(e => e.Fax).HasDefaultValueSql("NULL");
                entity.Property(e => e.HomePage).HasDefaultValueSql("NULL");
                entity.Property(e => e.Phone).HasDefaultValueSql("NULL");
                entity.Property(e => e.PostalCode).HasDefaultValueSql("NULL");
                entity.Property(e => e.Region).HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Unicode>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("NULL");
                entity.Property(e => e.Name).HasDefaultValueSql("NULL");
                entity.Property(e => e.Rtl).HasDefaultValueSql("NULL");
                entity.Property(e => e.Text).HasDefaultValueSql("NULL");
                entity.Property(e => e.UnicodeName).HasDefaultValueSql("NULL");
            });
        }
    }
}