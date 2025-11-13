using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AMIProjectView.Services
{
    public class TokenDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _context;

        public TokenDelegatingHandler(IHttpContextAccessor context)
        {
            _context = context;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _context.HttpContext?.Session?.GetString("ApiToken");
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
