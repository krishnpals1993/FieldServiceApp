using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FieldServiceApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceApp.Controllers
{
    public class DownloadController : Controller
    {
        private readonly DBContext _dbContext;

        public DownloadController(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ActionResult LoadBase64(int id)
        {
            var checkImage = _dbContext.tbl_OrderImages.Where(w => w.OrderImageId == id).FirstOrDefault();
            string base64 = Convert.ToBase64String(checkImage.Base64) ;
            byte[] bytes = checkImage.Base64;
            return File(bytes, "image/png", "HtmlToImage.png");
            
        }
    }
}