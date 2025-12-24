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
using System.IO;
using System.Drawing;

namespace FieldServiceApp.Controllers
{
    [Authentication]
    public class JobImagesController : Controller
    {

        private readonly IOptions<Appsettings> _appSettings;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly DBContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private int _userId = 0;
        private int _employeeId = 0;

        public JobImagesController(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings, DBContext dbContext, IHttpContextAccessor HttpContextAccessor)
        {
            _appSettings = appSettings;
            _emailSettings = emailSettings;
            _dbContext = dbContext;
            _httpContextAccessor = HttpContextAccessor;
            int.TryParse(_session.GetString("UserId"), out _userId);
            _employeeId = _dbContext.tbl_EmployeeMaster.Where(w => w.UserId == _userId).Select(s => s.EmployeeId).FirstOrDefault();
        }


        public IActionResult List()
        {


            var items = (from images in _dbContext.tbl_OrderImages
                         join order in _dbContext.tbl_OrderMaster on images.OrderId equals order.OrderId
                         join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustomerId
                         where images.CreatedBy == _userId
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
            return View(items);
        }

        public IActionResult ImageList()
        {


            var items = (from images in _dbContext.tbl_OrderImages
                         join order in _dbContext.tbl_OrderMaster on images.OrderId equals order.OrderId
                         join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustomerId
                         join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                         into orderAssign
                         from orderAssign1 in orderAssign.DefaultIfEmpty()
                         join employee in _dbContext.tbl_EmployeeMaster on orderAssign1.EmployeeId equals employee.EmployeeId
                         into employee
                         from employee1 in employee.DefaultIfEmpty()
                         select new OrderImageViewModel
                         {
                             JobImageId = images.OrderImageId,
                             IsActive = images.IsActive,
                             OrderId = order.OrderId,
                             CustomerName = customer.CompanyName,
                             Image = images.Image,
                             Description = images.Description,
                             Base64 = images.Base64,
                             EmployeeName = employee1.FirstName ?? "" + " " + employee1.LastName ?? ""

                         }).ToList();
            return View(items);
        }

        public IActionResult Add()
        {
            OrderImageViewModel model = new OrderImageViewModel();
            try
            {

                model.OrderList = (from order in _dbContext.tbl_OrderMaster
                                   join customer in _dbContext.tbl_CustomerMaster
                                   on order.CustomerId equals customer.CustomerId
                                   join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                                   where orderAssign.EmployeeId == _employeeId
                                   select new ServiceFormOrderViewModel
                                   {
                                       OrderId = order.OrderId,
                                       CustomerName = customer.CompanyName + " (Order #" + order.OrderId + ")"

                                   }).ToList();

            }
            catch (Exception ex)
            {

                var a = "";
            }


            return View(model);
        }

        [HttpPost]
        public IActionResult Add(OrderImageViewModel model, IFormFile file)
        {
            try
            {

                model.OrderList = (from order in _dbContext.tbl_OrderMaster
                                   join customer in _dbContext.tbl_CustomerMaster
                                   on order.CustomerId equals customer.CustomerId
                                   join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                                   where orderAssign.EmployeeId == _employeeId
                                   select new ServiceFormOrderViewModel
                                   {
                                       OrderId = order.OrderId,
                                       CustomerName = customer.CompanyName + " (Order #" + order.OrderId + ")"

                                   }).ToList();

                if (file == null || file.Length == 0)
                {
                    ViewBag.ErroMessage = "Please select file";
                    return View(model);
                }


                //convert uploaded image as image object like given below
                Image image = Image.FromStream(file.OpenReadStream());
                //call 'ImageToBase64' function here
                byte[] base64String = ImageToBase64(image, System.Drawing.Imaging.ImageFormat.Jpeg);

                OrderImage orderImage = new OrderImage
                {
                    OrderId = model.OrderId,
                    Description = model.Description,
                    Image = file.FileName,
                    IsActive = 1,
                    CreatedBy = _userId,
                    CreatedDate = DateTime.UtcNow,
                    Base64 = base64String


                };
                _dbContext.tbl_OrderImages.Add(orderImage);
                _dbContext.SaveChanges();

                ViewBag.SuccessMessage = "Detail added successfully";
                return View(model);

            }
            catch (Exception ex)
            {

                var a = "";
            }


            return View(model);
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

        public IActionResult GetImagesPopup(int id)
        {
            OrderImageViewModel model = new OrderImageViewModel();
            try
            {
                model = (from image in _dbContext.tbl_OrderImages
                         join order in _dbContext.tbl_OrderMaster on image.OrderId equals order.OrderId
                         join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustomerId
                         join shipping in _dbContext.tbl_CustmoerShipping on order.ShipId equals shipping.CustomerShipId
                         join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                          into city
                         from city1 in city.DefaultIfEmpty()
                         join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                         into state
                         from state1 in state.DefaultIfEmpty()
                         where image.OrderImageId == id
                         select new OrderImageViewModel
                         {
                             JobImageId = image.OrderImageId,
                             Description = image.Description,
                             Base64 = image.Base64,
                             OrderId = order.OrderId,
                             CustomerName = customer.CompanyName,
                             ShipAddress = (city1.CityName ?? "") + " " + (state1.StateName ?? "") + " " + (shipping.Address ?? "") + " " + (shipping.Address2 ?? "")
                         }).FirstOrDefault();
            }
            catch (Exception ex)
            {


            }




            return PartialView("_ImageDetail", model);

        }

        public IActionResult GetImageShareEmail(int id)
        {
            OrderImageViewModel model = new OrderImageViewModel();
            try
            {
                model = (from image in _dbContext.tbl_OrderImages
                         join order in _dbContext.tbl_OrderMaster on image.OrderId equals order.OrderId
                         join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustomerId
                         join shipping in _dbContext.tbl_CustmoerShipping on order.ShipId equals shipping.CustomerShipId
                         join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                          into city
                         from city1 in city.DefaultIfEmpty()
                         join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                         into state
                         from state1 in state.DefaultIfEmpty()
                         where image.OrderImageId == id
                         select new OrderImageViewModel
                         {
                             JobImageId = image.OrderImageId,
                             Description = image.Description,
                             Base64 = image.Base64,
                             OrderId = order.OrderId,
                             CustomerName = customer.CompanyName,
                             ShipAddress = (city1.CityName ?? "") + " " + (state1.StateName ?? "") + " " + (shipping.Address ?? "") + " " + (shipping.Address2 ?? "")
                         }).FirstOrDefault();
            }
            catch (Exception ex)
            {


            }




            return PartialView("_ImageShareEmail", model);

        }

        [HttpPost]
        public JsonResult ShareWithEmail(int orderId, string Email1, string Email2, string Email3)
        {
            DashboardOrderViewModel model = new DashboardOrderViewModel();
            try
            {
                DashboardUtility _dashboardUtility = new DashboardUtility(_dbContext);
                var orderDetail = (from image in _dbContext.tbl_OrderImages
                                   join order in _dbContext.tbl_OrderMaster on image.OrderId equals order.OrderId
                                   join customer in _dbContext.tbl_CustomerMaster on order.CustomerId equals customer.CustomerId
                                   join orderAssign in _dbContext.tbl_OrderAssignment on order.OrderId equals orderAssign.OrderId
                                   into orderAssign
                                   from orderAssign1 in orderAssign.DefaultIfEmpty()
                                   join employee in _dbContext.tbl_EmployeeMaster on orderAssign1.EmployeeId equals employee.EmployeeId
                                   into employee
                                   from employee1 in employee.DefaultIfEmpty()
                                   join shipping in _dbContext.tbl_CustmoerShipping on order.ShipId equals shipping.CustomerShipId
                                   join city in _dbContext.tbl_Cities on shipping.CityId equals city.CityId
                                                    into city
                                   from city1 in city.DefaultIfEmpty()
                                   join state in _dbContext.tbl_States on shipping.StateId equals state.StateId
                                   into state
                                   from state1 in state.DefaultIfEmpty()
                                   where image.OrderImageId == orderId
                                   select new OrderImageViewModel
                                   {
                                       EmployeeName = (employee1.FirstName + " " + (employee1.MiddleName ?? "") + " " + employee1.LastName) ?? "",
                                       JobImageId = image.OrderImageId,
                                       Description = image.Description,
                                       Base64 = image.Base64,
                                       OrderId = order.OrderId,
                                       CustomerName = customer.CompanyName,
                                       ShipAddress = (city1.CityName ?? "") + " " + (state1.StateName ?? "") + " " + (shipping.Address ?? "") + " " + (shipping.Address2 ?? "")
                                   }).FirstOrDefault();

                var toEmails = Email1;
                if (!String.IsNullOrEmpty(Email2))
                {
                    toEmails = toEmails + ", " + Email2;
                }

                if (!String.IsNullOrEmpty(Email3))
                {
                    toEmails = toEmails + ", " + Email3;
                }

                EmailUtility _emaUtility = new EmailUtility(_appSettings, _emailSettings, _dbContext);

                string emailBody = @"<p>Picture uploaded by " + orderDetail.EmployeeName + "</p>" +
                                    "<br />" +
                                  "<p><img style='max-width: 470px; margin-top: 50px;' src='data:image/png;base64," + (Convert.ToBase64String(orderDetail.Base64)) + "'/></p>";

                _emaUtility.SendEmailWithImage("", "Job Image for Order #" + orderDetail.OrderId + " - Customer: " + orderDetail.CustomerName + " (" + orderDetail.ShipAddress + ") ", emailBody, toEmails.Split(","), orderDetail.Base64);

                OrderImageShareDetail orderImageShareDetail = new OrderImageShareDetail
                {
                    OrderId = orderDetail.OrderId,
                    OrderImageId = orderDetail.JobImageId,
                    Subject = "Job Image for Order #" + orderDetail.OrderId + " - Customer: " + orderDetail.CustomerName + " (" + orderDetail.ShipAddress + ") ",
                    Type = "Email",
                    Body = emailBody,
                    ToEmail = toEmails,
                    IsActive = 1,
                    CreatedBy = _userId,
                    CreatedDate = DateTime.UtcNow
                };
                _dbContext.tbl_OrderImageShareDetail.Add(orderImageShareDetail);
                _dbContext.SaveChanges();


            }
            catch (Exception ex)
            {
                var a = "";
            }

            return Json(model);
        }


        [HttpPost]
        public JsonResult DeleteImage(int id)
        {
            ResponseModel response = new ResponseModel();
            try
            {

                var checkImage = _dbContext.tbl_OrderImages.Where(w => w.OrderImageId == id).FirstOrDefault();
                if (checkImage != null)
                {
                    checkImage.IsActive = 0;
                    checkImage.ModifiedBy = _userId;
                    checkImage.ModifiedDate = DateTime.UtcNow;
                    _dbContext.SaveChanges();

                    response.Status = "1";
                    response.Message = "Detail deleted successfully";
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