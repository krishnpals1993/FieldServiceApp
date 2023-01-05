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

namespace LaCafelogy.Controllers
{
    [Authentication]
    public class MasterController : Controller
    {
        private readonly IOptions<Appsettings> _appSettings;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly DBContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private int _userId = 0;


        public MasterController(IOptions<Appsettings> appSettings, DBContext dbContext, IHttpContextAccessor HttpContextAccessor)
        {
            _appSettings = appSettings;
            _dbContext = dbContext;
            _httpContextAccessor = HttpContextAccessor;
            int.TryParse(_session.GetString("UserId"), out _userId);

        }
        public IActionResult StateList()
        {
            var states = _dbContext.tbl_States.Select(s => new StateViewModel
            {
                StateId = s.StateId,
                StateName = s.StateName,
                IsActive = s.IsActive
            }).ToList();

            return View(states);
        }

        public IActionResult AddState()
        {
            StateViewModel model = new StateViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddState(StateViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var checkState = _dbContext.tbl_States.Where(w => w.StateName == model.StateName).FirstOrDefault();
                    if (checkState != null)
                    {
                        ViewBag.ErrorMessage = "State already exists with this name";
                    }
                    else
                    {
                        State state = new State()
                        {
                            StateName = model.StateName,
                            IsActive = 1,
                            CreatedBy = 1,
                            CreatedDate = DateTime.Now
                        };

                        _dbContext.tbl_States.Add(state);
                        _dbContext.SaveChanges();
                        ViewBag.SuccessMessage = "State added successfully";

                    }
                }
            }
            catch (Exception ex)
            {
                var a = "";
            }

            return View(model);
        }


        public IActionResult EditState(int id)
        {
            StateViewModel model = new StateViewModel();
            var checkState = _dbContext.tbl_States.Where(w => w.StateId == id).FirstOrDefault();
            if (checkState != null)
            {
                model.StateId = checkState.StateId;
                model.StateName = checkState.StateName;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditState(StateViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var checkState = _dbContext.tbl_States.Where(w => w.StateId != model.StateId && w.StateName == model.StateName).FirstOrDefault();
                    if (checkState != null)
                    {
                        ViewBag.ErrorMessage = "State already exists with this name";
                    }
                    else
                    {
                        checkState = _dbContext.tbl_States.Where(w => w.StateId == model.StateId).FirstOrDefault();
                        checkState.StateName = model.StateName;
                        checkState.ModifiedBy = _userId;
                        checkState.ModifiedDate = DateTime.Now;

                        _dbContext.SaveChanges();
                        ViewBag.SuccessMessage = "State updated successfully";

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
        public JsonResult DeleteState(int id)
        {
            ResponseModel response = new ResponseModel();
            try
            {

                var checkCity = _dbContext.tbl_Cities.Where(w => w.StateId == id).FirstOrDefault();
                if (checkCity != null)
                {
                    response.Status = "0";
                    response.Message = "City exists for this state";

                }
                else
                {
                    var checkState = _dbContext.tbl_States.Where(w => w.StateId == id).FirstOrDefault();
                    if (checkState != null)
                    {
                        checkState.IsActive = 0;
                        checkState.ModifiedBy = 1;
                        checkState.ModifiedDate = DateTime.Now;
                        _dbContext.SaveChanges();

                        response.Status = "1";
                        response.Message = "State deleted successfully";
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



        public IActionResult CityList()
        {
            var cities = (from city in _dbContext.tbl_Cities
                          join state in _dbContext.tbl_States
                          on city.StateId equals state.StateId
                          select new CityViewModel
                          {
                              CityId = city.CityId,
                              StateId = state.StateId,
                              IsActive = city.IsActive,
                              CityName = city.CityName,
                              StateName = state.StateName,
                              Tax = city.Tax

                          }).ToList();


            return View(cities);
        }

        public IActionResult AddCity()
        {
            CityViewModel model = new CityViewModel();
            model.StateList = _dbContext.tbl_States.Where(w=>w.IsActive==1).Select(s => new StateViewModel
            {
                StateId = s.StateId,
                StateName = s.StateName,
                IsActive = s.IsActive
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult AddCity(CityViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var checkCity = _dbContext.tbl_Cities.Where(w => w.CityName == model.CityName).FirstOrDefault();
                    if (checkCity != null)
                    {
                        ViewBag.ErrorMessage = "City already exists with this name";
                    }
                    else
                    {
                        City city = new City()
                        {
                            CityName = model.CityName,
                            StateId = model.StateId,
                            Tax= model.Tax,
                            IsActive = 1,
                            CreatedBy = 1,
                            CreatedDate = DateTime.Now
                        };

                        _dbContext.tbl_Cities.Add(city);
                        _dbContext.SaveChanges();
                        ViewBag.SuccessMessage = "City added successfully";

                    }
                }
            }
            catch (Exception ex)
            {
                var a = "";
            }
            model.StateList = _dbContext.tbl_States.Where(w => w.IsActive == 1).Select(s => new StateViewModel
            {
                StateId = s.StateId,
                StateName = s.StateName,
                IsActive = s.IsActive
            }).ToList();

            return View(model);
        }


        public IActionResult EditCity(int id)
        {
            CityViewModel model = new CityViewModel();
            var checkCity = _dbContext.tbl_Cities.Where(w => w.CityId == id).FirstOrDefault();
            if (checkCity != null)
            {
                model.CityId = checkCity.CityId;
                model.StateId = checkCity.StateId;
                model.CityName = checkCity.CityName;
                model.Tax = checkCity.Tax;
            }
            model.StateList = _dbContext.tbl_States.Where(w => w.IsActive == 1).Select(s => new StateViewModel
            {
                StateId = s.StateId,
                StateName = s.StateName,
                IsActive = s.IsActive
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult EditCity(CityViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var checkCity = _dbContext.tbl_Cities.Where(w => w.CityId != model.CityId && w.CityName == model.CityName && model.IsActive ==1).FirstOrDefault();
                    if (checkCity != null)
                    {
                        ViewBag.ErrorMessage = "City already exists with this name";
                    }
                    else
                    {
                        checkCity = _dbContext.tbl_Cities.Where(w => w.CityId == model.CityId).FirstOrDefault();
                        checkCity.CityName = model.CityName;
                        checkCity.StateId = model.StateId;
                        checkCity.ModifiedBy = _userId;
                        checkCity.ModifiedDate = DateTime.Now;
                        checkCity.Tax = model.Tax;
                        _dbContext.SaveChanges();
                        ViewBag.SuccessMessage = "City updated successfully";

                    }
                }
            }
            catch (Exception ex)
            {
                var a = "";
            }
            model.StateList = _dbContext.tbl_States.Where(w => w.IsActive == 1).Select(s => new StateViewModel
            {
                StateId = s.StateId,
                StateName = s.StateName,
                IsActive = s.IsActive
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public JsonResult DeleteCity(int id)
        {
            ResponseModel response = new ResponseModel();
            try
            {

                var checkCity = _dbContext.tbl_Cities.Where(w => w.CityId == id).FirstOrDefault();
                if (checkCity != null)
                {
                    checkCity.IsActive = 0;
                    checkCity.ModifiedBy = 1;
                    checkCity.ModifiedDate = DateTime.Now;
                    _dbContext.SaveChanges();

                    response.Status = "1";
                    response.Message = "City deleted successfully";
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