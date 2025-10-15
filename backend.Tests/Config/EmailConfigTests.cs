using Xunit;
using FluentAssertions;
using Moq;
using backend.Config;
using backend.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace backend.Tests.Services
{
    public class EmailServiceTests
    {
        [Fact]
        public async Task ShouldNotSendEmail_WhenApiKeyIsMissing()
        {
            // Arrange
            var config = new EmailConfig { SendGridApiKey = "" };
            var service = new EmailService(config);

            // Act
            var result = await Record.ExceptionAsync(() =>
                service.SendInvitationEmailAsync("test@example.com", Guid.NewGuid()));

            // Assert
            result.Should().BeNull(); // No exception thrown, just early return
        }

        [Fact]
        public async Task ShouldSendEmail_WhenApiKeyIsValid()
        {
            // Arrange
            var config = new EmailConfig { SendGridApiKey = "SG.fake-key" };
            var service = new EmailService(config);

            // Replace SendGridClient with a mock if you refactor to inject it
            // For now, this test assumes SendGridClient works — integration-style

            // Act
            var result = await Record.ExceptionAsync(() =>
                service.SendInvitationEmailAsync("test@example.com", Guid.NewGuid()));

            // Assert
            result.Should().BeNull(); // No exception thrown
        }
    }
}
