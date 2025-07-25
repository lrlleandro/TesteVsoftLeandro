using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TesteVsoft.Domain.Entities;
using TesteVsoft.Infrastructure.Security;

namespace TesteVsoft.Infrastructure.Tests.UnitTests.Secutity;

[TestFixture]
public class JwtTokenGeneratorUnitTests
{
    private IConfiguration _configuration;
    private JwtTokenGenerator _tokenGenerator;
    private Faker _faker;

    [SetUp]
    public void SetUp()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Jwt:Key", "lçejfçpolejmpoi3roi3qçmdçKNLHNIUknslkdnwjqNMOIHNiubkjbjIUJbjk"},
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" },
            { "Jwt:Expiration", "1" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _tokenGenerator = new JwtTokenGenerator(_configuration);
        _faker = new Faker("pt_BR");
    }

    private User CreateUser(string? password = null)
    {
        return User.Create(_faker.Name.FirstName(),
            _faker.Internet.UserName(),
            string.IsNullOrEmpty(password) ? _faker.Internet.Password() : password,
            _faker.Internet.Email());
    }

    [Test]
    public void GenerateToken_ShouldIncludeJtiClaim()
    {
        // Arrange
        var user = CreateUser();

        // Act
        var tokenDto = _tokenGenerator.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(tokenDto.AccessToken);

        // Assert
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
        tokenDto.UserId.Should().Be(user.Id);
        tokenDto.UserName.Should().Be(user.UserName);
    }



    [Test]
    public void GenerateToken_ShouldSetCorrectIssuerAudienceAndExpiration()
    {
        // Arrange
        var user = CreateUser();

        // Act
        var tokenDto = _tokenGenerator.GenerateToken(user);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenDto.AccessToken);

        // Assert
        jwt.Issuer.Should().Be("TestIssuer");
        jwt.Audiences.Should().Contain("TestAudience");
        jwt.ValidTo.Should().BeAfter(DateTime.UtcNow);
        tokenDto.UserId.Should().Be(user.Id);
         tokenDto.UserName.Should().Be(user.UserName);
    }
}
