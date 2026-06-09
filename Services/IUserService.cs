using DoAnCoSo.Models.ViewModels;
using System.Threading.Tasks;

namespace DoAnCoSo.Services
{
    public interface IUserService
    {
        Task<UserSessionVM?> AuthenticateAsync(string username, string password);
    }
}