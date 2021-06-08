using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using FieldServiceApp.Models;
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Intuit.Ipp.OAuth2PlatformClient;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using System.Net;
using System.Collections.Generic;
using FieldServiceApp.Filters;

namespace FieldServiceApp.Controllers
{
    //[Authentication]
    public class QuickBookController : Controller
    {
        private readonly IOptions<Appsettings> _appSettings;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly DBContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        DiscoveryClient discoveryClient;
        DiscoveryResponse doc;
        AuthorizeRequest request;
        public static IList<JsonWebKey> keys;
        public static string scope;
        public static string authorizeUrl;

        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private int _userId = 0;

        public QuickBookController(IOptions<Appsettings> appSettings, IOptions<EmailSettings> emailSettings, DBContext dbContext, IHttpContextAccessor HttpContextAccessor)
        {
            _appSettings = appSettings;
            _emailSettings = emailSettings;
            _dbContext = dbContext;
            _httpContextAccessor = HttpContextAccessor;
            int.TryParse(_session.GetString("UserId"), out _userId);

        }

        public async Task<ActionResult> Home()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            _session.Clear();
            
            //Intialize DiscoverPolicy
            DiscoveryPolicy dpolicy = new DiscoveryPolicy();
            dpolicy.RequireHttps = true;
            dpolicy.ValidateIssuerName = true;


            //Assign the Sandbox Discovery url for the Apps' Dev clientid and clientsecret that you use
            //Or
            //Assign the Production Discovery url for the Apps' Production clientid and clientsecret that you use

            string discoveryUrl = _appSettings.Value.QBSetting.DiscoveryUrl;

            if (discoveryUrl != null && AppController.clientid != null && AppController.clientsecret != null)
            {
                discoveryClient = new DiscoveryClient(discoveryUrl);
            }
            else
            {
                Exception ex = new Exception("Discovery Url missing!");
                throw ex;
            }
            doc = await discoveryClient.GetAsync();

            if (doc.StatusCode == HttpStatusCode.OK)
            {
                //Authorize endpoint
                AppController.authorizeUrl = doc.AuthorizeEndpoint;

                //Token endpoint
                AppController.tokenEndpoint = doc.TokenEndpoint;

                //Token Revocation enpoint
                AppController.revocationEndpoint = doc.RevocationEndpoint;

                //UserInfo endpoint
                AppController.userinfoEndpoint = doc.UserInfoEndpoint;

                //Issuer endpoint
                AppController.issuerEndpoint = doc.Issuer;

                //JWKS Keys
                AppController.keys = doc.KeySet.Keys;
            }

            //Get mod and exponent value
            foreach (var key in AppController.keys)
            {
                if (key.N != null)
                {
                    //Mod
                    AppController.mod = key.N;
                }
                if (key.E != null)
                {
                    //Exponent
                    AppController.expo = key.E;
                }
            }



            return View();
        }


        [HttpPost]
        public ActionResult MyAction(string submitButton)
        {
            switch (submitButton)
            {
                case "C2QB":
                    // delegate sending to C2QB Action
                    return (C2QB());
                case "GetAppNow":
                    // call another action to GetAppNow
                    return (GetAppNow());
                case "SIWI":
                    // call another action to SIWI
                    return (SIWI());
                default:
                    // If they've submitted the form without a submitButton, 
                    // just return the view again.
                    return (View());
            }
        }


        private ActionResult C2QB()
        {
            scope = OidcScopes.Accounting.GetStringValue();
            authorizeUrl = GetAuthorizeUrl(scope);
            // perform the redirect here.
            return Redirect(authorizeUrl);
        }

        private ActionResult GetAppNow()
        {
            scope = OidcScopes.Accounting.GetStringValue() + " " + OidcScopes.Payment.GetStringValue() + " " + OidcScopes.OpenId.GetStringValue() + " " + OidcScopes.Address.GetStringValue()
                 + " " + OidcScopes.Email.GetStringValue() + " " + OidcScopes.Phone.GetStringValue()
                 + " " + OidcScopes.Profile.GetStringValue();
            authorizeUrl = GetAuthorizeUrl(scope);
            // perform the redirect here.
            return Redirect(authorizeUrl);
        }

        private ActionResult SIWI()
        {
            scope = OidcScopes.OpenId.GetStringValue() + " " + OidcScopes.Address.GetStringValue()
                 + " " + OidcScopes.Email.GetStringValue() + " " + OidcScopes.Phone.GetStringValue()
                 + " " + OidcScopes.Profile.GetStringValue();
            authorizeUrl = GetAuthorizeUrl(scope);
            // perform the redirect here.
            return Redirect(authorizeUrl);
        }




        private void SetTempState(string state)
        {
            var tempId = new ClaimsIdentity("TempState");
            tempId.AddClaim(new Claim("state", state));

           // Request.GetOwinContext().Authentication.SignIn(tempId);
        }

        private string GetAuthorizeUrl(string scope)
        {
            var state = Guid.NewGuid().ToString("N");

            SetTempState(state);

             

            //Make Authorization request
            var request = new AuthorizeRequest(AppController.authorizeUrl);

            string url = request.CreateAuthorizeUrl(
               clientId: _appSettings.Value.QBSetting.clientid,
               responseType: OidcConstants.AuthorizeResponse.Code,
               scope: scope,
               redirectUri: _appSettings.Value.QBSetting.redirectUrl,
               state: state);

            return url;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}