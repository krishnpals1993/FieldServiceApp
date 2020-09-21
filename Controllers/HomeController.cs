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
    //[Authentication]
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
            return View(model);
        }

        public IActionResult GetOrderPopup(int id)
        {
            DashboardUtility _dashboardUtility = new DashboardUtility(_dbContext);
            var model = _dashboardUtility.getOrderDetail(id);
            return PartialView("_DashboardOrder", model);

        }

        

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}