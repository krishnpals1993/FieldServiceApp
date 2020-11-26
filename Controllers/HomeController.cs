using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using FieldServiceApp.Filters;
using FieldServiceApp.Models;
using FieldServiceApp.Utility;
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

namespace FieldServiceApp.Controllers
{
    [Authentication]
    public class HomeController : Controller
    {
        private readonly IOptions<Appsettings> _appSettings;
        private readonly DBContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private string _rolename = "";
        private int _userId = 0;

        public HomeController(IOptions<Appsettings> appSettings, DBContext dbContext, IHttpContextAccessor HttpContextAccessor)
        {
            _appSettings = appSettings;
            _dbContext = dbContext;
            _httpContextAccessor = HttpContextAccessor;
            _rolename = _session.GetString("RoleName");
            int.TryParse(_session.GetString("UserId"), out _userId);

        }

        public ActionResult Dashboard()
        {
            DashboardViewModel model = new DashboardViewModel();
            DashboardUtility _dashboardUtility = new DashboardUtility(_dbContext);
            model = _dashboardUtility.getDashBoardDetail(_rolename,_userId);
            var todayDayName = DateTime.Now.DayOfWeek.ToString();
            var checkCalenderHour = _dbContext.tbl_CalenderWorkingHours.Where(w=>w.DayName == todayDayName).FirstOrDefault();
            if (checkCalenderHour != null)
            {
                model.CalenderWorkingHour.StartTime = checkCalenderHour.StartTime;
                model.CalenderWorkingHour.EndTime = checkCalenderHour.EndTime;
                model.CalenderWorkingHour.Id = checkCalenderHour.Id;
                CommanUtility _commanUtility = new CommanUtility(_appSettings);
                if (model.CalenderWorkingHour.StartTime != null)
                {
                    model.CalenderWorkingHour.StartTime = _commanUtility.RoundUp(model.CalenderWorkingHour.StartTime.Value, TimeSpan.FromMinutes(30));
                    model.CalenderWorkingHour.EndTime = _commanUtility.RoundUp(model.CalenderWorkingHour.EndTime.Value, TimeSpan.FromMinutes(30));

                }
            }
            else
            {
                model.CalenderWorkingHour.Id = 0;
            }
            return View(model);
        }

        public IActionResult GetOrderPopup(int id)
        {
            DashboardUtility _dashboardUtility = new DashboardUtility(_dbContext);
            var model = _dashboardUtility.getOrderDetail(id);
            return PartialView("_DashboardOrder", model);

        }


        [HttpPost]
        public JsonResult GetDayTimeDetail(int day )
        {
            CalenderWorkingHourViewModel model = new CalenderWorkingHourViewModel();
            var checkCalenderHour = _dbContext.tbl_CalenderWorkingHours.Where(w => w.Day == day).FirstOrDefault();
            if (checkCalenderHour != null)
            {
                model.StartTimeFormatted = checkCalenderHour.StartTime != null  ?checkCalenderHour.StartTime.Value.ToString("HH:mm:ss"):"00:00:00";
                model.EndTimeFormatted = checkCalenderHour.EndTime != null ? checkCalenderHour.EndTime.Value.ToString("HH:mm:ss") : "00:00:00";
                model.Id = checkCalenderHour.Id;
                model.DayName = checkCalenderHour.DayName;

            }
            return Json(model);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}