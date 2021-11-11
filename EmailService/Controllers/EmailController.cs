using System.Threading.Tasks;
using EmailService.Dtos;
using EmailService.EmailProviders;
using EmailService.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailProviderPipeline _emailProviderPipeline;

        public EmailController(IEmailProviderPipeline emailProviderPipeline)
        {
            _emailProviderPipeline = emailProviderPipeline;
        }

        /// <summary>
        /// Sends an email using the providers configured in appsettings.json.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
        {
            var email = (Email) request;
            await _emailProviderPipeline.SendAsync(email);
            return Ok();
        }
    }
}
