using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using LaCafelogy.Models;
using LaCafelogy.Utility;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FieldServiceApp.ApiController
{


    [Route("AccountApi")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly IOptions<Appsettings> _appSettings;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly DBContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public AccountApiController(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings, DBContext dbContext, IHttpContextAccessor HttpContextAccessor)
        {
            _appSettings = appSettings;
            _emailSettings = emailSettings;
            _dbContext = dbContext;
            _httpContextAccessor = HttpContextAccessor;
        }



        // POST: api/Account
        [Route("Login")]
        [EnableCors("AllowOrigin")]
        [HttpPost]
        public ApiResponseModel Login(LoginApiModel model)
        {
            LoginApiResponseModel response = new LoginApiResponseModel();

            try
            {
                DbfunctionUtility dbfunction = new DbfunctionUtility(_appSettings);
                DataSet ds = dbfunction.GetDataset(@"select * from ""Users"" u join ""Roles"" r on u.""RoleId"" = r.""RoleId""
                                                        where ""Email"" = '" + model.Username + "' " +
                                                    "and \"Password\"  = '" + model.Password + "' ");

                if (ds.Tables[0].Rows.Count == 0)
                {
                    response.ReturnMessage = "Incorrect username or password";
                    response.ReturnStatus = "0";
                    return response;
                }
                else
                {
                    response.UserId = Convert.ToString(ds.Tables[0].Rows[0]["UserId"]);
                    response.RoleName = Convert.ToString(ds.Tables[0].Rows[0]["RoleName"]);
                    response.Username = model.Username;
                    response.ReturnMessage = "Success";
                    response.ReturnStatus = "1";
                    return response;

                }

               
            }
            catch (Exception ex)
            {

                response.ReturnMessage = ex.Message;
                response.ReturnStatus = "0";
                return response;
            }

        }




    }
}
