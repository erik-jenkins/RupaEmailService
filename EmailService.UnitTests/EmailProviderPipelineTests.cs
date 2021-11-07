using System;
using System.Threading.Tasks;
using EmailService.EmailProviders;
using EmailService.Models;
using Moq;
using Xunit;

namespace EmailService.UnitTests
{
    public class EmailProviderPipelineTests
    {
        [Fact]
        public async Task SendAsync_NoProviders_ThrowsException()
        {
            // arrange
            var emailProviderPipeline = new EmailProviderPipeline();

            // act & assert
            var email = GetTestingEmail();
            await Assert.ThrowsAsync<Exception>(async () => await emailProviderPipeline.SendAsync(email));
        }

        [Fact]
        public async Task SendAsync_OneEmailProvider_UsesOnlyEmailProvider()
        {
            // arrange
            var emailProviderPipeline = new EmailProviderPipeline();
            var provider = new Mock<IEmailProvider>();
            emailProviderPipeline.AddEmailProvider(provider.Object);

            // act
            var email = GetTestingEmail();
            await emailProviderPipeline.SendAsync(email);

            // assert
            provider.Verify(p => p.SendAsync(email), Times.Once);
        }

        [Fact]
        public async Task SendAsync_FirstOfTwoProvidersFails_UsesSecondProvider()
        {
            // arrange
            var pipeline = new EmailProviderPipeline();
            var failingProvider = new Mock<IEmailProvider>();
            failingProvider.Setup(p => p.SendAsync(It.IsAny<Email>())).Throws<Exception>();
            var backupProvider = new Mock<IEmailProvider>();

            pipeline.AddEmailProvider(failingProvider.Object);
            pipeline.AddEmailProvider(backupProvider.Object);

            // act
            var email = GetTestingEmail();
            await pipeline.SendAsync(email);

            // assert
            backupProvider.Verify(p => p.SendAsync(email), Times.Once);
        }

        private Email GetTestingEmail()
        {
            return new Email("test@example.com", "John Doe", "test2@example.com", "Jane Doe", "Testing", "This is a test!");
        }
    }
}
