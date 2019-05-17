
using System.Collections.Generic;
using System.Threading.Tasks;
using University.Data.Models;

namespace University.Service.Users
{
    public interface IUserService
    {
        Task<User> GetDetailsById(int id);
        Task<User> GetDetailsByEmail(string email);
        Task<bool> ResetPassword(User user);
        Task<bool> Add(User user);
        Task<bool> Update(User user);
        Task<bool> Remove(User user);
    }
}
