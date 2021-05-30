using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace UserManager
{
    public static class GetUserDetails
    {
        [Function("GetUserDetails")]
        public static HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var userId = GetUserId(req);
            var response = CreateResponse(req, userId);

            return response;
        }

        private static string GetUserId(HttpRequestData req)
        {
            var queryParams = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            return queryParams["id"] ?? throw new Exception("id parameter not provided");
        }

        private static HttpResponseData CreateResponse(HttpRequestData req, string userId)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString($"Welcome, {userId}, to Azure Functions!");
            return response;
        }
    }
}
