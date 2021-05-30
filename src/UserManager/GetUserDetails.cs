using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using UserManager.Application;

namespace UserManager
{
    public class GetUserDetails
    {
        private readonly IUserRepository _userRepository;

        public GetUserDetails(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Function("GetUserDetails")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")]
            HttpRequestData req,
            FunctionContext executionContext)
        {
            try
            {
                var userId = GetUserId(req);
                var userDetails = await _userRepository.GetById(userId);
                var response = CreateResponse(req, userDetails);

                return response;
            }
            catch (ArgumentException)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        private static string GetUserId(HttpRequestData req)
        {
            var queryParams = HttpUtility.ParseQueryString(req.Url.Query);
            var id = queryParams["id"];
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id parameter not provided in query string", "id");

            return id;
        }

        private static HttpResponseData CreateResponse(HttpRequestData req, UserDetails userDetails)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString($"Welcome, {userDetails.UserName}, to Azure Functions!");
            return response;
        }
    }
}