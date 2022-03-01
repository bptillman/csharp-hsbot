namespace Hsbot.Hosting.Web.Infrastructure
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    public static class SecurityHeadersExtension
    {
        public static IApplicationBuilder UseSecurityHeadersPolicy(this IApplicationBuilder builder) => builder.UseMiddleware<SecurityHeadersMiddleware>();
    }

    public interface ISecurityHeadersPolicy
    {
        public IDictionary<string, string> SetHeaders { get; set; }
        public ISet<string> RemoveHeaders { get; set; }
    }

    public class DefaultSecurityHeadersPolicy : ISecurityHeadersPolicy
    {
        public IDictionary<string, string> SetHeaders { get; set; }
        public ISet<string> RemoveHeaders { get; set; }

        public DefaultSecurityHeadersPolicy()
        {
            SetHeaders = new Dictionary<string, string>
                         {
                             {"X-Content-Type-Options", "nosniff"},
                             {"X-XSS-Protection", "1; mode=block"},
                             {"Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload"},
                             {"x-frame-options","SAMEORIGIN" },
                             {"Cache-Control", "public, max-age=3600"},
                             {"access-control-allow-credentials","true"},
                             {"access-control-allow-headers","Origin, X-Requested-With, Content-Type, Accept"},
                             {"access-control-allow-methods","GET, PUT, POST, DELETE, PATCH, OPTIONS"},
                             {"content-security-policy","default-src 'unsafe-inline' 'unsafe-eval' 'self' *.googleapis.com *.gstatic.com; script-src 'unsafe-inline' 'unsafe-eval' 'self' *.googleapis.com *.gstatic.com; img-src 'unsafe-inline' 'self' *.googleapis.com *.gstatic.com data:; font-src 'unsafe-inline' 'self' *.googleapis.com *.gstatic.com data:; connect-src 'unsafe-inline' 'self' *.googleapis.com *.gstatic.com; upgrade-insecure-requests; block-all-mixed-content"}
                         };

            RemoveHeaders = new HashSet<string>()
            {
            };
        }
    }

    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISecurityHeadersPolicy _policy;

        public SecurityHeadersMiddleware(RequestDelegate next, ISecurityHeadersPolicy policy)
        {
            _next = next;
            _policy = policy;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var headers = context.Response.Headers;

            foreach (var headerValuePair in _policy.SetHeaders)
                headers[headerValuePair.Key] = headerValuePair.Value;

            foreach (var header in _policy.RemoveHeaders)
                headers.Remove(header);

            await _next(context);
        }
    }
}
