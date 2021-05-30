using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using UserManager.Application;
using Xunit;

namespace UserManager.UnitTests.Tests
{
    public class GetUserDetailsShould
    {
        private readonly FunctionContext _context;
        private readonly Mock<IUserRepository> _userRepository;

        public GetUserDetailsShould()
        {
            _context = new FunctionContextCreator().GetContext();
            _userRepository = new Mock<IUserRepository>(MockBehavior.Strict);
        }

        [Fact]
        public async Task ReturnAnOkResponseAsync()
        {
            var request = CreateRequest("id=test");
            AllowRetrievingUserDetails();

            var response = await RunFunction(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ReturnTheNameProperty()
        {
            var user = new UserDetails("id", "Bob the builder");
            _userRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(user);
            var request = CreateRequest("id=test");

            var response = await RunFunction(request);

            var responseDetails = ReadBody<UserDetailsResponse>(response);
            Assert.Equal(user.UserName, responseDetails.name);
        }

        [Fact]
        public async Task ReturnTheIdProperty()
        {
            UserDetails user = new("id", "Bob the builder");
            _userRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(user);
            var request = CreateRequest("id=test");

            var response = await RunFunction(request);

            var responseDetails = ReadBody<UserDetailsResponse>(response);
            Assert.Equal(user.Id, responseDetails.id);
        }

        [Fact]
        public void QueryForTheUserDetails()
        {
            const string userId = "test-user-id";
            var request = CreateRequest($"id={userId}");
            AllowRetrievingUserDetails();

            RunFunction(request);

            _userRepository.Verify(x => x.GetById(userId), Times.Once);
        }

        [Fact]
        public async Task ReturnBadRequestWhenTheIdIsEmpty()
        {
            var request = CreateRequest($"id=");

            var response = await RunFunction(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ReturnBadRequestWhenTheIdParamIsNotProvided()
        {
            var request = CreateRequest(string.Empty);

            var response = await RunFunction(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private Task<HttpResponseData> RunFunction(HttpRequestData request)
        {
            return new GetUserDetails(_userRepository.Object).Run(request, _context);
        }

        private void AllowRetrievingUserDetails()
        {
            var userDetails = new UserDetails("id", "bob");
            _userRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(userDetails);
        }

        private HttpRequestData CreateRequest(string queryString)
        {
            var response = new MockHttpResponseData(_context);
            return CreateRequest(queryString, response);
        }

        private HttpRequestData CreateRequest(string queryString, HttpResponseData response)
        {
            var request = new Mock<HttpRequestData>(MockBehavior.Strict, _context);
            var uri = new Uri($"http://test.function.com/GetUserDetails?{queryString}");
            request.Setup(x => x.Url).Returns(uri);
            request.Setup(x => x.CreateResponse()).Returns(response);
            return request.Object;
        }

        private T ReadBody<T>(HttpResponseData response)
        {
            var body = ReadBody(response);
            return JsonSerializer.Deserialize<T>(body) ?? throw new Exception($"Failed to convert to json: {body}");
        }

        private string ReadBody(HttpResponseData response)
        {
            response.Body.Position = 0;
            using var reader = new StreamReader(response.Body);
            return reader.ReadToEnd();
        }
    }

    internal record UserDetailsResponse(string id, string name);
}