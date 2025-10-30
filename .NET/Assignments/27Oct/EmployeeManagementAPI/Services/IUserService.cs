using EmployeeManagementAPI.Models;

namespace EmployeeManagementAPI.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
    }
}