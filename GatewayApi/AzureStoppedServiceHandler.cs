using System.Net;
using System.Text;

namespace GatewayApi;

public sealed class AzureStoppedServiceHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Forbidden &&
            string.Equals(response.Content.Headers.ContentType?.MediaType, "text/html", StringComparison.OrdinalIgnoreCase))
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (body.Contains("Web App - Unavailable", StringComparison.OrdinalIgnoreCase))
            {
                response.Dispose();
                return new HttpResponseMessage(HttpStatusCode.BadGateway)
                {
                    RequestMessage = request,
                    Content = new StringContent(
                        """{"message":"El microservicio downstream no está disponible (detenido en Azure)."}""",
                        Encoding.UTF8,
                        "application/json")
                };
            }

            response.Content = new StringContent(body, Encoding.UTF8, "text/html");
        }

        return response;
    }
}
