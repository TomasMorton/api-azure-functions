using System.Threading.Tasks;

namespace UserManager.Application
{
    public interface IUserRepository
    {
        Task<string> GetById(string userId);
    }
}