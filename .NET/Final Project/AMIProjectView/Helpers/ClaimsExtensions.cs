using System.Security.Claims;

namespace AMIProjectView.Helpers
{
    public static class ClaimsExtensions
    {
        public static int? GetConsumerId(this ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type == "ConsumerId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        public static bool IsUser(this ClaimsPrincipal user)
        {
            return user.HasClaim(c => c.Type == "UserType" && c.Value == "User");
        }
    }
}
