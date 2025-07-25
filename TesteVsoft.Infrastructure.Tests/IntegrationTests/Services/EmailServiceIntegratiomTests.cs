using Microsoft.Extensions.Configuration;
using TesteVsoft.Application.Interfaces.Services;

namespace TesteVsoft.Tests.Integration;

[TestFixture]
public class EmailServiceIntegratiomTests
{
    private IEmailService _emailService = null!;

    [SetUp]
    public void Setup()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "MailSettings:Host", "sandbox.smtp.mailtrap.io" },
            { "MailSettings:Port", "587" },
            { "MailSettings:From", "no-reply@demomailtrap.co" },
            { "MailSettings:User", "e09907b53bb33a" },
            { "MailSettings:Password", "95ff2bb142367b" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _emailService = new EmailService(configuration);
    }

    [Test]
    public async Task SendAsync_ShouldSendEmailWithoutExceptions()
    {
        // Arrange
        var to = "teste@email.com";
        var subject = "Test Subject";
        var htmlBody = "<h1>Hello from integration test</h1>";

        // Act & Assert
        Assert.DoesNotThrowAsync(async () =>
        {
            await _emailService.SendAsync(to, subject, htmlBody);
        });
    }
}
