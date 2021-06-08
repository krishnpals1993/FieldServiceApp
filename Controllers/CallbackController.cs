//using IdentityModel;
//using IdentityModel.Client;
using Intuit.Ipp.OAuth2PlatformClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using FieldServiceApp.Models;
using System.Net.Http;
using FieldServiceApp.Filters;

namespace FieldServiceApp.Controllers
{
   
    public class CallbackController : Controller
	{


        string sub = "";
        string email = "";
        string emailVerified = "";
        string givenName = "";
        string familyName = "";
        string phoneNumber = "";
        string phoneNumberVerified = "";
        string streetAddress = "";
        string locality = "";
        string region = "";
        string postalCode = "";
        string country = "";

        public static string clientid = "";
        public static string clientsecret = "";
        public static string redirectUrl = "";

        IdTokenJWTClaimTypes payloadData;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<Appsettings> _appSettings;

        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public CallbackController(IOptions<Appsettings> appSettings, IHttpContextAccessor HttpContextAccessor)
        {
            _appSettings = appSettings;
            _httpContextAccessor = HttpContextAccessor;
            clientid = _appSettings.Value.QBSetting.clientid;
            clientsecret = _appSettings.Value.QBSetting.clientsecret;
            redirectUrl = _appSettings.Value.QBSetting.redirectUrl;
        }





        public IActionResult Index()
		{
            
            ViewBag.Code = HttpContext.Request.Query["code"];
            ViewBag.RealmId = HttpContext.Request.Query["realmId"];

            var state = HttpContext.Request.Query["state"].ToString();
            //var tempState = await GetTempStateAsync();

            //if (state.Equals(tempState.Item1, StringComparison.Ordinal))
            //{
            //    ViewBag.State = state + " (valid)";
            //}
            //else
            //{
            //    ViewBag.State = state + " (invalid)";
            //}

            ViewBag.Error = HttpContext.Request.Query["error"];

            return View();
		}

        [HttpPost]
        [ActionName("Index")]
        public async Task<ActionResult> Token()
        {
            
            //Request Oauth2 tokens
            var tokenClient = new TokenClient(AppController.tokenEndpoint, _appSettings.Value.QBSetting.clientid, _appSettings.Value.QBSetting.clientsecret);


            var code = HttpContext.Request.Query["code"].ToString();
            var realmId = HttpContext.Request.Query["realmId"].ToString();
            if (realmId != null)
            {
                _session.SetString("realmId", realmId);
            }
          //  var tempState = await GetTempStateAsync();
            //Request.GetOwinContext().Authentication.SignOut("TempState");

            TokenResponse response = await tokenClient.RequestTokenFromCodeAsync(code, _appSettings.Value.QBSetting.redirectUrl);

            await ValidateResponseAndSignInAsync(response);

            if (!string.IsNullOrEmpty(response.IdentityToken))
            {
                ViewBag.IdentityToken = response.IdentityToken; 
            }
            if (!string.IsNullOrEmpty(response.AccessToken))
            {
                ViewBag.AccessToken = response.AccessToken; 
            }


            var principal = User as ClaimsPrincipal;


            string query = "select * from Customer";
            // build the  request
            string encodedQuery = WebUtility.UrlEncode(query);
            if (_session.Get("realmId") != null)
            {
                string qboBaseUrl = _appSettings.Value.QBSetting.QBOBaseUrl;

                //add qbobase url and query
                string uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, realmId, encodedQuery);

                string result = "";

                try
                {
                    var client = new HttpClient();

                    client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                    client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ViewBag.AccessToken);


                    result = await client.GetStringAsync(uri);
                    return View("CallService", (object)("QBO API call success! " + result));
                }
                catch (Exception ex)
                {
                    return View("CallService", (object)"QBO API call Failed!");
                }
            }


            return View("Token", response);
        }

        private async Task ValidateResponseAndSignInAsync(TokenResponse response)
        {
            var claims = new List<Claim>();
            if (!string.IsNullOrWhiteSpace(response.IdentityToken))
            {
                
                bool isIdTokenValid = ValidateToken(response.IdentityToken);//throw error is not valid

                if (isIdTokenValid == true)
                {
                    claims.Add(new Claim("id_token", response.IdentityToken));
                   
                }
            }
            if (_session.GetString("realmId") != null)
            {
                claims.Add(new Claim("realmId", _session.GetString("realmId").ToString()));
            }


            if (!string.IsNullOrWhiteSpace(response.AccessToken))
            {
                var userClaims = await GetUserInfoClaimsAsync(response.AccessToken);
                claims.AddRange(userClaims);

                claims.Add(new Claim("access_token", response.AccessToken));
                claims.Add(new Claim("access_token_expires_at", (DateTime.Now.AddSeconds(response.AccessTokenExpiresIn)).ToString()));
            }

            if (!string.IsNullOrWhiteSpace(response.RefreshToken))
            {
                claims.Add(new Claim("refresh_token", response.RefreshToken));
                claims.Add(new Claim("refresh_token_expires_at", (DateTime.Now.AddSeconds(response.RefreshTokenExpiresIn)).ToString()));
                
            }
            
            var id = new ClaimsIdentity(claims, "Cookies");
           // Request.GetOwinContext().Authentication.SignIn(id);
            
        }

        private bool ValidateToken(string identityToken)
        {
            if (AppController.keys != null)
            {
               
                //IdentityToken
                if (identityToken != null)
                {
                    //Split the identityToken to get Header and Payload
                    string[] splitValues = identityToken.Split('.');
                    if (splitValues[0] != null)
                    {

                        //Decode header 
                        var headerJson = Encoding.UTF8.GetString(Base64Url.Decode(splitValues[0].ToString()));

                        //Deserilaize headerData
                        IdTokenHeader headerData = JsonConvert.DeserializeObject<IdTokenHeader>(headerJson);

                        //Verify if the key id of the key used to sign the payload is not null
                        if (headerData.Kid == null)
                        {
                            return false;
                        }

                        //Verify if the hashing alg used to sign the payload is not null
                        if (headerData.Alg == null)
                        {
                            return false;
                        }

                    }
                    if (splitValues[1] != null)
                    {
                        //Decode payload
                        var payloadJson = Encoding.UTF8.GetString(Base64Url.Decode(splitValues[1].ToString()));


                         payloadData = JsonConvert.DeserializeObject<IdTokenJWTClaimTypes>(payloadJson);

                       

                        //Verify Aud matches ClientId
                        if (payloadData.Aud != null)
                        {
                            if (payloadData.Aud[0].ToString() != AppController.clientid)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }


                        //Verify Authtime matches the time the ID token was authorized.                
                        if (payloadData.Auth_time == null)
                        {
                            return false;
                        }



                        //Verify exp matches the time the ID token expires, represented in Unix time (integer seconds).                
                        if (payloadData.Exp != null)
                        {
                            long expiration = Convert.ToInt64(payloadData.Exp);                            
                            long currentEpochTime = EpochTimeExtensions.ToEpochTime(DateTime.UtcNow);
                            //Verify the ID expiration time with what expiry time you have calculated and saved in your application
                            //If they are equal then it means IdToken has expired 

                            if ((expiration - currentEpochTime) <= 0)
                            {
                                return false;
                                
                            }                            

                        }

                        //Verify Iat matches the time the ID token was issued, represented in Unix time (integer seconds).            
                        if (payloadData.Iat == null)
                        {
                            return false;
                        }


                        //verify Iss matches the  issuer identifier for the issuer of the response.     
                        if (payloadData.Iss != null)
                        {
                            if (payloadData.Iss.ToString() != AppController.issuerEndpoint)
                            {

                               return false;
                            }
                        }
                        else
                        {
                            return false;
                        }



                        //Verify sub. Sub is an identifier for the user, unique among all Intuit accounts and never reused. 
                        //An Intuit account can have multiple emails at different points in time, but the sub value is never changed.
                        //Use sub within your application as the unique-identifier key for the user.
                        if (payloadData.Sub == null)
                        {

                            return false;
                        }



                    }

                    //Use external lib to decode mod and expo value and generte hashes
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

                    //Read values of n and e from discovery document.
                    rsa.ImportParameters(
                      new RSAParameters()
                      {
                          //Read values from discovery document
                          Modulus = Base64Url.Decode(AppController.mod),
                          Exponent = Base64Url.Decode(AppController.expo)
                      });

                    //Verify Siganture hash matches the signed concatenation of the encoded header and the encoded payload with the specified algorithm
                    SHA256 sha256 = SHA256.Create();

                    byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(splitValues[0] + '.' + splitValues[1]));

                    RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                    rsaDeformatter.SetHashAlgorithm("SHA256");
                    if (rsaDeformatter.VerifySignature(hash, Base64Url.Decode(splitValues[2])))
                    {
                        //identityToken is valid
                        return true;
                    }
                    else
                    {
                        //identityToken is not valid
                        return false;

                    }
                }
                else
                {
                    //identityToken is not valid
                    return false;
                }
            }
            else
            {
                //Missing mod and expo values
                return false;
            }
        }

        private async Task<IEnumerable<Claim>> GetUserInfoClaimsAsync(string accessToken)
        {
            IEnumerable<Claim> userData = new List<Claim>();
            //Get UserInfo data when correct scope is set for SIWI and Get App now flows
            var userInfoClient = new UserInfoClient(AppController.userinfoEndpoint);
            
            UserInfoResponse userInfoResponse = await userInfoClient.GetAsync(accessToken);
            if (userInfoResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                //Read UserInfo Details
                userData = userInfoResponse.Json.ToClaims();

                foreach (Claim item in userData)
                {
                    if (item.Type == "sub" && item.Value != null)
                        sub = item.Value;
                    if (item.Type == "email" && item.Value != null)
                        email = item.Value;
                    if (item.Type == "emailVerified" && item.Value != null)
                        emailVerified = item.Value;
                    if (item.Type == "givenName" && item.Value != null)
                        givenName = item.Value;
                    if (item.Type == "familyName" && item.Value != null)
                        familyName = item.Value;
                    if (item.Type == "phoneNumber" && item.Value != null)
                        phoneNumber = item.Value;
                    if (item.Type == "phoneNumberVerified" && item.Value != null)
                        phoneNumberVerified = item.Value;

                    if (item.Type == "address" && item.Value != null)
                    {

                        Address jsonObject = JsonConvert.DeserializeObject<Address>(item.Value);

                        if (jsonObject.StreetAddress != null)
                            streetAddress = jsonObject.StreetAddress;
                        if (jsonObject.Locality != null)
                            locality = jsonObject.Locality;
                        if (jsonObject.Region != null)
                            region = jsonObject.Region;
                        if (jsonObject.PostalCode != null)
                            postalCode = jsonObject.PostalCode;
                        if (jsonObject.Country != null)
                            country = jsonObject.Country;
                    }

                }

            }
           
            return userData;
        }

        

        //private async Task<Tuple<string>> GetTempStateAsync()
        //{
        //    var data = HttpContext.Authentication;

        //    var state = data.Identity.FindFirst("state").Value;
        //    //  var state =  Tuple.Create("Steve" );

        //    return Tuple.Create(state);
        //}
	}
}