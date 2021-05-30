using System.Threading.Tasks;
using UserManager.Application;

namespace UserManager.Data.InMemory
{
    public class InMemoryUserRepository : IUserRepository
    {
        public Task<string> GetById(string userId)
        {
            return Task.FromResult($"FakeUser_{userId}");
        }
    }
}