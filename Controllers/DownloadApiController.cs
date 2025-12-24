using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FieldServiceApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadApiController : ControllerBase
    {
        private readonly DBContext _dbContext;

        public DownloadApiController( DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public static Image LoadBase64(int id)
        {
            string base64 = "";
            byte[] bytes = Convert.FromBase64String(base64);
            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }
            return image;
        }

    }
}