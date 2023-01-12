using JqueryDataTables.ServerSide.AspNetCoreWeb.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaCafelogy.Models
{
    public class ResponseModel
    {
        public string Status { get; set; }
        public string Message { get; set; }

        public object AdditionalData { get; set; }
    }
    public class LoginViewModel
    {

        [Required(ErrorMessage = "Please enter username")]
        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string Username { get; set; }


        [Required(ErrorMessage = "Please enter password")]
        public string Password { get; set; }
    }
    public class UserViewModel
    {

        public UserViewModel()
        {
            RoleList = new List<RoleViewModel>();

        }

        public List<RoleViewModel> RoleList { get; set; }

        public List<EmployeeMasterViewModel> EmployeeList { get; set; }


        public int UserId { get; set; }

        [Required(ErrorMessage = "Please enter an email")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter valid email")]
        [StringLength(50)]
        public string Email { get; set; }
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [Display(Name = "Password")]
        [MaxLength(15, ErrorMessage = "Password should be less then or equal to 15 characters long")]
        [MinLength(4, ErrorMessage = "Password at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter confirm password")]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please select role")]
        [StringLength(50)]
        public string RoleId { get; set; }
        public int iRoleId { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string RoleName { get; set; }
        public string EmployeeName { get; set; }

        //[Display(Name = "First Name")]
        //[Required(ErrorMessage = "Please enter first name")]
        //public string FirstName { get; set; }

        //public string MiddleName { get; set; }

        //[Display(Name = "Last Name")]
        //[Required(ErrorMessage = "Please enter last name")]
        //public string LastName { get; set; }

        //[Display(Name = "Employee No")]
        //[Required(ErrorMessage = "Please enter employee no")]
        //public string EmployeeNo { get; set; }

        //[Display(Name = "Hr Group")]
        //[Required(ErrorMessage = "Please select hr group")]
        //public int HrGroupId { get; set; }
        //public string HrGroupName { get; set; }

    }
    public class RoleViewModel
    {

        public int RoleId { get; set; }

        public string Rolename { get; set; }

        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
    public class MenuViewModel
    {
        public int MenuId { get; set; }
        public string Name { get; set; }
        public string Parent { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Childmenus { get; set; }
        public string Icon { get; set; }
        public string Parenticon { get; set; }
        public bool Ischecked { get; set; }

    }
    public class UserPermissionViewModel : RoleViewModel
    {
        public UserPermissionViewModel()
        {
            Menus = new List<MenuViewModel>();
        }

        public List<MenuViewModel> Menus { get; set; }
    }
    public class ForgetPasswordViewModel
    {

        [Required(ErrorMessage = "Please enter an email")]
        [Display(Name = "Email")]
        [StringLength(50)]
        public string Email { get; set; }

    }
    public class RegisterViewModel
    {

        [Required(ErrorMessage = "Please enter an email")]
        [Display(Name = "Email")]
        [StringLength(50)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [Display(Name = "Password")]
        [MaxLength(15, ErrorMessage = "Password should be less then or equal to 15 characters long")]
        [MinLength(4, ErrorMessage = "Password at least 4 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter confirm password")]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

    }
    public class ResetPasswordViewModel
    {

        [Required(ErrorMessage = "Please enter an email")]
        [Display(Name = "Email")]
        [StringLength(50)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [Display(Name = "Password")]
        [MaxLength(15, ErrorMessage = "Password should be less then or equal to 15 characters long")]
        [MinLength(6, ErrorMessage = "Password at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter confirm password")]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

    }
    public class UserclaimViewModel
    {
        public int ClaimId { get; set; }
        public int MenuId { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int RoleId { get; set; }

    }
    public class CustomerMasterViewModel
    {
        public CustomerMasterViewModel()
        {
            Contact = new CustmoerContactViewModel();
            Shipping = new CustmoerShippingViewModel();
            StateList = new List<StateViewModel>();
            CityList = new List<CityViewModel>();
            Contacts = new List<CustmoerContactViewModel>();
            Shippings = new List<CustmoerShippingViewModel>();

        }
        public int ShipId { get; set; }

        public int BillingId { get; set; }
        public int CustomerBillingId { get; set; }
        public string Notes { get; set; }
        public int CustmoerId { get; set; }
        public string CompanyName { get; set; }

        //[Required(ErrorMessage = "Please enter code")]
        public string Code { get; set; }
        public string Address { get; set; }
        public string Zip1 { get; set; }
        public string Zip2 { get; set; }
        public string CompanyType { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string ShipAddress { get; set; }
        public CustmoerContactViewModel Contact { get; set; }
        public List<CustmoerContactViewModel> Contacts { get; set; }
        public CustmoerShippingViewModel Shipping { get; set; }
        public List<CustmoerShippingViewModel> Shippings { get; set; }
        public List<StateViewModel> StateList { get; set; }
        public List<CityViewModel> CityList { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string AltPhone { get; set; }
        public string Fax { get; set; }
        public string MiddleName { get; set; }

    }
    public class CustomerDetailViewModel
    {
        public CustomerDetailViewModel()
        {
            Shipping = new CustmoerShippingViewModel();

        }
        public int ShipId { get; set; }
        public int CustomerBillingId { get; set; }
        public string Notes { get; set; }
        public int CustmoerId { get; set; }
        public string CompanyName { get; set; }

        //[Required(ErrorMessage = "Please enter code")]
        public string Code { get; set; }
        public string Address { get; set; }
        public string Zip1 { get; set; }
        public string Zip2 { get; set; }
        public string CompanyType { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string ShipAddress { get; set; }
        public CustmoerShippingViewModel Shipping { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string AltPhone { get; set; }
        public string Fax { get; set; }

    }
    public class CustomerMasterViewModel_datatable
    {


        public int CustmoerId { get; set; }

        [Sortable]
        [SearchableString]
        [Display(Name = "Customer")]
        public string CompanyName { get; set; }

        [Sortable]
        [SearchableString]
        [Display(Name = "CompanyType")]
        public string CompanyType { get; set; }

        [Sortable]
        [Display(Name = "Status")]
        public Int16 IsActive { get; set; }

        public string Action { get; set; }



    }
    public class CustmoerContactViewModel
    {
        public int CustmoerContactId { get; set; }

        public int ShipId { get; set; }
        public int CustmoerId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }

        public string Phone { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
    public class CustmoerShippingViewModel
    {
        public CustmoerShippingViewModel()
        {

            StateList = new List<StateViewModel>();
            CityList = new List<CityViewModel>();
            ApartmentList = new List<CustomerShippingApartmentViewModel>();
        }
        public int ShipId { get; set; }
        public int CustmoerId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<StateViewModel> StateList { get; set; }
        public List<CityViewModel> CityList { get; set; }
        public List<CustomerShippingApartmentViewModel> ApartmentList { get; set; }
        public string Zip1 { get; set; }
        public string Zip2 { get; set; }
        public string CityName { get; internal set; }
        public string StateName { get; internal set; }
        public string Notes { get; set; }
        public int CustomerBillingId { get; set; }
        public decimal? Tax { get; set; }
    }
    public class ItemMasterViewModel
    {
        public ItemMasterViewModel()
        {
            UnitList = new List<UnitViewModel>();
            ItemCategoryList = new List<ItemCategoryViewModel>();
        }
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Please select category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please enter item code")]
        public string ItemCd { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }

        public string ItemUnitId { get; set; }
        public int iItemUnitId { get; set; }
        public int? ItemQOH { get; set; }

        public decimal? ItemCost { get; set; }

        public decimal? ItemPrice { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string UnitName { get; set; }
        public int ItemPriceId { get; set; }

        public string Taxable { get; set; }

        public string Sellable { get; set; }

        public string Service { get; set; }
        public List<UnitViewModel> UnitList { get; set; }

        public List<ItemCategoryViewModel> ItemCategoryList { get; set; }
        public string CategoryName { get; set; }
        public string Type { get; set; }

        public string IsCombo { get; set; }
    }

    public class OrderItemViewModel
    {
        public int ItemId { get; set; }
        public string ItemCd { get; set; }
        public decimal? ItemPrice { get; set; }
        public string ItemDescription { get; set; }
        public string IsCombo { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
    }
    public class ItemPriceViewModel
    {
        public ItemPriceViewModel()
        {
            ItemList = new List<ItemMasterViewModel>();
        }
        public int ItemPriceId { get; set; }

        public int PricCustId { get; set; }

        [Required(ErrorMessage = "Please select item")]
        public int PricItemId { get; set; }

        [Required(ErrorMessage = "Please enter price")]
        public decimal PricPrice { get; set; }
        public List<ItemMasterViewModel> ItemList { get; set; }
    }
    public class OrderMasterViewModel
    {
        public OrderMasterViewModel()
        {
            ItemList = new List<OrderItemViewModel>();
            EmployeeList = new List<EmployeeMasterViewModel>();
            CustomerList = new List<CustomerMasterViewModel>();
            CustomerShipingAddressList = new List<CustmoerShippingViewModel>();
            CustomerDetail = new CustomerMasterViewModel();
            CustomerShippingDetail = new CustmoerShippingViewModel();
            ApartmentList = new List<CustomerShippingApartmentViewModel>();
            OrderList = new List<ServiceFormOrderViewModel>();
            OrderItemList = new List<OrderDetailViewModel>();
        }
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Please enter order no")]
        public int OrderNo { get; set; }

        [Display(Name = "Order Date")]
        [Required(ErrorMessage = "Please enter order date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Total Amount")]
        [Required(ErrorMessage = "Please enter total amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Customer")]
        [Required(ErrorMessage = "Please select customer")]
        public int CustomerId { get; set; }

        [Display(Name = "Ship Address")]
        [Required(ErrorMessage = "Please select ship address")]
        public int ShipId { get; set; }

        //[Display(Name = "Shil Date")]
        //[Required(ErrorMessage = "Please select ship date")]
        public DateTime? ShipStartDate { get; set; }
        public DateTime? ShipStartTime { get; set; }
        public DateTime? ShipEndDate { get; set; }
        public DateTime? ShipEndTime { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public string CancelReason { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public int ItemId { get; set; }
        public string Description { get; set; }
        public int UnitId { get; set; }
        public int Quantity { get; set; }
        public decimal PerUnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int EmployeeId { get; set; }

        public string AssigneeId { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public string CustomerShipAddress { get; set; }
        public string Status { get; set; }
        public List<int> ApartmentId { get; set; }

        public string ApartmentIds { get; set; }
        public Int16? IsFollowUp { get; set; }
        public int? ParentOrderId { get; set; }
        public string ReOccurence { get; set; }
        public int? ReOccurenceFrequency { get; set; }
        public string ReOccurenceCycle { get; set; }
        public string ReOccurenceWeekday { get; set; }
        public DateTime? ReOccurenceStartDate { get; set; }
        public DateTime? ReOccurenceEndDate { get; set; }
        public DateTime? MaxReOccurenceEndDate { get; set; }
        public int? ReOccurenceParentOrderId { get; set; }
        public int ReOccurenceOrderCount { get; set; }
        public int RemainingReOccurenceOrderCount { get; set; }
        public CustomerMasterViewModel CustomerDetail { get; set; }
        public CustmoerShippingViewModel CustomerShippingDetail { get; set; }
        public List<OrderItemViewModel> ItemList { get; set; }
        public List<EmployeeMasterViewModel> EmployeeList { get; set; }
        public List<CustomerMasterViewModel> CustomerList { get; set; }
        public List<CustmoerShippingViewModel> CustomerShipingAddressList { get; set; }
        public List<CustomerShippingApartmentViewModel> ApartmentList { get; set; }
        public List<OrderDetailViewModel> OrderItemList { get; set; }
        public List<ServiceFormOrderViewModel> OrderList { get; set; }
        public int ReOccurenceOrders { get; set; }
        public bool ScheduledOnNonWorkingDay { get; set; }
        public string Color { get; set; }
        public int? ReoccurrenceOrderNo { get; set; }
        public string ReOccurenceStartDateSetBy { get; set; }
        public string ItemName { get; set; }
        public decimal? Tax { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? GrossAmount { get; set; }
        public string Notes { get; set; }
    }

    public class OrderMasterViewModel_Datatable
    {
        [Sortable]
        [SearchableString]
        [Display(Name = "Order No")]
        public int OrderId { get; set; }

        [Sortable]
        [SearchableString]
        [Display(Name = "Order No")]
        public int OrderNo { get; set; }

        [Display(Name = "Main Order No")]
        public int? ReOccurenceParentOrderId { get; set; }


        [Sortable]
        [SearchableString]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
        [Sortable]
        [SearchableString]
        [Display(Name = "Customer")]
        public string CustomerName { get; set; }
        [Sortable]
        [SearchableString]
        [Display(Name = "Ship Date")]
        public DateTime? ShipStartDate { get; set; }
        [Sortable]
        [SearchableString]
        [Display(Name = "Amount($)")]

        public decimal TotalAmount { get; set; }
        [Sortable]
        [SearchableString]
        [Display(Name = "Assigned To")]
        public string EmployeeName { get; set; }
        [Sortable]
        [SearchableString]
        [Display(Name = "Ship Address")]
        public string CustomerShipAddress { get; set; }

        [Display(Name = "Status")]
        public Int16 IsActive { get; set; }
        public bool ScheduledOnNonWorkingDay { get; set; }
        public string Action { get; set; }
        public string ShipStartDateFormatted { get; set; }

    }

    public class CustmoerShippingViewModel_Datatable
    {

        public int CustomerBillingId { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Zip1 { get; set; }
        public string Zip2 { get; set; }
        public string CityName { get; internal set; }
        public string StateName { get; internal set; }
        public int ShipId { get; set; }
    }


    public class OrderDetailViewModel
    {

        public OrderDetailViewModel()
        {
            ItemList = new List<ItemMasterViewModel>();
        }

        public List<ItemMasterViewModel> ItemList { get; set; }
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
    public class OrderAssignmentViewModel
    {
        public int OrderAssignmentId { get; set; }
        public int OrderId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string Status { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int CompletedBy { get; set; }
        public string Notes { get; set; }
    }
    public class CityViewModel
    {
        public CityViewModel()
        {
            StateList = new List<StateViewModel>();
        }
        public int CityId { get; set; }

        [Required(ErrorMessage = "Please enter city name")]
        public string CityName { get; set; }

        [Required(ErrorMessage = "Please select state")]
        public int StateId { get; set; }

        [MaxValue(100, ErrorMessage = "Value must be less than 100")]
        public Decimal? Tax { get; set; }
        public string StateName { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<StateViewModel> StateList { get; set; }
    }
    public class StateViewModel
    {

        public int StateId { get; set; }

        [Required(ErrorMessage = "Please select state name")]
        public string StateName { get; set; }

        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
    public class UnitViewModel
    {

        public int UnitId { get; set; }

        [Required(ErrorMessage = "Please enter unit name")]
        public string UnitName { get; set; }

        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
    public class EmployeeMasterViewModel
    {
        public int EmployeeId { get; set; }
        [Required(ErrorMessage = "Please enter first name")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter phone")]
        public string Phone { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? UserId { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        [Display(Name = "Password")]
        [MaxLength(15, ErrorMessage = "Password should be less then or equal to 15 characters long")]
        [MinLength(4, ErrorMessage = "Password at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter confirm password")]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string Color { get; set; }

    }
    public class EmployeeMasterEditViewModel
    {
        public int EmployeeId { get; set; }
        [Required(ErrorMessage = "Please enter first name")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter phone")]
        public string Phone { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? UserId { get; set; }
        public string Color { get; set; }
    }
    public class DashboardViewModel
    {
        public List<OrderMasterViewModel> OrderList { get; set; }
        public List<EmployeeMasterViewModel> EmployeeList { get; set; }



    }
    public class DashboardOrderViewModel
    {
        public DashboardOrderViewModel()
        {
            EmployeeList = new List<EmployeeMasterViewModel>();
            OrderImageList = new List<OrderImageViewModel>();
        }
        public int OrderId { get; set; }
        public DateTime? ShipStartDate { get; set; }
        public DateTime? ShipEndDate { get; set; }
        public int EmployeeId { get; set; }
        public string AssigneeId { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public string CustomerShipAddress { get; set; }
        public string Status { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public List<EmployeeMasterViewModel> EmployeeList { get; set; }
        public string Color { get; set; }
        public string Apartments { get; set; }
        public int ReOccurenceParentOrderId { get; set; }
        public string Description { get; set; }

        public List<OrderImageViewModel> OrderImageList { get; set; }
    }
    public class ServiceFormViewModel
    {
        public ServiceFormViewModel()
        {
            ItemList = new List<ItemMasterViewModel>();
            CustomerShipingAddressList = new List<CustmoerShippingViewModel>();
            OrderList = new List<ServiceFormOrderViewModel>();
            ItemCategoryList = new List<ItemCategoryViewModel>();
            ApartmentList = new List<CustomerShippingApartmentViewModel>();
        }
        public int ServiceFormLogId { get; set; }

        [Required(ErrorMessage = "Please select order")]
        public int OrderId { get; set; }
        public int ShipId { get; set; }

        [Display(Name = "Date Of Service")]
        [Required(ErrorMessage = "Please select date of service")]
        public DateTime? DateOfService { get; set; }
        public string Apartment { get; set; }

        [Display(Name = "Item")]
        public int? ItemId { get; set; }

        [Display(Name = "Quantity")]
        public decimal? Quantity { get; set; }
        public string Signature { get; set; }
        public string ShipAddress { get; set; }
        public string ServiceType { get; set; }
        public string Locations { get; set; }
        public string ItemCd { get; set; }
        public string CategoryName { get; set; }
        public int IsActive { get; set; }
        public string CustomerName { get; set; }
        public int ApartmentId { get; set; }
        public string ApartmentName { get; set; }
        public string ApartmentNo { get; set; }
        public string IsFollowUp { get; set; }
        public List<CustomerShippingApartmentViewModel> ApartmentList { get; set; }
        public List<ItemMasterViewModel> ItemList { get; set; }
        public List<CustmoerShippingViewModel> CustomerShipingAddressList { get; set; }
        public List<ServiceFormOrderViewModel> OrderList { get; set; }
        public List<ItemCategoryViewModel> ItemCategoryList { get; set; }
    }
    public class ServiceFormOrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
    }
    public class ItemCategoryViewModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please enter category name")]
        public string CategoryName { get; set; }
        public string RoleIds { get; set; }

        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
    public class ItemGroupViewModel 
    {
        public int GroupId { get; set; }

        [Required(ErrorMessage = "Please enter category name")]
        public string GroupName { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
    public class CustomerShippingApartmentViewModel
    {
        public string Notes { get; set; }
        public int ApartmentId { get; set; }
        public int ShipId { get; set; }
        public string ApartmentNo { get; set; }
        public string ApartmentName { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
    public class OrderListViewModel
    {
        public OrderListViewModel()
        {

            OrderList = new List<OrderMasterViewModel>();
        }
        public List<OrderMasterViewModel> OrderList { get; set; }

        public DateTime? ShipDateFrom { get; set; }
        public DateTime? ShipDateTo { get; set; }
        public DateTime? OrderDateFrom { get; set; }
        public DateTime? OrderDateTo { get; set; }
        public string CustomerName { get; set; }
        public string OrderNo { get; set; }



    }
    public class MaxValueAttribute : ValidationAttribute
    {
        private readonly int _maxValue;

        public MaxValueAttribute(int maxValue)
        {
            _maxValue = maxValue;
        }

        public override bool IsValid(object value)
        {
            if (value != null)
            {
                return Convert.ToDecimal(value) <= Convert.ToDecimal(_maxValue);
            }
            else
            {
                return true;
            }

        }
    }

    public class OrderNoteViewModel : DashboardOrderViewModel
    {
        public OrderNoteViewModel()
        {
            OrderNoteList = new List<OrderNoteDetailViewModel>();
        }
        public int OrderNoteId { get; set; }

        [Required(ErrorMessage = "Please enter notes")]
        public string Note { get; set; }

        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<OrderNoteDetailViewModel> OrderNoteList { get; set; }
    }

    public class OrderNoteDetailViewModel
    {
        public string Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }

    public class OrderImageViewModel
    {
        public OrderImageViewModel()
        {

            CustomerShipingAddressList = new List<CustmoerShippingViewModel>();
            OrderList = new List<ServiceFormOrderViewModel>();
        }
        public int JobImageId { get; set; }

        [Required(ErrorMessage = "Please select order")]
        public int OrderId { get; set; }
        public int ShipId { get; set; }
        public string ShipAddress { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public Byte[] Base64 { get; set; }
        public string IsFollowUp { get; set; }
        public Int16 IsActive { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public List<CustmoerShippingViewModel> CustomerShipingAddressList { get; set; }
        public List<ServiceFormOrderViewModel> OrderList { get; set; }
    }


    public class ComboOfferMasterViewModel
    {
        public ComboOfferMasterViewModel()
        {
            ItemList = new List<ComboOfferDetailViewModel>();
        }
        public int ComboOfferMasterId { get; set; }
        public string ComboOfferName { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public List<ComboOfferDetailViewModel> ItemList { get; set; }

    }

    public class ComboOfferDetailViewModel
    {
        public int ComboOfferDetailId { get; set; }
        public int ComboOfferMasterId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ItemName { get; set; }
    }


    public class OrderViewModel
    {
        public OrderViewModel() {
            ItemList = new List<ItemMasterViewModel>();
            OrderItems = new List<OrderItemsViewModel>();
            ContactList = new List<OrderCustomerDetailViewModel>();
        }
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public string ContactNo { get; set; }
        public string Name { get; set; }
        public decimal TotalAmount { get; set; }
        public  string Remarks { get; set; }
        public List<ItemMasterViewModel> ItemList { get; set; }
        public List<OrderItemsViewModel> OrderItems { get; set; }
        public List<OrderCustomerDetailViewModel> ContactList { get; set; }
    }

    public class OrderItemsViewModel
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int ComboOfferId { get; set; }
        public int Quantity { get; set; }
        public int Amount { get; set; }
        public string IsCombo { get; set; }
        public string Description { get; set; }
    }

    public class OrderCustomerDetailViewModel
    {
        public string ContactNo { get; set; }
        public string Name { get; set; }

    }


}