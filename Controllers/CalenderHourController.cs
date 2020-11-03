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
    public class CalenderHourController : Controller
    {
        private readonly IOptions<Appsettings> _appSettings;
        private readonly DBContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;


        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private int _userId = 0;

        public CalenderHourController(IOptions<Appsettings> appSettings, DBContext dbContext, IHttpContextAccessor HttpContextAccessor)
        {
            _appSettings = appSettings;
            _dbContext = dbContext;
            _httpContextAccessor = HttpContextAccessor;
            int.TryParse(_session.GetString("UserId"), out _userId);

        }

        public IActionResult Index()
        {
            var checkWeekOffs = _dbContext.tbl_CalenderWorkingDays.Where(w => w.DayName != null).Select(s => s.DayName).ToList();
            CalenderWorkingHourViewModel model = new CalenderWorkingHourViewModel();
            var checkCalenderHourList = _dbContext.tbl_CalenderWorkingHours.Where(w => (!checkWeekOffs.Contains(w.DayName))).Select(s => new CalenderWorkingHourViewModel
            {
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Id = s.Id,
                DayName = s.DayName,
            }).ToList();

            return View(checkCalenderHourList);

        }

        public IActionResult NonWorkingDayList()
        {
            var checkCalenderHourList = _dbContext.tbl_CalenderWorkingDays.Select(s => new CalenderWorkingDayViewModel
            {
                Type = s.Type,
                HolidayDate = s.HolidayDate,
                Id = s.Id,
                DayName = s.DayName,
            }).ToList();

            return View(checkCalenderHourList);

        }

        public IActionResult Add()
        {
            ViewBag.CheckWeekOffs = _dbContext.tbl_CalenderWorkingDays.Where(w => w.DayName != null).Select(s => s.DayName).ToList();
            return View(new CalenderWorkingHourViewModel());
        }

        public IActionResult AddWorkingDay()
        {
            return View(new CalenderWorkingDayViewModel());
        }
        [HttpPost]
        public IActionResult AddWorkingDay(CalenderWorkingDayViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Type == "Week Of")
                    {
                        var checkCalenderDay = _dbContext.tbl_CalenderWorkingDays.Where(w => w.DayName == model.DayName).FirstOrDefault();
                        if (checkCalenderDay != null)
                        {
                            checkCalenderDay.ModifiedBy = _userId;
                            checkCalenderDay.ModifiedDate = DateTime.Now;
                            checkCalenderDay.Type = model.Type;
                            checkCalenderDay.DayName = model.DayName;
                            _dbContext.SaveChanges();
                        }
                        else
                        {
                            CalenderWorkingDay calenderWorkingDay = new CalenderWorkingDay
                            {

                                DayName = model.DayName,
                                Type = model.Type,
                                IsActive = 1,
                                CreatedBy = _userId,
                                CreatedDate = DateTime.Now

                            };

                            _dbContext.tbl_CalenderWorkingDays.Add(calenderWorkingDay);
                            _dbContext.SaveChanges();

                        }
                    }
                    else
                    {
                        var checkCalenderDay = _dbContext.tbl_CalenderWorkingDays.Where(w => w.HolidayDate == model.HolidayDate).FirstOrDefault();
                        if (checkCalenderDay != null)
                        {
                            checkCalenderDay.ModifiedBy = _userId;
                            checkCalenderDay.ModifiedDate = DateTime.Now;
                            checkCalenderDay.Type = model.Type;
                            checkCalenderDay.HolidayDate = model.HolidayDate;
                            _dbContext.SaveChanges();
                        }
                        else
                        {
                            CalenderWorkingDay calenderWorkingDay = new CalenderWorkingDay
                            {

                                HolidayDate = model.HolidayDate,
                                Type = model.Type,
                                IsActive = 1,
                                CreatedBy = _userId,
                                CreatedDate = DateTime.Now

                            };

                            _dbContext.tbl_CalenderWorkingDays.Add(calenderWorkingDay);
                            _dbContext.SaveChanges();

                        }
                    }


                    ViewBag.SuccessMessage = "Detail added successfully";
                }
            }
            catch (Exception ex)
            {
                var a = "";
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Add(CalenderWorkingHourViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var checkCalenderHour = _dbContext.tbl_CalenderWorkingHours.FirstOrDefault();
                    if (checkCalenderHour != null)
                    {
                        checkCalenderHour.ModifiedBy = _userId;
                        checkCalenderHour.ModifiedDate = DateTime.Now;
                        checkCalenderHour.StartTime = model.StartTime;
                        checkCalenderHour.EndTime = model.EndTime;
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        CalenderWorkingHour calenderWorkingHour = new CalenderWorkingHour
                        {

                            StartTime = model.StartTime,
                            EndTime = model.EndTime,
                            IsActive = 1,
                            CreatedBy = _userId,
                            CreatedDate = DateTime.Now

                        };

                        _dbContext.tbl_CalenderWorkingHours.Add(calenderWorkingHour);
                        _dbContext.SaveChanges();

                    }

                    ViewBag.SuccessMessage = "Detail added successfully";
                }
            }
            catch (Exception ex)
            {
                var a = "";
            }

            return View(model);
        }


        public IActionResult Edit(int id)
        {
            
            CalenderWorkingHourViewModel model = new CalenderWorkingHourViewModel();
            var checkCalenderHour = _dbContext.tbl_CalenderWorkingHours.Where(w => w.Id == id).FirstOrDefault();
            if (checkCalenderHour != null)
            {
                model.StartTime = checkCalenderHour.StartTime;
                model.EndTime = checkCalenderHour.EndTime;
                model.Id = checkCalenderHour.Id;
                model.DayName = checkCalenderHour.DayName;




            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CalenderWorkingHourViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var checkCalenderHour = _dbContext.tbl_CalenderWorkingHours.Where(w => w.Id == model.Id).FirstOrDefault();
                    if (checkCalenderHour != null)
                    {
                        checkCalenderHour.ModifiedBy = _userId;
                        checkCalenderHour.ModifiedDate = DateTime.Now;
                        checkCalenderHour.StartTime = model.StartTime;
                        checkCalenderHour.EndTime = model.EndTime;
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        CalenderWorkingHour calenderWorkingHour = new CalenderWorkingHour
                        {

                            StartTime = model.StartTime,
                            EndTime = model.EndTime,
                            IsActive = 1,
                            CreatedBy = _userId,
                            CreatedDate = DateTime.Now

                        };

                        _dbContext.tbl_CalenderWorkingHours.Add(calenderWorkingHour);
                        _dbContext.SaveChanges();

                    }

                    ViewBag.SuccessMessage = "Detail added successfully";
                }
            }
            catch (Exception ex)
            {
                var a = "";
            }

            return View(model);
        }
    }
}