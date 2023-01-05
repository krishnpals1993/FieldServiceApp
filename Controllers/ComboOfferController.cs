using AutoMapper;
using LaCafelogy.Filters;
using LaCafelogy.Models;
using LaCafelogy.Utility;
using Fingers10.ExcelExport.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq.Dynamic;
using System.Linq;
using LaCafelogy.Models;

namespace LaCafelogy.Controllers
{
    [Authentication]
    public class ComboOfferController : Controller
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


        public ComboOfferController(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings, DBContext dbContext, IHttpContextAccessor HttpContextAccessor, IConfigurationProvider mappingConfiguration)
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
            ComboOfferMasterViewModel model = new ComboOfferMasterViewModel();
            model.ItemList   = _dbContext.tbl_ItemMaster.Select(s => new ComboOfferDetailViewModel
            {
                ItemId = s.ItemId,
                ItemName= s.ItemCd,
                Price = s.ItemPrice
            }).ToList();
            return View(model);
        }

        public IActionResult List()
        {

           



            


            var comboList = _dbContext.tbl_ComboOfferMasters
                            .Select(s => new ComboOfferMasterViewModel
                            {
                                ComboOfferName = s.ComboOfferName,
                                ComboOfferMasterId = s.ComboOfferMasterId,
                                Price = s.ComboOfferMasterId,
                                Type = s.Type
                            }).ToList();

            foreach (var combo in comboList)
            {
                combo.ItemList = (from offerItem in _dbContext.tbl_ComboOfferDetails
                                  join item in _dbContext.tbl_ItemMaster on offerItem.ItemId equals item.ItemId
                                  where offerItem.ComboOfferMasterId == combo.ComboOfferMasterId
                                  select new ComboOfferDetailViewModel
                                  {
                                      ItemName = item.ItemCd,
                                      Price = item.ItemPrice,
                                      Quantity = offerItem.Quantity

                                  }).ToList();
               

            }


            return View(comboList);
        }

        [HttpPost]
        public JsonResult AddComboOffer(string model)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                ComboOfferMasterViewModel comboOfferMasterModel = JsonConvert.DeserializeObject<ComboOfferMasterViewModel>(model);

                ComboOfferMaster comboOfferMaster = new ComboOfferMaster()
                {
                    ComboOfferName = comboOfferMasterModel.ComboOfferName,
                    Type = comboOfferMasterModel.Type,
                    Price = comboOfferMasterModel.Price,
                    IsActive = 1,
                    CreatedBy = _userId,
                    CreatedDate = DateTime.Now,
                };

                _dbContext.tbl_ComboOfferMasters.Add(comboOfferMaster);
                _dbContext.SaveChanges();

                foreach (var item in comboOfferMasterModel.ItemList)
                {

                    ComboOfferDetail comboOfferDetail = new ComboOfferDetail()
                    {
                        ComboOfferMasterId = comboOfferMaster.ComboOfferMasterId,
                        ItemId = item.ItemId,
                        Quantity = item.Quantity,
                        IsActive = 1,
                        CreatedBy = _userId,
                        CreatedDate = DateTime.Now,
                    };

                    _dbContext.tbl_ComboOfferDetails.Add(comboOfferDetail);
                    _dbContext.SaveChanges();

                }

                response.Status = "1";
                response.Message = "Details addedd successfully";
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
