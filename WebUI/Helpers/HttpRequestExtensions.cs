using Microsoft.AspNetCore.Http;

namespace WebUI.Helpers;

public static class HttpRequestExtensions
{
    public static bool IsAjaxRequest(this HttpRequest request)
    {
        if (request == null) return false;
        return request.Headers != null && request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
}
