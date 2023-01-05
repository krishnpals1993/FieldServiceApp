using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using LaCafelogy.Models;
using LaCafelogy.Utility;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using LaCafelogy.Filters;

namespace LaCafelogy.Controllers
{
    [Authentication]
    public class UserController : Controller
    {
        private readonly IOptions<Appsettings> _appSettings;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly DBContext _dbContext;

        public UserController(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings, DBContext dbContext)
        {
            _appSettings = appSettings;
            _emailSettings = emailSettings;
            _dbContext = dbContext;
        }
        public IActionResult RoleList()
        {
            var roles = _dbContext.tbl_Roles.Select(s => new RoleViewModel
            {
                Rolename = s.RoleName,
                IsActive = s.IsActive
            }).ToList();

            return View(roles);
        }

        public IActionResult UserList()
        {
            List<UserViewModel> users = (from user in _dbContext.tbl_Users
                                         join role in _dbContext.tbl_Roles on user.RoleId equals role.RoleId
                                         join employee in _dbContext.tbl_EmployeeMaster on user.UserId equals employee.UserId
                                         into employee
                                         from employee1 in employee.DefaultIfEmpty()
                                         select new UserViewModel
                                         {
                                             RoleName = role.RoleName,
                                             Email = user.Email,
                                             EmployeeName = employee1.FirstName + " " + (employee1.MiddleName ?? "") + " " + employee1.LastName,
                                             IsActive = user.IsActive
                                         })
                                                   .ToList();

            return View(users);
        }


        public IActionResult AddUser()
        {
            UserViewModel model = new UserViewModel();
            model.RoleList = _dbContext.tbl_Roles.Select(s => new RoleViewModel
            {
                Rolename = s.RoleName,
                RoleId = s.RoleId

            }).ToList();

            model.EmployeeList = _dbContext.tbl_EmployeeMaster.Where(w=>w.UserId==0)
                              .Select(s => new EmployeeMasterViewModel
                              {
                                  EmployeeId = s.EmployeeId,
                                  FirstName = s.FirstName,
                                  LastName = s.LastName,
                                  MiddleName = s.MiddleName
                              })
                              .ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult AddUser(UserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var checkUser = _dbContext.tbl_Users.Where(w => w.Email == model.Email).FirstOrDefault();
                    if (checkUser != null)
                    {
                        ViewBag.ErrorMessage = "User already exists with this email";
                    }
                    else
                    {
                        int iRoleId = 0;
                        int.TryParse(model.RoleId,out iRoleId);
                        model.iRoleId = iRoleId;
                        User user = new User()
                        {
                            Email = model.Email,
                            UserName = model.Username,
                            Password = model.Password,
                            RoleId = model.iRoleId,
                            IsActive = 1,
                            CreatedBy = 1,
                            CreatedDate = DateTime.Now
                        };

                        _dbContext.tbl_Users.Add(user);
                        _dbContext.SaveChanges();



                        ViewBag.SuccessMessage = "User added successfully";

                    }
                }
            }
            catch (Exception ex)
            {
                var a = "";
            }

            model.RoleList = _dbContext.tbl_Roles.Select(s => new RoleViewModel
            {
                Rolename = s.RoleName,
                RoleId = s.RoleId

            }).ToList();

            model.EmployeeList = _dbContext.tbl_EmployeeMaster.Where(w => w.UserId == 0)
                              .Select(s => new EmployeeMasterViewModel
                              {
                                  EmployeeId = s.EmployeeId,
                                  FirstName = s.FirstName,
                                  LastName = s.LastName,
                                  MiddleName = s.MiddleName
                              })
                              .ToList();

            return View(model);
        }

    }
}