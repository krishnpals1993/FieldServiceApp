﻿
using Intuit.Ipp.OAuth2PlatformClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using System.Net;
using System.Net.Http.Headers;
using LaCafelogy.Filters;
using Microsoft.AspNetCore.Mvc;
using LaCafelogy.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace LaCafelogy.Controllers
{
    

    [Authentication]
    public class AppController : Controller
    {
        public static string mod;
        public static string expo;

        public static string clientid = "";
        public static string clientsecret = "";
        public static string redirectUrl = "";
        public static string stateCSRFToken = "";

        public static string authorizeUrl = "";
        public static string tokenEndpoint = "";
        public static string revocationEndpoint = "";
        public static string userinfoEndpoint = "";
        public static string issuerEndpoint = "";
        public static string code = "";

        public static string access_token = "";
        public static string refresh_token = "";
        public static string identity_token = "";
        public static IList<JsonWebKey> keys;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<Appsettings> _appSettings;

        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public AppController(IOptions<Appsettings> appSettings, IHttpContextAccessor HttpContextAccessor)
        {
            _appSettings = appSettings;
            _httpContextAccessor = HttpContextAccessor;
            clientid = _appSettings.Value.QBSetting.clientid;
            clientsecret = _appSettings.Value.QBSetting.clientsecret;
            redirectUrl = _appSettings.Value.QBSetting.redirectUrl;
        }

        public ActionResult Index()
        {
            return View();
        }

       

        public async Task<ActionResult> CallService()
        {
            var principal = User as ClaimsPrincipal;

          
            string query = "select * from Customer";
            // build the  request
            string encodedQuery = WebUtility.UrlEncode(query);
            if (_session.Get("realmId") != null)
            {
                string realmId = _session.Get("realmId").ToString();

                string qboBaseUrl = _appSettings.Value.QBSetting.QBOBaseUrl;

                //add qbobase url and query
                string uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, realmId, encodedQuery);
               
                string result="";
                
                try
                {
                    var client = new HttpClient();
                    
                    client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                    client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + principal.FindFirst("access_token").Value);
                    

                    result = await client.GetStringAsync(uri);
                    return View("CallService",(object)( "QBO API call success! " + result));
                }
                catch (Exception ex)
                {
                    return View("CallService",(object)"QBO API call Failed!");
                }
            }
            else
                return View("CallService",(object)"QBO API call Failed!");
        }

        public async Task<ActionResult> RefreshToken()
        {
            //Refresh Token call
            var tokenClient = new TokenClient(AppController.tokenEndpoint, AppController.clientid, AppController.clientsecret);
            var principal = User as ClaimsPrincipal;
            var refreshToken = principal.FindFirst("refresh_token").Value;

            TokenResponse response = await tokenClient.RequestRefreshTokenAsync(refreshToken);
            UpdateCookie(response);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> RevokeAccessToken()
        {
            var accessToken = (User as ClaimsPrincipal).FindFirst("access_token").Value;

            //Revoke Access token call
            var revokeClient = new TokenRevocationClient(AppController.revocationEndpoint, clientid, clientsecret);

            //Revoke access token
            TokenRevocationResponse revokeAccessTokenResponse = await revokeClient.RevokeAccessTokenAsync(accessToken);
            if (revokeAccessTokenResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                _session.Clear();
                //Request.GetOwinContext().Authentication.SignOut();
                
            }//delete claims and cookies
           
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> RevokeRefreshToken()
        {
            var refreshToken = (User as ClaimsPrincipal).FindFirst("refresh_token").Value;
            
            //Revoke Refresh token call
            var revokeClient = new TokenRevocationClient(AppController.revocationEndpoint, clientid, clientsecret);

            //Revoke refresh token
            TokenRevocationResponse revokeAccessTokenResponse = await revokeClient.RevokeAccessTokenAsync(refreshToken);
            if (revokeAccessTokenResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                _session.Clear();
                //Request.GetOwinContext().Authentication.SignOut();
            }
            //return RedirectToAction("Index");
            return RedirectToAction("Index");
        }

        private void UpdateCookie(TokenResponse response)
        {
            if (response.IsError)
            {
                throw new Exception(response.Error);
            }

            var identity = (User as ClaimsPrincipal).Identities.First();
            var result = from c in identity.Claims
                         where c.Type != "access_token" &&
                               c.Type != "refresh_token" &&
                               c.Type != "access_token_expires_at" &&
                               c.Type != "access_token_expires_at" 
                         select c;

            var claims = result.ToList();

            claims.Add(new Claim("access_token", response.AccessToken));
           
            claims.Add(new Claim("access_token_expires_at", (DateTime.Now.AddSeconds(response.AccessTokenExpiresIn)).ToString()));
            claims.Add(new Claim("refresh_token", response.RefreshToken));
           
            claims.Add(new Claim("refresh_token_expires_at", (DateTime.UtcNow.ToEpochTime() + response.RefreshTokenExpiresIn).ToDateTimeFromEpoch().ToString()));
           
            var newId = new ClaimsIdentity(claims, "Cookies");
            //Request.GetOwinContext().Authentication.SignIn(newId);
        }
        
    }
}