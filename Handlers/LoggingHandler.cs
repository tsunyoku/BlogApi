namespace BlogApi.Handlers;

public class LoggingHandler(ILogger<LoggingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Sending request to {Path} ({Content})",
            request.RequestUri?.ToString(),
            await request.Content!.ReadAsStringAsync(cancellationToken));

        return await base.SendAsync(request, cancellationToken);
    }
}