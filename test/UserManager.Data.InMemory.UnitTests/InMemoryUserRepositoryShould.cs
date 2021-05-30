using System.Threading.Tasks;
using Xunit;

namespace UserManager.Data.InMemory.UnitTests
{
    public class InMemoryUserRepositoryShould
    {
        [Fact]
        public async Task ReturnFakeDataContainingTheId()
        {
            const string userId = "test123";
            var repository = new InMemoryUserRepository();

            var userDetails = await repository.GetById(userId);
            
            Assert.Equal($"FakeUser_{userId}", userDetails);
        }
    }
}
