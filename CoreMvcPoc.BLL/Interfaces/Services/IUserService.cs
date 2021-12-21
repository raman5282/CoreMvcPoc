using System;
using System.Threading.Tasks;
using ServiceManager;
using System.Net.Http;
using CoreMvcPoc.Entities;
namespace CoreMvcPoc.BLL
{
    public interface IUserService
    {
        ApiResponse SignUp(User model);
        User Login(string username, string password);
        bool UpdateUser(User model);
        bool CheckUserToken(int userId, string token);
        bool Logout (int id);
    }
}