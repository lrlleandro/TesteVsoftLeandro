using Bogus;
using FluentAssertions;
using TesteVsoft.Domain.Entities;
using TesteVsoft.Infrastructure.Services;

namespace TesteVsoft.Infrastructure.Tests.UnitTests.Services;

[TestFixture]
public class IcsCalendarGeneratorUnitTests
{
    private Faker _faker;

    [SetUp]
    public async Task Setup()
    {
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
    public void GenerateICalendar_ShouldGenerateValidIcsContent()
    {
        // Arrange
        var userTask = CreateUserTask(CreateUser());
        var generator = new IcsCalendarGenerator();

        // Act
        var result = generator.GenerateICalendar(userTask);
        var ics = new string(result);

        // Assert
        ics.Should().Contain("BEGIN:VCALENDAR");
        ics.Should().Contain("VERSION:2.0");
        ics.Should().Contain("PRODID:-//Vsoft//TaskNotification//EN");
        ics.Should().Contain("CALSCALE:GREGORIAN");
        ics.Should().Contain("METHOD:PUBLISH");
        ics.Should().Contain("BEGIN:VEVENT");
        ics.Should().Contain($"UID:{userTask.Id}");
        ics.Should().Contain("DTSTAMP:"); // Timestamp gerado dinamicamente
        ics.Should().Contain($"DTSTART:");
        ics.Should().Contain($"DTEND:{userTask.DueDate.AddHours(1):yyyyMMddTHHmmssZ}");
        ics.Should().Contain($"SUMMARY:{userTask.Title}");
        ics.Should().Contain($"DESCRIPTION:{userTask.Description}");
        ics.Should().Contain("STATUS:CONFIRMED");
        ics.Should().Contain("END:VEVENT");
        ics.Should().Contain("END:VCALENDAR");
    }

    [Test]
    public void GenerateICalendar_ShouldReturnNonEmptyContent()
    {
        // Arrange
        var userTask = CreateUserTask(CreateUser());
        var generator = new IcsCalendarGenerator();

        // Act
        var result = generator.GenerateICalendar(userTask);

        // Assert
        result.Should().NotBeNullOrEmpty();
        new string(result).Length.Should().BeGreaterThan(100);
    }
}
