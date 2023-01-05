using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using LaCafelogy.Filters;
using LaCafelogy.Models;
using LaCafelogy.Utility;
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace LaCafelogy.Controllers
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
                    CreatedDate = DateTime.Now,
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