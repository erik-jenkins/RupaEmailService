using System.Threading.Tasks;
using EmailService.Models;

namespace EmailService.EmailProviders
{
    public interface IEmailProvider
    {
        public Task SendAsync(Email email);
    }
}
