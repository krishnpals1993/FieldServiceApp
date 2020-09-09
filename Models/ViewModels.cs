
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FieldServiceApp.Models
{
    public class ResponseModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
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

        public int CustmoerId { get; set; }

        [Required(ErrorMessage = "Please enter company name")]
        public string CompanyName { get; set; }

       
        public string Address { get; set; }

        public int ? CityId { get; set; }

        public int ?  StateId { get; set; }

        public string FirstName { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public CustmoerContactViewModel Contact { get; set; }
        public List<CustmoerContactViewModel> Contacts { get; set; }
        public CustmoerShippingViewModel Shipping { get; set; }
        public List<CustmoerShippingViewModel> Shippings { get; set; }
        public List<StateViewModel> StateList { get; set; }
        public List<CityViewModel> CityList { get; set; }
    }
    public class CustmoerContactViewModel
    {
        public int CustmoerContactId { get; set; }

        public int ShipId { get; set; }
        public int CustmoerId { get; set; }

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
    public class CustmoerShippingViewModel
    {
        public CustmoerShippingViewModel()
        {
            
            StateList = new List<StateViewModel>();
            CityList = new List<CityViewModel>();
        }
        public int ShipId { get; set; }
        public int CustmoerId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<StateViewModel> StateList { get; set; }
        public List<CityViewModel> CityList { get; set; }
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
            ItemList = new List<ItemMasterViewModel>();
            EmployeeList = new List<EmployeeMasterViewModel>();
            CustomerList = new List<CustomerMasterViewModel>();
            CustomerShipingAddressList = new List<CustmoerShippingViewModel>();
        }
        public int OrderId { get; set; }

        [Display(Name = "Order Date")]
        [Required(ErrorMessage = "Please enter order date")]
        [DataType(DataType.DateTime, ErrorMessage = "Please enter valid date")]
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
        public DateTime? ShipEndDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public string CancelReason { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "Item")]
        [Required(ErrorMessage = "Please select item")]
        public int ItemId { get; set; }
        public string Description { get; set; }
        public int UnitId { get; set; }

        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "Please enter quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Per Unit Price")]
        [Required(ErrorMessage = "Please enter per unit price")]
        public decimal PerUnitPrice { get; set; }

        [Display(Name = "Total Price")]
        [Required(ErrorMessage = "Please enter total price")]
        public decimal TotalPrice { get; set; }
        public int EmployeeId { get; set; }

        public string AssigneeId { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public string CustomerShipAddress { get; set; }
        public string Status { get; set; }
        public List<ItemMasterViewModel> ItemList { get; set; }
        public List<EmployeeMasterViewModel> EmployeeList { get; set; }
        public List<CustomerMasterViewModel> CustomerList { get; set; }
        public List<CustmoerShippingViewModel> CustomerShipingAddressList { get; set; }

    }
    public class OrderDetailViewModel
    {
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
        public int CityId { get; set; }

        public string CityName { get; set; }

        public int StateId { get; set; }
        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
    public class StateViewModel
    {

        public int StateId { get; set; }

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
       

    }

    public class DashboardViewModel
    {
        public List<OrderMasterViewModel> OrderList { get; set; }
        public List<EmployeeMasterViewModel> EmployeeList { get; set; }

        public DashboardViewModel()
        {
            OrderList = new List<OrderMasterViewModel>();
            EmployeeList = new List<EmployeeMasterViewModel>();
        }

    }

    public class DashboardOrderViewModel
    {
        public DashboardOrderViewModel()
        {
            EmployeeList = new List<EmployeeMasterViewModel>();
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
    }

    public class ServiceFormViewModel
    {
        public ServiceFormViewModel()
        {
            ItemList = new List<ItemMasterViewModel>();
            CustomerShipingAddressList = new List<CustmoerShippingViewModel>();
            OrderList = new List<ServiceFormOrderViewModel>();
            ItemCategoryList = new List<ItemCategoryViewModel>();
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

        [Required(ErrorMessage ="Please enter category name")]
        public string CategoryName { get; set; }
        public string RoleIds { get; set; }

        public Int16 IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }


}