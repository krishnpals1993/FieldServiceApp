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
using System.Drawing;
using System.IO;

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
            var todayDayName = DateTime.UtcNow.DayOfWeek.ToString();
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
            var model = _dashboardUtility.getOrderDetailWithApartemt(id);

            var items = (from images in _dbContext.tbl_OrderImages
                         join order in _dbContext.tbl_OrderMaster on images.OrderId equals order.OrderId
                         join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustomerId
                         where images.CreatedBy == _userId && order.OrderId == model.OrderId
                         select new OrderImageViewModel
                         {
                             JobImageId = images.OrderImageId,
                             IsActive = images.IsActive,
                             OrderId = order.OrderId,
                             CustomerName = customer.CompanyName,
                             Image = images.Image,
                             Description = images.Description,
                             Base64 = images.Base64

                         }).ToList();

            model.OrderImageList = items;

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

        [HttpPost]
        public JsonResult AddImage( IFormFile file, int OrderId, string Description)
        {
            try
            {

              

                if (file == null || file.Length == 0)
                {
                    ViewBag.ErroMessage = "Please select file";
                    return Json("1");

                }

                //convert uploaded image as image object like given below
                Image image = Image.FromStream(file.OpenReadStream(), true, true);
                //call 'ImageToBase64' function here
                byte[] base64String = ImageToBase64(image, System.Drawing.Imaging.ImageFormat.Jpeg);

                OrderImage orderImage = new OrderImage
                {
                    OrderId = OrderId,
                    Description =  Description,
                    Image = file.FileName,
                    IsActive = 1,
                    CreatedBy = _userId,
                    CreatedDate = DateTime.UtcNow,
                    Base64 = base64String


                };
                _dbContext.tbl_OrderImages.Add(orderImage);
                _dbContext.SaveChanges();

                ViewBag.SuccessMessage = "Detail added successfully";
                return Json("1");

            }
            catch (Exception ex)
            {

                var a = "";
            }


            return Json("0");
        }

        public static byte[] ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                //Convert byte[] to Base64 String
                //string base64String = Convert.ToBase64String(imageBytes);
                return imageBytes;
            }
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}