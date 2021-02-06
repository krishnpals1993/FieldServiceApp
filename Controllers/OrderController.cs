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
using JqueryDataTables.ServerSide.AspNetCoreWeb.Models;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace FieldServiceApp.Controllers
{
    [Authentication]
    public class OrderController : Controller
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
        public OrderController(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings, DBContext dbContext, IHttpContextAccessor HttpContextAccessor, IConfigurationProvider mappingConfiguration)
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

        public IActionResult Add()
        {
            OrderMasterViewModel model = new OrderMasterViewModel();

            try
            {
                var checkTemData = TempData["NewCustomer"];
                if (checkTemData != null)
                {
                    model.CustomerId = (int)checkTemData;
                }

                model.ItemList = _dbContext.tbl_ItemMaster
                    .Where(w => w.IsActive == 1)
                            .Select(s => new ItemMasterViewModel
                            {
                                ItemId = s.ItemId,
                                ItemCd = s.ItemCd,
                                ItemPrice = s.ItemPrice,
                                ItemDescription = s.ItemDescription
                            })
                            .ToList();

                model.OrderItemList.Add(new OrderDetailViewModel());

                model.CustomerList = _orderUtility.GetCustomerListWithShipAddress();


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


                model.CustomerDetail.CityList = _dbContext.tbl_Cities
                  .Where(w => w.IsActive == 1)
                          .Select(s => new CityViewModel
                          {
                              CityId = s.CityId,
                              CityName = s.CityName
                          })
                          .ToList();

                model.CustomerDetail.StateList = _dbContext.tbl_States
                    .Where(w => w.IsActive == 1)
                            .Select(s => new StateViewModel
                            {
                                StateId = s.StateId,
                                StateName = s.StateName
                            })
                            .ToList();
                model.CustomerDetail.Contacts.Add(new CustmoerContactViewModel());
                model.CustomerDetail.Shippings.Add(new CustmoerShippingViewModel());





                try
                {
                    model.OrderNo = _dbContext.tbl_OrderMaster.Max(m => m.OrderId) + 1;

                }
                catch (Exception)
                {

                    model.OrderNo = 1;
                }

                model.OrderList = (from order in _dbContext.tbl_OrderMaster
                                   join customer in _dbContext.tbl_CustomerMaster
                                   on order.CustomerId equals customer.CustomerId
                                   where ((order.IsFollowUp ?? 0) == 1)
                                   select new ServiceFormOrderViewModel
                                   {
                                       OrderId = order.OrderId,
                                       CustomerName = customer.CompanyName + " (Order #" + order.OrderId + ")"

                                   }).ToList();


                var checkReOccurrenceEndDate = _dbContext.tbl_GlobalSetting
                                                           .Where(w => w.Name == "ReOccurrenceEndDate")
                                                           .Select(s => s.Value).FirstOrDefault();

                model.MaxReOccurenceEndDate = Convert.ToDateTime(checkReOccurrenceEndDate);


            }
            catch (Exception ex)
            {

                model.OrderNo = (model.OrderNo == 0 ? 1 : model.OrderNo);
                var a = "";
            }


            return View(model);
        }

        public IActionResult AddModal(string id)
        {
            ViewBag.dt = id;
            OrderMasterViewModel model = new OrderMasterViewModel();
            DateTime modelDate = DateTime.Now;
            try
            {
                try
                {
                    modelDate = Convert.ToDateTime(id);
                }
                catch (Exception)
                {
                    modelDate = DateTime.Now;

                }

                var checkTemData = TempData["NewCustomer"];
                if (checkTemData != null)
                {
                    model.CustomerId = (int)checkTemData;
                }

                model.ItemList = _dbContext.tbl_ItemMaster
                    .Where(w => w.IsActive == 1)
                            .Select(s => new ItemMasterViewModel
                            {
                                ItemId = s.ItemId,
                                ItemCd = s.ItemCd,
                                ItemPrice = s.ItemPrice,
                                ItemDescription = s.ItemDescription
                            })
                            .ToList();

                model.OrderItemList.Add(new OrderDetailViewModel());

                model.CustomerList = _orderUtility.GetCustomerListWithShipAddress();


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


                model.CustomerDetail.CityList = _dbContext.tbl_Cities
                  .Where(w => w.IsActive == 1)
                          .Select(s => new CityViewModel
                          {
                              CityId = s.CityId,
                              CityName = s.CityName
                          })
                          .ToList();

                model.CustomerDetail.StateList = _dbContext.tbl_States
                    .Where(w => w.IsActive == 1)
                            .Select(s => new StateViewModel
                            {
                                StateId = s.StateId,
                                StateName = s.StateName
                            })
                            .ToList();
                model.CustomerDetail.Contacts.Add(new CustmoerContactViewModel());
                model.CustomerDetail.Shippings.Add(new CustmoerShippingViewModel());





                try
                {
                    model.OrderNo = _dbContext.tbl_OrderMaster.Max(m => m.OrderId) + 1;

                }
                catch (Exception)
                {

                    model.OrderNo = 1;
                }

                model.OrderList = (from order in _dbContext.tbl_OrderMaster
                                   join customer in _dbContext.tbl_CustomerMaster
                                   on order.CustomerId equals customer.CustomerId
                                   where ((order.IsFollowUp ?? 0) == 1)
                                   select new ServiceFormOrderViewModel
                                   {
                                       OrderId = order.OrderId,
                                       CustomerName = customer.CompanyName + " (Order #" + order.OrderId + ")"

                                   }).ToList();


                var checkReOccurrenceEndDate = _dbContext.tbl_GlobalSetting
                                                           .Where(w => w.Name == "ReOccurrenceEndDate")
                                                           .Select(s => s.Value).FirstOrDefault();

                model.MaxReOccurenceEndDate = Convert.ToDateTime(checkReOccurrenceEndDate);

                model.OrderDate = modelDate;
                model.ShipDate = modelDate;


            }
            catch (Exception ex)
            {

                model.OrderNo = (model.OrderNo == 0 ? 1 : model.OrderNo);
                var a = "";
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult Add(OrderMasterViewModel model)
        {
            var _customerd = model.CustomerId;
            model.CustomerId = _dbContext.tbl_CustomerBillings.Where(w => w.CustomerBillingId == model.CustomerId).Select(s => s.CustomerId).FirstOrDefault();
            try
            {
                model.CustomerDetail.Code = "1";

                {
                    var checkOrderNo = _dbContext.tbl_OrderMaster.Where(w => w.OrderNo == model.OrderNo).FirstOrDefault();
                    if (checkOrderNo != null)
                    {
                        ViewBag.ErrorMessage = "Order no already exists";
                        model.ItemList = _dbContext.tbl_ItemMaster
                .Where(w => w.IsActive == 1)
                        .Select(s => new ItemMasterViewModel
                        {
                            ItemId = s.ItemId,
                            ItemCd = s.ItemCd,
                            ItemPrice = s.ItemPrice,
                            ItemDescription = s.ItemDescription
                        })
                        .ToList();
                        model.CustomerList = _dbContext.tbl_CustomerMaster
                            .Where(w => w.IsActive == 1)
                                        .Select(s => new CustomerMasterViewModel
                                        {
                                            CustmoerId = s.CustomerId,
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


                        model.CustomerDetail.CityList = _dbContext.tbl_Cities
                          .Where(w => w.IsActive == 1)
                                  .Select(s => new CityViewModel
                                  {
                                      CityId = s.CityId,
                                      CityName = s.CityName
                                  })
                                  .ToList();

                        model.CustomerDetail.StateList = _dbContext.tbl_States
                            .Where(w => w.IsActive == 1)
                                    .Select(s => new StateViewModel
                                    {
                                        StateId = s.StateId,
                                        StateName = s.StateName
                                    })
                                    .ToList();
                        model.CustomerDetail.Contacts.Add(new CustmoerContactViewModel());
                        model.CustomerDetail.Shippings.Add(new CustmoerShippingViewModel());

                        return View(model);


                    }

                    CommanUtility _commanUtility = new CommanUtility(_appSettings);
                    DateTime? shipDate = null;
                    if (model.ShipStartDate != null)
                    {
                        if (model.ShipStartTime != null)
                        {
                            model.ShipStartDate = Convert.ToDateTime(model.ShipStartDate.Value.ToString("MM/dd/yyyy") + " " + model.ShipStartTime.Value.ToString("HH:mm"));
                        }
                        else
                        {
                            var curDayName = model.ShipStartDate.Value.DayOfWeek.ToString();
                            var checkCalenderDate1 = _dbContext.tbl_CalenderWorkingHours.Where(w => w.DayName == curDayName).FirstOrDefault();
                            if (checkCalenderDate1 != null)
                            {
                                model.ShipStartDate = Convert.ToDateTime(model.ShipStartDate.Value.ToString("MM/dd/yyyy") + " " + checkCalenderDate1.StartTime.Value.ToString("HH:mm"));
                            }

                        }


                        shipDate = _commanUtility.RoundUp(model.ShipStartDate.Value, TimeSpan.FromMinutes(15));

                        if (model.ShipEndDate != null)
                        {
                            if (model.ShipEndTime != null)
                            {
                                model.ShipEndDate = Convert.ToDateTime(model.ShipEndDate.Value.ToString("MM/dd/yyyy") + " " + model.ShipEndTime.Value.ToString("HH:mm"));
                                model.ShipEndDate = shipDate.Value.AddMinutes((model.ShipEndDate.Value - model.ShipStartDate.Value).TotalMinutes);
                            }
                            else
                            {
                                model.ShipEndDate = shipDate.Value.AddMinutes(30);
                            }


                        }
                        var checkCalenderDate = _dbContext.tbl_CalenderWorkingHours.FirstOrDefault();
                        if (checkCalenderDate != null)
                        {
                            var calenderStartDate = Convert.ToDateTime(shipDate.Value.ToString("MM/dd/yyyy") + " " + checkCalenderDate.StartTime.Value.ToString("HH:mm"));
                            var calenderEndDate = Convert.ToDateTime(shipDate.Value.ToString("MM/dd/yyyy") + " " + checkCalenderDate.EndTime.Value.ToString("HH:mm"));
                            if ((shipDate < calenderStartDate) || (shipDate > calenderEndDate))
                            {
                                // shipDate = calenderStartDate;
                                //shipDate = _commanUtility.RoundUp(model.ShipStartDate.Value, TimeSpan.FromMinutes(15));

                            }
                        }

                    }
                    if (model.ApartmentId != null)
                    {
                        var csvList = model.ApartmentId.Select(s => "/" + s + "/").ToList();
                        model.ApartmentIds = String.Join(',', csvList);

                    }



                    OrderMaster orderMaster = new OrderMaster()
                    {
                        OrderDate = model.OrderDate,
                        OrderNo = model.OrderNo,
                        ShipStartDate = shipDate,
                        ShipEndDate = model.ShipEndDate,
                        ShipDate = model.ShipStartDate,
                        ShipId = model.ShipId,
                        CustomerId = model.CustomerId,
                        TotalAmount = model.TotalAmount,
                        TaxAmount = model.TaxAmount,
                        IsActive = 1,
                        CreatedBy = 1,
                        CreatedDate = DateTime.Now,
                        ApartmentIds = model.ApartmentIds,
                        ParentOrderId = model.ParentOrderId,
                        ReOccurenceCycle = model.ReOccurenceCycle,
                        ReOccurenceFrequency = model.ReOccurenceFrequency,
                        ReOccurenceWeekday = model.ReOccurenceWeekday,
                        ReOccurenceStartDate = model.ReOccurenceStartDate,
                        ReOccurenceEndDate = model.ReOccurenceEndDate

                    };

                    List<OrderDetailViewModel> orderDetailReoccurence = new List<OrderDetailViewModel>();
                    OrderAssignment orderAssignmentReoccurence = new OrderAssignment();


                    if (model.ReOccurence == "Yes")
                    {
                        orderMaster.ReOccurence = 1;
                    }
                    else
                    {
                        orderMaster.ReOccurence = 0;
                    }

                    _dbContext.tbl_OrderMaster.Add(orderMaster);
                    _dbContext.SaveChanges();

                    var reoccurrenceOrderCount = 1;

                    foreach (var item in model.OrderItemList)
                    {
                        OrderDetail orderDetail = new OrderDetail()
                        {
                            ItemId = item.ItemId,
                            Description = item.Description,
                            OrderId = orderMaster.OrderId,
                            PerUnitPrice = item.PerUnitPrice,
                            Quantity = item.Quantity,
                            TotalPrice = item.TotalPrice,
                            UnitId = item.UnitId,
                            IsActive = 1,
                            CreatedBy = 1,
                            CreatedDate = DateTime.Now
                        };
                        _dbContext.tbl_OrderDetail.Add(orderDetail);
                        _dbContext.SaveChanges();

                    }

                    orderDetailReoccurence = model.OrderItemList;

                    if (Convert.ToString(model.AssigneeId) != "")
                    {
                        model.EmployeeId = Convert.ToInt32(model.AssigneeId);
                        if (model.EmployeeId > 0)
                        {
                            OrderAssignment orderAssignment = new OrderAssignment()
                            {
                                OrderId = orderMaster.OrderId,
                                EmployeeId = model.EmployeeId,
                                AssignmentDate = DateTime.Now,
                                Status = "Assigned"
                            };
                            _dbContext.tbl_OrderAssignment.Add(orderAssignment);
                            _dbContext.SaveChanges();
                            orderAssignmentReoccurence = orderAssignment;
                        }

                    }

                    if (model.ReOccurence == "Yes")
                    {
                        if (model.ReOccurenceEndDate == null)
                        {

                            var checkReOccurrenceEndDate = _dbContext.tbl_GlobalSetting
                                                           .Where(w => w.Name == "ReOccurrenceEndDate")
                                                           .Select(s => s.Value).FirstOrDefault();

                            model.ReOccurenceEndDate = Convert.ToDateTime(checkReOccurrenceEndDate);
                        }



                        var checkShipStartDateFormatted = (model.ShipStartDate == null ? DateTime.Now.AddDays(-1).ToShortDateString() : model.ShipStartDate.Value.ToShortDateString());
                        OrderUtility _orderUtility = new OrderUtility(_dbContext);
                        var orderId = orderMaster.OrderId;
                        if (model.ReOccurenceCycle == "Days")
                        {
                            var count = 0;
                            for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                            {
                                if (checkShipStartDateFormatted != date.ToShortDateString())
                                {
                                    count++;
                                    if (count == model.ReOccurenceFrequency)
                                    {

                                        if (reoccurrenceOrderCount < 12)
                                        {
                                            count = 0;
                                            _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                            reoccurrenceOrderCount++;
                                        }

                                    }
                                }

                            }
                        }

                        if (model.ReOccurenceCycle == "Weeks")
                        {
                            var count = 0;
                            for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                            {
                                if (checkShipStartDateFormatted != date.ToShortDateString())
                                {


                                    count++;
                                    if (count == ((model.ReOccurenceFrequency * 7)))
                                    {

                                        if (reoccurrenceOrderCount < 12)
                                        {
                                            count = 0;
                                            _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                            reoccurrenceOrderCount++;
                                        }

                                    }
                                }
                            }
                        }

                        if (model.ReOccurenceCycle == "Months")
                        {
                            var count = 0;
                            for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                            {
                                if (checkShipStartDateFormatted != date.ToShortDateString())
                                {


                                    count++;
                                    if (count == ((model.ReOccurenceFrequency * 30)))
                                    {
                                        if (reoccurrenceOrderCount < 12)
                                        {
                                            count = 0;
                                            _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                            reoccurrenceOrderCount++;
                                        }


                                    }
                                }
                            }
                        }

                        if (model.ReOccurenceCycle == "WeekDay")
                        {

                            for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                            {
                                if (checkShipStartDateFormatted != date.ToShortDateString())
                                {

                                    if (model.ReOccurenceFrequency == 1)
                                    {
                                        if (date.Day >= 1 && date.Day <= 7)
                                        {
                                            if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                            {
                                                if (model.ReOccurenceStartDateSetBy != "System")
                                                {
                                                    if (reoccurrenceOrderCount < 12)
                                                    {
                                                        _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                        reoccurrenceOrderCount++;
                                                    }

                                                }
                                                else
                                                {
                                                    if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                    {
                                                        if (reoccurrenceOrderCount < 12)
                                                        {
                                                            _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                            reoccurrenceOrderCount++;
                                                        }

                                                    }

                                                }

                                            }
                                        }
                                    }
                                    else if (model.ReOccurenceFrequency == 0)
                                    {
                                        if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                        {
                                            if (model.ReOccurenceStartDateSetBy != "System")
                                            {
                                                if (reoccurrenceOrderCount < 12)
                                                {
                                                    _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                    reoccurrenceOrderCount++;
                                                }

                                            }
                                            else
                                            {
                                                if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                {
                                                    if (reoccurrenceOrderCount < 12)
                                                    {
                                                        _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                        reoccurrenceOrderCount++;
                                                    }

                                                }

                                            }
                                        }
                                    }
                                    else if (model.ReOccurenceFrequency > 1)
                                    {
                                        if (date.Day > ((model.ReOccurenceFrequency - 1) * 7) && date.Day <= ((model.ReOccurenceFrequency) * 7))
                                        {
                                            if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                            {
                                                if (model.ReOccurenceStartDateSetBy != "System")
                                                {
                                                    if (reoccurrenceOrderCount < 12)
                                                    {
                                                        _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                        reoccurrenceOrderCount++;

                                                    }
                                                }
                                                else
                                                {
                                                    if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                    {
                                                        if (reoccurrenceOrderCount < 12)
                                                        {
                                                            _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                            reoccurrenceOrderCount++;
                                                        }

                                                    }

                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }

                        if (model.ReOccurenceCycle == "Month's Day")
                        {

                            for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                            {
                                if (checkShipStartDateFormatted != date.ToShortDateString())
                                {
                                    if (date.Day == model.ReOccurenceFrequency)
                                    {
                                        if (reoccurrenceOrderCount < 12)
                                        {
                                            _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                            reoccurrenceOrderCount++;
                                        }

                                    }
                                }
                            }
                        }

                    }


                    ViewBag.SuccessMessage = "Order added successfully";

                }
            }
            catch (Exception ex)
            {
                var a = "";
            }

            model.CustomerId = _customerd;
            model.ItemList = _dbContext.tbl_ItemMaster
             .Where(w => w.IsActive == 1)
                     .Select(s => new ItemMasterViewModel
                     {
                         ItemId = s.ItemId,
                         ItemCd = s.ItemCd,
                         ItemPrice = s.ItemPrice,
                         ItemDescription = s.ItemDescription
                     })
                     .ToList();
            model.CustomerList = _orderUtility.GetCustomerListWithShipAddress();



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


            model.CustomerDetail.CityList = _dbContext.tbl_Cities
              .Where(w => w.IsActive == 1)
                      .Select(s => new CityViewModel
                      {
                          CityId = s.CityId,
                          CityName = s.CityName
                      })
                      .ToList();

            model.CustomerDetail.StateList = _dbContext.tbl_States
                .Where(w => w.IsActive == 1)
                        .Select(s => new StateViewModel
                        {
                            StateId = s.StateId,
                            StateName = s.StateName
                        })
                        .ToList();
            model.CustomerDetail.Contacts.Add(new CustmoerContactViewModel());
            model.CustomerDetail.Shippings.Add(new CustmoerShippingViewModel());

            model.OrderList = (from order in _dbContext.tbl_OrderMaster
                               join customer in _dbContext.tbl_CustomerMaster
                               on order.CustomerId equals customer.CustomerId
                               where ((order.IsFollowUp ?? 0) == 1)
                               select new ServiceFormOrderViewModel
                               {
                                   OrderId = order.OrderId,
                                   CustomerName = customer.CompanyName + " (Order #" + order.OrderId + ")"

                               }).ToList();



            return View(model);
        }

        [HttpPost]
        public ActionResult AddModal(OrderMasterViewModel model)
        {
            var _customerd = model.CustomerId;
            model.CustomerId = _dbContext.tbl_CustomerBillings.Where(w => w.CustomerBillingId == model.CustomerId).Select(s => s.CustomerId).FirstOrDefault();

            try
            {
                model.CustomerDetail.Code = "1";

                {
                    var checkOrderNo = _dbContext.tbl_OrderMaster.Where(w => w.OrderNo == model.OrderNo).FirstOrDefault();
                    if (checkOrderNo != null)
                    {
                        ViewBag.ErrorMessage = "Order no already exists";
                        model.ItemList = _dbContext.tbl_ItemMaster
                .Where(w => w.IsActive == 1)
                        .Select(s => new ItemMasterViewModel
                        {
                            ItemId = s.ItemId,
                            ItemCd = s.ItemCd,
                            ItemPrice = s.ItemPrice,
                            ItemDescription = s.ItemDescription
                        })
                        .ToList();
                        model.CustomerList = _dbContext.tbl_CustomerMaster
                            .Where(w => w.IsActive == 1)
                                        .Select(s => new CustomerMasterViewModel
                                        {
                                            CustmoerId = s.CustomerId,
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


                        model.CustomerDetail.CityList = _dbContext.tbl_Cities
                          .Where(w => w.IsActive == 1)
                                  .Select(s => new CityViewModel
                                  {
                                      CityId = s.CityId,
                                      CityName = s.CityName
                                  })
                                  .ToList();

                        model.CustomerDetail.StateList = _dbContext.tbl_States
                            .Where(w => w.IsActive == 1)
                                    .Select(s => new StateViewModel
                                    {
                                        StateId = s.StateId,
                                        StateName = s.StateName
                                    })
                                    .ToList();
                        model.CustomerDetail.Contacts.Add(new CustmoerContactViewModel());
                        model.CustomerDetail.Shippings.Add(new CustmoerShippingViewModel());

                        return View(model);


                    }

                    CommanUtility _commanUtility = new CommanUtility(_appSettings);
                    DateTime? shipDate = null;
                    if (model.ShipStartDate != null)
                    {
                        if (model.ShipStartTime != null)
                        {
                            model.ShipStartDate = Convert.ToDateTime(model.ShipStartDate.Value.ToString("MM/dd/yyyy") + " " + model.ShipStartTime.Value.ToString("HH:mm"));
                        }
                        else
                        {
                            var curDayName = model.ShipStartDate.Value.DayOfWeek.ToString();
                            var checkCalenderDate1 = _dbContext.tbl_CalenderWorkingHours.Where(w => w.DayName == curDayName).FirstOrDefault();
                            if (checkCalenderDate1 != null)
                            {
                                model.ShipStartDate = Convert.ToDateTime(model.ShipStartDate.Value.ToString("MM/dd/yyyy") + " " + checkCalenderDate1.StartTime.Value.ToString("HH:mm"));
                            }

                        }


                        shipDate = _commanUtility.RoundUp(model.ShipStartDate.Value, TimeSpan.FromMinutes(15));

                        if (model.ShipEndDate != null)
                        {
                            if (model.ShipEndTime != null)
                            {
                                model.ShipEndDate = Convert.ToDateTime(model.ShipEndDate.Value.ToString("MM/dd/yyyy") + " " + model.ShipEndTime.Value.ToString("HH:mm"));
                                model.ShipEndDate = shipDate.Value.AddMinutes((model.ShipEndDate.Value - model.ShipStartDate.Value).TotalMinutes);
                            }
                            else
                            {
                                model.ShipEndDate = shipDate.Value.AddMinutes(30);
                            }


                        }
                        var checkCalenderDate = _dbContext.tbl_CalenderWorkingHours.FirstOrDefault();
                        if (checkCalenderDate != null)
                        {
                            var calenderStartDate = Convert.ToDateTime(shipDate.Value.ToString("MM/dd/yyyy") + " " + checkCalenderDate.StartTime.Value.ToString("HH:mm"));
                            var calenderEndDate = Convert.ToDateTime(shipDate.Value.ToString("MM/dd/yyyy") + " " + checkCalenderDate.EndTime.Value.ToString("HH:mm"));
                            if ((shipDate < calenderStartDate) || (shipDate > calenderEndDate))
                            {
                                // shipDate = calenderStartDate;
                                //shipDate = _commanUtility.RoundUp(model.ShipStartDate.Value, TimeSpan.FromMinutes(15));

                            }
                        }

                    }
                    if (model.ApartmentId != null)
                    {
                        var csvList = model.ApartmentId.Select(s => "/" + s + "/").ToList();
                        model.ApartmentIds = String.Join(',', csvList);

                    }

                    OrderMaster orderMaster = new OrderMaster()
                    {
                        OrderDate = model.OrderDate,
                        OrderNo = model.OrderNo,
                        ShipStartDate = shipDate,
                        ShipEndDate = model.ShipEndDate,
                        ShipDate = model.ShipStartDate,
                        ShipId = model.ShipId,
                        CustomerId = model.CustomerId,
                        TotalAmount = model.TotalAmount,
                        TaxAmount = model.TaxAmount,
                        IsActive = 1,
                        CreatedBy = 1,
                        CreatedDate = DateTime.Now,
                        ApartmentIds = model.ApartmentIds,
                        ParentOrderId = model.ParentOrderId,
                        ReOccurenceCycle = model.ReOccurenceCycle,
                        ReOccurenceFrequency = model.ReOccurenceFrequency,
                        ReOccurenceWeekday = model.ReOccurenceWeekday,
                        ReOccurenceStartDate = model.ReOccurenceStartDate,
                        ReOccurenceEndDate = model.ReOccurenceEndDate

                    };

                    List<OrderDetailViewModel> orderDetailReoccurence = new List<OrderDetailViewModel>();
                    OrderAssignment orderAssignmentReoccurence = new OrderAssignment();


                    if (model.ReOccurence == "Yes")
                    {
                        orderMaster.ReOccurence = 1;
                    }
                    else
                    {
                        orderMaster.ReOccurence = 0;
                    }

                    _dbContext.tbl_OrderMaster.Add(orderMaster);
                    _dbContext.SaveChanges();

                    var reoccurrenceOrderCount = 1;

                    foreach (var item in model.OrderItemList)
                    {
                        OrderDetail orderDetail = new OrderDetail()
                        {
                            ItemId = item.ItemId,
                            Description = item.Description,
                            OrderId = orderMaster.OrderId,
                            PerUnitPrice = item.PerUnitPrice,
                            Quantity = item.Quantity,
                            TotalPrice = item.TotalPrice,
                            UnitId = item.UnitId,
                            IsActive = 1,
                            CreatedBy = 1,
                            CreatedDate = DateTime.Now
                        };
                        _dbContext.tbl_OrderDetail.Add(orderDetail);
                        _dbContext.SaveChanges();

                    }

                    orderDetailReoccurence = model.OrderItemList;

                    if (Convert.ToString(model.AssigneeId) != "")
                    {
                        model.EmployeeId = Convert.ToInt32(model.AssigneeId);
                        if (model.EmployeeId > 0)
                        {
                            OrderAssignment orderAssignment = new OrderAssignment()
                            {
                                OrderId = orderMaster.OrderId,
                                EmployeeId = model.EmployeeId,
                                AssignmentDate = DateTime.Now,
                                Status = "Assigned"
                            };
                            _dbContext.tbl_OrderAssignment.Add(orderAssignment);
                            _dbContext.SaveChanges();
                            orderAssignmentReoccurence = orderAssignment;
                        }

                    }

                    if (model.ReOccurence == "Yes")
                    {
                        if (model.ReOccurenceEndDate == null)
                        {

                            var checkReOccurrenceEndDate = _dbContext.tbl_GlobalSetting
                                                           .Where(w => w.Name == "ReOccurrenceEndDate")
                                                           .Select(s => s.Value).FirstOrDefault();

                            model.ReOccurenceEndDate = Convert.ToDateTime(checkReOccurrenceEndDate);
                        }



                        var checkShipStartDateFormatted = (model.ShipStartDate == null ? DateTime.Now.AddDays(-1).ToShortDateString() : model.ShipStartDate.Value.ToShortDateString());
                        OrderUtility _orderUtility = new OrderUtility(_dbContext);
                        var orderId = orderMaster.OrderId;
                        if (model.ReOccurenceCycle == "Days")
                        {
                            var count = 0;
                            for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                            {
                                if (checkShipStartDateFormatted != date.ToShortDateString())
                                {
                                    count++;
                                    if (count == model.ReOccurenceFrequency)
                                    {

                                        if (reoccurrenceOrderCount < 12)
                                        {
                                            count = 0;
                                            _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                            reoccurrenceOrderCount++;
                                        }

                                    }
                                }

                            }
                        }

                        if (model.ReOccurenceCycle == "Weeks")
                        {
                            var count = 0;
                            for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                            {
                                if (checkShipStartDateFormatted != date.ToShortDateString())
                                {


                                    count++;
                                    if (count == ((model.ReOccurenceFrequency * 7)))
                                    {

                                        if (reoccurrenceOrderCount < 12)
                                        {
                                            count = 0;
                                            _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                            reoccurrenceOrderCount++;
                                        }

                                    }
                                }
                            }
                        }

                        if (model.ReOccurenceCycle == "Months")
                        {
                            var count = 0;
                            for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                            {
                                if (checkShipStartDateFormatted != date.ToShortDateString())
                                {


                                    count++;
                                    if (count == ((model.ReOccurenceFrequency * 30)))
                                    {
                                        if (reoccurrenceOrderCount < 12)
                                        {
                                            count = 0;
                                            _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                            reoccurrenceOrderCount++;
                                        }


                                    }
                                }
                            }
                        }

                        if (model.ReOccurenceCycle == "WeekDay")
                        {

                            for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                            {
                                if (checkShipStartDateFormatted != date.ToShortDateString())
                                {

                                    if (model.ReOccurenceFrequency == 1)
                                    {
                                        if (date.Day >= 1 && date.Day <= 7)
                                        {
                                            if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                            {
                                                if (model.ReOccurenceStartDateSetBy != "System")
                                                {
                                                    if (reoccurrenceOrderCount < 12)
                                                    {
                                                        _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                        reoccurrenceOrderCount++;
                                                    }

                                                }
                                                else
                                                {
                                                    if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                    {
                                                        if (reoccurrenceOrderCount < 12)
                                                        {
                                                            _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                            reoccurrenceOrderCount++;
                                                        }

                                                    }

                                                }

                                            }
                                        }
                                    }
                                    else if (model.ReOccurenceFrequency == 0)
                                    {
                                        if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                        {
                                            if (model.ReOccurenceStartDateSetBy != "System")
                                            {
                                                if (reoccurrenceOrderCount < 12)
                                                {
                                                    _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                    reoccurrenceOrderCount++;
                                                }

                                            }
                                            else
                                            {
                                                if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                {
                                                    if (reoccurrenceOrderCount < 12)
                                                    {
                                                        _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                        reoccurrenceOrderCount++;
                                                    }

                                                }

                                            }
                                        }
                                    }
                                    else if (model.ReOccurenceFrequency > 1)
                                    {
                                        if (date.Day > ((model.ReOccurenceFrequency - 1) * 7) && date.Day <= ((model.ReOccurenceFrequency) * 7))
                                        {
                                            if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                            {
                                                if (model.ReOccurenceStartDateSetBy != "System")
                                                {
                                                    if (reoccurrenceOrderCount < 12)
                                                    {
                                                        _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                        reoccurrenceOrderCount++;

                                                    }
                                                }
                                                else
                                                {
                                                    if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                    {
                                                        if (reoccurrenceOrderCount < 12)
                                                        {
                                                            _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                            reoccurrenceOrderCount++;
                                                        }

                                                    }

                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }

                        if (model.ReOccurenceCycle == "Month's Day")
                        {

                            for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                            {
                                if (checkShipStartDateFormatted != date.ToShortDateString())
                                {
                                    if (date.Day == model.ReOccurenceFrequency)
                                    {
                                        if (reoccurrenceOrderCount < 12)
                                        {
                                            _orderUtility.SaveReOccurenceOrder(orderId, orderMaster, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                            reoccurrenceOrderCount++;
                                        }

                                    }
                                }
                            }
                        }

                    }


                    ViewBag.SuccessMessage = "Order added successfully";

                }
            }
            catch (Exception ex)
            {
                var a = "";
            }

            model.CustomerId = _customerd;

            model.ItemList = _dbContext.tbl_ItemMaster
             .Where(w => w.IsActive == 1)
                     .Select(s => new ItemMasterViewModel
                     {
                         ItemId = s.ItemId,
                         ItemCd = s.ItemCd,
                         ItemPrice = s.ItemPrice,
                         ItemDescription = s.ItemDescription
                     })
                     .ToList();
            model.CustomerList = _orderUtility.GetCustomerListWithShipAddress();



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


            model.CustomerDetail.CityList = _dbContext.tbl_Cities
              .Where(w => w.IsActive == 1)
                      .Select(s => new CityViewModel
                      {
                          CityId = s.CityId,
                          CityName = s.CityName
                      })
                      .ToList();

            model.CustomerDetail.StateList = _dbContext.tbl_States
                .Where(w => w.IsActive == 1)
                        .Select(s => new StateViewModel
                        {
                            StateId = s.StateId,
                            StateName = s.StateName
                        })
                        .ToList();
            model.CustomerDetail.Contacts.Add(new CustmoerContactViewModel());
            model.CustomerDetail.Shippings.Add(new CustmoerShippingViewModel());

            model.OrderList = (from order in _dbContext.tbl_OrderMaster
                               join customer in _dbContext.tbl_CustomerMaster
                               on order.CustomerId equals customer.CustomerId
                               where ((order.IsFollowUp ?? 0) == 1)
                               select new ServiceFormOrderViewModel
                               {
                                   OrderId = order.OrderId,
                                   CustomerName = customer.CompanyName + " (Order #" + order.OrderId + ")"

                               }).ToList();



            return View(model);
        }

        [HttpPost]
        public ActionResult AddNewCustomer(OrderMasterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var checkCustomer = _dbContext.tbl_CustomerMaster.Where(w => w.CompanyName == model.CustomerDetail.CompanyName).FirstOrDefault();
                    if (checkCustomer != null)
                    {
                        ViewBag.ErrorMessage = "Customer name is already exists";

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
                                            CustmoerId = s.CustomerId,
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


                        model.CustomerDetail.CityList = _dbContext.tbl_Cities
                          .Where(w => w.IsActive == 1)
                                  .Select(s => new CityViewModel
                                  {
                                      CityId = s.CityId,
                                      CityName = s.CityName
                                  })
                                  .ToList();

                        model.CustomerDetail.StateList = _dbContext.tbl_States
                            .Where(w => w.IsActive == 1)
                                    .Select(s => new StateViewModel
                                    {
                                        StateId = s.StateId,
                                        StateName = s.StateName
                                    })
                                    .ToList();
                        model.CustomerDetail.Contacts.Add(new CustmoerContactViewModel());
                        model.CustomerDetail.Shippings.Add(new CustmoerShippingViewModel());



                        return View(model);

                    }

                    CustomerMaster customerMaster = new CustomerMaster()
                    {
                        CompanyName = model.CustomerDetail.CompanyName,
                        //CityId = model.CustomerDetail.CityId == null ? 0 : model.CustomerDetail.CityId.Value,
                        //StateId = model.CustomerDetail.StateId == null ? 0 : model.CustomerDetail.StateId.Value,
                        //Address = model.CustomerDetail.Address,
                        CompanyType = model.CustomerDetail.CompanyType,
                        //Zip1 = model.CustomerDetail.Zip1,
                        //Zip2 = model.CustomerDetail.Zip2,
                        CompanyCode = model.CustomerDetail.Code,
                        IsActive = 1,
                        CreatedBy = 1,
                        CreatedDate = DateTime.Now
                    };

                    _dbContext.tbl_CustomerMaster.Add(customerMaster);
                    _dbContext.SaveChanges();

                    model.CustomerId = customerMaster.CustomerId;

                    foreach (var item in model.CustomerDetail.Shippings)
                    {
                        if (Convert.ToString(item.FirstName ?? "") != "" ||
                            Convert.ToString(item.MiddleName ?? "") != "" ||
                            Convert.ToString(item.LastName ?? "") != "" ||
                            Convert.ToString(item.Email ?? "") != "" ||
                            Convert.ToString(item.Phone ?? "") != "" ||
                            Convert.ToString(item.StateId) != "0" ||
                            Convert.ToString(item.CityId) != "0" ||
                            Convert.ToString(item.Address ?? "") != "")
                        {
                            CustomerShipping custmoerShipping = new CustomerShipping()
                            {
                                CustomerId = customerMaster.CustomerId,
                                FirstName = item.FirstName,
                                MiddleName = item.MiddleName,
                                LastName = item.LastName,
                                Email = item.Email,
                                Phone = item.Phone,
                                CityId = item.CityId == null ? 0 : item.CityId.Value,
                                StateId = item.StateId == null ? 0 : item.StateId.Value,
                                Address = item.Address,
                                IsActive = 1,
                                CreatedBy = 1,
                                CreatedDate = DateTime.Now
                            };
                            _dbContext.tbl_CustmoerShipping.Add(custmoerShipping);
                            _dbContext.SaveChanges();

                        }
                    }

                    foreach (var item in model.CustomerDetail.Contacts)
                    {
                        if (Convert.ToString(item.FirstName ?? "") != "" ||
                            Convert.ToString(item.MiddleName ?? "") != "" ||
                            Convert.ToString(item.LastName ?? "") != "" ||
                            Convert.ToString(item.Email ?? "") != "" ||
                            Convert.ToString(item.Phone ?? "") != "")
                        {
                            CustomerContact customerContactDetail = new CustomerContact()
                            {
                                FirstName = item.FirstName,
                                MiddleName = item.MiddleName,
                                LastName = item.LastName,
                                Email = item.Email,
                                Phone = item.Phone,
                                CustomerId = customerMaster.CustomerId,
                                IsActive = 1,
                                CreatedBy = 1,
                                CreatedDate = DateTime.Now
                            };
                            _dbContext.tbl_CustmoerContact.Add(customerContactDetail);
                            _dbContext.SaveChanges();
                        }
                    }


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
                                        CustmoerId = s.CustomerId,
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


                    model.CustomerDetail.CityList = _dbContext.tbl_Cities
                      .Where(w => w.IsActive == 1)
                              .Select(s => new CityViewModel
                              {
                                  CityId = s.CityId,
                                  CityName = s.CityName
                              })
                              .ToList();

                    model.CustomerDetail.StateList = _dbContext.tbl_States
                        .Where(w => w.IsActive == 1)
                                .Select(s => new StateViewModel
                                {
                                    StateId = s.StateId,
                                    StateName = s.StateName
                                })
                                .ToList();
                    model.CustomerDetail.Contacts.Add(new CustmoerContactViewModel());
                    model.CustomerDetail.Shippings.Add(new CustmoerShippingViewModel());

                    ViewBag.SuccessMessage = "Customer added successfully";

                }


            }
            catch (Exception ex)
            {
                var a = "";
            }


            TempData["NewCustomer"] = model.CustomerId;

            return RedirectToAction("Add");
        }

        public IActionResult List()
        {
            OrderListViewModel model = new OrderListViewModel();

            model.OrderList = new List<OrderMasterViewModel>();
            try
            {
                //model.OrderList = (from order in _dbContext.tbl_OrderMaster
                //                   join ship in _dbContext.tbl_CustmoerShipping on order.ShipId equals ship.CustomerShipId
                //                   join city in _dbContext.tbl_Cities on ship.CityId equals city.CityId
                //                   into city
                //                   from city1 in city.DefaultIfEmpty()
                //                   join state in _dbContext.tbl_States on ship.StateId equals state.StateId
                //                   into state
                //                   from state1 in state.DefaultIfEmpty()
                //                   join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustomerId
                //                   join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                //                   into orderAssign
                //                   from orderAssign1 in orderAssign.DefaultIfEmpty()
                //                   join employee in _dbContext.tbl_EmployeeMaster on orderAssign1.EmployeeId equals employee.EmployeeId
                //                   into employee
                //                   from employee1 in employee.DefaultIfEmpty()
                //                   where order.IsActive == 1
                //                   select new OrderMasterViewModel
                //                   {
                //                       OrderId = order.OrderId,
                //                       OrderNo = order.OrderNo,
                //                       OrderDate = order.OrderDate,
                //                       ShipStartDate = order.ShipStartDate,
                //                       CustomerName = customer.CompanyName,
                //                       EmployeeName = employee1.FirstName + " " + (employee1.MiddleName ?? "") + " " + employee1.LastName,
                //                       TotalAmount = order.TotalAmount,
                //                       IsActive = order.IsActive,
                //                       ScheduledOnNonWorkingDay = false,
                //                       CustomerShipAddress = (city1.CityName ?? "") + " " + (state1.StateName ?? "") + " " + (ship.Address ?? "") + (ship.Address2 ?? ""),
                //                       ReOccurenceParentOrderId = order.ReOccurenceParentOrderId

                //                   })
                //                               .ToList().OrderByDescending(o => o.OrderId).ToList();



                //var checkWeekOffs = _dbContext.tbl_CalenderWorkingDays.Where(w => w.DayName != null).Select(s => s.DayName).ToList();
                //var checkHolidays = _dbContext.tbl_CalenderWorkingDays.Where(w => w.HolidayDate != null).Select(s => s.HolidayDate).ToList();


                //foreach (var order in model.OrderList)
                //{
                //    if (order.ReOccurenceParentOrderId != null)
                //    {
                //        order.ReoccurrenceOrderNo = _dbContext.tbl_OrderMaster.Where(w => w.OrderId == order.ReOccurenceParentOrderId).Select(s => s.OrderNo).FirstOrDefault();
                //    }

                //    if (checkWeekOffs.Contains(order.ShipStartDate?.DayOfWeek.ToString()))
                //    {
                //        order.ScheduledOnNonWorkingDay = true;
                //    }
                //    else
                //    {
                //        var checkHoliday = checkHolidays.Where(w => w.Value.Day == order.ShipStartDate?.Day && w.Value.Month == order.ShipStartDate?.Month).Count();
                //        if (checkHoliday > 0)
                //        {
                //            order.ScheduledOnNonWorkingDay = true;
                //        }

                //    }
                //}




            }
            catch (Exception ex)
            {

                var a = "";
            }


            return View(new OrderMasterViewModel_Datatable());

        }

        [HttpPost]
        public IActionResult List(OrderListViewModel model)
        {
            model.OrderList = new List<OrderMasterViewModel>();



            try
            {
                model.OrderList = (from order in _dbContext.tbl_OrderMaster
                                   join ship in _dbContext.tbl_CustmoerShipping on order.ShipId equals ship.CustomerShipId
                                   join city in _dbContext.tbl_Cities on ship.CityId equals city.CityId
                                   into city
                                   from city1 in city.DefaultIfEmpty()
                                   join state in _dbContext.tbl_States on ship.StateId equals state.StateId
                                   into state
                                   from state1 in state.DefaultIfEmpty()
                                   join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustomerId
                                   join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                                   into orderAssign
                                   from orderAssign1 in orderAssign.DefaultIfEmpty()
                                   join employee in _dbContext.tbl_EmployeeMaster on orderAssign1.EmployeeId equals employee.EmployeeId
                                   into employee
                                   from employee1 in employee.DefaultIfEmpty()
                                   where order.IsActive == 1
                                   select new OrderMasterViewModel
                                   {
                                       OrderId = order.OrderId,
                                       OrderNo = order.OrderNo,
                                       OrderDate = order.OrderDate,
                                       ShipStartDate = order.ShipStartDate,
                                       CustomerName = customer.CompanyName,
                                       EmployeeName = employee1.FirstName + " " + (employee1.MiddleName ?? "") + " " + employee1.LastName,
                                       TotalAmount = order.TotalAmount,
                                       IsActive = order.IsActive,
                                       CustomerShipAddress = (city1.CityName ?? "") + " " + (state1.StateName ?? "") + " " + (ship.Address ?? "") + (ship.Address2 ?? ""),
                                       ReOccurenceParentOrderId = order.ReOccurenceParentOrderId

                                   })
                                               .ToList();
                if (Convert.ToString(model.OrderNo ?? "") != "")
                {
                    model.OrderList = model.OrderList.Where(w => (w.OrderNo).ToString().Contains(model.OrderNo)).ToList();
                }
                if (Convert.ToString(model.CustomerName ?? "") != "")
                {
                    model.OrderList = model.OrderList.Where(w => (w.CustomerName ?? "").ToString().Contains(model.CustomerName)).ToList();
                }

                if (model.ShipDateFrom != null && model.ShipDateTo != null)
                {
                    model.OrderList = model.OrderList.Where(w => (w.ShipStartDate == null ? false : (w.ShipStartDate.Value >= model.ShipDateFrom.Value)) && (w.ShipStartDate == null ? false : (w.ShipStartDate.Value <= model.ShipDateTo.Value))).ToList();
                }

                if (model.OrderDateFrom != null && model.OrderDateTo != null)
                {
                    model.OrderList = model.OrderList.Where(w => w.OrderDate >= model.OrderDateFrom.Value && w.OrderDate <= model.OrderDateTo.Value).ToList();
                }

                var checkWeekOffs = _dbContext.tbl_CalenderWorkingDays.Where(w => w.DayName != null).Select(s => s.DayName).ToList();
                var checkHolidays = _dbContext.tbl_CalenderWorkingDays.Where(w => w.HolidayDate != null).Select(s => s.HolidayDate).ToList();


                foreach (var order in model.OrderList)
                {
                    if (order.ReOccurenceParentOrderId != null)
                    {
                        order.ReoccurrenceOrderNo = _dbContext.tbl_OrderMaster.Where(w => w.OrderId == order.ReOccurenceParentOrderId).Select(s => s.OrderNo).FirstOrDefault();
                    }
                    if (checkWeekOffs.Contains(order.ShipStartDate?.DayOfWeek.ToString()))
                    {
                        order.ScheduledOnNonWorkingDay = true;
                    }
                    else
                    {
                        var checkHoliday = checkHolidays.Where(w => w.Value.Day == order.ShipStartDate?.Day && w.Value.Month == order.ShipStartDate?.Month).Count();
                        if (checkHoliday > 0)
                        {
                            order.ScheduledOnNonWorkingDay = true;
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

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody]JqueryDataTablesParameters param)
        {
            try
            {
                //return new JsonResult(new { error = "Internal Server Error" });

                HttpContext.Session.SetString(nameof(JqueryDataTablesParameters), System.Text.Json.JsonSerializer.Serialize(param));

                IQueryable<OrderMasterViewModel_Datatable> customerList;
                List<OrderMasterViewModel_Datatable> customerListMain = new List<OrderMasterViewModel_Datatable>();

                customerListMain = (from order in _dbContext.tbl_OrderMaster
                                    join ship in _dbContext.tbl_CustmoerShipping on order.ShipId equals ship.CustomerShipId
                                    join city in _dbContext.tbl_Cities on ship.CityId equals city.CityId
                                    into city
                                    from city1 in city.DefaultIfEmpty()
                                    join state in _dbContext.tbl_States on ship.StateId equals state.StateId
                                    into state
                                    from state1 in state.DefaultIfEmpty()
                                    join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustomerId
                                    join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                                    into orderAssign
                                    from orderAssign1 in orderAssign.DefaultIfEmpty()
                                    join employee in _dbContext.tbl_EmployeeMaster on orderAssign1.EmployeeId equals employee.EmployeeId
                                    into employee
                                    from employee1 in employee.DefaultIfEmpty()
                                    where order.IsActive == 1
                                    select new OrderMasterViewModel_Datatable
                                    {
                                        OrderId = order.OrderId,
                                        OrderNo = order.OrderNo,
                                        OrderDate = order.OrderDate,
                                        ShipStartDateFormatted = (order.ShipStartDate == null ? "" : (order.ShipStartDate.Value.DayOfWeek.ToString() + " " + order.ShipStartDate.Value.ToString("MM/dd/yyyy hh:mm tt"))),
                                        ShipStartDate = order.ShipStartDate,
                                        CustomerName = customer.CompanyName,
                                        EmployeeName = ((employee1.FirstName + " " + (employee1.MiddleName ?? "") + " " + employee1.LastName) ?? ""),
                                        TotalAmount = order.TotalAmount,
                                        IsActive = order.IsActive,
                                        ScheduledOnNonWorkingDay = false,
                                        CustomerShipAddress = (((city1.CityName ?? "") + " " + (state1.StateName ?? "") + " " + (ship.Address ?? "") + (ship.Address2 ?? "")) ?? ""),
                                        ReOccurenceParentOrderId = order.ReOccurenceParentOrderId ?? 0

                                    }).ToList();




                var size = customerListMain.Count();

                if (Convert.ToString(param.Search?.Value) != "")
                {
                    var serchValue = param.Search?.Value.ToLower();
                    customerListMain = customerListMain.Where(w =>
                                     (((w.OrderNo)).ToString().ToLower().Contains(serchValue) ? true :
                                     ((w.OrderDate).ToString().ToLower().Contains(serchValue) ? true :
                                     (((w.ShipStartDateFormatted.ToString())).ToLower().Contains(serchValue) ? true :
                                     ((w.CustomerName.ToString()).ToLower().Contains(serchValue) ? true :
                                     ((w.EmployeeName.ToString()).ToLower().Contains(serchValue) ? true :
                                     ((w.CustomerShipAddress).ToLower().Contains(serchValue) ? true :
                                     (((w.ReOccurenceParentOrderId)).ToString().ToLower().Contains(serchValue) ? true :
                                     ((w.TotalAmount).ToString().ToLower().Contains(serchValue) ? true : false)))))))))
                          .ToList();
                }

                if (param.Length == -1)
                {
                    var items = customerListMain
                                      //.ProjectTo<OrderMasterViewModel_Datatable>(_mappingConfiguration)
                                      .ToArray();


                    var result = new JqueryDataTablesPagedResults<OrderMasterViewModel_Datatable>
                    {
                        Items = items,
                        TotalSize = size
                    };

                    return new JsonResult(new JqueryDataTablesResult<OrderMasterViewModel_Datatable>
                    {
                        Draw = param.Draw,
                        Data = result.Items,
                        RecordsFiltered = result.TotalSize,
                        RecordsTotal = result.TotalSize
                    });
                }
                else
                {

                    try
                    {
                        var orderbyColumn = param.Columns[param.Order[0].Column].Data ?? "OrderNo";
                        var orderbyDir = param.Order[0].Dir.ToString().ToUpper();
                        if (orderbyColumn.ToString() == "OrderNo")
                        {
                            if (orderbyDir == "ASC")
                            {
                                customerListMain = customerListMain.OrderBy(o => o.OrderNo).ToList();
                            }
                            else
                            {
                                customerListMain = customerListMain.OrderByDescending(o => o.OrderNo).ToList();
                            }

                        }
                        else if (orderbyColumn.ToString() == "ReoccurrenceOrderNo")
                        {
                            if (orderbyDir == "ASC")
                            {
                                customerListMain = customerListMain.OrderBy(o => o.ReOccurenceParentOrderId).ToList();
                            }
                            else
                            {
                                customerListMain = customerListMain.OrderByDescending(o => o.ReOccurenceParentOrderId).ToList();
                            }

                        }
                        else if (orderbyColumn.ToString() == "OrderDate")
                        {
                            if (orderbyDir == "ASC")
                            {
                                customerListMain = customerListMain.OrderBy(o => o.OrderDate).ToList();
                            }
                            else
                            {
                                customerListMain = customerListMain.OrderByDescending(o => o.OrderDate).ToList();
                            }

                        }
                        else if (orderbyColumn.ToString() == "CustomerName")
                        {
                            if (orderbyDir == "ASC")
                            {
                                customerListMain = customerListMain.OrderBy(o => o.CustomerName).ToList();
                            }
                            else
                            {
                                customerListMain = customerListMain.OrderByDescending(o => o.CustomerName).ToList();
                            }

                        }
                        else if (orderbyColumn.ToString() == "ShipStartDate")
                        {
                            if (orderbyDir == "ASC")
                            {
                                customerListMain = customerListMain.OrderBy(o => o.ShipStartDate).ToList();
                            }
                            else
                            {
                                customerListMain = customerListMain.OrderByDescending(o => o.ShipStartDate).ToList();
                            }

                        }
                        else if (orderbyColumn.ToString() == "TotalAmount")
                        {
                            if (orderbyDir == "ASC")
                            {
                                customerListMain = customerListMain.OrderBy(o => o.TotalAmount).ToList();
                            }
                            else
                            {
                                customerListMain = customerListMain.OrderByDescending(o => o.TotalAmount).ToList();
                            }

                        }
                        else if (orderbyColumn.ToString() == "EmployeeName")
                        {
                            if (orderbyDir == "ASC")
                            {
                                customerListMain = customerListMain.OrderBy(o => o.EmployeeName).ToList();
                            }
                            else
                            {
                                customerListMain = customerListMain.OrderByDescending(o => o.EmployeeName).ToList();
                            }

                        }
                        else if (orderbyColumn.ToString() == "CustomerShipAddress")
                        {
                            if (orderbyDir == "ASC")
                            {
                                customerListMain = customerListMain.OrderBy(o => o.CustomerShipAddress).ToList();
                            }
                            else
                            {
                                customerListMain = customerListMain.OrderByDescending(o => o.CustomerShipAddress).ToList();
                            }

                        }
                        else if (orderbyColumn.ToString() == "IsActive")
                        {
                            if (orderbyDir == "ASC")
                            {
                                customerListMain = customerListMain.OrderBy(o => o.IsActive).ToList();
                            }
                            else
                            {
                                customerListMain = customerListMain.OrderByDescending(o => o.IsActive).ToList();
                            }

                        }
                        else
                        {
                            customerListMain = customerListMain.OrderByDescending(o => o.OrderNo).ToList();
                        }

                    }
                    catch (Exception ex)
                    {


                    }

                    var items = customerListMain
                    .Skip((param.Start / param.Length) * param.Length)
                    .Take(param.Length)
                    //.ProjectTo<OrderMasterViewModel_Datatable>(_mappingConfiguration)
                    .ToArray();
                    var checkWeekOffs = _dbContext.tbl_CalenderWorkingDays.Where(w => w.DayName != null).Select(s => s.DayName).ToList();
                    var checkHolidays = _dbContext.tbl_CalenderWorkingDays.Where(w => w.HolidayDate != null).Select(s => s.HolidayDate).ToList();

                    var allOrderIds = items.Select(s => s.OrderId).ToList();
                    var filterOrders = _dbContext.tbl_OrderMaster.Where(w =>  allOrderIds.Contains(w.OrderId)).ToList();

                    foreach (var order in items)
                    {
                        if (order.ReOccurenceParentOrderId != null)
                        {
                            order.ReOccurenceParentOrderId = filterOrders.Where(w => w.OrderId == order.ReOccurenceParentOrderId).Select(s => s.OrderNo).FirstOrDefault();
                        }

                        if (checkWeekOffs.Contains(order.ShipStartDate?.DayOfWeek.ToString()))
                        {
                            order.ScheduledOnNonWorkingDay = true;
                        }
                        else
                        {
                            var checkHoliday = checkHolidays.Where(w => w.Value.Day == order.ShipStartDate?.Day && w.Value.Month == order.ShipStartDate?.Month).Count();
                            if (checkHoliday > 0)
                            {
                                order.ScheduledOnNonWorkingDay = true;
                            }

                        }
                    }

                    var result = new JqueryDataTablesPagedResults<OrderMasterViewModel_Datatable>
                    {
                        Items = items,
                        TotalSize = size
                    };

                    return new JsonResult(new JqueryDataTablesResult<OrderMasterViewModel_Datatable>
                    {
                        Draw = param.Draw,
                        Data = result.Items,
                        RecordsFiltered = result.TotalSize,
                        RecordsTotal = result.TotalSize
                    });
                }

            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return new JsonResult(new { error = "Internal Server Error" });
            }
        }


        [HttpPost]
        public JsonResult UpdateOrderDate(int orderId, DateTime? start, DateTime? end, int employeeId, string status)
        {
            DashboardOrderViewModel model = new DashboardOrderViewModel();
            try
            {
                var checkOrder = _dbContext.tbl_OrderMaster.Where(w => w.OrderId == orderId).FirstOrDefault();

                if (checkOrder != null)
                {
                    {
                        checkOrder.ShipStartDate = start;
                        if (start != null)
                        {
                            if (start.Value.ToShortTimeString() == "12:00 AM")
                            {
                                var currentDayName = start.Value.DayOfWeek.ToString();
                                var checkCalenderDate = _dbContext.tbl_CalenderWorkingHours.Where(w => w.DayName == currentDayName).FirstOrDefault();
                                if (checkCalenderDate != null)
                                {
                                    var calenderStartDate = Convert.ToDateTime(start.Value.ToString("MM/dd/yyyy") + " " + checkCalenderDate.StartTime.Value.ToString("HH:mm"));
                                    start = calenderStartDate;
                                    CommanUtility _commanUtility = new CommanUtility(_appSettings);
                                    //start = _commanUtility.RoundUp(start.Value, TimeSpan.FromMinutes(15));

                                }

                            }



                        }
                        checkOrder.ShipDate = start;
                        checkOrder.ShipStartDate = start;
                        checkOrder.ShipEndDate = end;
                        _dbContext.SaveChanges();
                        if (status == "day")
                        {
                            var checkOrderAssignee = _dbContext.tbl_OrderAssignment.Where(w => w.OrderId == orderId).FirstOrDefault();
                            if (checkOrderAssignee == null)
                            {
                                OrderAssignment orderAssignment = new OrderAssignment()
                                {
                                    OrderId = orderId,
                                    EmployeeId = employeeId,
                                    AssignmentDate = DateTime.Now,
                                    Status = "Assigned"
                                };
                                _dbContext.tbl_OrderAssignment.Add(orderAssignment);
                                _dbContext.SaveChanges();
                            }
                            else
                            {
                                checkOrderAssignee.EmployeeId = employeeId;
                                _dbContext.SaveChanges();
                            }


                        }

                    }

                }
                DashboardUtility _dashboardUtility = new DashboardUtility(_dbContext);
                model = _dashboardUtility.getOrderDetail(orderId);
            }
            catch (Exception ex)
            {
                var a = "";
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
                else
                {
                    OrderAssignment orderAssignment = new OrderAssignment()
                    {
                        OrderId = orderId,
                        EmployeeId = employeeId,
                        AssignmentDate = DateTime.Now,
                        Status = "Assigned"
                    };
                    _dbContext.tbl_OrderAssignment.Add(orderAssignment);
                    _dbContext.SaveChanges();
                }

                var checkEmployeeColor = _dbContext.tbl_EmployeeMaster.Where(w => w.EmployeeId == employeeId)
                    .Select(s => s.Color).FirstOrDefault();

                response.Status = "1";
                response.Message = (checkEmployeeColor == "" ? "rgb(228 211 91 / 63%)" : checkEmployeeColor);
            }
            catch (Exception ex)
            {
            }

            return Json(response);
        }

        [HttpPost]
        public JsonResult GetCustomerShippingAddress(int CustomerId)
        {
            var CustomerShipingAddressList = new List<CustmoerShippingViewModel>();
            int cityId = _dbContext.tbl_CustomerBillings.Where(w => w.CustomerBillingId == CustomerId)
                         .Select(s => s.CityId).FirstOrDefault();

            decimal? tax = 0;
            tax = _dbContext.tbl_Cities.Where(w => w.CityId == cityId).Select(s => s.Tax).FirstOrDefault();
            if (tax!=null)
            {
                if (tax==0)
                {
                    tax = 1;
                }
                
            }
            else
            {
                tax = 1;
            }
            

            try
            {
                CustomerShipingAddressList = (from shipping in _dbContext.tbl_CustmoerShipping
                                              join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                              into city
                                              from city1 in city.DefaultIfEmpty()
                                              join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                              into state
                                              from state1 in state.DefaultIfEmpty()
                                              where (CustomerId == 0 ? true : shipping.CustomerBillingId == CustomerId) && ((Convert.ToString(shipping.Address) != "") || (Convert.ToString(shipping.Address2) != ""))
                                              select new CustmoerShippingViewModel
                                              {
                                                  Tax= tax,
                                                  ShipId = shipping.CustomerShipId,
                                                  Address = (city1.CityName ?? "") + " " + (state1.StateName ?? "") + " " + (shipping.Address ?? "") + " " + (shipping.Address2 ?? ""),
                                                  CustmoerId = shipping.CustomerBillingId
                                              })
                                                   .ToList();

                if (CustomerShipingAddressList.Where(w => w.Address.Trim() != "").Count() == 0)
                {
                    CustomerId = _dbContext.tbl_CustomerBillings.Where(w => w.CustomerBillingId == CustomerId).Select(s => s.CustomerId).FirstOrDefault();
                    CustomerShipingAddressList = (from shipping in _dbContext.tbl_CustmoerShipping
                                                  join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                                  into city
                                                  from city1 in city.DefaultIfEmpty()
                                                  join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                                  into state
                                                  from state1 in state.DefaultIfEmpty()
                                                  where (CustomerId == 0 ? true : shipping.CustomerId == CustomerId) && ((Convert.ToString(shipping.Address) != "") || (Convert.ToString(shipping.Address2) != ""))
                                                  select new CustmoerShippingViewModel
                                                  {
                                                      Tax = tax,
                                                      ShipId = shipping.CustomerShipId,
                                                      Address = (city1.CityName ?? "") + " " + (state1.StateName ?? "") + " " + (shipping.Address ?? "") + " " + (shipping.Address2 ?? ""),
                                                      CustmoerId = shipping.CustomerId
                                                  })
                                                  .ToList();

                }

            }
            catch (Exception ex)
            {

            }

            return Json(CustomerShipingAddressList);
        }

        [HttpPost]
        public JsonResult GetCustomerShippingApartment(int ShipId)
        {


            var CustomerShipingApartmentList = new List<CustomerShippingApartmentViewModel>(); ;

            int billingId = _dbContext.tbl_CustmoerShipping.Where(w => w.CustomerShipId == ShipId)
                         .Select(s => s.CityId).FirstOrDefault();

            int cityId = _dbContext.tbl_CustomerBillings.Where(w => w.CustomerBillingId == billingId)
                         .Select(s => s.CityId).FirstOrDefault();


            decimal? tax = 0;
            tax = _dbContext.tbl_Cities.Where(w => w.CityId == cityId).Select(s => s.Tax).FirstOrDefault();
            if (tax != null)
            {
                if (tax == 0)
                {
                    tax = 1;
                }

            }
            else
            {
                tax = 1;
            }
            try
            {
                CustomerShipingApartmentList = (from apartment in _dbContext.tbl_CustomerShippingApartments
                                                where apartment.CustomerShipId == ShipId
                                                select new CustomerShippingApartmentViewModel
                                                {
                                                    ApartmentId = apartment.ApartmentId,
                                                    ApartmentNo = apartment.ApartmentNo
                                                })
                                                .ToList();
            }
            catch (Exception ex)
            {

            }

            return Json( new { aList= CustomerShipingApartmentList  , tax= tax });
        }

        [HttpPost]
        public JsonResult GetFollowUpOrderDetail(int OrderId)
        {
            OrderMasterViewModel model = new OrderMasterViewModel();
            try
            {
                int orderId = OrderId;
                var checkOrder = _dbContext.tbl_OrderMaster.Where(w => w.OrderId == orderId).FirstOrDefault();
                if (checkOrder != null)
                {
                    model.OrderId = orderId;
                    model.OrderNo = checkOrder.OrderNo;
                    model.OrderDate = checkOrder.OrderDate;
                    model.ShipStartDate = checkOrder.ShipStartDate;
                    model.ShipEndDate = checkOrder.ShipEndDate;
                    model.ShipId = checkOrder.ShipId;
                    model.CustomerId = checkOrder.CustomerId;
                    model.TotalAmount = checkOrder.TotalAmount;
                    model.ApartmentIds = checkOrder.ApartmentIds;

                    if (model.ApartmentIds != null)
                    {
                        model.ApartmentList = _dbContext.tbl_CustomerShippingApartments.
                            Select(s => new CustomerShippingApartmentViewModel()
                            {
                                ApartmentId = s.ApartmentId,
                                ApartmentNo = s.ApartmentNo,
                                ApartmentName = s.ApartmentName
                            }).Where(w => model.ApartmentIds.Contains("/" + w.ApartmentId.ToString() + "/")).ToList();
                    }

                    model.ApartmentIds = (checkOrder.ApartmentIds ?? "").Replace('/', ' ');

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

                    model.CustomerShipingAddressList = (from shipping in _dbContext.tbl_CustmoerShipping
                                                        join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                                        join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                                        where shipping.CustomerId == model.CustomerId
                                                        select new CustmoerShippingViewModel
                                                        {
                                                            ShipId = shipping.CustomerShipId,
                                                            Address = city.CityName + " " + state.StateName + " " + shipping.Address ?? ""
                                                        })
                                                 .ToList();

                }

            }
            catch (Exception ex)
            {

                var a = "";
            }

            return Json(model);
        }


        public IActionResult Edit(string id)
        {
            OrderMasterViewModel model = new OrderMasterViewModel();
            var checkReOccurrenceEndDate = _dbContext.tbl_GlobalSetting
                                                        .Where(w => w.Name == "ReOccurrenceEndDate")
                                                        .Select(s => s.Value).FirstOrDefault();

            model.MaxReOccurenceEndDate = Convert.ToDateTime(checkReOccurrenceEndDate);

            try
            {
                model.ItemList = _dbContext.tbl_ItemMaster
                    .Where(w => w.IsActive == 1)
                            .Select(s => new ItemMasterViewModel
                            {
                                ItemId = s.ItemId,
                                ItemCd = s.ItemCd,
                                ItemPrice = s.ItemPrice,
                                ItemDescription = s.ItemDescription
                            })
                            .ToList();
                model.CustomerList = _orderUtility.GetCustomerListWithShipAddress();

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

                model.OrderList = (from order in _dbContext.tbl_OrderMaster
                                   join customer in _dbContext.tbl_CustomerMaster
                                   on order.CustomerId equals customer.CustomerId
                                   where ((order.IsFollowUp ?? 0) == 1)
                                   select new ServiceFormOrderViewModel
                                   {
                                       OrderId = order.OrderId,
                                       CustomerName = customer.CompanyName + " (Order #" + order.OrderId + ")"

                                   }).ToList();

                int orderId = 0;
                int.TryParse(id, out orderId);
                var checkOrder = _dbContext.tbl_OrderMaster.Where(w => w.OrderId == orderId).FirstOrDefault();
                if (checkOrder != null)
                {
                    model.OrderId = orderId;
                    model.OrderNo = checkOrder.OrderNo;
                    model.OrderDate = checkOrder.OrderDate;
                    model.ShipStartDate = checkOrder.ShipStartDate;
                    model.ShipEndDate = checkOrder.ShipEndDate;
                    model.ShipStartTime = checkOrder.ShipStartDate;
                    model.ShipEndTime = checkOrder.ShipEndDate;
                    model.ShipId = checkOrder.ShipId;
                    model.CustomerId = checkOrder.CustomerId;
                    model.TotalAmount = checkOrder.TotalAmount;
                    model.TaxAmount = checkOrder.TaxAmount??0;
                    model.GrossAmount = model.TotalAmount + model.TaxAmount;
                    model.ApartmentIds = checkOrder.ApartmentIds;
                    model.ParentOrderId = checkOrder.ParentOrderId;

                    model.ReOccurenceCycle = checkOrder.ReOccurenceCycle;
                    model.ReOccurenceFrequency = checkOrder.ReOccurenceFrequency;
                    model.ReOccurenceWeekday = checkOrder.ReOccurenceWeekday;
                    model.ReOccurenceStartDate = checkOrder.ReOccurenceStartDate;
                    model.ReOccurenceEndDate = checkOrder.ReOccurenceEndDate;

                    if (checkOrder.ReOccurence == 1)
                    {
                        model.ReOccurence = "Yes";
                        model.ReOccurenceOrderCount = _dbContext.tbl_OrderMaster.Where(w => w.ReOccurenceParentOrderId == model.OrderId).Count();
                    }


                    if (model.ApartmentIds != null)
                    {
                        model.ApartmentList = _dbContext.tbl_CustomerShippingApartments.
                            Select(s => new CustomerShippingApartmentViewModel()
                            {
                                ApartmentId = s.ApartmentId,
                                ApartmentNo = s.ApartmentNo,
                                ApartmentName = s.ApartmentName
                            }).Where(w => model.ApartmentIds.Contains("/" + w.ApartmentId.ToString() + "/")).ToList();
                    }

                    model.ApartmentIds = (checkOrder.ApartmentIds ?? "").Replace('/', ' ');

                    var orderDetail = _dbContext.tbl_OrderDetail.Where(w => w.OrderId == orderId && w.IsActive == 1).ToList();

                    if (orderDetail != null)
                    {
                        model.OrderItemList = orderDetail.Select(s => new OrderDetailViewModel
                        {
                            OrderDetailId = s.OrderDetailId,
                            ItemId = s.ItemId,
                            Description = s.Description,
                            PerUnitPrice = s.PerUnitPrice,
                            Quantity = s.Quantity,
                            TotalPrice = s.TotalPrice,
                            UnitId = s.UnitId,

                        }).ToList();



                    }

                    var orderAssignment = _dbContext.tbl_OrderAssignment.Where(w => w.OrderId == orderId).FirstOrDefault();

                    if (orderAssignment != null)
                    {
                        model.AssigneeId = orderAssignment.EmployeeId.ToString();
                    }

                    model.CustomerId = _dbContext.tbl_CustmoerShipping.Where(w => w.CustomerShipId == model.ShipId).Select(s => s.CustomerBillingId).FirstOrDefault();

                }

            }
            catch (Exception ex)
            {

                var a = "";
            }


            return View(model);
        }

        public IActionResult EditModal(string id)
        {
            OrderMasterViewModel model = new OrderMasterViewModel();
            var checkReOccurrenceEndDate = _dbContext.tbl_GlobalSetting
                                                        .Where(w => w.Name == "ReOccurrenceEndDate")
                                                        .Select(s => s.Value).FirstOrDefault();

            model.MaxReOccurenceEndDate = Convert.ToDateTime(checkReOccurrenceEndDate);

            try
            {
                model.ItemList = _dbContext.tbl_ItemMaster
                    .Where(w => w.IsActive == 1)
                            .Select(s => new ItemMasterViewModel
                            {
                                ItemId = s.ItemId,
                                ItemCd = s.ItemCd,
                                ItemPrice = s.ItemPrice,
                                ItemDescription = s.ItemDescription
                            })
                            .ToList();
                model.CustomerList = _orderUtility.GetCustomerListWithShipAddress();

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

                model.OrderList = (from order in _dbContext.tbl_OrderMaster
                                   join customer in _dbContext.tbl_CustomerMaster
                                   on order.CustomerId equals customer.CustomerId
                                   where ((order.IsFollowUp ?? 0) == 1)
                                   select new ServiceFormOrderViewModel
                                   {
                                       OrderId = order.OrderId,
                                       CustomerName = customer.CompanyName + " (Order #" + order.OrderId + ")"

                                   }).ToList();

                int orderId = 0;
                int.TryParse(id, out orderId);
                var checkOrder = _dbContext.tbl_OrderMaster.Where(w => w.OrderId == orderId).FirstOrDefault();
                if (checkOrder != null)
                {
                    model.OrderId = orderId;
                    model.OrderNo = checkOrder.OrderNo;
                    model.OrderDate = checkOrder.OrderDate;
                    model.ShipStartDate = checkOrder.ShipStartDate;
                    model.ShipEndDate = checkOrder.ShipEndDate;
                    model.ShipStartTime = checkOrder.ShipStartDate;
                    model.ShipEndTime = checkOrder.ShipEndDate;
                    model.ShipId = checkOrder.ShipId;
                    model.CustomerId = checkOrder.CustomerId;
                    model.TotalAmount = checkOrder.TotalAmount;
                    model.TaxAmount = checkOrder.TaxAmount ?? 0;
                    model.GrossAmount = model.TotalAmount + model.TaxAmount;
                    model.ApartmentIds = checkOrder.ApartmentIds;
                    model.ParentOrderId = checkOrder.ParentOrderId;

                    model.ReOccurenceCycle = checkOrder.ReOccurenceCycle;
                    model.ReOccurenceFrequency = checkOrder.ReOccurenceFrequency;
                    model.ReOccurenceWeekday = checkOrder.ReOccurenceWeekday;
                    model.ReOccurenceStartDate = checkOrder.ReOccurenceStartDate;
                    model.ReOccurenceEndDate = checkOrder.ReOccurenceEndDate;

                    if (checkOrder.ReOccurence == 1)
                    {
                        model.ReOccurence = "Yes";
                        model.ReOccurenceOrderCount = _dbContext.tbl_OrderMaster.Where(w => w.ReOccurenceParentOrderId == model.OrderId).Count();
                    }


                    if (model.ApartmentIds != null)
                    {
                        model.ApartmentList = _dbContext.tbl_CustomerShippingApartments.
                            Select(s => new CustomerShippingApartmentViewModel()
                            {
                                ApartmentId = s.ApartmentId,
                                ApartmentNo = s.ApartmentNo,
                                ApartmentName = s.ApartmentName
                            }).Where(w => model.ApartmentIds.Contains("/" + w.ApartmentId.ToString() + "/")).ToList();
                    }

                    model.ApartmentIds = (checkOrder.ApartmentIds ?? "").Replace('/', ' ');

                    var orderDetail = _dbContext.tbl_OrderDetail.Where(w => w.OrderId == orderId && w.IsActive == 1).ToList();

                    if (orderDetail != null)
                    {
                        model.OrderItemList = orderDetail.Select(s => new OrderDetailViewModel
                        {
                            OrderDetailId = s.OrderDetailId,
                            ItemId = s.ItemId,
                            Description = s.Description,
                            PerUnitPrice = s.PerUnitPrice,
                            Quantity = s.Quantity,
                            TotalPrice = s.TotalPrice,
                            UnitId = s.UnitId,

                        }).ToList();



                    }

                    var orderAssignment = _dbContext.tbl_OrderAssignment.Where(w => w.OrderId == orderId).FirstOrDefault();

                    if (orderAssignment != null)
                    {
                        model.AssigneeId = orderAssignment.EmployeeId.ToString();
                    }

                }

                model.CustomerId = _dbContext.tbl_CustmoerShipping.Where(w => w.CustomerShipId == model.ShipId).Select(s => s.CustomerBillingId).FirstOrDefault();


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
            var _customerd = model.CustomerId;
            model.CustomerId = _dbContext.tbl_CustomerBillings.Where(w => w.CustomerBillingId == model.CustomerId).Select(s => s.CustomerId).FirstOrDefault();

            try
            {
                // if (ModelState.IsValid)
                {
                    OrderAssignment orderAssignmentReoccurence = new OrderAssignment();
                    List<OrderDetailViewModel> orderDetailReoccurence = new List<OrderDetailViewModel>();
                    var checkOrderNo = _dbContext.tbl_OrderMaster.Where(w => w.OrderNo == model.OrderNo && w.OrderId != model.OrderId).FirstOrDefault();
                    if (checkOrderNo != null)
                    {
                        ViewBag.ErrorMessage = "Order no already exists";
                        model.ItemList = _dbContext.tbl_ItemMaster
                .Where(w => w.IsActive == 1)
                        .Select(s => new ItemMasterViewModel
                        {
                            ItemId = s.ItemId,
                            ItemCd = s.ItemCd,
                            ItemPrice = s.ItemPrice,
                            ItemDescription = s.ItemDescription
                        })
                        .ToList();
                        model.CustomerList = _orderUtility.GetCustomerListWithShipAddress();



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


                        model.CustomerDetail.CityList = _dbContext.tbl_Cities
                          .Where(w => w.IsActive == 1)
                                  .Select(s => new CityViewModel
                                  {
                                      CityId = s.CityId,
                                      CityName = s.CityName
                                  })
                                  .ToList();

                        model.CustomerDetail.StateList = _dbContext.tbl_States
                            .Where(w => w.IsActive == 1)
                                    .Select(s => new StateViewModel
                                    {
                                        StateId = s.StateId,
                                        StateName = s.StateName
                                    })
                                    .ToList();
                        model.CustomerDetail.Contacts.Add(new CustmoerContactViewModel());
                        model.CustomerDetail.Shippings.Add(new CustmoerShippingViewModel());

                        return View(model);

                    }


                    var checkOrder = _dbContext.tbl_OrderMaster.Where(w => w.OrderId == model.OrderId).FirstOrDefault();
                    if (checkOrder != null)
                    {
                        if (model.ApartmentId != null)
                        {
                            var csvList = model.ApartmentId.Select(s => "/" + s + "/").ToList();
                            model.ApartmentIds = String.Join(',', csvList);

                        }

                        var prevReOccurence = checkOrder.ReOccurence;
                        var prevReOccurenceCycle = checkOrder.ReOccurenceCycle;
                        var prevReOccurenceFrequency = checkOrder.ReOccurenceFrequency;
                        var prevReOccurenceWeekday = checkOrder.ReOccurenceWeekday;
                        var prevReOccurenceStartDate = checkOrder.ReOccurenceStartDate;
                        var prevReOccurenceEndDate = checkOrder.ReOccurenceEndDate;


                        DateTime? shipDate = null;

                        if (model.ShipStartDate != null)
                        {
                            CommanUtility _commanUtility = new CommanUtility(_appSettings);




                            if (model.ShipStartTime != null)
                            {
                                model.ShipStartDate = Convert.ToDateTime(model.ShipStartDate.Value.ToString("MM/dd/yyyy") + " " + model.ShipStartTime.Value.ToString("HH:mm"));
                            }
                            else
                            {
                                var curDayName = model.ShipStartDate.Value.DayOfWeek.ToString();
                                var checkCalenderDate1 = _dbContext.tbl_CalenderWorkingHours.Where(w => w.DayName == curDayName).FirstOrDefault();
                                if (checkCalenderDate1 != null)
                                {
                                    model.ShipStartDate = Convert.ToDateTime(model.ShipStartDate.Value.ToString("MM/dd/yyyy") + " " + checkCalenderDate1.StartTime.Value.ToString("HH:mm"));
                                }

                            }

                            shipDate = _commanUtility.RoundUp(model.ShipStartDate.Value, TimeSpan.FromMinutes(15));



                            if (model.ShipEndDate != null)
                            {
                                if (model.ShipEndTime != null)
                                {
                                    model.ShipEndDate = Convert.ToDateTime(model.ShipEndDate.Value.ToString("MM/dd/yyyy") + " " + model.ShipEndTime.Value.ToString("HH:mm"));
                                    model.ShipEndDate = shipDate.Value.AddMinutes((model.ShipEndDate.Value - model.ShipStartDate.Value).TotalMinutes);
                                }
                                else
                                {
                                    model.ShipEndDate = shipDate.Value.AddMinutes(30);
                                }


                            }

                            var checkCalenderDate = _dbContext.tbl_CalenderWorkingHours.FirstOrDefault();
                            if (checkCalenderDate != null)
                            {
                                var calenderStartDate = Convert.ToDateTime(shipDate.Value.ToString("MM/dd/yyyy") + " " + checkCalenderDate.StartTime.Value.ToString("HH:mm"));
                                var calenderEndDate = Convert.ToDateTime(shipDate.Value.ToString("MM/dd/yyyy") + " " + checkCalenderDate.EndTime.Value.ToString("HH:mm"));
                                if ((shipDate < calenderStartDate) || (shipDate > calenderEndDate))
                                {
                                    // shipDate = calenderStartDate;
                                    //shipDate = _commanUtility.RoundUp(model.ShipStartDate.Value, TimeSpan.FromMinutes(15));

                                }
                            }

                        }

                        checkOrder.OrderDate = model.OrderDate;
                        checkOrder.ShipStartDate = shipDate;
                        checkOrder.ShipEndDate = model.ShipEndDate;
                        checkOrder.ShipDate = model.ShipEndDate;
                        checkOrder.ShipId = model.ShipId;
                        checkOrder.CustomerId = model.CustomerId;
                        checkOrder.TotalAmount = model.TotalAmount;
                        checkOrder.TaxAmount = model.TaxAmount;
                        checkOrder.ModifiedBy = 1;
                        checkOrder.ModifiedDate = DateTime.Now;
                        checkOrder.OrderNo = model.OrderNo;
                        checkOrder.ApartmentIds = model.ApartmentIds;
                        checkOrder.ParentOrderId = model.ParentOrderId;
                        if ((model.ReOccurence == "Yes"))
                        {
                            if (model.ReOccurenceEndDate == null)
                            {
                                var checkReOccurrenceEndDate = _dbContext.tbl_GlobalSetting
                                                      .Where(w => w.Name == "ReOccurrenceEndDate")
                                                      .Select(s => s.Value).FirstOrDefault();

                                model.ReOccurenceEndDate = Convert.ToDateTime(checkReOccurrenceEndDate);
                            }

                            checkOrder.ReOccurence = 1;
                        }
                        else
                        {
                            checkOrder.ReOccurence = 0;
                        }

                        checkOrder.ReOccurenceCycle = model.ReOccurenceCycle;
                        checkOrder.ReOccurenceFrequency = model.ReOccurenceFrequency;
                        checkOrder.ReOccurenceWeekday = model.ReOccurenceWeekday;
                        checkOrder.ReOccurenceStartDate = model.ReOccurenceStartDate;
                        checkOrder.ReOccurenceEndDate = model.ReOccurenceEndDate;
                        _dbContext.SaveChanges();

                        var checkExistingItems = _dbContext.tbl_OrderDetail.Where(w => w.OrderId == model.OrderId).ToList();

                        foreach (var item in checkExistingItems)
                        {
                            item.IsActive = 0;
                            item.ModifiedBy = _userId;
                            item.ModifiedDate= DateTime.Now;
                            _dbContext.SaveChanges();
                        
                        }



                        foreach (var item in model.OrderItemList)
                        {
                            if (item.OrderDetailId > 0)
                            {
                                var checkOrderDetail = _dbContext.tbl_OrderDetail.Where(w => w.OrderDetailId == item.OrderDetailId).FirstOrDefault();
                                if (checkOrderDetail != null)
                                {
                                    checkOrderDetail.ItemId = item.ItemId;
                                    checkOrderDetail.Description = item.Description;
                                    checkOrderDetail.OrderId = checkOrder.OrderId;
                                    checkOrderDetail.PerUnitPrice = item.PerUnitPrice;
                                    checkOrderDetail.Quantity = item.Quantity;
                                    checkOrderDetail.TotalPrice = item.TotalPrice;
                                    checkOrderDetail.UnitId = item.UnitId;
                                    checkOrderDetail.IsActive = 1;
                                    checkOrderDetail.ModifiedBy = _userId;
                                    checkOrderDetail.ModifiedDate = DateTime.Now;
                                    _dbContext.SaveChanges();



                                }
                            }
                            else
                            {
                                OrderDetail orderDetail = new OrderDetail()
                                {
                                    ItemId = item.ItemId,
                                    Description = item.Description,
                                    OrderId = checkOrder.OrderId,
                                    PerUnitPrice = item.PerUnitPrice,
                                    Quantity = item.Quantity,
                                    TotalPrice = item.TotalPrice,
                                    UnitId = item.UnitId,
                                    IsActive = 1,
                                    CreatedBy = 1,
                                    CreatedDate = DateTime.Now
                                };
                                _dbContext.tbl_OrderDetail.Add(orderDetail);
                                _dbContext.SaveChanges();
                            }

                        }
                        orderDetailReoccurence = model.OrderItemList;


                        var checkOrderAssignment = _dbContext.tbl_OrderAssignment.Where(w => w.OrderId == model.OrderId).FirstOrDefault();
                        if (checkOrderAssignment != null)
                        {
                            if (Convert.ToString(model.AssigneeId) != "")
                            {
                                model.EmployeeId = Convert.ToInt32(model.AssigneeId);
                                checkOrderAssignment.EmployeeId = model.EmployeeId;
                                _dbContext.SaveChanges();
                                orderAssignmentReoccurence = checkOrderAssignment;
                            }
                            else
                            {
                                checkOrderAssignment.EmployeeId = 0;
                                _dbContext.SaveChanges();
                                orderAssignmentReoccurence = checkOrderAssignment;
                            }
                        }
                        else
                        {
                            if (Convert.ToString(model.AssigneeId) != "")
                            {
                                model.EmployeeId = Convert.ToInt32(model.AssigneeId);
                                OrderAssignment orderAssignment = new OrderAssignment()
                                {
                                    OrderId = checkOrder.OrderId,
                                    EmployeeId = model.EmployeeId,
                                    AssignmentDate = DateTime.Now,
                                    Status = "Assigned"
                                };
                                _dbContext.tbl_OrderAssignment.Add(orderAssignment);
                                _dbContext.SaveChanges();
                                orderAssignmentReoccurence = orderAssignment;

                            }
                        }

                        var checkShipStartDateFormatted = (model.ShipStartDate == null ? DateTime.Now.AddDays(-1).ToShortDateString() : model.ShipStartDate.Value.ToShortDateString());

                        if (prevReOccurence == 0 && checkOrder.ReOccurence == 1)
                        {
                            OrderUtility _orderUtility = new OrderUtility(_dbContext);
                            var orderId = checkOrder.OrderId;
                            int reoccurrenceOrderCount = 1;
                            if (model.ReOccurenceCycle == "Days")
                            {
                                var count = 0;
                                for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                {
                                    if (checkShipStartDateFormatted != date.ToShortDateString())
                                    {
                                        count++;
                                        if (count == model.ReOccurenceFrequency)
                                        {
                                            if (reoccurrenceOrderCount < 12)
                                            {
                                                count = 0;
                                                _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                reoccurrenceOrderCount++;
                                            }

                                        }
                                    }

                                }
                            }

                            if (model.ReOccurenceCycle == "Weeks")
                            {
                                var count = 0;
                                for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                {
                                    if (checkShipStartDateFormatted != date.ToShortDateString())
                                    {


                                        count++;
                                        if (count == ((model.ReOccurenceFrequency * 7)))
                                        {
                                            if (reoccurrenceOrderCount < 12)
                                            {
                                                count = 0;
                                                _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                reoccurrenceOrderCount++;
                                            }

                                        }
                                    }
                                }
                            }

                            if (model.ReOccurenceCycle == "Months")
                            {
                                var count = 0;
                                for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                {
                                    if (checkShipStartDateFormatted != date.ToShortDateString())
                                    {


                                        count++;
                                        if (count == ((model.ReOccurenceFrequency * 30)))
                                        {
                                            if (reoccurrenceOrderCount < 12)
                                            {
                                                count = 0;
                                                _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                reoccurrenceOrderCount++;
                                            }

                                        }
                                    }
                                }
                            }


                            if (model.ReOccurenceCycle == "WeekDay")
                            {

                                for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                {
                                    if (checkShipStartDateFormatted != date.ToShortDateString())
                                    {

                                        if (model.ReOccurenceFrequency == 1)
                                        {
                                            if (date.Day >= 1 && date.Day <= 7)
                                            {
                                                if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                                {
                                                    if (model.ReOccurenceStartDateSetBy != "System")
                                                    {
                                                        if (reoccurrenceOrderCount < 12)
                                                        {
                                                            _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                            reoccurrenceOrderCount++;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                        {
                                                            if (reoccurrenceOrderCount < 12)
                                                            {
                                                                _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                                reoccurrenceOrderCount++;
                                                            }

                                                        }

                                                    }
                                                }


                                            }
                                        }
                                        else if (model.ReOccurenceFrequency == 0)
                                        {
                                            if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                            {
                                                if (model.ReOccurenceStartDateSetBy != "System")
                                                {
                                                    if (reoccurrenceOrderCount < 12)
                                                    {
                                                        _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                        reoccurrenceOrderCount++;
                                                    }

                                                }
                                                else
                                                {
                                                    if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                    {
                                                        if (reoccurrenceOrderCount < 12)
                                                        {
                                                            _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                            reoccurrenceOrderCount++;
                                                        }

                                                    }

                                                }
                                            }
                                        }
                                        else if (model.ReOccurenceFrequency > 1)
                                        {
                                            if (date.Day > ((model.ReOccurenceFrequency - 1) * 7) && date.Day <= ((model.ReOccurenceFrequency) * 7))
                                            {
                                                if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                                {
                                                    if (model.ReOccurenceStartDateSetBy != "System")
                                                    {
                                                        if (reoccurrenceOrderCount < 12)
                                                        {
                                                            _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                            reoccurrenceOrderCount++;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                        {
                                                            if (reoccurrenceOrderCount < 12)
                                                            {
                                                                _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                                reoccurrenceOrderCount++;
                                                            }

                                                        }

                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                            }

                            if (model.ReOccurenceCycle == "Month's Day")
                            {

                                for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                {
                                    if (checkShipStartDateFormatted != date.ToShortDateString())
                                    {
                                        if (date.Day == model.ReOccurenceFrequency)
                                        {
                                            if (reoccurrenceOrderCount < 12)
                                            {
                                                _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                reoccurrenceOrderCount++;
                                            }

                                        }
                                    }
                                }
                            }

                        }

                        if (prevReOccurence == 1 && checkOrder.ReOccurence == 0)
                        {

                            var checkReOccurenceOrders = _dbContext.tbl_OrderMaster.Where(w => w.ReOccurenceParentOrderId == checkOrder.OrderId).ToList();
                            foreach (var item in checkReOccurenceOrders)
                            {
                                item.IsActive = 0;
                                item.ModifiedBy = _userId;
                                item.ModifiedDate = DateTime.Now;
                                _dbContext.SaveChanges();

                            }

                        }

                        if (prevReOccurence == 1 && checkOrder.ReOccurence == 1)
                        {
                            var orderId = checkOrder.OrderId;
                            if (!((prevReOccurence == checkOrder.ReOccurence) &&
                                (prevReOccurenceCycle == checkOrder.ReOccurenceCycle) &&
                                (prevReOccurenceStartDate == checkOrder.ReOccurenceStartDate) &&
                                (prevReOccurenceEndDate == checkOrder.ReOccurenceEndDate) &&
                                (prevReOccurenceFrequency == checkOrder.ReOccurenceFrequency)
                                ))
                            {
                                if (model.ReOccurenceOrderCount != -1)
                                {

                                    var checkReOccurenceOrders = _dbContext.tbl_OrderMaster.Where(w => w.ReOccurenceParentOrderId == checkOrder.OrderId).ToList();
                                    foreach (var item in checkReOccurenceOrders)
                                    {
                                        item.IsActive = 0;
                                        item.ModifiedBy = _userId;
                                        item.ModifiedDate = DateTime.Now;
                                        _dbContext.SaveChanges();

                                    }
                                    int reoccurrenceOrderCount = 1;
                                    OrderUtility _orderUtility = new OrderUtility(_dbContext);

                                    if (model.ReOccurenceCycle == "Days")
                                    {
                                        var count = 0;
                                        for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                        {
                                            if (checkShipStartDateFormatted != date.ToShortDateString())
                                            {
                                                count++;
                                                if (count == model.ReOccurenceFrequency)
                                                {
                                                    if (reoccurrenceOrderCount < 12)
                                                    {
                                                        count = 0;
                                                        _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                        reoccurrenceOrderCount++;
                                                    }

                                                }
                                            }

                                        }
                                    }

                                    if (model.ReOccurenceCycle == "Weeks")
                                    {
                                        var count = 0;
                                        for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                        {
                                            if (checkShipStartDateFormatted != date.ToShortDateString())
                                            {


                                                count++;
                                                if (count == ((model.ReOccurenceFrequency * 7)))
                                                {
                                                    if (reoccurrenceOrderCount < 12)
                                                    {
                                                        count = 0;
                                                        _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                        reoccurrenceOrderCount++;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (model.ReOccurenceCycle == "Months")
                                    {
                                        var count = 0;
                                        for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                        {
                                            if (checkShipStartDateFormatted != date.ToShortDateString())
                                            {


                                                count++;
                                                if (count == ((model.ReOccurenceFrequency * 30)))
                                                {
                                                    if (reoccurrenceOrderCount < 12)
                                                    {
                                                        count = 0;
                                                        _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                        reoccurrenceOrderCount++;
                                                    }
                                                }
                                            }
                                        }
                                    }


                                    if (model.ReOccurenceCycle == "WeekDay")
                                    {

                                        for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                        {
                                            if (checkShipStartDateFormatted != date.ToShortDateString())
                                            {

                                                if (model.ReOccurenceFrequency == 1)
                                                {
                                                    if (date.Day >= 1 && date.Day <= 7)
                                                    {
                                                        if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                                        {
                                                            if (model.ReOccurenceStartDateSetBy != "System")
                                                            {
                                                                if (reoccurrenceOrderCount < 12)
                                                                {
                                                                    _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                                    reoccurrenceOrderCount++;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                                {
                                                                    if (reoccurrenceOrderCount < 12)
                                                                    {
                                                                        _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                                        reoccurrenceOrderCount++;
                                                                    }
                                                                }

                                                            }

                                                        }
                                                    }
                                                }
                                                else if (model.ReOccurenceFrequency == 0)
                                                {
                                                    if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                                    {
                                                        if (model.ReOccurenceStartDateSetBy != "System")
                                                        {
                                                            if (reoccurrenceOrderCount < 12)
                                                            {
                                                                _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                                reoccurrenceOrderCount++;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                            {
                                                                if (reoccurrenceOrderCount < 12)
                                                                {
                                                                    _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                                    reoccurrenceOrderCount++;
                                                                }
                                                            }

                                                        }
                                                    }
                                                }
                                                else if (model.ReOccurenceFrequency > 1)
                                                {
                                                    if (date.Day > ((model.ReOccurenceFrequency - 1) * 7) && date.Day <= ((model.ReOccurenceFrequency) * 7))
                                                    {
                                                        if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                                        {
                                                            if (model.ReOccurenceStartDateSetBy != "System")
                                                            {
                                                                if (reoccurrenceOrderCount < 12)
                                                                {
                                                                    _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                                    reoccurrenceOrderCount++;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                                {
                                                                    if (reoccurrenceOrderCount < 12)
                                                                    {
                                                                        _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                                        reoccurrenceOrderCount++;
                                                                    }
                                                                }

                                                            }

                                                        }
                                                    }
                                                }
                                            }

                                        }
                                    }

                                    if (model.ReOccurenceCycle == "Month's Day")
                                    {

                                        for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                        {
                                            if (checkShipStartDateFormatted != date.ToShortDateString())
                                            {
                                                if (date.Day == model.ReOccurenceFrequency)
                                                {
                                                    if (reoccurrenceOrderCount < 12)
                                                    {
                                                        _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                        reoccurrenceOrderCount++;
                                                    }
                                                }
                                            }
                                        }
                                    }


                                }

                            }
                            else
                            {
                                if (model.ReOccurenceOrderCount != -1)
                                {
                                    var checkReOccurenceOrders = _dbContext.tbl_OrderMaster.Where(w => w.ReOccurenceParentOrderId == checkOrder.OrderId).ToList();
                                    foreach (var item in checkReOccurenceOrders)
                                    {
                                        _orderUtility.updateReOccurenceOrder(item.OrderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, item.OrderDate);

                                    }
                                }
                            }
                        }

                        if (model.ReOccurenceOrderCount != -1)
                        {
                            var checkReoccurenceOrders = _dbContext.tbl_OrderMaster.Where(w => w.ReOccurenceParentOrderId == checkOrder.OrderId).ToList();
                            foreach (var item in checkReoccurenceOrders)
                            {
                                item.OrderDate = checkOrder.OrderDate;
                                item.ShipId = checkOrder.ShipId;
                                item.CustomerId = checkOrder.CustomerId;
                                item.TotalAmount = checkOrder.TotalAmount;
                                item.ModifiedBy = 1;
                                item.ModifiedDate = DateTime.Now;
                                item.ApartmentIds = checkOrder.ApartmentIds;
                                item.ParentOrderId = checkOrder.ParentOrderId;

                                _dbContext.SaveChanges();

                                if (model.OrderItemList.Count() > 0)
                                {

                                    var checkOrderItems = _dbContext.tbl_OrderDetail.Where(w => w.OrderId == item.OrderId).ToList();
                                    foreach (var item1 in checkOrderItems)
                                    {
                                        item1.IsActive = 0;
                                        item1.ModifiedDate = DateTime.Now;
                                        _dbContext.SaveChanges();
                                    }

                                    foreach (var item1 in model.OrderItemList)
                                    {
                                        OrderDetail orderDetail = new OrderDetail()
                                        {
                                            ItemId = item1.ItemId,
                                            Description = item1.Description,
                                            OrderId = item.OrderId,
                                            PerUnitPrice = item1.PerUnitPrice,
                                            Quantity = item1.Quantity,
                                            TotalPrice = item1.TotalPrice,
                                            UnitId = item1.UnitId,
                                            IsActive = 1,
                                            CreatedBy = 1,
                                            CreatedDate = DateTime.Now
                                        };
                                        _dbContext.tbl_OrderDetail.Add(orderDetail);
                                        _dbContext.SaveChanges();

                                    }

                                    if (orderAssignmentReoccurence != null)
                                    {
                                        var checkOrderAssignmentDetail = _dbContext.tbl_OrderAssignment.Where(w => w.OrderId == item.OrderId).FirstOrDefault();
                                        if (checkOrderAssignmentDetail != null)
                                        {
                                            if (Convert.ToString(model.AssigneeId) != "")
                                            {
                                                model.EmployeeId = Convert.ToInt32(model.AssigneeId);
                                                checkOrderAssignmentDetail.EmployeeId = model.EmployeeId;
                                                _dbContext.SaveChanges();

                                            }
                                            else
                                            {
                                                checkOrderAssignmentDetail.EmployeeId = 0;
                                                _dbContext.SaveChanges();

                                            }
                                        }
                                        else
                                        {
                                            if (Convert.ToString(model.AssigneeId) != "")
                                            {
                                                OrderAssignment orderAssignment = new OrderAssignment()
                                                {
                                                    OrderId = item.OrderId,
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
                        }
                    }

                    ViewBag.SuccessMessage = "Order update successfully";

                }
            }
            catch (Exception ex)
            {
                var a = "";
            }
            model.CustomerId = _customerd;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditModal(OrderMasterViewModel model)
        {
            var _customerd = model.CustomerId;
            model.CustomerId = _dbContext.tbl_CustomerBillings.Where(w => w.CustomerBillingId == model.CustomerId).Select(s => s.CustomerId).FirstOrDefault();

            try
            {
                // if (ModelState.IsValid)
                {
                    OrderAssignment orderAssignmentReoccurence = new OrderAssignment();
                    List<OrderDetailViewModel> orderDetailReoccurence = new List<OrderDetailViewModel>();
                    var checkOrderNo = _dbContext.tbl_OrderMaster.Where(w => w.OrderNo == model.OrderNo && w.OrderId != model.OrderId).FirstOrDefault();
                    if (checkOrderNo != null)
                    {
                        ViewBag.ErrorMessage = "Order no already exists";
                        model.ItemList = _dbContext.tbl_ItemMaster
                .Where(w => w.IsActive == 1)
                        .Select(s => new ItemMasterViewModel
                        {
                            ItemId = s.ItemId,
                            ItemCd = s.ItemCd,
                            ItemPrice = s.ItemPrice,
                            ItemDescription = s.ItemDescription
                        })
                        .ToList();
                        model.CustomerList = _orderUtility.GetCustomerListWithShipAddress();



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


                        model.CustomerDetail.CityList = _dbContext.tbl_Cities
                          .Where(w => w.IsActive == 1)
                                  .Select(s => new CityViewModel
                                  {
                                      CityId = s.CityId,
                                      CityName = s.CityName
                                  })
                                  .ToList();

                        model.CustomerDetail.StateList = _dbContext.tbl_States
                            .Where(w => w.IsActive == 1)
                                    .Select(s => new StateViewModel
                                    {
                                        StateId = s.StateId,
                                        StateName = s.StateName
                                    })
                                    .ToList();
                        model.CustomerDetail.Contacts.Add(new CustmoerContactViewModel());
                        model.CustomerDetail.Shippings.Add(new CustmoerShippingViewModel());

                        return View(model);

                    }


                    var checkOrder = _dbContext.tbl_OrderMaster.Where(w => w.OrderId == model.OrderId).FirstOrDefault();
                    if (checkOrder != null)
                    {
                        if (model.ApartmentId != null)
                        {
                            var csvList = model.ApartmentId.Select(s => "/" + s + "/").ToList();
                            model.ApartmentIds = String.Join(',', csvList);

                        }

                        var prevReOccurence = checkOrder.ReOccurence;
                        var prevReOccurenceCycle = checkOrder.ReOccurenceCycle;
                        var prevReOccurenceFrequency = checkOrder.ReOccurenceFrequency;
                        var prevReOccurenceWeekday = checkOrder.ReOccurenceWeekday;
                        var prevReOccurenceStartDate = checkOrder.ReOccurenceStartDate;
                        var prevReOccurenceEndDate = checkOrder.ReOccurenceEndDate;


                        DateTime? shipDate = null;

                        if (model.ShipStartDate != null)
                        {
                            CommanUtility _commanUtility = new CommanUtility(_appSettings);




                            if (model.ShipStartTime != null)
                            {
                                model.ShipStartDate = Convert.ToDateTime(model.ShipStartDate.Value.ToString("MM/dd/yyyy") + " " + model.ShipStartTime.Value.ToString("HH:mm"));
                            }
                            else
                            {
                                var curDayName = model.ShipStartDate.Value.DayOfWeek.ToString();
                                var checkCalenderDate1 = _dbContext.tbl_CalenderWorkingHours.Where(w => w.DayName == curDayName).FirstOrDefault();
                                if (checkCalenderDate1 != null)
                                {
                                    model.ShipStartDate = Convert.ToDateTime(model.ShipStartDate.Value.ToString("MM/dd/yyyy") + " " + checkCalenderDate1.StartTime.Value.ToString("HH:mm"));
                                }

                            }

                            shipDate = _commanUtility.RoundUp(model.ShipStartDate.Value, TimeSpan.FromMinutes(15));



                            if (model.ShipEndDate != null)
                            {
                                if (model.ShipEndTime != null)
                                {
                                    model.ShipEndDate = Convert.ToDateTime(model.ShipEndDate.Value.ToString("MM/dd/yyyy") + " " + model.ShipEndTime.Value.ToString("HH:mm"));
                                    model.ShipEndDate = shipDate.Value.AddMinutes((model.ShipEndDate.Value - model.ShipStartDate.Value).TotalMinutes);
                                }
                                else
                                {
                                    model.ShipEndDate = shipDate.Value.AddMinutes(30);
                                }


                            }

                            var checkCalenderDate = _dbContext.tbl_CalenderWorkingHours.FirstOrDefault();
                            if (checkCalenderDate != null)
                            {
                                var calenderStartDate = Convert.ToDateTime(shipDate.Value.ToString("MM/dd/yyyy") + " " + checkCalenderDate.StartTime.Value.ToString("HH:mm"));
                                var calenderEndDate = Convert.ToDateTime(shipDate.Value.ToString("MM/dd/yyyy") + " " + checkCalenderDate.EndTime.Value.ToString("HH:mm"));
                                if ((shipDate < calenderStartDate) || (shipDate > calenderEndDate))
                                {
                                    // shipDate = calenderStartDate;
                                    //shipDate = _commanUtility.RoundUp(model.ShipStartDate.Value, TimeSpan.FromMinutes(15));

                                }
                            }

                        }

                        checkOrder.OrderDate = model.OrderDate;
                        checkOrder.ShipStartDate = shipDate;
                        checkOrder.ShipEndDate = model.ShipEndDate;
                        checkOrder.ShipDate = model.ShipEndDate;
                        checkOrder.ShipId = model.ShipId;
                        checkOrder.CustomerId = model.CustomerId;
                        checkOrder.TotalAmount = model.TotalAmount;
                        checkOrder.TaxAmount = model.TaxAmount;
                        checkOrder.ModifiedBy = 1;
                        checkOrder.ModifiedDate = DateTime.Now;
                        checkOrder.OrderNo = model.OrderNo;
                        checkOrder.ApartmentIds = model.ApartmentIds;
                        checkOrder.ParentOrderId = model.ParentOrderId;
                        if ((model.ReOccurence == "Yes"))
                        {
                            if (model.ReOccurenceEndDate == null)
                            {
                                var checkReOccurrenceEndDate = _dbContext.tbl_GlobalSetting
                                                      .Where(w => w.Name == "ReOccurrenceEndDate")
                                                      .Select(s => s.Value).FirstOrDefault();

                                model.ReOccurenceEndDate = Convert.ToDateTime(checkReOccurrenceEndDate);
                            }

                            checkOrder.ReOccurence = 1;
                        }
                        else
                        {
                            checkOrder.ReOccurence = 0;
                        }

                        checkOrder.ReOccurenceCycle = model.ReOccurenceCycle;
                        checkOrder.ReOccurenceFrequency = model.ReOccurenceFrequency;
                        checkOrder.ReOccurenceWeekday = model.ReOccurenceWeekday;
                        checkOrder.ReOccurenceStartDate = model.ReOccurenceStartDate;
                        checkOrder.ReOccurenceEndDate = model.ReOccurenceEndDate;
                        _dbContext.SaveChanges();

                        foreach (var item in model.OrderItemList)
                        {
                            if (item.OrderDetailId > 0)
                            {
                                var checkOrderDetail = _dbContext.tbl_OrderDetail.Where(w => w.OrderDetailId == item.OrderDetailId).FirstOrDefault();
                                if (checkOrderDetail != null)
                                {
                                    checkOrderDetail.ItemId = item.ItemId;
                                    checkOrderDetail.Description = item.Description;
                                    checkOrderDetail.OrderId = checkOrder.OrderId;
                                    checkOrderDetail.PerUnitPrice = item.PerUnitPrice;
                                    checkOrderDetail.Quantity = item.Quantity;
                                    checkOrderDetail.TotalPrice = item.TotalPrice;
                                    checkOrderDetail.UnitId = item.UnitId;

                                    _dbContext.SaveChanges();



                                }
                            }
                            else
                            {
                                OrderDetail orderDetail = new OrderDetail()
                                {
                                    ItemId = item.ItemId,
                                    Description = item.Description,
                                    OrderId = checkOrder.OrderId,
                                    PerUnitPrice = item.PerUnitPrice,
                                    Quantity = item.Quantity,
                                    TotalPrice = item.TotalPrice,
                                    UnitId = item.UnitId,
                                    IsActive = 1,
                                    CreatedBy = 1,
                                    CreatedDate = DateTime.Now
                                };
                                _dbContext.tbl_OrderDetail.Add(orderDetail);
                                _dbContext.SaveChanges();
                            }

                        }
                        orderDetailReoccurence = model.OrderItemList;


                        var checkOrderAssignment = _dbContext.tbl_OrderAssignment.Where(w => w.OrderId == model.OrderId).FirstOrDefault();
                        if (checkOrderAssignment != null)
                        {
                            if (Convert.ToString(model.AssigneeId) != "")
                            {
                                model.EmployeeId = Convert.ToInt32(model.AssigneeId);
                                checkOrderAssignment.EmployeeId = model.EmployeeId;
                                _dbContext.SaveChanges();
                                orderAssignmentReoccurence = checkOrderAssignment;
                            }
                            else
                            {
                                checkOrderAssignment.EmployeeId = 0;
                                _dbContext.SaveChanges();
                                orderAssignmentReoccurence = checkOrderAssignment;
                            }
                        }
                        else
                        {
                            if (Convert.ToString(model.AssigneeId) != "")
                            {
                                model.EmployeeId = Convert.ToInt32(model.AssigneeId);
                                OrderAssignment orderAssignment = new OrderAssignment()
                                {
                                    OrderId = checkOrder.OrderId,
                                    EmployeeId = model.EmployeeId,
                                    AssignmentDate = DateTime.Now,
                                    Status = "Assigned"
                                };
                                _dbContext.tbl_OrderAssignment.Add(orderAssignment);
                                _dbContext.SaveChanges();
                                orderAssignmentReoccurence = orderAssignment;

                            }
                        }

                        var checkShipStartDateFormatted = (model.ShipStartDate == null ? DateTime.Now.AddDays(-1).ToShortDateString() : model.ShipStartDate.Value.ToShortDateString());

                        if (prevReOccurence == 0 && checkOrder.ReOccurence == 1)
                        {
                            OrderUtility _orderUtility = new OrderUtility(_dbContext);
                            var orderId = checkOrder.OrderId;
                            int reoccurrenceOrderCount = 1;
                            if (model.ReOccurenceCycle == "Days")
                            {
                                var count = 0;
                                for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                {
                                    if (checkShipStartDateFormatted != date.ToShortDateString())
                                    {
                                        count++;
                                        if (count == model.ReOccurenceFrequency)
                                        {
                                            if (reoccurrenceOrderCount < 12)
                                            {
                                                count = 0;
                                                _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                reoccurrenceOrderCount++;
                                            }

                                        }
                                    }

                                }
                            }

                            if (model.ReOccurenceCycle == "Weeks")
                            {
                                var count = 0;
                                for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                {
                                    if (checkShipStartDateFormatted != date.ToShortDateString())
                                    {


                                        count++;
                                        if (count == ((model.ReOccurenceFrequency * 7)))
                                        {
                                            if (reoccurrenceOrderCount < 12)
                                            {
                                                count = 0;
                                                _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                reoccurrenceOrderCount++;
                                            }

                                        }
                                    }
                                }
                            }

                            if (model.ReOccurenceCycle == "Months")
                            {
                                var count = 0;
                                for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                {
                                    if (checkShipStartDateFormatted != date.ToShortDateString())
                                    {


                                        count++;
                                        if (count == ((model.ReOccurenceFrequency * 30)))
                                        {
                                            if (reoccurrenceOrderCount < 12)
                                            {
                                                count = 0;
                                                _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                reoccurrenceOrderCount++;
                                            }

                                        }
                                    }
                                }
                            }


                            if (model.ReOccurenceCycle == "WeekDay")
                            {

                                for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                {
                                    if (checkShipStartDateFormatted != date.ToShortDateString())
                                    {

                                        if (model.ReOccurenceFrequency == 1)
                                        {
                                            if (date.Day >= 1 && date.Day <= 7)
                                            {
                                                if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                                {
                                                    if (model.ReOccurenceStartDateSetBy != "System")
                                                    {
                                                        if (reoccurrenceOrderCount < 12)
                                                        {
                                                            _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                            reoccurrenceOrderCount++;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                        {
                                                            if (reoccurrenceOrderCount < 12)
                                                            {
                                                                _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                                reoccurrenceOrderCount++;
                                                            }

                                                        }

                                                    }
                                                }


                                            }
                                        }
                                        else if (model.ReOccurenceFrequency == 0)
                                        {
                                            if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                            {
                                                if (model.ReOccurenceStartDateSetBy != "System")
                                                {
                                                    if (reoccurrenceOrderCount < 12)
                                                    {
                                                        _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                        reoccurrenceOrderCount++;
                                                    }

                                                }
                                                else
                                                {
                                                    if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                    {
                                                        if (reoccurrenceOrderCount < 12)
                                                        {
                                                            _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                            reoccurrenceOrderCount++;
                                                        }

                                                    }

                                                }
                                            }
                                        }
                                        else if (model.ReOccurenceFrequency > 1)
                                        {
                                            if (date.Day > ((model.ReOccurenceFrequency - 1) * 7) && date.Day <= ((model.ReOccurenceFrequency) * 7))
                                            {
                                                if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                                {
                                                    if (model.ReOccurenceStartDateSetBy != "System")
                                                    {
                                                        if (reoccurrenceOrderCount < 12)
                                                        {
                                                            _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                            reoccurrenceOrderCount++;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                        {
                                                            if (reoccurrenceOrderCount < 12)
                                                            {
                                                                _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                                reoccurrenceOrderCount++;
                                                            }

                                                        }

                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                            }

                            if (model.ReOccurenceCycle == "Month's Day")
                            {

                                for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                {
                                    if (checkShipStartDateFormatted != date.ToShortDateString())
                                    {
                                        if (date.Day == model.ReOccurenceFrequency)
                                        {
                                            if (reoccurrenceOrderCount < 12)
                                            {
                                                _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                reoccurrenceOrderCount++;
                                            }

                                        }
                                    }
                                }
                            }

                        }

                        if (prevReOccurence == 1 && checkOrder.ReOccurence == 0)
                        {

                            var checkReOccurenceOrders = _dbContext.tbl_OrderMaster.Where(w => w.ReOccurenceParentOrderId == checkOrder.OrderId).ToList();
                            foreach (var item in checkReOccurenceOrders)
                            {
                                item.IsActive = 0;
                                item.ModifiedBy = _userId;
                                item.ModifiedDate = DateTime.Now;
                                _dbContext.SaveChanges();

                            }

                        }

                        if (prevReOccurence == 1 && checkOrder.ReOccurence == 1)
                        {
                            var orderId = checkOrder.OrderId;
                            if (!((prevReOccurence == checkOrder.ReOccurence) &&
                                (prevReOccurenceCycle == checkOrder.ReOccurenceCycle) &&
                                (prevReOccurenceStartDate == checkOrder.ReOccurenceStartDate) &&
                                (prevReOccurenceEndDate == checkOrder.ReOccurenceEndDate) &&
                                (prevReOccurenceFrequency == checkOrder.ReOccurenceFrequency)
                                ))
                            {
                                if (model.ReOccurenceOrderCount != -1)
                                {

                                    var checkReOccurenceOrders = _dbContext.tbl_OrderMaster.Where(w => w.ReOccurenceParentOrderId == checkOrder.OrderId).ToList();
                                    foreach (var item in checkReOccurenceOrders)
                                    {
                                        item.IsActive = 0;
                                        item.ModifiedBy = _userId;
                                        item.ModifiedDate = DateTime.Now;
                                        _dbContext.SaveChanges();

                                    }
                                    int reoccurrenceOrderCount = 1;
                                    OrderUtility _orderUtility = new OrderUtility(_dbContext);

                                    if (model.ReOccurenceCycle == "Days")
                                    {
                                        var count = 0;
                                        for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                        {
                                            if (checkShipStartDateFormatted != date.ToShortDateString())
                                            {
                                                count++;
                                                if (count == model.ReOccurenceFrequency)
                                                {
                                                    if (reoccurrenceOrderCount < 12)
                                                    {
                                                        count = 0;
                                                        _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                        reoccurrenceOrderCount++;
                                                    }

                                                }
                                            }

                                        }
                                    }

                                    if (model.ReOccurenceCycle == "Weeks")
                                    {
                                        var count = 0;
                                        for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                        {
                                            if (checkShipStartDateFormatted != date.ToShortDateString())
                                            {


                                                count++;
                                                if (count == ((model.ReOccurenceFrequency * 7)))
                                                {
                                                    if (reoccurrenceOrderCount < 12)
                                                    {
                                                        count = 0;
                                                        _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                        reoccurrenceOrderCount++;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (model.ReOccurenceCycle == "Months")
                                    {
                                        var count = 0;
                                        for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                        {
                                            if (checkShipStartDateFormatted != date.ToShortDateString())
                                            {


                                                count++;
                                                if (count == ((model.ReOccurenceFrequency * 30)))
                                                {
                                                    if (reoccurrenceOrderCount < 12)
                                                    {
                                                        count = 0;
                                                        _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                        reoccurrenceOrderCount++;
                                                    }
                                                }
                                            }
                                        }
                                    }


                                    if (model.ReOccurenceCycle == "WeekDay")
                                    {

                                        for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                        {
                                            if (checkShipStartDateFormatted != date.ToShortDateString())
                                            {

                                                if (model.ReOccurenceFrequency == 1)
                                                {
                                                    if (date.Day >= 1 && date.Day <= 7)
                                                    {
                                                        if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                                        {
                                                            if (model.ReOccurenceStartDateSetBy != "System")
                                                            {
                                                                if (reoccurrenceOrderCount < 12)
                                                                {
                                                                    _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                                    reoccurrenceOrderCount++;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                                {
                                                                    if (reoccurrenceOrderCount < 12)
                                                                    {
                                                                        _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                                        reoccurrenceOrderCount++;
                                                                    }
                                                                }

                                                            }

                                                        }
                                                    }
                                                }
                                                else if (model.ReOccurenceFrequency == 0)
                                                {
                                                    if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                                    {
                                                        if (model.ReOccurenceStartDateSetBy != "System")
                                                        {
                                                            if (reoccurrenceOrderCount < 12)
                                                            {
                                                                _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                                reoccurrenceOrderCount++;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                            {
                                                                if (reoccurrenceOrderCount < 12)
                                                                {
                                                                    _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                                    reoccurrenceOrderCount++;
                                                                }
                                                            }

                                                        }
                                                    }
                                                }
                                                else if (model.ReOccurenceFrequency > 1)
                                                {
                                                    if (date.Day > ((model.ReOccurenceFrequency - 1) * 7) && date.Day <= ((model.ReOccurenceFrequency) * 7))
                                                    {
                                                        if (date.DayOfWeek.ToString() == model.ReOccurenceWeekday)
                                                        {
                                                            if (model.ReOccurenceStartDateSetBy != "System")
                                                            {
                                                                if (reoccurrenceOrderCount < 12)
                                                                {
                                                                    _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                                    reoccurrenceOrderCount++;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (!(date.Month == model.ShipStartDate.Value.Month && date.Year == model.ShipStartDate.Value.Year))
                                                                {
                                                                    if (reoccurrenceOrderCount < 12)
                                                                    {
                                                                        _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                                        reoccurrenceOrderCount++;
                                                                    }
                                                                }

                                                            }

                                                        }
                                                    }
                                                }
                                            }

                                        }
                                    }

                                    if (model.ReOccurenceCycle == "Month's Day")
                                    {

                                        for (DateTime date = model.ReOccurenceStartDate.Value; date.Date <= model.ReOccurenceEndDate.Value; date = date.AddDays(1))
                                        {
                                            if (checkShipStartDateFormatted != date.ToShortDateString())
                                            {
                                                if (date.Day == model.ReOccurenceFrequency)
                                                {
                                                    if (reoccurrenceOrderCount < 12)
                                                    {
                                                        _orderUtility.SaveReOccurenceOrder(orderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, date);
                                                        reoccurrenceOrderCount++;
                                                    }
                                                }
                                            }
                                        }
                                    }


                                }

                            }
                            else
                            {
                                if (model.ReOccurenceOrderCount != -1)
                                {
                                    var checkReOccurenceOrders = _dbContext.tbl_OrderMaster.Where(w => w.ReOccurenceParentOrderId == checkOrder.OrderId).ToList();
                                    foreach (var item in checkReOccurenceOrders)
                                    {
                                        _orderUtility.updateReOccurenceOrder(item.OrderId, checkOrder, orderDetailReoccurence, orderAssignmentReoccurence, item.OrderDate);

                                    }
                                }
                            }
                        }

                        if (model.ReOccurenceOrderCount != -1)
                        {
                            var checkReoccurenceOrders = _dbContext.tbl_OrderMaster.Where(w => w.ReOccurenceParentOrderId == checkOrder.OrderId).ToList();
                            foreach (var item in checkReoccurenceOrders)
                            {
                                item.OrderDate = checkOrder.OrderDate;
                                item.ShipId = checkOrder.ShipId;
                                item.CustomerId = checkOrder.CustomerId;
                                item.TotalAmount = checkOrder.TotalAmount;
                                item.ModifiedBy = 1;
                                item.ModifiedDate = DateTime.Now;
                                item.ApartmentIds = checkOrder.ApartmentIds;
                                item.ParentOrderId = checkOrder.ParentOrderId;

                                _dbContext.SaveChanges();

                                if (model.OrderItemList.Count() > 0)
                                {

                                    var checkOrderItems = _dbContext.tbl_OrderDetail.Where(w => w.OrderId == item.OrderId).ToList();
                                    foreach (var item1 in checkOrderItems)
                                    {
                                        item1.IsActive = 0;
                                        item1.ModifiedDate = DateTime.Now;
                                        _dbContext.SaveChanges();
                                    }

                                    foreach (var item1 in model.OrderItemList)
                                    {
                                        OrderDetail orderDetail = new OrderDetail()
                                        {
                                            ItemId = item1.ItemId,
                                            Description = item1.Description,
                                            OrderId = item.OrderId,
                                            PerUnitPrice = item1.PerUnitPrice,
                                            Quantity = item1.Quantity,
                                            TotalPrice = item1.TotalPrice,
                                            UnitId = item1.UnitId,
                                            IsActive = 1,
                                            CreatedBy = 1,
                                            CreatedDate = DateTime.Now
                                        };
                                        _dbContext.tbl_OrderDetail.Add(orderDetail);
                                        _dbContext.SaveChanges();

                                    }

                                    if (orderAssignmentReoccurence != null)
                                    {
                                        var checkOrderAssignmentDetail = _dbContext.tbl_OrderAssignment.Where(w => w.OrderId == item.OrderId).FirstOrDefault();
                                        if (checkOrderAssignmentDetail != null)
                                        {
                                            if (Convert.ToString(model.AssigneeId) != "")
                                            {
                                                model.EmployeeId = Convert.ToInt32(model.AssigneeId);
                                                checkOrderAssignmentDetail.EmployeeId = model.EmployeeId;
                                                _dbContext.SaveChanges();

                                            }
                                            else
                                            {
                                                checkOrderAssignmentDetail.EmployeeId = 0;
                                                _dbContext.SaveChanges();

                                            }
                                        }
                                        else
                                        {
                                            if (Convert.ToString(model.AssigneeId) != "")
                                            {
                                                OrderAssignment orderAssignment = new OrderAssignment()
                                                {
                                                    OrderId = item.OrderId,
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
                        }
                    }

                    ViewBag.SuccessMessage = "Order update successfully";

                }
            }
            catch (Exception ex)
            {
                var a = "";
            }
            model.CustomerId = _customerd;
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

        [HttpPost]
        public JsonResult CheckNonworkingDay(DateTime date)
        {
            ResponseModel response = new ResponseModel();
            try
            {


                var checkCount = 0;
                var daysList = _dbContext.tbl_CalenderWorkingDays.ToList();
                checkCount = daysList.Where(w => w.HolidayDate != null).Where(w => w.HolidayDate.Value.Day == date.Day && w.HolidayDate.Value.Month == date.Month).Count();
                if (checkCount > 0)
                {
                    response.Status = "0";
                    return Json(response);
                }
                checkCount = daysList.Where(w => w.DayName == date.DayOfWeek.ToString()).Count();
                if (checkCount > 0)
                {
                    response.Status = "0";
                    return Json(response);
                }

                response.Status = "1";
                return Json(response);

            }
            catch (Exception ex)
            {
                response.Status = "0";
                response.Message = "Error occurred";
            }
            response.Status = "1";
            return Json(response);

        }

        [HttpPost]
        public JsonResult AddShipping(int CustomerId, string FirstName, string MiddleName, string LastName, string Email, string Phone, int StateId, int CityId, string Address)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (Convert.ToString(FirstName ?? "") != "" ||
                            Convert.ToString(MiddleName ?? "") != "" ||
                            Convert.ToString(LastName ?? "") != "" ||
                            Convert.ToString(Email ?? "") != "" ||
                            Convert.ToString(Phone ?? "") != "" ||
                            Convert.ToString(StateId) != "0" ||
                            Convert.ToString(CityId) != "0" ||
                            Convert.ToString(Address ?? "") != "")
                {
                    CustomerShipping custmoerShipping = new CustomerShipping()
                    {
                        CustomerId = CustomerId,
                        FirstName = FirstName,
                        MiddleName = MiddleName,
                        LastName = LastName,
                        Email = Email,
                        Phone = Phone,
                        CityId = CityId,
                        StateId = StateId,
                        Address = Address,
                        IsActive = 1,
                        CreatedBy = 1,
                        CreatedDate = DateTime.Now
                    };
                    _dbContext.tbl_CustmoerShipping.Add(custmoerShipping);
                    _dbContext.SaveChanges();

                    response.Status = custmoerShipping.CustomerShipId.ToString();
                }


                response.Message = "Detail updated successfully";
            }
            catch (Exception ex)
            {
            }

            return Json(response);
        }


        [HttpPost]
        public JsonResult AddApartment(int ShipId, string ApartmentNo, string ApartmentName, string Notes, string ApartmentId)
        {
            ResponseModel response = new ResponseModel();
            var CustomerShipingApartmentList = new List<CustomerShippingApartmentViewModel>();
            try
            {
                if (Convert.ToString(ApartmentNo ?? "") != "" ||
                            Convert.ToString(ApartmentName ?? "") != "" ||
                            Convert.ToString(Notes ?? "") != "")
                {
                    CustomerShippingApartment customerShippingApartment = new CustomerShippingApartment()
                    {
                        ApartmentNo = ApartmentNo,
                        ApartmentName = ApartmentName,
                        Notes = Notes,
                        IsActive = 1,
                        CreatedBy = 1,
                        CreatedDate = DateTime.Now,
                        CustomerShipId = ShipId
                    };
                    _dbContext.tbl_CustomerShippingApartments.Add(customerShippingApartment);
                    _dbContext.SaveChanges();

                    var ApartmentIds = (ApartmentId ?? "").Split(',').ToList();



                   
                    ApartmentIds.Add(customerShippingApartment.ApartmentId.ToString());

                    var apartmentCsv = String.Join(",", ApartmentIds);


                    CustomerShipingApartmentList = (from apartment in _dbContext.tbl_CustomerShippingApartments
                                                    where apartment.CustomerShipId == ShipId
                                                    select new CustomerShippingApartmentViewModel
                                                    {
                                                        ApartmentId = apartment.ApartmentId,
                                                        ApartmentNo = apartment.ApartmentNo,
                                                        ApartmentName = apartmentCsv
                                                    })
                                                    .ToList();


                }


                response.Message = "Detail added successfully";
            }
            catch (Exception ex)
            {
            }

            return Json(CustomerShipingApartmentList);
        }


        public IActionResult GetOrderDetail(int id)
        {
            var model = new OrderDetailViewModel();
            ViewBag.index = id;

            model.ItemList = _dbContext.tbl_ItemMaster
                    .Where(w => w.IsActive == 1)
                            .Select(s => new ItemMasterViewModel
                            {
                                ItemId = s.ItemId,
                                ItemCd = s.ItemCd,
                                ItemPrice = s.ItemPrice,
                                ItemDescription = s.ItemDescription
                            })
                            .ToList();

            return PartialView("_ItemDetail", model);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}