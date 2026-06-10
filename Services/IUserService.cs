using DoAnCoSo.Models;
using DoAnCoSo.Models.ViewModels;
using System.Threading.Tasks;

namespace DoAnCoSo.Services
{
    public interface IUserService
    {
        Task<UserSessionVM?> AuthenticateAsync(string username, string password);
        Task<List<UserVM>> GetAllUsersAsync();
        Task<(bool IsSuccess, string Message)> CreateUserAsync(User model, string rawPassword);
    }
}