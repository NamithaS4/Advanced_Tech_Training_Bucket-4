using System.Security.Claims;

namespace AMIProjectAPI.Services
{
    public interface ITokenService
    {
        string CreateToken(IEnumerable<Claim> claims);
    }
}