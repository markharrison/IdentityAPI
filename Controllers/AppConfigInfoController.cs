using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace IdentityAPI.Controllers;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class AppConfigInfoController : ControllerBase
{

    private readonly ILogger<AppConfigInfoController> _logger;
    private readonly IConfiguration _config;

    public AppConfigInfoController(ILogger<AppConfigInfoController> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    [HttpGet]
    public ContentResult GetPage()
    {
        string EchoData(string key, string value)
        {
            return key + ": <span style='color: blue'>" + value + "</span><br/>";
        }

        string EchoDataBull(string key, string value)
        {
            return EchoData("&nbsp;&bull;&nbsp;" + key, value);
        }

        string vBase = "";
        string vHTML = "";

        vHTML += "<h2>AppConfigInfo</h2>";
        vHTML += EchoData("OS Description", System.Runtime.InteropServices.RuntimeInformation.OSDescription);
        vHTML += EchoData("Framework Description", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
        vHTML += EchoData("ASPNETCORE_ENVIRONMENT", (_config.GetValue<string>("ASPNETCORE_ENVIRONMENT")) ?? "");
        vHTML += EchoData("ENVIRONMENT", (_config.GetValue<string>("ENVIRONMENT")) ?? "");
        vHTML += EchoData("BuildIdentifier", (_config.GetValue<string>("BuildIdentifier")) ?? "");

        if (_config.GetValue<string>("AdminPW") == HttpContext.Request.Query["pw"].ToString())
        {
            vHTML += EchoData("Instance", _config.GetValue<string>("BuildIdentifier") ?? "");
            vHTML += EchoData("Domain", _config.GetValue<string>("Domain") ?? "");
            vHTML += EchoData("TenantId", _config.GetValue<string>("TenantId") ?? "");
            vHTML += EchoData("ClientId", _config.GetValue<string>("ClientId") ?? "");
            vHTML += EchoData("ClientSecret", _config.GetValue<string>("ClientSecret") ?? "");
            vHTML += EchoData("CallbackPath", _config.GetValue<string>("CallbackPath") ?? "");

            vHTML += "RequestInfo: <br/>";
            vHTML += EchoDataBull("host", HttpContext.Request.Host.ToString());
            vHTML += EchoDataBull("ishttps", HttpContext.Request.IsHttps.ToString());
            vHTML += EchoDataBull("method", HttpContext.Request.Method.ToString());
            vHTML += EchoDataBull("path", HttpContext.Request.Path.ToString());
            vHTML += EchoDataBull("pathbase", HttpContext.Request.PathBase.ToString());
            vHTML += EchoDataBull("pathbase", HttpContext.Request.Protocol.ToString());
            vHTML += EchoDataBull("pathbase", HttpContext.Request.QueryString.ToString());
            vHTML += EchoDataBull("scheme", HttpContext.Request.Scheme.ToString());

            vHTML += "Headers: <br/>";
            foreach (var key in HttpContext.Request.Headers.Keys)
            {
                vHTML += EchoDataBull(key, $"{HttpContext.Request.Headers[key]}");
            }

            vHTML += "Connection:<br/>";
            vHTML += EchoDataBull("localipaddress", HttpContext.Connection.LocalIpAddress?.ToString() ?? "null");
            vHTML += EchoDataBull("localport", HttpContext.Connection.LocalPort.ToString());
            vHTML += EchoDataBull("remoteipaddress", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "null");
            vHTML += EchoDataBull("remoteport", HttpContext.Connection.RemotePort.ToString());

        }

        vBase += "<html lang='en' data-theme='light'><head>" +
             "<meta charset='utf-8'>" +
             "<meta name='viewport' content='width=device-width, initial-scale=1'>" +
             "<title>AppConfigInfo</title>" +
             "</head>" +
             "<body style='font-family: Segoe UI;'><main style='width=100%; padding-right:50px; padding-left:50px; margin-right: auto; margin-left: auto; '>" +
             vHTML +
             "</main></body></html>";

        return base.Content(vBase, "text/html");
    }

}
