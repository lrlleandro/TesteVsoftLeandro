using Bogus;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using TesteVsoft.Domain.Entities;
using TesteVsoft.Infrastructure.Services;

namespace TesteVsoft.Tests.Unit.Services;

[TestFixture]
public partial class EmailTemplateServiceUnitTests
{
    private string _templatePath = null!;
    private string _templateContent = """
        <html>
        <body>
            <p>Olá {Name},</p>
            <p>Você recebeu uma nova tarefa: {Title}</p>
            <p>{Description}</p>
            <p>Data: {DueDate}</p>
            <p>Status: {Status}</p>
            <p>{Ics}</p>
            <footer>&copy; {Year}</footer>
        </body>
        </html>
        """;
    private Faker _faker;

    [SetUp]
    public void Setup()
    {
        var dir = Path.Combine(Directory.GetCurrentDirectory(), "..", "TesteVsoft.Infrastructure", "EmailsTemplating");
        Directory.CreateDirectory(dir);
        _templatePath = Path.Combine(dir, "TaskAssignedTemplate.html");

        File.WriteAllText(_templatePath, _templateContent);

        _faker = new Faker("pt_BR");
    }

    private User CreateUser(string? name = null, string? email = null)
    {
        return User.Create(string.IsNullOrEmpty(name) ? _faker.Name.FirstName() : name,
            _faker.Internet.UserName(),
            _faker.Internet.Password(),
            string.IsNullOrEmpty(email) ? _faker.Internet.Email() : email);
    }

    private UserTask CreateUserTask(User user)
    {
        return UserTask.Create(
            title: _faker.Lorem.Word(),
            description: _faker.Lorem.Paragraph(),
            dueDate: DateTime.UtcNow.AddDays(3),
            assignedUser: user
        );

    }

    [Test]
    public async Task GenerateAsync_ShouldReplacePlaceholdersCorrectly()
    {
        // Arrange
        var fakeIcs = new FakeIcsCalendarGenerator();
        var service = new EmailTemplateService(fakeIcs);
        var userTask = CreateUserTask(CreateUser());
        var status = userTask.Status.GetType()
                        .GetMember(userTask.Status.ToString())[0]
                        .GetCustomAttribute<DisplayAttribute>()?
                        .Name ?? userTask.Status.ToString();

        // Act
        var result = await service.GenerateAsync(userTask);

        // Assert
        result.Should().Contain($"<p>Olá {userTask.AssignedUser!.Name},</p>");
        result.Should().Contain($"<p>Você recebeu uma nova tarefa: {userTask.Title}</p>");
        result.Should().Contain($"<p>{userTask.Description}</p>");
        result.Should().Contain($"<p>Data: {userTask.DueDate:dd/MM/yyyy}</p>");
        result.Should().Contain($"<p>Status: {status}</p>");
        result.Should().Contain($"<p>https://calendar.google.com/calendar/render?action=TEMPLATE");
        result.Should().Contain($"&text={userTask.Title}");
        result.Should().Contain($"&dates=");
        result.Should().Contain($"/{userTask.DueDate:yyyyMMddTHHmmssZ}");
        result.Should().Contain($"&details={userTask.Description}");
        result.Should().Contain($"&status=CONFIRMED</p>");
        result.Should().Contain($"<footer>&copy; {DateTime.Now:yyyy}</footer>");
    }

    [TearDown]
    public void Cleanup()
    {
        if (File.Exists(_templatePath))
            File.Delete(_templatePath);
    }
}
