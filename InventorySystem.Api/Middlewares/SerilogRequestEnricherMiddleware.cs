public class SerilogRequestEnricherMiddleware
{
    private readonly RequestDelegate _next;

    public SerilogRequestEnricherMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        using (Serilog.Context.LogContext.PushProperty("RequestId", context.TraceIdentifier))
        {
            await _next(context);
        }
    }
}
