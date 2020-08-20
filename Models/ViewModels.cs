
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
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string RoleName { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please enter first name")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Please enter last name")]
        public string LastName { get; set; }

        [Display(Name = "Employee No")]
        [Required(ErrorMessage = "Please enter employee no")]
        public string EmployeeNo { get; set; }

        [Display(Name = "Hr Group")]
        [Required(ErrorMessage = "Please select hr group")]
        public int HrGroupId { get; set; }
        public string HrGroupName { get; set; }

    }
    public class RoleViewModel
    {

        public int RoleId { get; set; }

        public string Rolename { get; set; }

        public bool? IsActive { get; set; }
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


}