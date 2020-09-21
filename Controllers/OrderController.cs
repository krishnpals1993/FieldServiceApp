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

namespace FieldServiceApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOptions<Appsettings> _appSettings;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly DBContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private string _rolename = "";
        private int _userId = 0;
        public OrderController(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings, DBContext dbContext, IHttpContextAccessor HttpContextAccessor)
        {
            _appSettings = appSettings;
            _emailSettings = emailSettings;
            _dbContext = dbContext;
            _httpContextAccessor = HttpContextAccessor;
            _rolename = _session.GetString("RoleName");
            int.TryParse(_session.GetString("UserId"), out _userId);
        }


        public IActionResult Add()
        {
            OrderMasterViewModel model = new OrderMasterViewModel();
            try
            {
                model.ItemList = _dbContext.tbl_ItemMaster
                    .Where(w => w.IsActive == 1)
                            .Select(s => new ItemMasterViewModel
                            {
                                ItemId = s.ItemId,
                                ItemCd = s.ItemCd,
                                ItemPrice = s.ItemPrice
                            })
                            .ToList();
                model.CustomerList = _dbContext.tbl_CustomerMaster
                    .Where(w => w.IsActive == 1)
                                .Select(s => new CustomerMasterViewModel
                                {
                                    CustmoerId = s.CustmoerId,
                                    CompanyName = s.CompanyName
                                })
                                .ToList();



                model.EmployeeList = _dbContext.tbl_EmployeeMaster
                    .Where(w => w.IsActive == 1)
                                .Select(s => new EmployeeMasterViewModel
                                {
                                    EmployeeId = s.EmployeeId,
                                    FirstName = s.FirstName,
                                    LastName = s.LastName,
                                    MiddleName = s.MiddleName
                                })
                                .ToList();
            }
            catch (Exception ex)
            {

                var a = "";
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult Add(OrderMasterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CommanUtility _commanUtility = new CommanUtility(_appSettings);
                    var shipDate = _commanUtility.RoundUp(model.ShipStartDate.Value, TimeSpan.FromMinutes(15));

                    OrderMaster orderMaster = new OrderMaster()
                    {
                        OrderDate = model.OrderDate,
                        ShipStartDate = shipDate,
                        ShipEndDate = model.ShipEndDate,
                        ShipDate = model.ShipStartDate,
                        ShipId = model.ShipId,
                        CustomerId = model.CustomerId,
                        TotalAmount = model.TotalAmount,
                        IsActive = 1,
                        CreatedBy = 1,
                        CreatedDate = DateTime.Now,

                    };

                    _dbContext.tbl_OrderMaster.Add(orderMaster);
                    _dbContext.SaveChanges();

                    OrderDetail orderDetail = new OrderDetail()
                    {
                        ItemId = model.ItemId,
                        Description = model.Description,
                        OrderId = orderMaster.OrderId,
                        PerUnitPrice = model.PerUnitPrice,
                        Quantity = model.Quantity,
                        TotalPrice = model.TotalPrice,
                        UnitId = model.UnitId,
                        IsActive = 1,
                        CreatedBy = 1,
                        CreatedDate = DateTime.Now
                    };
                    _dbContext.tbl_OrderDetail.Add(orderDetail);
                    _dbContext.SaveChanges();

                    if (Convert.ToString(model.AssigneeId) != "")
                    {
                        model.EmployeeId = Convert.ToInt32(model.AssigneeId);
                        OrderAssignment orderAssignment = new OrderAssignment()
                        {
                            OrderId = orderMaster.OrderId,
                            EmployeeId = model.EmployeeId,
                            AssignmentDate = DateTime.Now,
                            Status = "Assigned"
                        };
                        _dbContext.tbl_OrderAssignment.Add(orderAssignment);
                        _dbContext.SaveChanges();
                    }




                    ViewBag.SuccessMessage = "Order added successfully";

                }
            }
            catch (Exception ex)
            {
                var a = "";
            }

            return View(model);
        }
        public IActionResult List()
        {

            List<OrderMasterViewModel> orders = (from order in _dbContext.tbl_OrderMaster
                                                 join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustmoerId
                                                 join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                                                 into orderAssign
                                                 from orderAssign1 in orderAssign.DefaultIfEmpty()
                                                 join employee in _dbContext.tbl_EmployeeMaster on orderAssign1.EmployeeId equals employee.EmployeeId
                                                 into employee
                                                 from employee1 in employee.DefaultIfEmpty()
                                                 select new OrderMasterViewModel
                                                 {
                                                     OrderId = order.OrderId,
                                                     OrderDate = order.OrderDate,
                                                     ShipStartDate = order.ShipStartDate,
                                                     CustomerName = customer.CompanyName,
                                                     EmployeeName = employee1.FirstName + " " + (employee1.MiddleName ?? "") + " " + employee1.LastName,
                                                     TotalAmount = order.TotalAmount,
                                                     IsActive = order.IsActive

                                                 })
                                                 .ToList();
            return View(orders);

        }

        [HttpPost]
        public JsonResult UpdateOrderDate(int orderId, DateTime? start, DateTime? end)
        {
            DashboardOrderViewModel model = new DashboardOrderViewModel();
            try
            {
                var checkOrder = _dbContext.tbl_OrderMaster.Where(w => w.OrderId == orderId).FirstOrDefault();
                if (checkOrder != null)
                {
                    checkOrder.ShipStartDate = start;
                    checkOrder.ShipDate = start;
                    checkOrder.ShipEndDate = end;
                    _dbContext.SaveChanges();
                }
                DashboardUtility _dashboardUtility = new DashboardUtility(_dbContext);
                model  = _dashboardUtility.getOrderDetail(orderId);
            }
            catch (Exception ex)
            {
            }

            return Json(model);
        }

        [HttpPost]
        public JsonResult UpdateOrderAssignee(int orderId, int employeeId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var checkOrder = _dbContext.tbl_OrderAssignment.Where(w => w.OrderId == orderId).FirstOrDefault();
                if (checkOrder != null)
                {
                    checkOrder.EmployeeId = employeeId;
                    _dbContext.SaveChanges();
                }
                response.Status = "1";
                response.Message = "Detail updated successfully";
            }
            catch (Exception ex)
            {
            }

            return Json(response);
        }

        [HttpPost]
        public JsonResult GetCustomerShippingAddress(int CustomerId)
        {
            var CustomerShipingAddressList = new List<CustmoerShippingViewModel>(); ;
            try
            {
                CustomerShipingAddressList = (from shipping in _dbContext.tbl_CustmoerShipping
                                              join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                              join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                              where shipping.CustmoerId == CustomerId
                                              select new CustmoerShippingViewModel
                                              {
                                                  ShipId = shipping.ShipId,
                                                  Address = city.CityName + " " + state.StateName + " " + shipping.Address ?? ""
                                              })
                                                   .ToList();
            }
            catch (Exception ex)
            {

            }

            return Json(CustomerShipingAddressList);
        }


        public IActionResult Edit(string id)
        {
            OrderMasterViewModel model = new OrderMasterViewModel();
            try
            {
                model.ItemList = _dbContext.tbl_ItemMaster
                    .Where(w => w.IsActive == 1)
                            .Select(s => new ItemMasterViewModel
                            {
                                ItemId = s.ItemId,
                                ItemCd = s.ItemCd,
                                ItemPrice = s.ItemPrice
                            })
                            .ToList();
                model.CustomerList = _dbContext.tbl_CustomerMaster
                    .Where(w => w.IsActive == 1)
                                .Select(s => new CustomerMasterViewModel
                                {
                                    CustmoerId = s.CustmoerId,
                                    CompanyName = s.CompanyName
                                })
                                .ToList();

                model.EmployeeList = _dbContext.tbl_EmployeeMaster
                    .Where(w => w.IsActive == 1)
                                .Select(s => new EmployeeMasterViewModel
                                {
                                    EmployeeId = s.EmployeeId,
                                    FirstName = s.FirstName,
                                    LastName = s.LastName,
                                    MiddleName = s.MiddleName
                                })
                                .ToList();

                int orderId = 0;
                int.TryParse(id, out orderId);
                var checkOrder = _dbContext.tbl_OrderMaster.Where(w => w.OrderId == orderId).FirstOrDefault();
                if (checkOrder != null)
                {
                    model.OrderId = orderId;
                    model.OrderDate = checkOrder.OrderDate;
                    model.ShipStartDate = checkOrder.ShipStartDate;
                    model.ShipEndDate = checkOrder.ShipEndDate;
                    model.ShipId = checkOrder.ShipId;
                    model.CustomerId = checkOrder.CustomerId;
                    model.TotalAmount = checkOrder.TotalAmount;

                    var orderDetail = _dbContext.tbl_OrderDetail.Where(w => w.OrderId == orderId).FirstOrDefault();

                    if (orderDetail != null)
                    {
                        model.ItemId = orderDetail.ItemId;
                        model.Description = orderDetail.Description;
                        model.PerUnitPrice = orderDetail.PerUnitPrice;
                        model.Quantity = orderDetail.Quantity;
                        model.TotalPrice = orderDetail.TotalPrice;
                        model.UnitId = orderDetail.UnitId;

                    }

                    var orderAssignment = _dbContext.tbl_OrderAssignment.Where(w => w.OrderId == orderId).FirstOrDefault();

                    if (orderAssignment != null)
                    {
                        model.AssigneeId = orderAssignment.EmployeeId.ToString();
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
        public ActionResult Edit(OrderMasterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var checkOrder = _dbContext.tbl_OrderMaster.Where(w => w.OrderId == model.OrderId).FirstOrDefault();
                    if (checkOrder != null)
                    {
                        checkOrder.OrderDate = model.OrderDate;
                        checkOrder.ShipStartDate = model.ShipStartDate;
                        checkOrder.ShipEndDate = model.ShipEndDate;
                        checkOrder.ShipId = model.ShipId;
                        checkOrder.CustomerId = model.CustomerId;
                        checkOrder.TotalAmount = model.TotalAmount;
                        checkOrder.ModifiedBy = 1;
                        checkOrder.ModifiedDate = DateTime.Now;
                        _dbContext.SaveChanges();

                        var checkOrderDetail = _dbContext.tbl_OrderDetail.Where(w => w.OrderId == model.OrderId).FirstOrDefault();
                        if (checkOrderDetail != null)
                        {
                            checkOrderDetail.ItemId = model.ItemId;
                            checkOrderDetail.Description = model.Description;
                            checkOrderDetail.OrderId = checkOrder.OrderId;
                            checkOrderDetail.PerUnitPrice = model.PerUnitPrice;
                            checkOrderDetail.Quantity = model.Quantity;
                            checkOrderDetail.TotalPrice = model.TotalPrice;
                            checkOrderDetail.UnitId = model.UnitId;

                            _dbContext.SaveChanges();

                            {
                                var checkOrderAssignment = _dbContext.tbl_OrderAssignment.Where(w => w.OrderId == model.OrderId).FirstOrDefault();
                                if (checkOrderAssignment != null)
                                {
                                    if (Convert.ToString(model.AssigneeId) != "")
                                    {
                                        model.EmployeeId = Convert.ToInt32(model.AssigneeId);
                                        checkOrderAssignment.EmployeeId = model.EmployeeId;
                                        _dbContext.SaveChanges();
                                    }
                                    else
                                    {
                                        checkOrderAssignment.EmployeeId = 0;
                                        _dbContext.SaveChanges();
                                    }
                                }
                                else
                                {
                                    if (Convert.ToString(model.AssigneeId) != "")
                                    {
                                        OrderAssignment orderAssignment = new OrderAssignment()
                                        {
                                            OrderId = checkOrder.OrderId,
                                            EmployeeId = model.EmployeeId,
                                            AssignmentDate = DateTime.Now,
                                            Status = "Assigned"
                                        };
                                        _dbContext.tbl_OrderAssignment.Add(orderAssignment);
                                        _dbContext.SaveChanges();

                                    }
                                }



                            }


                        }

                    }

                    ViewBag.SuccessMessage = "Order update successfully";

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

                var checkOrder = _dbContext.tbl_OrderMaster.Where(w => w.OrderId == id).FirstOrDefault();
                if (checkOrder != null)
                {
                    checkOrder.IsActive = 0;
                    checkOrder.ModifiedBy = 1;
                    checkOrder.ModifiedDate = DateTime.Now;
                    _dbContext.SaveChanges();

                    response.Status = "1";
                    response.Message = "Order deleted successfully";
                }




            }
            catch (Exception ex)
            {
                response.Status = "0";
                response.Message = "Error occurred";
            }

            return Json(response);
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}