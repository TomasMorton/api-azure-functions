using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using UserManager.Application;

namespace UserManager
{
    public static class GetUserDetails
    {
        [Function("GetUserDetails")]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")]
            HttpRequestData req,
            IUserRepository userRepoObject,
            FunctionContext executionContext)
        {
            try
            {
                var userId = GetUserId(req);
                var username = await userRepoObject.GetById(userId); //TODO: return username
                var response = CreateResponse(req, userId);

                return response;
            }
            catch (ArgumentException)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        private static string GetUserId(HttpRequestData req)
        {
            var queryParams = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var id = queryParams["id"];
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id parameter not provided in query string", "id");

            return id;
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