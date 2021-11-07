using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly ILogger<EmailController> _logger;

        public EmailController(
            ILogger<EmailController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail()
        {
            return Ok();
        }
    }
}
