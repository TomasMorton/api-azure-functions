using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using UserManager.Application;
using Xunit;

namespace UserManager.UnitTests
{
    public class GetUserDetailsShould
    {
        private readonly Mock<FunctionContext> _context;
        private readonly Mock<IUserRepository> _userRepository;

        public GetUserDetailsShould()
        {
            _userRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            _context = new Mock<FunctionContext>(MockBehavior.Strict);
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
        public async Task ReturnAWelcomeMessage()
        {
            var request = CreateRequest("id=test");
            AllowRetrievingUserDetails();

            var response = await RunFunction(request);

            var responseText = ReadBody(response);
            Assert.Contains("Welcome", responseText);
        }

        [Fact]
        public async Task ReturnTheUsername()
        {
            UserDetails user = new("Bob the builder");
            _userRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(user);
            var request = CreateRequest("id=test");

            var response = await RunFunction(request);

            var responseText = ReadBody(response);
            Assert.Contains(user.UserName, responseText);
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
            return new GetUserDetails(_userRepository.Object).Run(request, _context.Object);
        }

        private void AllowRetrievingUserDetails()
        {
            var userDetails = new UserDetails("bob");
            _userRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(userDetails);
        }

        private HttpRequestData CreateRequest(string queryString)
        {
            var response = new MockHttpResponseData(_context.Object);
            return CreateRequest(queryString, response);
        }

        private HttpRequestData CreateRequest(string queryString, HttpResponseData response)
        {
            var request = new Mock<HttpRequestData>(MockBehavior.Strict, _context.Object);
            var uri = new Uri($"http://test.function.com/GetUserDetails?{queryString}");
            request.Setup(x => x.Url).Returns(uri);
            request.Setup(x => x.CreateResponse()).Returns(response);
            return request.Object;
        }

        private string ReadBody(HttpResponseData response)
        {
            response.Body.Position = 0;
            using var reader = new StreamReader(response.Body);
            return reader.ReadToEnd();
        }
    }
}