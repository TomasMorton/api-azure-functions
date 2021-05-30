using System.Threading.Tasks;
using UserManager.Application;

namespace UserManager.Data.InMemory
{
    public class InMemoryUserRepository : IUserRepository
    {
        public Task<UserDetails> GetById(string userId)
        {
            return Task.FromResult(new UserDetails(userId, $"FakeUser_{userId}"));
        }
    }
}