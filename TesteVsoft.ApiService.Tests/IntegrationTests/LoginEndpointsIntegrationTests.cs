using FluentAssertions;
using System.Net.Http.Json;
using TesteVsoft.Application.Commands;
using TesteVsoft.Application.Dtos;

namespace TesteVsoft.Tests.Integration.Endpoints;

[TestFixture]
public class LoginEndpointsIntegrationTests
{

    [Test]
    public async Task PostLogin_ShouldReturnJwtTokenDto_WhenCredentialsAreValid()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.TesteVsoft_AppHost>();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        await using var app = await appHost.BuildAsync();
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync();

        // Act
        var httpClient = app.CreateHttpClient("api-service");
        await resourceNotificationService.WaitForResourceAsync("api-service", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var command = new LoginCommand("admin", "vsoft");
        var response = await httpClient.PostAsJsonAsync("/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<JwtTokenDto>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task PostLogin_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.TesteVsoft_AppHost>();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        await using var app = await appHost.BuildAsync();
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync();

        // Act
        var httpClient = app.CreateHttpClient("api-service");
        await resourceNotificationService.WaitForResourceAsync("api-service", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var command = new LoginCommand("admin", "wrong-password");
        var response = await httpClient.PostAsJsonAsync("/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }    
}
