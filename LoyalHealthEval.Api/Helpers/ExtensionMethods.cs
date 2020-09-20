using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoyalHealthEval.Api.Helpers
{
    public static class ExtensionMethods
    {
        public static IActionResult HandleException(this Exception ex, HttpContext context)
        {
            //Uncomment when we are ready to start using AppInsights.
            //var parameter = new ExceptionParameter
            //{

            //};

            //AppInsightsExceptionTracker.TrackException(ex, parameter);

            return
                new ObjectResult(new
                {
                    statusCode = context.Response.StatusCode,
                    currentDate = DateTime.Now
                })
                {
                    StatusCode = context.Response.StatusCode,
                    Value = ex.Message
                };
        }
    }
}
