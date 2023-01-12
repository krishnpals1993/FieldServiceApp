using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaCafelogy.Models
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options)
           : base(options) { }

        public DbSet<User> tbl_Users { get; set; }
        public DbSet<Role> tbl_Roles { get; set; }
        public DbSet<Menu> tbl_Menus { get; set; }
        public DbSet<CustomerMaster> tbl_CustomerMaster { get; set; }
        public DbSet<CustomerContact> tbl_CustmoerContact { get; set; }
        public DbSet<CustomerShipping> tbl_CustmoerShipping { get; set; }
        public DbSet<CustomerBilling> tbl_CustomerBillings { get; set; }
        public DbSet<CustomerShippingApartment> tbl_CustomerShippingApartments { get; set; }
        public DbSet<Userclaim> tbl_Userclaim { get; set; }
        public DbSet<ItemMaster> tbl_ItemMaster { get; set; }
        public DbSet<ItemPrice> tbl_ItemPrice { get; set; }
        public DbSet<OrderMaster> tbl_OrderMaster { get; set; }
        public DbSet<OrderDetail> tbl_OrderDetail { get; set; }
        public DbSet<Order> tbl_Order { get; set; }
        public DbSet<OrderItem> tbl_OrderItem { get; set; }
        public DbSet<OrderAssignment> tbl_OrderAssignment { get; set; }
        public DbSet<State> tbl_States { get; set; }
        public DbSet<City> tbl_Cities { get; set; }
        public DbSet<Unit> tbl_Units { get; set; }
        public DbSet<EmployeeMaster> tbl_EmployeeMaster { get; set; }
        public DbSet<ItemCategory> tbl_ItemCategory { get; set; }
        public DbSet<ItemGroup> tbl_ItemGroup { get; set; }
        public DbSet<BilHeader> tbl_BilHeaders { get; set; }
        public DbSet<BilDetail> tbl_BilDetail { get; set; }
        public DbSet<GlobalSetting> tbl_GlobalSetting { get; set; }
        public DbSet<OrderNote> tbl_OrderNotes { get; set; }
        public DbSet<OrderImage> tbl_OrderImages { get; set; }
        public DbSet<OrderImageShareDetail> tbl_OrderImageShareDetail { get; set; }
        public DbSet<EmailAddressDetail> tbl_EmailAddressDetail { get; set; }
        public DbSet<ComboOfferMaster> tbl_ComboOfferMasters { get; set; }
        public DbSet<ComboOfferDetail> tbl_ComboOfferDetails { get; set; }
      
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
        public Int16 IsActive { get; set; }
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

        public Int16 IsActive { get; set; }
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
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int RoleId { get; set; }

    }

    [Table("ItemMaster")]
    public class ItemMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemId { get; set; }
        public int? CategoryId { get; set; }
        public string ItemCd { get; set; }
        public string ItemDescription { get; set; }
        public int ItemUnitId { get; set; }
        public int ItemQOH { get; set; }
        public decimal ItemCost { get; set; }
        public decimal ItemPrice { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Taxable { get; set; }
        public string Sellable { get; set; }
        public string Service { get; set; }
        public string QBID { get; set; }
        public string QBDesc { get; set; }
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
        public int OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int CustomerId { get; set; }
        public int ShipId { get; set; }
        public DateTime? ShipStartDate { get; set; }
        public DateTime? ShipEndDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public string CancelReason { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ApartmentIds { get; set; }
        public Int16? IsFollowUp { get; set; }
        public int? ParentOrderId { get; set; }
        public Int16? ReOccurence { get; set; }
        public int? ReOccurenceFrequency { get; set; }
        public string ReOccurenceCycle { get; set; }
        public string ReOccurenceWeekday { get; set; }
        public DateTime? ReOccurenceStartDate { get; set; }
        public DateTime? ReOccurenceEndDate { get; set; }
        public int? ReOccurenceParentOrderId { get; set; }
        public decimal? TaxAmount { get; set; }
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
        public Int16 IsActive { get; set; }
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
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int StateId { get; set; }
        public Decimal? Tax { get; set; }
    }

    [Table("States")]
    public class State
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StateId { get; set; }

        public string StateName { get; set; }
        public Int16 IsActive { get; set; }
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

        public Int16 IsActive { get; set; }
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
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int UserId { get; set; }
        public string Color { get; set; }
    }

    [Table("ItemCategories")]
    public class ItemCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
        public string RoleIds { get; set; }

        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("ItemGroup")]
    public class ItemGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("BilHeader")]
    public class BilHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BillId { get; set; }
        public int BillNo { get; set; }
        public int OrderId { get; set; }
        public DateTime BillInvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int CustomerId { get; set; }
        public int ShipId { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("BilDetail")]
    public class BilDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BilDetailId { get; set; }
        public int Bill2No { get; set; }
        public int Bill2Seq { get; set; }
        public int Bill2ItemId { get; set; }
        public string Bill2Description { get; set; }
        public decimal Bill2Price { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
    [Table("CustomerMaster")]
    public class CustomerMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyName1 { get; set; }
        public string CompanyType { get; set; }
        public string CompanyCode { get; set; }
        public string Notes { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string ID { get; set; }
        public string Email { get; set; }
        public string AltPhone { get; set; }
        public string Fax { get; set; }
    }

    [Table("CustomerBilling")]
    public class CustomerBilling
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerBillingId { get; set; }
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }
        public string Zip1 { get; set; }
        public string Zip2 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Notes { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Address4 { get; set; }
        public string Address5 { get; set; }
    }

    [Table("CustomerContact")]
    public class CustomerContact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerContactId { get; set; }
        public int CustomerShipId { get; set; }
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("CustomerShipping")]
    public class CustomerShipping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerShipId { get; set; }
        public int CustomerId { get; set; }
        public int CustomerBillingId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Zip1 { get; set; }
        public string Zip2 { get; set; }
        public string Notes { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Name { get; set; }
        public string Address4 { get; set; }
        public string Address5 { get; set; }

        public string QBID { get; set; }
    }

    [Table("CustomerShippingApartment")]
    public class CustomerShippingApartment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApartmentId { get; set; }
        public int CustomerShipId { get; set; }
        public string ApartmentNo { get; set; }
        public string ApartmentName { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Notes { get; set; }
    }

    [Table("GlobalSetting")]
    public class GlobalSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("OrderNotes")]
    public class OrderNote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderNoteId { get; set; }

        public int OrderId { get; set; }

        public string Note { get; set; }

        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("OrderImages")]
    public class OrderImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderImageId { get; set; }

        public int OrderId { get; set; }

        public string Image { get; set; }
        public string Description { get; set; }
        public Byte[] Base64 { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("OrderImageShareDetail")]
    public class OrderImageShareDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderImageShareDetailId { get; set; }
        public int OrderId { get; set; }
        public int OrderImageId { get; set; }
        public string Type { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("EmailAddressDetail")]
    public class EmailAddressDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmailAddressDetailId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    [Table("ComboOfferMaster")]
    public class ComboOfferMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ComboOfferMasterId { get; set; }
        public string ComboOfferName { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public Int16 IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("ComboOfferDetail")]
    public class ComboOfferDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ComboOfferDetailId { get; set; }
        public int ComboOfferMasterId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public Int16 IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("Orders")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public string ContactNo { get; set; }
        public string Name { get; set; }
        public decimal TotalAmount { get; set; }
        public string Remarks { get; set; }
        public Int16 IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Table("OrderItems")]
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int ComboOfferId { get; set; }
        public int Quantity { get; set; }
        public int Amount { get; set; }
        public Int16 IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }


}