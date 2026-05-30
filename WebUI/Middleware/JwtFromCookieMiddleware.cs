using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace WebUI.Middleware;

public class JwtFromCookieMiddleware
{
    private readonly RequestDelegate _next;

    public JwtFromCookieMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // If there's no Authorization header but there's an AuthToken cookie,
        // copy the cookie value into the Authorization header so JwtBearer
        // middleware can pick it up.
        if (!context.Request.Headers.ContainsKey("Authorization")
            && context.Request.Cookies.TryGetValue("AuthToken", out var token)
            && !string.IsNullOrWhiteSpace(token))
        {
            context.Request.Headers.Append("Authorization", $"Bearer {token}");
        }

        await _next(context);
    }

}

public static class JwtFromCookieMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtFromCookie(this IApplicationBuilder app)
        => app.UseMiddleware<JwtFromCookieMiddleware>();
}

