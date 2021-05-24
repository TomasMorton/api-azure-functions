using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using Xunit;

namespace UserManager.UnitTests
{
    public class GetUserDetailsShould
    {
        private readonly Mock<FunctionContext> _context;

        public GetUserDetailsShould()
        {
            _context = new Mock<FunctionContext>(MockBehavior.Strict);
        }

        [Fact]
        public void ReturnAnOkResponseAsync()
        {
            var request = CreateRequest();
            var response = GetUserDetails.Run(request, _context.Object);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private HttpRequestData CreateRequest()
        {
            var response = new MockHttpResponseData(_context.Object);
            return CreateRequest(response);
        }

        private HttpRequestData CreateRequest(HttpResponseData response)
        {
            var request = new Mock<HttpRequestData>(MockBehavior.Strict, _context.Object);
            request.Setup(x => x.CreateResponse()).Returns(response);
            return request.Object;
        }
    }
}
