using System.Threading.Tasks;
using Xunit;

namespace UserManager.Data.InMemory.UnitTests
{
    public class InMemoryUserRepositoryShould
    {
        private readonly InMemoryUserRepository _repository;

        public InMemoryUserRepositoryShould()
        {
            _repository = new InMemoryUserRepository();
        }

        [Fact]
        public async Task ReturnTheUserId()
        {
            const string userId = "test123";

            var userDetails = await _repository.GetById(userId);

            Assert.Equal(userId, userDetails.Id);
        }

        [Fact]
        public async Task ReturnAFakeUsernameContainingTheId()
        {
            const string userId = "test123";

            var userDetails = await _repository.GetById(userId);

            Assert.Equal($"FakeUser_{userId}", userDetails.UserName);
        }
    }
}