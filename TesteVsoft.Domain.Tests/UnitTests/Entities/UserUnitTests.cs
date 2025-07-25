using Bogus;
using FluentAssertions;
using TesteVsoft.Domain.Entities;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace TesteVsoft.Domain.Tests.UnitTests.Entities;

[TestFixture]
public class UserUnitTests
{
    private Faker _faker;

    [SetUp]
    public void Setup()
    {
        _faker = new Faker("pt_BR");
    }

    [Test]
    public void EmptyConstructor_ShouldInializeDefaultProperties()
    {
        // Arrange & Act
        var user = Activator.CreateInstance(typeof(User), true) as User;

        // Assert
        user!.Name.Should().BeNull();
        user.UserName.Should().BeNull();
        user.Password.Should().BeNull();
        user.Password.Should().BeNull();
        user.Email.Should().BeNull();
        user.LastLoginOn.Should().BeNull();
    }

    [Test]
    public void Create_ValidUser_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = _faker.Name.FullName();
        var userName = _faker.Internet.UserName();
        var password = _faker.Internet.Password();
        var email = _faker.Internet.Email();

        // Act
        var user = User.Create(name, userName, password, email);

        // Assert
        user.Name.Should().Be(name);
        user.UserName.Should().Be(userName);
        user.Password.Should().NotBe(password);
        BCrypt.Net.BCrypt.Verify(password, user.Password).Should().BeTrue();
        user.Email!.Address.Should().Be(email);
        user.LastLoginOn.Should().BeNull();
    }

    [TestCase(null, "user", "pass", "email@email.com", "O nome é obrigatório.")]
    [TestCase("name", null, "pass", "email@email.com", "O nome de usuário é obrigatório.")]
    [TestCase("name", "user", null, "email@email.com", "A senha é obrigatória.")]
    [TestCase("name", "user", "pass", "emailinvalido", "O formato do e-mail é inválido")]
    public void Create_InvalidData_ShouldThrowValidationException(string? name, string? userName, string? password, string? email, string? expectedMessage)
    {
        // Arrange & Act
        Action act = () => User.Create(name!, userName!, password!, email!);

        // Assert
        act.Should().Throw<ValidationException>().WithMessage($"*{expectedMessage}*");
    }

    [Test]
    public void Create_WithEmptyEmail_ShouldSetEmailToNull()
    {
        // Arrange
        var name = _faker.Name.FullName();
        var userName = _faker.Internet.UserName();
        var password = _faker.Internet.Password();

        // Arrange
        var user = User.Create(name, userName, password, "");

        // Assert
        user.Email.Should().BeNull();
    }

    [Test]
    public void CreateRandom_ShouldCreateUserWithRandomValues()
    {
        // Arrange & Act
        var user = User.CreateRandom("user_{{random}}");

        // Assert
        user.Should().NotBeNull();
        user.UserName.Should().StartWith("user_");
        user.Name.Should().StartWith("User ");
        user.Email.Should().BeNull();
    }

    [Test]
    public void CreateRandom_ShouldThowWhenMaskPatternIsNotPresent()
    {
        // Arrange & Act
        var act = () => User.CreateRandom("user_");

        // Assert
        act.Should().Throw<ValidationException>()
           .WithMessage("A máscara deve conter o marcador {{random}} para gerar valores aleatórios.");
    }

    [Test]
    public void Login_WithCorrectPassword_ShouldUpdateLastLoginAndReturnTrue()
    {
        // Arrange
        var password = _faker.Internet.Password();
        var user = User.Create(_faker.Name.FullName(), _faker.Internet.UserName(), password, _faker.Internet.Email());

        // Act
        var result = user.Login(password);

        // Assert
        result.Should().BeTrue();
        user.LastLoginOn.Should().NotBeNull();
    }

    [Test]
    public void Login_WithIncorrectPassword_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var user = User.Create(_faker.Name.FullName(), _faker.Internet.UserName(), "Senha123", _faker.Internet.Email());

        // Act
        Action act = () => user.Login("senha_errada");

        // Assert
        act.Should().Throw<UnauthorizedAccessException>()
           .WithMessage("Nome de usuário ou senha incorretas.");

        user.LastLoginOn.Should().BeNull();
    }

    [Test]
    public void Update_ShouldUpdateAllFields_WhenValid()
    {
        // Arrange
        var oldPassword = "Senha123";
        var user = User.Create(_faker.Name.FullName(), _faker.Internet.UserName(), oldPassword, _faker.Internet.Email());

        var newName = _faker.Name.FirstName();
        var newUserName = _faker.Internet.UserName();
        var newPassword = _faker.Internet.Password();
        var newEmail = _faker.Internet.Email();

        // Act
        user.Update(newName, newUserName, oldPassword, newPassword, newEmail);

        // Assert
        user.Name.Should().Be(newName);
        user.UserName.Should().Be(newUserName);
        user.Email!.Address.Should().Be(newEmail);
        BCrypt.Net.BCrypt.Verify(newPassword, user.Password).Should().BeTrue();
    }

    [Test]
    public void Update_ShouldNotUpdatePassword_WhenOldPasswordIsInvalid()
    {
        // Arrange
        var user = User.Create(_faker.Name.FullName(), _faker.Internet.UserName(), "Senha123", _faker.Internet.Email());
        var newPassword = _faker.Internet.Password();

        // Act
        user.Update("NovoNome", "NovoUser", "senha_errada", newPassword, _faker.Internet.Email());

        // Assert
        BCrypt.Net.BCrypt.Verify(newPassword, user.Password).Should().BeFalse();
    }

    [Test]
    public void Update_ShouldSkipNullOrEmptyFields()
    {
        // Arrange
        var password = _faker.Internet.Password();

        // Act
        var user = User.Create(_faker.Name.FullName(), _faker.Internet.UserName(), password, _faker.Internet.Email());

        var originalName = user.Name;
        var originalUserName = user.UserName;
        var originalEmail = user.Email;

        // Act
        user.Update(null!, null!, null!, null!, null);

        // Assert
        user.Name.Should().Be(originalName);
        user.UserName.Should().Be(originalUserName);
        user.Email.Should().Be(originalEmail);
        BCrypt.Net.BCrypt.Verify(password, user.Password).Should().BeTrue();
    }

    [Test]
    public void GenerateRandomString_ShouldReturnCorrectLength()
    {
        var method = typeof(User)
            .GetMethod("GenerateRandomString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var result = method!.Invoke(null, new object[] { 10 }) as string;

        result.Should().NotBeNull();
        result!.Length.Should().Be(10);
    }
}
