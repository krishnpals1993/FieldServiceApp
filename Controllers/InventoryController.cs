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
using Microsoft.Owin.Security.Provider;
using DocumentFormat.OpenXml.EMMA;

namespace LaCafelogy.Controllers
{
    [Authentication]
    public class InventoryController : Controller
    {
        private readonly IOptions<Appsettings> _appSettings;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly DBContext _dbContext;

        public InventoryController(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings, DBContext dbContext)
        {
            _appSettings = appSettings;
            _emailSettings = emailSettings;
            _dbContext = dbContext;
        }
        public IActionResult ItemList()
        {
            List<ItemMasterViewModel> items = (from item in _dbContext.tbl_ItemMaster
                                               join unit in _dbContext.tbl_Units on item.ItemUnitId equals unit.UnitId
                                               into unit
                                               from unit1 in unit.DefaultIfEmpty()
                                               join category in _dbContext.tbl_ItemCategory on item.CategoryId equals category.CategoryId
                                               into category
                                               from category1 in category.DefaultIfEmpty()
                                               join Group in _dbContext.tbl_ItemGroup on item.GroupId equals Group.GroupId
                                               into Group
                                               from Group1 in Group.DefaultIfEmpty()
                                               select new ItemMasterViewModel
                                               {
                                                   ItemId = item.ItemId,
                                                   ItemCd = item.ItemCd,
                                                   ItemPrice = item.ItemPrice,
                                                   ItemDescription = item.ItemDescription,
                                                   UnitName = unit1.UnitName,
                                                   Sellable = item.Sellable,
                                                   Service = item.Service,
                                                   Taxable = item.Taxable,
                                                   IsActive = item.IsActive,
                                                   GroupName = Group1.GroupName,
                                                   CategoryName = category1.CategoryName
                                               })
                                                .ToList();
            return View(items);
        }


        public IActionResult AddItem()
        {
            ItemMasterViewModel model = new ItemMasterViewModel();
            model.UnitList = _dbContext.tbl_Units.Where(w => w.IsActive == 1).Select(s => new UnitViewModel
            {
                UnitId = s.UnitId,
                UnitName = s.UnitName

            }).ToList();
            model.ItemCategoryList = _dbContext.tbl_ItemCategory.Where(w => w.IsActive == 1).Select(s => new ItemCategoryViewModel
            {
                CategoryId = s.CategoryId,
                CategoryName = s.CategoryName

            }).ToList();
            model.ItemGroupList = _dbContext.tbl_ItemGroup.Where(w => w.IsActive == 1).Select(s => new ItemGroupViewModel
            {
                GroupId = s.GroupId,
                GroupName = s.GroupName
            }).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddItem(ItemMasterViewModel model)
        {
            try
            {


                if (ModelState.IsValid)
                {

                    var checkItem = _dbContext.tbl_ItemMaster.Where(w => w.ItemCd == model.ItemCd).FirstOrDefault();
                    if (checkItem != null)
                    {
                        ViewBag.ErrorMessage = "Item already exists with this code";

                    }
                    else
                    {
                        int iItemUnitId = 0;
                        int.TryParse(model.ItemUnitId, out iItemUnitId);
                        model.iItemUnitId = iItemUnitId;
                        ItemMaster item = new ItemMaster()
                        {
                            ItemCd = model.ItemCd,
                            ItemCost = model.ItemCost == null ? 0 : model.ItemCost.Value,
                            ItemDescription = model.ItemDescription,
                            ItemUnitId = model.iItemUnitId,
                            ItemPrice = model.ItemPrice == null ? 0 : model.ItemPrice.Value,
                            ItemQOH = (model.Service == "Y" ? 0 : (model.ItemQOH == null ? 0 : model.ItemQOH.Value)),
                            Sellable = model.Sellable,
                            Taxable = model.Taxable,
                            Service = model.Service,
                            CategoryId = model.CategoryId,
                            GroupId = model.GroupId,
                            IsActive = 1,
                            CreatedBy = 1,
                            CreatedDate = DateTime.Now
                        };

                        _dbContext.tbl_ItemMaster.Add(item);
                        _dbContext.SaveChanges();
                        ViewBag.SuccessMessage = "Item added successfully";

                    }
                }
            }
            catch (Exception ex)
            {
                var a = "";
            }

            model.UnitList = _dbContext.tbl_Units.Select(s => new UnitViewModel
            {
                UnitId = s.UnitId,
                UnitName = s.UnitName

            }).ToList();
            model.ItemCategoryList = _dbContext.tbl_ItemCategory.Where(w => w.IsActive == 1).Select(s => new ItemCategoryViewModel
            {
                CategoryId = s.CategoryId,
                CategoryName = s.CategoryName

            }).ToList();
            model.ItemGroupList = _dbContext.tbl_ItemGroup.Where(w => w.IsActive == 1).Select(s => new ItemGroupViewModel
            {
                GroupId = s.GroupId,
                GroupName = s.GroupName
            }).ToList();
            return View(model);
        }


        public IActionResult EditItem(string id)
        {
            ItemMasterViewModel model = new ItemMasterViewModel();
            try
            {
                model.UnitList = _dbContext.tbl_Units.Select(s => new UnitViewModel
                {
                    UnitId = s.UnitId,
                    UnitName = s.UnitName

                }).ToList();
                model.ItemCategoryList = _dbContext.tbl_ItemCategory.Where(w => w.IsActive == 1).Select(s => new ItemCategoryViewModel
                {
                    CategoryId = s.CategoryId,
                    CategoryName = s.CategoryName

                }).ToList();
                model.ItemGroupList = _dbContext.tbl_ItemGroup.Where(w => w.IsActive == 1).Select(s => new ItemGroupViewModel
                {
                    GroupId = s.GroupId,
                    GroupName = s.GroupName
                      
                }).ToList();
                int itemId = 0;
                int.TryParse(id, out itemId);
                var checkItem = _dbContext.tbl_ItemMaster.Where(w => w.ItemId == itemId).FirstOrDefault();

                model.ItemId = itemId;
                model.ItemCd = checkItem.ItemCd;
                model.ItemCost = checkItem.ItemCost;
                model.ItemDescription = checkItem.ItemDescription;
                model.ItemUnitId = checkItem.ItemUnitId.ToString();
                model.ItemPrice = checkItem.ItemPrice;
                model.ItemQOH = (checkItem.Service == "Y" ? 0 : (checkItem.ItemQOH));
                model.Sellable = checkItem.Sellable;
                model.Taxable = checkItem.Taxable;
                model.Service = checkItem.Service;
                model.CategoryId = checkItem.CategoryId ?? 0;
                model.GroupId = checkItem.GroupId;
            }
            catch (Exception ex)
            {

                var a = "";
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult EditItem(ItemMasterViewModel model)
        {
            try
            {


                if (ModelState.IsValid)
                {

                    var checkItem = _dbContext.tbl_ItemMaster.Where(w => w.ItemCd == model.ItemCd && w.ItemId != model.ItemId).FirstOrDefault();
                    if (checkItem != null)
                    {
                        ViewBag.ErrorMessage = "Item already exists with this code";

                    }
                    else
                    {
                        checkItem = _dbContext.tbl_ItemMaster.Where(w => w.ItemId == model.ItemId).FirstOrDefault();
                        int iItemUnitId = 0;
                        int.TryParse(model.ItemUnitId, out iItemUnitId);
                        model.iItemUnitId = iItemUnitId;
                        checkItem.ItemCd = model.ItemCd;
                        checkItem.ItemCost = model.ItemCost == null ? 0 : model.ItemCost.Value;
                        checkItem.ItemDescription = model.ItemDescription;
                        checkItem.ItemUnitId = model.iItemUnitId;
                        checkItem.ItemPrice = model.ItemPrice == null ? 0 : model.ItemPrice.Value;
                        checkItem.ItemQOH = (model.Service == "Y" ? 0 : (model.ItemQOH == null ? 0 : model.ItemQOH.Value));
                        checkItem.Sellable = model.Sellable;
                        checkItem.Taxable = model.Taxable;
                        checkItem.Service = model.Service;
                        checkItem.CategoryId = model.CategoryId;
                        checkItem.GroupId = model.GroupId;
                        checkItem.ModifiedBy = 1;
                        checkItem.ModifiedDate = DateTime.Now;
                        _dbContext.SaveChanges();
                        ViewBag.SuccessMessage = "Item update successfully";

                    }
                }
            }
            catch (Exception ex)
            {
                var a = "";
            }

            model.UnitList = _dbContext.tbl_Units.Select(s => new UnitViewModel
            {
                UnitId = s.UnitId,
                UnitName = s.UnitName

            }).ToList();
            model.ItemCategoryList = _dbContext.tbl_ItemCategory.Where(w => w.IsActive == 1).Select(s => new ItemCategoryViewModel
            {
                CategoryId = s.CategoryId,
                CategoryName = s.CategoryName

            }).ToList();
            model.ItemGroupList = _dbContext.tbl_ItemGroup.Where(w => w.IsActive == 1).Select(s => new ItemGroupViewModel
            {
                GroupId = s.GroupId,
                GroupName = s.GroupName
            }).ToList();
            return View(model);
        }


        public IActionResult UnitList()
        {
            List<UnitViewModel> units = (from unit in _dbContext.tbl_Units
                                         select new UnitViewModel
                                         {
                                             UnitId = unit.UnitId,
                                             UnitName = unit.UnitName,
                                             IsActive = unit.IsActive
                                         })
                                        .ToList();
            return View(units);
        }


        public IActionResult AddUnit()
        {
            UnitViewModel model = new UnitViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddUnit(UnitViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var checkUnit = _dbContext.tbl_Units.Where(w => w.UnitName == model.UnitName).FirstOrDefault();
                    if (checkUnit != null)
                    {
                        ViewBag.ErrorMessage = "Unit already exists with this name";
                    }
                    else
                    {
                        Unit unit = new Unit()
                        {
                            UnitName = model.UnitName,
                            IsActive = 1,
                            CreatedBy = 1,
                            CreatedDate = DateTime.Now
                        };

                        _dbContext.tbl_Units.Add(unit);
                        _dbContext.SaveChanges();
                        ViewBag.SuccessMessage = "Unit added successfully";

                    }
                }
            }
            catch (Exception ex)
            {
                var a = "";
            }

            return View(model);
        }


        public IActionResult ItemPriceList()
        {
            List<ItemMasterViewModel> items = (from item in _dbContext.tbl_ItemMaster
                                               join unit in _dbContext.tbl_Units on item.ItemUnitId equals unit.UnitId
                                               select new ItemMasterViewModel
                                               {
                                                   ItemId = item.ItemId,
                                                   ItemCd = item.ItemCd,
                                                   ItemPrice = item.ItemPrice,
                                                   ItemDescription = item.ItemDescription,
                                                   UnitName = unit.UnitName

                                               })
                                                .ToList();
            return View(items);
        }


        public IActionResult ItemPrice(int id)
        {
            ItemPriceViewModel model = new ItemPriceViewModel();
            var checkItemPrice = _dbContext.tbl_ItemMaster.Where(w => w.ItemId == id).FirstOrDefault();
            model.PricItemId = checkItemPrice.ItemId;
            model.PricPrice = checkItemPrice.ItemPrice;
            model.ItemList = _dbContext.tbl_ItemMaster.Select(s => new ItemMasterViewModel
            {
                ItemId = s.ItemId,
                ItemCd = s.ItemCd

            }).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult ItemPrice(ItemPriceViewModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    var checkItem = _dbContext.tbl_ItemMaster.Where(w => w.ItemId == model.PricItemId).FirstOrDefault();
                    checkItem.ItemPrice = model.PricPrice;
                    _dbContext.SaveChanges();

                    var checkItemPrice = _dbContext.tbl_ItemPrice.Where(w => w.PricItemId == model.PricItemId).FirstOrDefault();
                    if (checkItemPrice != null)
                    {
                        checkItemPrice.PricPrice = model.PricPrice;
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        ItemPrice itemPrice = new ItemPrice()
                        {
                            PricPrice = model.PricPrice,
                            PricItemId = model.PricItemId
                        };

                        _dbContext.tbl_ItemPrice.Add(itemPrice);
                        _dbContext.SaveChanges();
                        ViewBag.SuccessMessage = "Item price added successfully";

                    }
                }
            }
            catch (Exception ex)
            {
                var a = "";
            }

            model.ItemList = _dbContext.tbl_ItemMaster.Select(s => new ItemMasterViewModel
            {
                ItemId = s.ItemId,
                ItemCd = s.ItemCd

            }).ToList();

            return View(model);
        }


        [HttpPost]
        public JsonResult DeleteItem(int id)
        {
            ResponseModel response = new ResponseModel();
            try
            {

                var checkOrderDetail = _dbContext.tbl_OrderDetail.Where(w => w.ItemId == id).FirstOrDefault();
                if (checkOrderDetail != null)
                {
                    response.Status = "0";
                    response.Message = "Item has been used in order";

                }
                else
                {
                    var checkItem = _dbContext.tbl_ItemMaster.Where(w => w.ItemId == id).FirstOrDefault();
                    if (checkItem != null)
                    {
                        checkItem.IsActive = 0;
                        checkItem.ModifiedBy = 1;
                        checkItem.ModifiedDate = DateTime.Now;
                        _dbContext.SaveChanges();

                        response.Status = "1";
                        response.Message = "Item deleted successfully";
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

        public IActionResult ItemCategoryList()
        {
            List<ItemCategoryViewModel> units = (from category in _dbContext.tbl_ItemCategory
                                                 select new ItemCategoryViewModel
                                                 {
                                                     CategoryId = category.CategoryId,
                                                     CategoryName = category.CategoryName,
                                                     IsActive = category.IsActive
                                                 })
                                        .ToList();
            return View(units);
        }


        public IActionResult AddItemCategory()
        {
            ItemCategoryViewModel model = new ItemCategoryViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddItemCategory(ItemCategoryViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var checkUnit = _dbContext.tbl_ItemCategory.Where(w => w.CategoryName == model.CategoryName).FirstOrDefault();
                    if (checkUnit != null)
                    {
                        ViewBag.ErrorMessage = "Category already exists with this name";
                    }
                    else
                    {
                        ItemCategory itemCategory = new ItemCategory()
                        {
                            CategoryName = model.CategoryName,
                            IsActive = 1,
                            CreatedBy = 1,
                            CreatedDate = DateTime.Now
                        };

                        _dbContext.tbl_ItemCategory.Add(itemCategory);
                        _dbContext.SaveChanges();
                        ViewBag.SuccessMessage = "Category added successfully";

                    }
                }
            }
            catch (Exception ex)
            {
                var a = "";
            }

            return View(model);
        }

        public IActionResult ItemGroupList()
        {
            List<ItemGroupViewModel> group = _dbContext.tbl_ItemGroup.Select(s => new ItemGroupViewModel
            {
                GroupId = s.GroupId,
                GroupName = s.GroupName,
                IsActive = s.IsActive
            })
            .ToList();

            return View(group);
        }
        public IActionResult AddItemGroup()
        {
            ItemGroupViewModel model = new ItemGroupViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddItemGroup(ItemGroupViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var checkgroup = _dbContext.tbl_ItemGroup.Where(w => w.GroupName == model.GroupName).FirstOrDefault();
                    if (checkgroup != null)
                    {
                        ViewBag.ErrorMessage = "Group already exists with this name";
                    }
                    else
                    {
                        ItemGroup groups = new ItemGroup()
                        {

                            GroupName = model.GroupName,
                            IsActive = 1,
                            CreatedBy = 1,
                            CreatedDate = DateTime.Now
                        };

                        _dbContext.tbl_ItemGroup.Add(groups);
                        _dbContext.SaveChanges();
                        ViewBag.SuccessMessage = "Group added successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                var a = " ";
            }
            return View(model);
        }

        public IActionResult EditItemGroup(string id)
        {
            ItemGroupViewModel model = new ItemGroupViewModel();
            try
            {
                int GroupId = 0;
                int.TryParse(id, out GroupId);
                var checkgroup = _dbContext.tbl_ItemGroup.Where(w => w.GroupId == GroupId).FirstOrDefault();
                model.GroupId = GroupId;
                model.GroupName = checkgroup.GroupName;
            }
            catch (Exception ex)
            {
                var a = " ";
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult EditItemGroup(ItemGroupViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var checkgroup = _dbContext.tbl_ItemGroup.Where(w => w.GroupName == model.GroupName).FirstOrDefault();
                    if (checkgroup != null)
                    {
                        ViewBag.ErrorMessage = "This GroupName is already exists"; 
                    }
                    else
                    {
                        checkgroup = _dbContext.tbl_ItemGroup.Where(w => w.GroupId == model.GroupId).FirstOrDefault();
                        checkgroup.GroupId = model.GroupId;
                        checkgroup.GroupName = model.GroupName;
                        checkgroup.ModifiedBy = 1;
                        checkgroup.ModifiedDate = DateTime.Now;
                        _dbContext.SaveChanges();
                        ViewBag.SuccessMessage = "GroupName update successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                var a = " ";
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult DeleteItemGroup(int id)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var checkItem = _dbContext.tbl_ItemGroup.Where(w => w.GroupId == id).FirstOrDefault();
                if (checkItem != null)
                {
                    checkItem.IsActive = 0;
                    checkItem.ModifiedBy = 1;
                    checkItem.ModifiedDate = DateTime.Now;
                    _dbContext.SaveChanges();

                    response.Status = "1";
                    response.Message = "ItemGroup deleted successfully";
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