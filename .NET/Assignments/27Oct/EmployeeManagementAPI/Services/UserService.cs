using EmployeeManagementAPI.Models;

namespace EmployeeManagementAPI.Services
{
    public class UserService : IUserService
    {
        private readonly EmployeeDbContext _context;

        public UserService(EmployeeDbContext context)
        {
            _context = context;
        }

        public User Authenticate(string username, string password)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == username && u.Password == password);
        }
    }
}