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
    public class ServiceController : Controller
    {

        private readonly IOptions<Appsettings> _appSettings;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly DBContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private int _userId = 0;
        private int _employeeId = 0;

        public ServiceController(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings, DBContext dbContext, IHttpContextAccessor HttpContextAccessor)
        {
            _appSettings = appSettings;
            _emailSettings = emailSettings;
            _dbContext = dbContext;
            _httpContextAccessor = HttpContextAccessor;
            int.TryParse(_session.GetString("UserId"), out _userId);
            _employeeId = _dbContext.tbl_EmployeeMaster.Where(w => w.UserId == _userId).Select(s => s.EmployeeId).FirstOrDefault();
        }


        public IActionResult List()
        {
            var items = (from servicelog in _dbContext.tbl_ServiceFormLogs
                         join order in _dbContext.tbl_OrderMaster on servicelog.OrderId equals order.OrderId
                         join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustmoerId
                         join item in _dbContext.tbl_ItemMaster on servicelog.ItemId equals item.ItemId
                         join category in _dbContext.tbl_ItemCategory on item.CategoryId equals category.CategoryId
                         join apartment in _dbContext.tbl_CustomerShippingApartments on servicelog.ApartmentId equals apartment.ApartmentId
                         into apartment
                         from apartment1 in apartment.DefaultIfEmpty()
                         where servicelog.CreatedBy == _userId
                         select new ServiceFormViewModel
                         {
                             ServiceFormLogId = servicelog.ServiceFormLogId,
                             ItemId = item.ItemId,
                             ItemCd = item.ItemCd,
                             IsActive = servicelog.IsActive,
                             CategoryName = category.CategoryName,
                             Quantity = servicelog.Qty,
                             DateOfService = servicelog.DateOfService,
                             OrderId = order.OrderId,
                             CustomerName = customer.CompanyName,
                             Locations = servicelog.Locations,
                             ApartmentName = apartment1.ApartmentName,
                             ApartmentNo = apartment1.ApartmentNo

                         }).ToList();
            return View(items);
        }

        public IActionResult Work()
        {
            ServiceFormViewModel model = new ServiceFormViewModel();
            try
            {
                //model.ItemList = _dbContext.tbl_ItemMaster
                //    .Where(w => w.IsActive == 1)
                //            .Select(s => new ItemMasterViewModel
                //            {
                //                ItemId = s.ItemId,
                //                ItemCd = s.ItemCd,
                //                ItemPrice = s.ItemPrice
                //            })
                //            .ToList();

                model.ItemCategoryList = _dbContext.tbl_ItemCategory.Where(w => w.IsActive == 1).Select(s => new ItemCategoryViewModel
                {
                    CategoryId = s.CategoryId,
                    CategoryName = s.CategoryName

                }).ToList();

                model.OrderList = (from order in _dbContext.tbl_OrderMaster
                                   join customer in _dbContext.tbl_CustomerMaster
                                   on order.CustomerId equals customer.CustmoerId
                                   join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                                   where orderAssign.EmployeeId == _employeeId
                                   select new ServiceFormOrderViewModel
                                   {
                                       OrderId = order.OrderId,
                                       CustomerName = customer.CompanyName + " (Order #" + order.OrderId + ")"

                                   }).ToList();

            }
            catch (Exception ex)
            {

                var a = "";
            }


            return View(model);
        }


        [HttpPost]
        public ActionResult Work(ServiceFormViewModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    int itemCategoryId = 0;
                    int.TryParse(model.ServiceType, out itemCategoryId);

                    ServiceFormLog ServiceFormLog = new ServiceFormLog()
                    {

                        OrderId = model.OrderId,
                        DateOfService = model.DateOfService.Value,
                        ItemCategoryId = itemCategoryId,
                        ItemId = model.ItemId.Value,
                        Qty = model.Quantity.Value,
                        Locations = model.Locations,
                        IsActive = 1,
                        CreatedBy = _userId,
                        CreatedDate = DateTime.Now,
                        ApartmentId = model.ApartmentId
                    };

                    _dbContext.tbl_ServiceFormLogs.Add(ServiceFormLog);
                    _dbContext.SaveChanges();

                    ViewBag.SuccessMessage = "Detail added successfully";


                }



            }
            catch (Exception ex)
            {


            }

            model.ItemCategoryList = _dbContext.tbl_ItemCategory.Where(w => w.IsActive == 1).Select(s => new ItemCategoryViewModel
            {
                CategoryId = s.CategoryId,
                CategoryName = s.CategoryName

            }).ToList();

            model.OrderList = (from order in _dbContext.tbl_OrderMaster
                               join customer in _dbContext.tbl_CustomerMaster
                               on order.CustomerId equals customer.CustmoerId
                               join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                               where orderAssign.EmployeeId == _employeeId
                               select new ServiceFormOrderViewModel
                               {
                                   OrderId = order.OrderId,
                                   CustomerName = customer.CompanyName + " (Order #" + order.OrderId + ")"

                               }).ToList();

            model.ServiceType = "";
            model.Quantity = 0;
            model.Locations = "";


            return View(model);
        }


        public IActionResult Edit(int id)
        {
            ServiceFormViewModel model = new ServiceFormViewModel();
            try
            {
                var checkServiceLog = _dbContext.tbl_ServiceFormLogs.Where(w => w.ServiceFormLogId == id).FirstOrDefault();
                if (checkServiceLog != null)
                {
                    model.ItemId = checkServiceLog.ItemId;
                    model.ServiceType = checkServiceLog.ItemCategoryId.ToString();
                    model.OrderId = checkServiceLog.OrderId;
                    model.Locations = checkServiceLog.Locations;
                    model.Quantity = checkServiceLog.Qty;
                    model.DateOfService = checkServiceLog.DateOfService;
                    model.ServiceFormLogId = checkServiceLog.ServiceFormLogId;
                    model.ApartmentId = checkServiceLog.ApartmentId ?? 0;
                }

                var orderItemId = _dbContext.tbl_OrderDetail.Where(w => w.OrderId == model.OrderId).Select(s => s.ItemId).FirstOrDefault();

                var shipId = _dbContext.tbl_OrderMaster.Where(w => w.OrderId == model.OrderId).Select(s => s.ShipId).FirstOrDefault();

                model.ApartmentList = _dbContext.tbl_CustomerShippingApartments.Where(w => w.ShipId == shipId).
                  Select(s => new CustomerShippingApartmentViewModel()
                  {
                      ApartmentId = s.ApartmentId,
                      ApartmentNo = s.ApartmentNo,
                      ApartmentName = s.ApartmentName
                  }).ToList();

                model.ShipAddress = (from shipping in _dbContext.tbl_CustmoerShipping
                                     join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                     join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                     where shipping.ShipId == shipId
                                     select city.CityName + " " + state.StateName + " " + shipping.Address ?? ""
                                     ).FirstOrDefault();

                model.ItemList = _dbContext.tbl_ItemMaster
                    .Where(w => w.IsActive == 1 && w.CategoryId == checkServiceLog.ItemCategoryId
                        && w.ItemId != orderItemId)
                            .Select(s => new ItemMasterViewModel
                            {
                                ItemId = s.ItemId,
                                ItemCd = s.ItemCd,
                                ItemPrice = s.ItemPrice
                            })
                            .ToList();

                model.ItemCategoryList = _dbContext.tbl_ItemCategory.Where(w => w.IsActive == 1).Select(s => new ItemCategoryViewModel
                {
                    CategoryId = s.CategoryId,
                    CategoryName = s.CategoryName

                }).ToList();

                model.OrderList = (from order in _dbContext.tbl_OrderMaster
                                   join customer in _dbContext.tbl_CustomerMaster
                                   on order.CustomerId equals customer.CustmoerId
                                   join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                                   where orderAssign.EmployeeId == _employeeId
                                   select new ServiceFormOrderViewModel
                                   {
                                       OrderId = order.OrderId,
                                       CustomerName = customer.CompanyName + " (Order #" + order.OrderId + ")"

                                   }).ToList();

            }
            catch (Exception ex)
            {

                var a = "";
            }


            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(ServiceFormViewModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    int itemCategoryId = 0;
                    int.TryParse(model.ServiceType, out itemCategoryId);
                    var checkServiceFormLog = _dbContext.tbl_ServiceFormLogs.Where(w => w.ServiceFormLogId == model.ServiceFormLogId).FirstOrDefault();
                    if (checkServiceFormLog != null)
                    {
                        checkServiceFormLog.OrderId = model.OrderId;
                        checkServiceFormLog.DateOfService = model.DateOfService.Value;
                        checkServiceFormLog.ItemCategoryId = itemCategoryId;
                        checkServiceFormLog.ItemId = model.ItemId.Value;
                        checkServiceFormLog.Qty = model.Quantity.Value;
                        checkServiceFormLog.Locations = model.Locations;
                        checkServiceFormLog.ModifiedBy = _userId;
                        checkServiceFormLog.ModifiedDate = DateTime.Now;
                        _dbContext.SaveChanges();
                        ViewBag.SuccessMessage = "Detail added successfully";
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Detail not exists";
                    }



                }



            }
            catch (Exception ex)
            {


            }

            model.ItemCategoryList = _dbContext.tbl_ItemCategory.Where(w => w.IsActive == 1).Select(s => new ItemCategoryViewModel
            {
                CategoryId = s.CategoryId,
                CategoryName = s.CategoryName

            }).ToList();

            model.OrderList = (from order in _dbContext.tbl_OrderMaster
                               join customer in _dbContext.tbl_CustomerMaster
                               on order.CustomerId equals customer.CustmoerId
                               join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                               where orderAssign.EmployeeId == _employeeId
                               select new ServiceFormOrderViewModel
                               {
                                   OrderId = order.OrderId,
                                   CustomerName = customer.CompanyName + " (Order #" + order.OrderId + ")"

                               }).ToList();

            model.ServiceType = "";
            model.Quantity = 0;
            model.Locations = "";


            return View(model);
        }


        [HttpPost]
        public JsonResult GetCustomerShippingAddress(int OrderId)
        {
            CustmoerShippingViewModel model = new CustmoerShippingViewModel();
            string CustomerShipingAddress = "";
            try
            {
                var orderDetail = _dbContext.tbl_OrderMaster.Where(w => w.OrderId == OrderId).FirstOrDefault();
                CustomerShipingAddress = (from shipping in _dbContext.tbl_CustmoerShipping
                                          join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                          join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                          where shipping.ShipId == orderDetail.ShipId
                                          select city.CityName + " " + state.StateName + " " + shipping.Address ?? ""
                                           )
                                              .FirstOrDefault();
               
                model.Address = CustomerShipingAddress;
                model.ApartmentList = _dbContext.tbl_CustomerShippingApartments.Where(w => w.ShipId == orderDetail.ShipId).
                    Select(s => new CustomerShippingApartmentViewModel()
                    {
                        ApartmentId = s.ApartmentId,
                        ApartmentNo = s.ApartmentNo,
                        ApartmentName = s.ApartmentName
                    }).ToList();

                orderDetail.ApartmentIds = orderDetail.ApartmentIds ?? "";
                if (orderDetail.ApartmentIds!="")
                {
                    model.ApartmentList = model.ApartmentList.Where(w => orderDetail.ApartmentIds.Contains("/" + w.ApartmentId.ToString() + "/")).ToList();

                }




            }
            catch (Exception ex)
            {

            }

            return Json(model);
        }


        [HttpPost]
        public JsonResult GetItemByServiceType(int CategoryId, int OrderId)
        {
            var ItemList = new List<ItemMasterViewModel>(); ;
            try
            {
                var orderItemId = _dbContext.tbl_OrderDetail.Where(w => w.OrderId == OrderId)
                                  .Select(s => s.ItemId).FirstOrDefault();
                ItemList = _dbContext.tbl_ItemMaster
                    .Where(w => w.IsActive == 1 && w.CategoryId == CategoryId && w.ItemId != orderItemId)
                            .Select(s => new ItemMasterViewModel
                            {
                                ItemId = s.ItemId,
                                ItemCd = s.ItemCd,
                                ItemPrice = s.ItemPrice
                            })
                            .ToList();
            }
            catch (Exception ex)
            {

            }

            return Json(ItemList);
        }


        public IActionResult GetOrderPopup(int id)
        {
            DashboardOrderViewModel model = new DashboardOrderViewModel();
            model = (from order in _dbContext.tbl_OrderMaster
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
                     }).FirstOrDefault();



            return PartialView("_OrderDetail", model);

        }

        [HttpPost]
        public JsonResult DeleteWork(int id)
        {
            ResponseModel response = new ResponseModel();
            try
            {

                var checkService = _dbContext.tbl_ServiceFormLogs.Where(w => w.ServiceFormLogId == id).FirstOrDefault();
                if (checkService != null)
                {
                    checkService.IsActive = 0;
                    checkService.ModifiedBy = _userId;
                    checkService.ModifiedDate = DateTime.Now;
                    _dbContext.SaveChanges();

                    response.Status = "1";
                    response.Message = "Detail deleted successfully";
                }


            }
            catch (Exception ex)
            {
                response.Status = "0";
                response.Message = "Error occurred";
            }

            return Json(response);
        }


        public IActionResult ServiceFormLog(int id)
        {
            var items = (from servicelog in _dbContext.tbl_ServiceFormLogs
                         join order in _dbContext.tbl_OrderMaster on servicelog.OrderId equals order.OrderId
                         join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustmoerId
                         join item in _dbContext.tbl_ItemMaster on servicelog.ItemId equals item.ItemId
                         join category in _dbContext.tbl_ItemCategory on item.CategoryId equals category.CategoryId
                         where servicelog.CreatedBy == _userId && order.OrderId == id
                         select new ServiceFormViewModel
                         {
                             ServiceFormLogId = servicelog.ServiceFormLogId,
                             ItemId = item.ItemId,
                             ItemCd = item.ItemCd,
                             IsActive = servicelog.IsActive,
                             CategoryName = category.CategoryName,
                             Quantity = servicelog.Qty,
                             DateOfService = servicelog.DateOfService,
                             OrderId = order.OrderId,
                             CustomerName = customer.CompanyName,
                             Locations = servicelog.Locations

                         }).ToList();
            return PartialView("_ServiceFormLogDetail", items);
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}