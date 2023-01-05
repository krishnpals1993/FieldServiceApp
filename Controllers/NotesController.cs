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
using JqueryDataTables.ServerSide.AspNetCoreWeb.Models;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Text.RegularExpressions;

namespace LaCafelogy.Controllers
{
    public class NotesController : Controller
    {
        private readonly IOptions<Appsettings> _appSettings;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly DBContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        OrderUtility _orderUtility;
        private readonly IConfigurationProvider _mappingConfiguration;

        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private string _rolename = "";
        private int _userId = 0;
        public NotesController(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings, DBContext dbContext, IHttpContextAccessor HttpContextAccessor, IConfigurationProvider mappingConfiguration)
        {
            _appSettings = appSettings;
            _emailSettings = emailSettings;
            _dbContext = dbContext;
            _httpContextAccessor = HttpContextAccessor;
            _rolename = _session.GetString("RoleName");
            int.TryParse(_session.GetString("UserId"), out _userId);
            _orderUtility = new OrderUtility(_dbContext);
            _mappingConfiguration = mappingConfiguration;
        }


        public IActionResult List()
        {
            List<OrderMasterViewModel> OrderList = new List<OrderMasterViewModel>();
             
            {
                var _employeeId = _dbContext.tbl_EmployeeMaster.Where(w => w.UserId == _userId).Select(s => s.EmployeeId).FirstOrDefault();
                 OrderList = (from order in _dbContext.tbl_OrderMaster
                                   join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustomerId
                                   join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                                   into orderAssign
                                   from orderAssign1 in orderAssign.DefaultIfEmpty()
                                   join employee in _dbContext.tbl_EmployeeMaster on orderAssign1.EmployeeId equals employee.EmployeeId
                                    into employee
                                   from employee1 in employee.DefaultIfEmpty()
                                   join shipping in _dbContext.tbl_CustmoerShipping on order.ShipId equals shipping.CustomerShipId
                                   join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                    into city
                                   from city1 in city.DefaultIfEmpty()
                                   join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                   into state
                                   from state1 in state.DefaultIfEmpty()
                                join note in _dbContext.tbl_OrderNotes on order.OrderId equals note.OrderId
                                let items = (from orderDetail in _dbContext.tbl_OrderDetail
                                                join item in _dbContext.tbl_ItemMaster on orderDetail.ItemId equals item.ItemId
                                                where item.ItemId == orderDetail.ItemId && orderDetail.OrderId == order.OrderId && order.IsActive == 1
                                                select item.ItemCd).Distinct().ToList()
                                   
                                   where orderAssign1.EmployeeId == _employeeId && order.IsActive == 1
                                   select new OrderMasterViewModel
                                   {
                                       OrderNo = order.OrderNo,
                                       OrderId = order.OrderId,
                                       OrderDate = order.OrderDate,
                                       ShipStartDate = order.ShipStartDate,
                                       ShipEndDate = order.ShipEndDate,
                                       CustomerName = customer.CompanyName,
                                       EmployeeName = (employee1.FirstName + " " + (employee1.MiddleName ?? "") + " " + employee1.LastName) ?? "",
                                       TotalAmount = order.TotalAmount,
                                       EmployeeId = orderAssign1.EmployeeId,
                                       ItemName = String.Join(',', items),
                                       //CustomerShipAddress = city1.CityName + " " + state1.StateName + " " + shipping.Address,
                                       CustomerShipAddress = (city1.CityName ?? "") + " " + (state1.StateName ?? "") + " " + (shipping.Address ?? "") + (shipping.Address2 ?? ""),
                                       Status = (orderAssign1 == null ? "New" : (orderAssign1.CompletedDate == null ? "Assigned" : "Completed")),
                                       Color = employee1.Color ?? "rgb(228 211 91 / 63%)",
                                       ReOccurenceParentOrderId = ((order.ReOccurence ?? 0) == 1 ? 1 : (order.ReOccurenceParentOrderId ?? 0)),
                                       Notes =  note.Note,
                                       CreatedDate = order.CreatedDate
                                   })
                              .ToList();
            }
            return View(OrderList);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}