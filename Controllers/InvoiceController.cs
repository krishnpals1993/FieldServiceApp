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
    public class InvoiceController : Controller
    {

        private readonly IOptions<Appsettings> _appSettings;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly DBContext _dbContext;

        public InvoiceController(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings, DBContext dbContext)
        {
            _appSettings = appSettings;
            _emailSettings = emailSettings;
            _dbContext = dbContext;
        }


        public IActionResult List()
        {

            List<BilHeaderViewModel> billHeaderList = new List<BilHeaderViewModel>();

            try
            {
                billHeaderList = (from billHeader in _dbContext.tbl_BilHeaders
                                  join order in _dbContext.tbl_OrderMaster on billHeader.OrderId equals order.OrderId
                                  join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustomerId
                                  select new BilHeaderViewModel
                                  {
                                      BillId = billHeader.BillId,
                                      BillNo = billHeader.BillNo,
                                      BillInvoiceDate = billHeader.BillInvoiceDate,
                                      OrderId = order.OrderId,
                                      OrderNo = order.OrderNo,
                                      OrderDate = order.OrderDate,
                                      CustomerName = customer.CompanyName,
                                      TotalAmount = billHeader.TotalAmount,
                                      IsActive = billHeader.IsActive,
                                  })
                                  .ToList();


            }
            catch (Exception ex)
            {

                var a = "";
            }


            return View(billHeaderList);

        }



        public IActionResult Add()
        {
            BilHeaderViewModel model = new BilHeaderViewModel();
            try
            {

            }
            catch (Exception ex)
            {

                var a = "";
            }


            return View(model);
        }

        [HttpPost]
        public IActionResult Add(BilHeaderViewModel model)
        {
            try
            {
                var checkOrder = _dbContext.tbl_OrderMaster.Where(w => w.OrderNo == model.OrderNo).FirstOrDefault();
                if (checkOrder != null)
                {

                    var checkInvoice = _dbContext.tbl_BilHeaders.Where(w => w.OrderId == checkOrder.OrderId).FirstOrDefault();
                    if (checkInvoice != null)
                    {
                        ViewBag.ErrorMessage = "Invoice already created for order no " + model.OrderNo;
                    }
                    else
                    {
                        return RedirectToAction("Create", "Invoice", new { id = model.OrderNo });
                    }


                }
                else
                {
                    ViewBag.ErrorMessage = "Order not found for order no " + model.OrderNo;
                }

            }
            catch (Exception ex)
            {

                var a = "";
            }
            return View(model);
        }

        public IActionResult Create(int id)
        {
            BilHeaderViewModel model = new BilHeaderViewModel();
            try
            {

                model.OrderNo = id;
                var checkOrder = _dbContext.tbl_OrderMaster.Where(w => w.OrderNo == id).FirstOrDefault();
                if (checkOrder != null)
                {
                    model = (from customer in _dbContext.tbl_CustomerMaster
                             //join state in _dbContext.tbl_States on customer.StateId equals state.StateId
                             //into state
                             //from state1 in state.DefaultIfEmpty()
                             //join city in _dbContext.tbl_Cities on customer.CityId equals city.CityId
                             //into city
                             //from city1 in city.DefaultIfEmpty()
                             where customer.CustomerId == checkOrder.CustomerId
                             select new BilHeaderViewModel
                             {
                                 //BilligFirstName = customer.FirstName,
                                 //BilligLastName = customer.LastName,
                                 BilligCompanyName = customer.CompanyName,
                                 BilligCompanyCode = customer.CompanyCode,
                                 //BilligAddress = customer.Address,
                                 //BilligAddress2 = customer.Address2,
                                 //BilligAddress3 = customer.Address3,
                                 //BilligCityName = city1.CityName,
                                 //BilligStateName = state1.StateName,
                                 //BilligZip1 = customer.Zip1,
                                 //BilligZip2 = customer.Zip2

                             }).FirstOrDefault();


                    model.OrderId = checkOrder.OrderId;
                    model.OrderNo = checkOrder.OrderNo;
                    model.ShipStartDate = checkOrder.ShipStartDate;
                    model.OrderDate = checkOrder.OrderDate;
                    model.TotalAmount = checkOrder.TotalAmount;

                    var checkOrderDetail = _dbContext.tbl_OrderDetail.Where(w => w.OrderId == model.OrderId).FirstOrDefault();
                    if (checkOrderDetail != null)
                    {
                        model.ItemId = checkOrderDetail.ItemId;
                        model.PerUnitPrice = checkOrderDetail.PerUnitPrice;
                        model.UnitId = checkOrderDetail.UnitId;
                        model.Quantity = checkOrderDetail.Quantity;
                        model.Description = checkOrderDetail.Description;
                    }

                    if (checkOrder.ShipId > 0)
                    {

                        model.Shipping = (from ship in _dbContext.tbl_CustmoerShipping
                                          join state in _dbContext.tbl_States on ship.StateId equals state.StateId
                                          into state
                                          from state1 in state.DefaultIfEmpty()
                                          join city in _dbContext.tbl_Cities on ship.CityId equals city.CityId
                                          into city
                                          from city1 in city.DefaultIfEmpty()
                                          where ship.CustomerShipId == checkOrder.ShipId
                                          select new CustmoerShippingViewModel
                                          {
                                              FirstName = ship.FirstName,
                                              LastName = ship.LastName,
                                              Address = ship.Address,
                                              CityName = city1.CityName,
                                              StateName = state1.StateName,
                                              Zip1 = ship.Zip1,
                                              Zip2 = ship.Zip2,

                                          }).FirstOrDefault();



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


                }
                else
                {
                    ViewBag.ErrorMessage = "Order not found for order no " + id;
                }

            }
            catch (Exception ex)
            {

                var a = "";
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(BilHeaderViewModel model)
        {
            try
            {

                {
                    var checkOrderNo = _dbContext.tbl_BilHeaders.Where(w => w.OrderId == model.OrderId).FirstOrDefault();
                    if (checkOrderNo != null)
                    {
                        ViewBag.ErrorMessage = "Invoice has been already created";
                        return View(model);


                    }

                    var orderDetail = _dbContext.tbl_OrderMaster.Where(w => w.OrderId == model.OrderId).FirstOrDefault();
                    var maxBillNo = _dbContext.tbl_BilHeaders.Count();

                    BilHeader bilHeader = new BilHeader()
                    {
                        BillNo = (maxBillNo + 1),
                        BillInvoiceDate = DateTime.Now,
                        OrderId = orderDetail.OrderId,
                        ShipId = model.ShipId,
                        CustomerId = orderDetail.CustomerId,
                        TotalAmount = model.TotalAmount,
                        IsActive = 1,
                        CreatedBy = 1,
                        CreatedDate = DateTime.Now,

                    };

                    _dbContext.tbl_BilHeaders.Add(bilHeader);
                    _dbContext.SaveChanges();

                    OrderDetail orderDetailReoccurence = new OrderDetail();
                    OrderAssignment orderAssignmentReoccurence = new OrderAssignment();



                    BilDetail bilDetail = new BilDetail()
                    {
                        Bill2No = bilHeader.BillId,
                        Bill2Seq = 1,
                        Bill2ItemId = model.ItemId,
                        Bill2Description = model.Description,
                        Bill2Price = model.PerUnitPrice,
                        IsActive = 1,
                        CreatedBy = 1,
                        CreatedDate = DateTime.Now
                    };
                    _dbContext.tbl_BilDetail.Add(bilDetail);
                    _dbContext.SaveChanges();

                    ViewBag.SuccessMessage = "Invoice created successfully";

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
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}