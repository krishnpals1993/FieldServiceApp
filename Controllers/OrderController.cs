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
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using Intuit.Ipp.Data;

namespace LaCafelogy.Controllers
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
            OrderViewModel model = new OrderViewModel();

            var orderNo = 0;

            try
            {
                orderNo = _dbContext.tbl_Order.Max(m => m.OrderId);
                orderNo = orderNo + 1;
            }
            catch (Exception ex)
            {

                orderNo = 1;
            }

            model.OrderNo = orderNo.ToString();

            model.ItemList = new List<ItemMasterViewModel>();

            var comboMaster = _dbContext.tbl_ComboOfferMasters
                            .Select(s => new ComboOfferMasterViewModel
                            {
                                ComboOfferName = s.ComboOfferName,
                                ComboOfferMasterId = s.ComboOfferMasterId,
                                Price = s.Price,
                                Type = s.Type
                            }).ToList();

            foreach (var combo in comboMaster)
            {
                var itemDetail = (from offerItem in _dbContext.tbl_ComboOfferDetails
                                  join item in _dbContext.tbl_ItemMaster on offerItem.ItemId equals item.ItemId
                                  where offerItem.ComboOfferMasterId == combo.ComboOfferMasterId
                                  select new ComboOfferDetailViewModel
                                  {
                                      ItemName = item.ItemCd,
                                      Price = item.ItemPrice,
                                      Quantity = offerItem.Quantity

                                  }).ToList();
                combo.Description = String.Join(",", itemDetail.Select(s => s.ItemName + " " + s.Quantity + "").ToList());
                model.ItemList.Add(new ItemMasterViewModel
                {
                    ItemName = "Rs." + combo.Price.ToString() + " " + combo.ComboOfferName + "(" + combo.Description + ")",
                    ItemId = combo.ComboOfferMasterId,
                    IsCombo = "Y",
                    ItemPrice = combo.Price
                });
            }

            var items = _dbContext.tbl_ItemMaster.Select(s => new ItemMasterViewModel
            {
                ItemId = s.ItemId,
                ItemName = "Rs." + s.ItemPrice + "-" + s.ItemCd,
                ItemPrice = s.ItemPrice,
                IsCombo = "N"
            }).ToList();

            foreach (var item in items)
            {
                model.ItemList.Add(item);
            }



            model.ContactList = _dbContext.tbl_Order.Select(s => new OrderCustomerDetailViewModel
            {
                ContactNo = s.ContactNo,
                Name = s.Name
            }).Distinct().ToList();
            return View(model);
        }

        public IActionResult OrderList()
        {

            var orderList = _dbContext.tbl_Order.Select(s => new OrderViewModel
            {
                OrderId = s.OrderId,
                OrderNo = s.OrderNo,
                Name = s.Name,
                ContactNo = s.ContactNo,
                TotalAmount = s.TotalAmount,
                Remarks = s.Remarks
            }).ToList();



            var orderItemList = (from orderItem in _dbContext.tbl_OrderItem
                                 select new OrderItemsViewModel
                                 {
                                     OrderId = orderItem.OrderId,
                                     ItemId = orderItem.ItemId,
                                     Amount = orderItem.Amount,
                                     Quantity = orderItem.Quantity,
                                     ComboOfferId = orderItem.ComboOfferId

                                 }).ToList();


            var comboMaster = _dbContext.tbl_ComboOfferMasters.Where(s =>
                            (orderItemList.Select(s => s.ComboOfferId).ToList().Contains(s.ComboOfferMasterId)))
                            .Select(s => new ComboOfferMasterViewModel
                            {
                                ComboOfferName = s.ComboOfferName,
                                ComboOfferMasterId = s.ComboOfferMasterId,
                                Price = s.ComboOfferMasterId,
                                Type = s.Type
                            }).ToList();

            foreach (var combo in comboMaster)
            {
                var itemDetail = (from offerItem in _dbContext.tbl_ComboOfferDetails
                                  join item in _dbContext.tbl_ItemMaster on offerItem.ItemId equals item.ItemId
                                  where offerItem.ComboOfferMasterId == combo.ComboOfferMasterId
                                  select new ComboOfferDetailViewModel
                                  {
                                      ItemName = item.ItemCd,
                                      Price = item.ItemPrice,
                                      Quantity = offerItem.Quantity

                                  }).ToList();
                combo.Description = String.Join(",", itemDetail.Select(s => s.ItemName + " " + s.Quantity + "").ToList());

            }

            var items = _dbContext.tbl_ItemMaster.Select(s => new ItemMasterViewModel
            {
                ItemId = s.ItemId,
                ItemName = "Rs." + s.ItemPrice + "-" + s.ItemCd,
                ItemPrice = s.ItemPrice,
                IsCombo = "N"
            }).ToList();

            foreach (var order in orderList)
            {
                var orderItemOnly = (from orderItem in orderItemList
                                     join item in items
                                     on orderItem.ItemId equals item.ItemId
                                     where orderItem.OrderId == order.OrderId
                                     select new OrderItemsViewModel
                                     {
                                         ItemName = item.ItemName,
                                         Amount = orderItem.Amount,
                                         Quantity = orderItem.Quantity

                                     }).ToList();

                order.OrderItems = orderItemOnly;

                var comboOffer = (from orderItem in orderItemList
                                  join combo in comboMaster
                                  on orderItem.ComboOfferId equals combo.ComboOfferMasterId
                                  where orderItem.OrderId == order.OrderId
                                  select new OrderItemsViewModel
                                  {
                                      ItemName = combo.ComboOfferName,
                                      Description = combo.Description,
                                      IsCombo = "Y",
                                      Amount = orderItem.Amount,
                                      Quantity = orderItem.Quantity

                                  }).ToList();

                order.OrderItems = orderItemOnly.Union(comboOffer).ToList();

            }






            return View(orderList);
        }

        [HttpPost]
        public JsonResult AddOrder(string model)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                OrderViewModel orderViewModel = JsonConvert.DeserializeObject<OrderViewModel>(model);

                Order order = new Order()
                {
                    OrderNo = orderViewModel.OrderNo,
                    TotalAmount = orderViewModel.TotalAmount,
                    ContactNo = orderViewModel.ContactNo,
                    Name = orderViewModel.Name,
                    Remarks = orderViewModel.Remarks,
                    IsActive = 1,
                    CreatedBy = _userId,
                    CreatedDate = DateTime.Now,
                };

                _dbContext.tbl_Order.Add(order);
                _dbContext.SaveChanges();

                foreach (var item in orderViewModel.OrderItems)
                {

                    OrderItem orderItem = new OrderItem()
                    {
                        OrderId = order.OrderId,
                        ItemId = item.IsCombo == "Y" ? 0 : item.ItemId,
                        Quantity = item.Quantity,
                        Amount = item.Amount,
                        ComboOfferId = item.IsCombo == "Y" ? item.ItemId : 0,
                        IsActive = 1,
                        CreatedBy = _userId,
                        CreatedDate = DateTime.Now,
                    };

                    _dbContext.tbl_OrderItem.Add(orderItem);
                    _dbContext.SaveChanges();

                }

                response.Status = "1";
                response.Message = "Order #" + order.OrderNo + " generated successfully";
            }
            catch (Exception ex)
            {
                response.Status = "0";
                response.Message = "Error occurred";
            }

            return Json(response);
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

        public IActionResult GetOrderNotePopup(int id)
        {
            try
            {
                DashboardUtility _dashboardUtility = new DashboardUtility(_dbContext);
                var model = _dashboardUtility.getOrderDetailWithApartemt(id);
                OrderNoteViewModel orderNote = new OrderNoteViewModel();
                orderNote.OrderId = model.OrderId;
                orderNote.CustomerName = model.CustomerName;
                orderNote.Apartments = model.Apartments;
                orderNote.CustomerShipAddress = model.CustomerShipAddress;
                orderNote.EmployeeList = model.EmployeeList;
                orderNote.ItemName = model.ItemName;
                orderNote.ItemDescription = model.ItemDescription;
                orderNote.OrderId = model.OrderId;

                orderNote.OrderNoteList = (from note in _dbContext.tbl_OrderNotes
                                           join user in _dbContext.tbl_Users on note.CreatedBy equals user.UserId
                                           where note.OrderId == orderNote.OrderId
                                           select new OrderNoteDetailViewModel
                                           {

                                               Note = note.Note,
                                               CreatedBy = user.UserName,
                                               CreatedDate = note.CreatedDate

                                           }).ToList();

                return PartialView("_OrderNotes", orderNote);
            }
            catch (Exception ex)
            {

                return PartialView("_OrderNotes", new OrderNoteViewModel());
            }


        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }

}