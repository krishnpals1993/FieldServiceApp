using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceApp.Models
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options)
           : base(options) { }

        public DbSet<User> tbl_Users { get; set; }
        public DbSet<Role> tbl_Roles { get; set; }
        public DbSet<Menu> tbl_Menus { get; set; }
        public DbSet<CustomerMaster> tbl_CustomerMaster { get; set; }
        public DbSet<CustmoerContact> tbl_CustmoerContact { get; set; }
        public DbSet<CustmoerShipping> tbl_CustmoerShipping { get; set; }
        public DbSet<Userclaim> tbl_Userclaim { get; set; }
        public DbSet<ItemMaster> tbl_ItemMaster { get; set; }
        public DbSet<ItemPrice> tbl_ItemPrice { get; set; }
        public DbSet<OrderMaster> tbl_OrderMaster { get; set; }
        public DbSet<OrderDetail> tbl_OrderDetail { get; set; }
        public DbSet<OrderAssignment> tbl_OrderAssignment { get; set; }
        public DbSet<State> tbl_States { get; set; }
        public DbSet<City> tbl_Cities { get; set; }
        public DbSet<Unit> tbl_Units { get; set; }
        public DbSet<EmployeeMaster> tbl_EmployeeMaster { get; set; }

    }

    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("Roles")]
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("Menus")]
    public class Menu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MenuId { get; set; }

        public string Name { get; set; }
        public string Parent { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
    }

    [Table("UserClaims")]
    public class Userclaim
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClaimId { get; set; }
        public int MenuId { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int RoleId { get; set; }

    }

    [Table("CustomerMaster")]
    public class CustomerMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustmoerId { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }
        public string FirstName { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("CustmoerContact")]
    public class CustmoerContact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustmoerContactId { get; set; }

        public int ShipId { get; set; }
        public int CustmoerId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("CustmoerShipping")]
    public class CustmoerShipping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShipId { get; set; }
        public int CustmoerId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("ItemMaster")]
    public class ItemMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemId { get; set; }

        public string ItemCd { get; set; }
        public string ItemDescription { get; set; }
        public int ItemUnitId { get; set; }
        public int ItemQOH { get; set; }
        public decimal ItemCost { get; set; }
        public decimal ItemPrice { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("ItemPrice")]
    public class ItemPrice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemPriceId { get; set; }

        public int PricCustId { get; set; }

        public int PricItemId { get; set; }
        public decimal PricPrice { get; set; }
    }

    [Table("OrderMaster")]
    public class OrderMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int CustomerId { get; set; }
        public int ShipId { get; set; }
        public DateTime ShipDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public string CancelReason { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("OrderDetail")]
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public string Description { get; set; }
        public int UnitId { get; set; }
        public int Quantity { get; set; }
        public decimal PerUnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("OrderAssignment")]
    public class OrderAssignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderAssignmentId { get; set; }
        public int OrderId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string Status { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int CompletedBy { get; set; }
        public string Notes { get; set; }
    }

    [Table("Cities")]
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CityId { get; set; }

        public string CityName { get; set; }

        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("States")]
    public class State
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StateId { get; set; }

        public string StateName { get; set; }

        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("Units")]
    public class Unit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UnitId { get; set; }

        public string UnitName { get; set; }

        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("EmployeeMaster")]
    public class EmployeeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }
       public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }



}