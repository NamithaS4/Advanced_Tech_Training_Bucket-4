namespace CollegeAppAPI.Services
{
    public interface IUserService
    {
        string? Authenticate(string username, string password);
    }
}
