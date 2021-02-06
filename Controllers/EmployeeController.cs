using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using FieldServiceApp.Models;
using FieldServiceApp.Utility;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using FieldServiceApp.Filters;

namespace FieldServiceApp.Controllers
{
    [Authentication]
    public class EmployeeController : Controller
    {
        private readonly IOptions<Appsettings> _appSettings;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly DBContext _dbContext;

        public EmployeeController(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings, DBContext dbContext)
        {
            _appSettings = appSettings;
            _emailSettings = emailSettings;
            _dbContext = dbContext;
        }
        public IActionResult List()
        {
            var employees = _dbContext.tbl_EmployeeMaster.Select(s => new EmployeeMasterViewModel
            {
                FirstName = s.FirstName,
                MiddleName = s.MiddleName,
                LastName = s.LastName,
                Phone = s.Phone,
                Email = s.Email,
                Color = s.Color,
                EmployeeId = s.EmployeeId,
                IsActive = s.IsActive
            }).ToList();
            return View(employees);
        }

        public IActionResult Add()
        {
            EmployeeMasterViewModel model = new EmployeeMasterViewModel();
            model.Color = "#269faf";
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(EmployeeMasterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var checkEmployee = _dbContext.tbl_EmployeeMaster.Where(w => w.Email == model.Email).FirstOrDefault();
                    if (checkEmployee != null)
                    {
                        ViewBag.ErrorMessage = "Employee already exists with this email";
                    }
                    else
                    {
                        var checkUser = _dbContext.tbl_Users.Where(w => w.Email == model.Email).FirstOrDefault();
                        if (checkUser != null)
                        {
                            ViewBag.ErrorMessage = "User already exists with this email";
                        }
                        else
                        {
                            var roleId = _dbContext.tbl_Roles.Where(w => w.RoleName == "Field User").Select(s => s.RoleId).FirstOrDefault();

                            User user = new User()
                            {
                                Email = model.Email,
                                UserName = model.Email,
                                Password = model.Password,
                                RoleId = roleId,
                                IsActive = 1,
                                CreatedBy = 1,
                                CreatedDate = DateTime.Now
                            };

                            _dbContext.tbl_Users.Add(user);
                            _dbContext.SaveChanges();

                            EmployeeMaster employeeMaster = new EmployeeMaster()
                            {
                                FirstName = model.FirstName,
                                MiddleName = model.MiddleName,
                                LastName = model.LastName,
                                Email = model.Email,
                                Phone = model.Phone,
                               Color = model.Color,
                                IsActive = 1,
                                CreatedBy = 1,
                                CreatedDate = DateTime.Now,
                                UserId = user.UserId,
                            };

                            _dbContext.tbl_EmployeeMaster.Add(employeeMaster);
                            _dbContext.SaveChanges();
                            ViewBag.SuccessMessage = "Employee added successfully";
                        }



                    }
                }
            }
            catch (Exception ex)
            {
                var a = "";
            }

            return View(model);
        }


        public IActionResult Edit(string id)
        {
            EmployeeMasterEditViewModel model = new EmployeeMasterEditViewModel();
            int employeeId = 0;
            int.TryParse(id, out employeeId);
            var checkEmployee = _dbContext.tbl_EmployeeMaster.Where(w => w.EmployeeId == employeeId).FirstOrDefault();
            if (checkEmployee != null)
            {
                model.FirstName = checkEmployee.FirstName;
                model.MiddleName = checkEmployee.MiddleName;
                model.LastName = checkEmployee.LastName;
                model.Email = checkEmployee.Email;
                model.Phone = checkEmployee.Phone;
                model.EmployeeId = checkEmployee.EmployeeId;
                model.Color = String.IsNullOrEmpty(checkEmployee.Color) ? "#269faf": checkEmployee.Color;
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EmployeeMasterEditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var checkEmployee = _dbContext.tbl_EmployeeMaster.Where(w => w.Email == model.Email && w.EmployeeId != model.EmployeeId).FirstOrDefault();
                    if (checkEmployee != null)
                    {
                        ViewBag.ErrorMessage = "Employee already exists with this email";
                    }
                    else
                    {

                        checkEmployee = _dbContext.tbl_EmployeeMaster.Where(w => w.EmployeeId == model.EmployeeId).FirstOrDefault();

                        checkEmployee.FirstName = model.FirstName;
                        checkEmployee.MiddleName = model.MiddleName;
                        checkEmployee.LastName = model.LastName;
                        checkEmployee.Email = model.Email;
                        checkEmployee.Phone = model.Phone;
                        checkEmployee.Color = model.Color;
                        _dbContext.SaveChanges();

                        ViewBag.SuccessMessage = "Employee updated successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                var a = "";
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            ResponseModel response = new ResponseModel();
            try
            {

                var checkOrderAssignment = _dbContext.tbl_OrderAssignment.Where(w => w.EmployeeId == id && w.CompletedBy==0).FirstOrDefault();
                if (checkOrderAssignment != null)
                {
                    response.Status = "0";
                    response.Message = "Order has been assigned to this employee";

                }
                else
                {
                    var checkEmployee = _dbContext.tbl_EmployeeMaster.Where(w => w.EmployeeId == id).FirstOrDefault();
                    if (checkEmployee != null)
                    {
                        checkEmployee.IsActive = 0;
                        checkEmployee.ModifiedBy = 1;
                        checkEmployee.ModifiedDate = DateTime.Now;
                        _dbContext.SaveChanges();

                        response.Status = "1";
                        response.Message = "Employee deleted successfully";
                    }

                }

              


            }
            catch (Exception ex)
            {
                response.Status = "0";
                response.Message = "Error occurred";
            }

            return Json(response);
        }

    }
}