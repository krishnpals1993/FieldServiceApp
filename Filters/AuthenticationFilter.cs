using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Data;

namespace LaCafelogy.Filters
{
    public class AuthenticationAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            String userId = context.HttpContext.Session.GetString("UserId");

            if (String.IsNullOrEmpty(userId))
            {
                RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();
                redirectTargetDictionary.Add("action", "Login");
                redirectTargetDictionary.Add("controller", "Account");

                context.Result = new RedirectToRouteResult(redirectTargetDictionary);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //To do : after the action executes
        }
    }
}