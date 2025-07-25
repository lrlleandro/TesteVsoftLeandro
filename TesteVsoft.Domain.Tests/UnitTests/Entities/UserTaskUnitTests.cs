using System;
using System.ComponentModel.DataAnnotations;
using Bogus;
using FluentAssertions;
using NUnit.Framework;
using TesteVsoft.Domain.Entities;
using TesteVsoft.Domain.Enums;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace TesteVsoft.Domain.Tests.UnitTests.Entities;

[TestFixture]
public class UserTaskUnitTests
{
    private Faker _faker;

    [SetUp]
    public void Setup()
    {
        _faker = new Faker("pt_BR");
    }

    private User GenerateUser()
    {
        return User.Create(_faker.Name.FirstName(), 
            _faker.Internet.UserName(),
            _faker.Internet.Password(),
            _faker.Internet.Email());
    }

    [Test]
    public void Create_ShouldInitializeCorrectly()
    {
        var title = _faker.Lorem.Sentence();
        var description = _faker.Lorem.Paragraph();
        var dueDate = DateTime.UtcNow.AddDays(1);
        var user = GenerateUser();

        var task = UserTask.Create(title, description, dueDate, user);

        task.Title.Should().Be(title);
        task.Description.Should().Be(description);
        task.DueDate.Should().BeCloseTo(dueDate, TimeSpan.FromSeconds(1));
        task.Status.Should().Be(UserTaskStatusTypes.Pending);
        task.AssignedUserId.Should().Be(user.Id);
        task.AssignedUser.Should().Be(user);
    }

    [Test]
    public void AssignToUser_ShouldAssignUser()
    {
        var task = new UserTask("Test", "Desc");
        var user = GenerateUser();

        task.AssignToUser(user);

        task.AssignedUser.Should().Be(user);
        task.AssignedUserId.Should().Be(user.Id);
    }

    [Test]
    public void AssignToUser_NullUser_ShouldThrow()
    {
        var task = new UserTask("Test", "Desc");

        Action act = () => task.AssignToUser(null!);

        act.Should().Throw<ValidationException>().WithMessage("Usuário não pode ser nulo");
    }

    [Test]
    public void ChangeStatus_ValidStatus_ShouldUpdate()
    {
        var task = new UserTask("Test", "Desc");

        task.ChangeStatus(UserTaskStatusTypes.InProgress);

        task.Status.Should().Be(UserTaskStatusTypes.InProgress);
    }

    [Test]
    public void ChangeStatus_SameStatus_ShouldNotUpdate()
    {
        var task = new UserTask("Test", "Desc");

        task.ChangeStatus(UserTaskStatusTypes.Pending);

        task.Status.Should().Be(UserTaskStatusTypes.Pending);
    }

    [Test]
    public void ChangeStatus_ToInvalidEnum_ShouldThrow()
    {
        var task = new UserTask("Test", "Desc");

        Action act = () => task.ChangeStatus((UserTaskStatusTypes)999);

        act.Should().Throw<ValidationException>().WithMessage("Status inválido");
    }

    [Test]
    public void ChangeStatus_WhenCompleted_ShouldThrow()
    {
        var task = new UserTask("Test", "Desc");
        task.ChangeStatus(UserTaskStatusTypes.Completed);

        Action act = () => task.ChangeStatus(UserTaskStatusTypes.Pending);

        act.Should().Throw<ValidationException>()
            .WithMessage("Tarefas concluídas não podem ser alteradas");
    }

    [Test]
    public void Update_ShouldChangeAllFields()
    {
        var task = new UserTask("Old Title", "Old Desc");
        var newTitle = _faker.Lorem.Sentence();
        var newDesc = _faker.Lorem.Paragraph();
        var dueDate = DateTime.UtcNow.AddDays(2);
        var status = UserTaskStatusTypes.InProgress;
        var user = GenerateUser();

        task.Update(newTitle, newDesc, dueDate, status, user);

        task.Title.Should().Be(newTitle);
        task.Description.Should().Be(newDesc);
        task.DueDate.Should().BeCloseTo(dueDate, TimeSpan.FromSeconds(1));
        task.Status.Should().Be(status);
        task.AssignedUser.Should().Be(user);
    }

    [Test]
    public void ChangeDueDate_WithPastDate_ShouldThrow()
    {
        var task = new UserTask("Test", "Desc");
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var user = GenerateUser();

        Action act = () => task.Update("title", "desc", pastDate, UserTaskStatusTypes.Pending, user);

        act.Should().Throw<ValidationException>()
           .WithMessage("A data de vencimento não deve ser menor que a data atual");
    }

    [Test]
    public void Constructor_ShouldSetDefaultValues()
    {
        var task = new UserTask("Title", "Desc");

        task.Status.Should().Be(UserTaskStatusTypes.Pending);
        task.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        task.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
