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

                model.OrderNo = _dbContext.tbl_OrderMaster.Max(m => m.OrderId) + 1;

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
                        shipDate = _commanUtility.RoundUp(model.ShipStartDate.Value, TimeSpan.FromMinutes(15));
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
                        IsActive = 1,
                        CreatedBy = 1,
                        CreatedDate = DateTime.Now,
                        ApartmentIds = model.ApartmentIds
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
                        }

                    }




                    ViewBag.SuccessMessage = "Order added successfully";

                }
            }
            catch (Exception ex)
            {
                var a = "";
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
                        CityId = model.CustomerDetail.CityId == null ? 0 : model.CustomerDetail.CityId.Value,
                        StateId = model.CustomerDetail.StateId == null ? 0 : model.CustomerDetail.StateId.Value,
                        Address = model.CustomerDetail.Address,
                        CompanyType = model.CustomerDetail.CompanyType,
                        Zip1 = model.CustomerDetail.Zip1,
                        Zip2 = model.CustomerDetail.Zip2,
                        Code = model.CustomerDetail.Code,
                        IsActive = 1,
                        CreatedBy = 1,
                        CreatedDate = DateTime.Now
                    };

                    _dbContext.tbl_CustomerMaster.Add(customerMaster);
                    _dbContext.SaveChanges();

                    model.CustomerId = customerMaster.CustmoerId;

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
                            CustmoerShipping custmoerShipping = new CustmoerShipping()
                            {
                                CustmoerId = customerMaster.CustmoerId,
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
                            CustmoerContact customerContactDetail = new CustmoerContact()
                            {
                                FirstName = item.FirstName,
                                MiddleName = item.MiddleName,
                                LastName = item.LastName,
                                Email = item.Email,
                                Phone = item.Phone,
                                CustmoerId = customerMaster.CustmoerId,
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
                model.OrderList = (from order in _dbContext.tbl_OrderMaster
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
                                       OrderNo = order.OrderNo,
                                       OrderDate = order.OrderDate,
                                       ShipStartDate = order.ShipStartDate,
                                       CustomerName = customer.CompanyName,
                                       EmployeeName = employee1.FirstName + " " + (employee1.MiddleName ?? "") + " " + employee1.LastName,
                                       TotalAmount = order.TotalAmount,
                                       IsActive = order.IsActive

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
        public IActionResult List(OrderListViewModel model)
        {
            model.OrderList = new List<OrderMasterViewModel>();



            try
            {
                model.OrderList = (from order in _dbContext.tbl_OrderMaster
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
                                       OrderNo = order.OrderNo,
                                       OrderDate = order.OrderDate,
                                       ShipStartDate = order.ShipStartDate,
                                       CustomerName = customer.CompanyName,
                                       EmployeeName = employee1.FirstName + " " + (employee1.MiddleName ?? "") + " " + employee1.LastName,
                                       TotalAmount = order.TotalAmount,
                                       IsActive = order.IsActive

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


            }
            catch (Exception ex)
            {

                var a = "";
            }


            return View(model);

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
                        checkOrder.ShipDate = start;
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

        [HttpPost]
        public JsonResult GetCustomerShippingApartment(int ShipId)
        {
            var CustomerShipingApartmentList = new List<CustomerShippingApartmentViewModel>(); ;
            try
            {
                CustomerShipingApartmentList = (from apartment in _dbContext.tbl_CustomerShippingApartments
                                                where apartment.ShipId == ShipId
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

            return Json(CustomerShipingApartmentList);
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
                                ItemPrice = s.ItemPrice,
                                ItemDescription = s.ItemDescription
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
               // if (ModelState.IsValid)
                {
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

                        checkOrder.OrderDate = model.OrderDate;
                        checkOrder.ShipStartDate = model.ShipStartDate;
                        checkOrder.ShipEndDate = model.ShipEndDate;
                        checkOrder.ShipId = model.ShipId;
                        checkOrder.CustomerId = model.CustomerId;
                        checkOrder.TotalAmount = model.TotalAmount;
                        checkOrder.ModifiedBy = 1;
                        checkOrder.ModifiedDate = DateTime.Now;
                        checkOrder.OrderNo = model.OrderNo;
                        checkOrder.ApartmentIds = model.ApartmentIds;
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
                    CustmoerShipping custmoerShipping = new CustmoerShipping()
                    {
                        CustmoerId = CustomerId,
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

                    response.Status = custmoerShipping.ShipId.ToString();
                }


                response.Message = "Detail updated successfully";
            }
            catch (Exception ex)
            {
            }

            return Json(response);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}