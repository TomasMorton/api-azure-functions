using System.Threading.Tasks;

namespace UserManager.Application
{
    public interface IUserRepository
    {
        Task<UserDetails> GetById(string userId);
    }
}