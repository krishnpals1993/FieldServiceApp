using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using FieldServiceApp.Filters;
using FieldServiceApp.Models;
using FieldServiceApp.Utility;
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

namespace FieldServiceApp.Controllers
{
    //[Authentication]
    public class HomeController : Controller
    {
        private readonly IOptions<Appsettings> _appSettings;
        private readonly DBContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private string _rolename = "";
        private int _userId = 0;

        public HomeController(IOptions<Appsettings> appSettings, DBContext dbContext, IHttpContextAccessor HttpContextAccessor)
        {
            _appSettings = appSettings;
            _dbContext = dbContext;
            _httpContextAccessor = HttpContextAccessor;
            _rolename = _session.GetString("RoleName");
            int.TryParse(_session.GetString("UserId"), out _userId);

        }

        public ActionResult Dashboard()
        {
            DashboardViewModel model = new DashboardViewModel();
            model.EmployeeList = _dbContext.tbl_EmployeeMaster
                               .Select(s => new EmployeeMasterViewModel
                               {
                                   EmployeeId = s.EmployeeId,
                                   FirstName = s.FirstName,
                                   LastName = s.LastName,
                                   MiddleName = s.MiddleName
                               })
                               .ToList();
            try
            {
                if (_rolename == "Admin")
                {
                    model.OrderList = (from order in _dbContext.tbl_OrderMaster
                                       join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustmoerId
                                       join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                                       into orderAssign
                                       from orderAssign1 in orderAssign.DefaultIfEmpty()
                                       join employee in _dbContext.tbl_EmployeeMaster on orderAssign1.EmployeeId equals employee.EmployeeId
                                        into employee
                                       from employee1 in employee.DefaultIfEmpty()
                                       join shipping in _dbContext.tbl_CustmoerShipping on order.ShipId equals shipping.ShipId
                                       join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                       join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                       select new OrderMasterViewModel
                                       {
                                           OrderId = order.OrderId,
                                           OrderDate = order.OrderDate,
                                           ShipStartDate = order.ShipStartDate,
                                           ShipEndDate = order.ShipEndDate,
                                           CustomerName = customer.CompanyName,
                                           EmployeeName = employee1.FirstName + " " + (employee1.MiddleName ?? "") + " " + employee1.LastName,
                                           TotalAmount = order.TotalAmount,
                                           EmployeeId = orderAssign1.EmployeeId,
                                           CustomerShipAddress = city.CityName + " " + state.StateName + " " + shipping.Address,
                                           Status = (orderAssign1 == null ? "New" : (orderAssign1.CompletedDate == null ? "Assigned" : "Completed")),


                                       })
                                               .ToList();
                }
                else
                {
                    var _employeeId = _dbContext.tbl_EmployeeMaster.Where(w => w.UserId == _userId).Select(s => s.EmployeeId).FirstOrDefault();
                    model.OrderList = (from order in _dbContext.tbl_OrderMaster
                                       join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustmoerId
                                       join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                                       join employee in _dbContext.tbl_EmployeeMaster on orderAssign.EmployeeId equals employee.EmployeeId
                                       join shipping in _dbContext.tbl_CustmoerShipping on order.ShipId equals shipping.ShipId
                                       join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                       join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                       where employee.EmployeeId == _employeeId
                                       select new OrderMasterViewModel
                                       {
                                           OrderId = order.OrderId,
                                           OrderDate = order.OrderDate,
                                           ShipStartDate = order.ShipStartDate,
                                           ShipEndDate = order.ShipEndDate,
                                           CustomerName = customer.CompanyName,
                                           EmployeeName = employee.FirstName + " " + (employee.MiddleName ?? "") + " " + employee.LastName,
                                           TotalAmount = order.TotalAmount,
                                           EmployeeId = orderAssign.EmployeeId,
                                           CustomerShipAddress = city.CityName + " " + state.StateName + " " + shipping.Address,
                                           Status = (orderAssign == null ? "New" : (orderAssign.CompletedDate == null ? "Assigned" : "Completed"))

                                       })
                                  .ToList();
                }


                return View(model);


            }
            catch (Exception ex)
            {


            }

            return View(model);
        }

        public IActionResult GetOrderPopup(int id)
        {
            DashboardOrderViewModel model = new DashboardOrderViewModel();
            model = (from order in _dbContext.tbl_OrderMaster
                     join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustmoerId
                     join orderDetail in _dbContext.tbl_OrderDetail on order.OrderId equals orderDetail.OrderId
                     join item in _dbContext.tbl_ItemMaster on orderDetail.ItemId equals item.ItemId
                     join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                     into orderAssign
                     from orderAssign1 in orderAssign.DefaultIfEmpty()
                     join employee in _dbContext.tbl_EmployeeMaster on orderAssign1.EmployeeId equals employee.EmployeeId
                      into employee
                     from employee1 in employee.DefaultIfEmpty()
                     join shipping in _dbContext.tbl_CustmoerShipping on order.ShipId equals shipping.ShipId
                     join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                     join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                     where order.OrderId == id
                     select new DashboardOrderViewModel
                     {
                         OrderId = order.OrderId,
                         ShipStartDate = order.ShipStartDate,
                         ShipEndDate = order.ShipEndDate,
                         CustomerName = customer.CompanyName,
                         EmployeeName = employee1.FirstName + " " + (employee1.MiddleName ?? "") + " " + employee1.LastName,
                         EmployeeId = orderAssign1.EmployeeId,
                         CustomerShipAddress = city.CityName + " " + state.StateName + " " + shipping.Address,
                         ItemName = item.ItemCd,
                         ItemDescription = item.ItemDescription,
                     }).FirstOrDefault();

            model.EmployeeList = _dbContext.tbl_EmployeeMaster
                            .Select(s => new EmployeeMasterViewModel
                            {
                                EmployeeId = s.EmployeeId,
                                FirstName = s.FirstName,
                                LastName = s.LastName,
                                MiddleName = s.MiddleName
                            })
                            .ToList();

            return PartialView("_DashboardOrder", model);

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}