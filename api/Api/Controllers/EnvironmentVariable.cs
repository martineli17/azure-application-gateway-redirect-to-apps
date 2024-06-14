using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Api.Controllers
{
    [ApiController]
    public class EnvironmentVariableController : ControllerBase
    {
        [HttpGet("environment-variable")]
       public IActionResult Get([FromServices] IConfiguration configuration)
        {
            var hostName = Dns.GetHostName();
            var addrress = Dns.GetHostAddresses(hostName)
                            .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            ?.MapToIPv4().ToString();
            var webAppName = configuration.GetValue<string>("WebAppName");
            return Ok(new { webAppName, addrress, hostName });
        }
    }
}
