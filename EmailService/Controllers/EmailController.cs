using System.Threading.Tasks;
using EmailService.Dtos;
using EmailService.EmailProviders;
using EmailService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailProviderPipeline _emailProviderPipeline;
        private readonly ILogger<EmailController> _logger;

        public EmailController(
            IEmailProviderPipeline emailProviderPipeline,
            ILogger<EmailController> logger)
        {
            _emailProviderPipeline = emailProviderPipeline;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
        {
            var email = (Email) request;
            await _emailProviderPipeline.SendAsync(email);
            return Ok();
        }
    }
}
