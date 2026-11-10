namespace AIStudyLab.Api.Middleware;

public sealed class RequestOriginValidationMiddleware(
    RequestDelegate next,
    IReadOnlySet<string> allowedOrigins)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var isUnsafeMethod = HttpMethods.IsPost(method)
            || HttpMethods.IsPut(method)
            || HttpMethods.IsPatch(method)
            || HttpMethods.IsDelete(method);

        if (context.Request.Path.StartsWithSegments("/api")
            && isUnsafeMethod
            && (!context.Request.Headers.TryGetValue("Origin", out var origin)
                || !allowedOrigins.Contains(origin.ToString())
                || !context.Request.Headers.TryGetValue("X-AIStudyLab-Request", out var requestHeader)
                || requestHeader != "1"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "Origem da requisição não permitida.",
                status = StatusCodes.Status403Forbidden
            });
            return;
        }

        await next(context);
    }
}
