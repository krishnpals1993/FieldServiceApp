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
using JqueryDataTables.ServerSide.AspNetCoreWeb.Infrastructure;
using JqueryDataTables.ServerSide.AspNetCoreWeb.Models;
using System.Text.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Threading.Tasks;
using LaCafelogy.Filters;
using System.Linq.Dynamic;

namespace LaCafelogy.Controllers
{
    [Authentication]
    public class CustomerController : Controller
    {
        private readonly IOptions<Appsettings> _appSettings;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly DBContext _dbContext;
        private readonly IConfigurationProvider _mappingConfiguration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private int UserId = 0;
        public CustomerController(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings, DBContext dbContext, IConfigurationProvider mappingConfiguration,
             IHttpContextAccessor HttpContextAccessor)
        {
            _appSettings = appSettings;
            _emailSettings = emailSettings;
            _dbContext = dbContext;
            _httpContextAccessor = HttpContextAccessor;
            _mappingConfiguration = mappingConfiguration;
            int.TryParse(_session.GetString("UserId"), out UserId);
        }

        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}