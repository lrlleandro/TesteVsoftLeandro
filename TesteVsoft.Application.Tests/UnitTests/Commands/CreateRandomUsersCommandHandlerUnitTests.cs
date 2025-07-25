using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TesteVsoft.Application.Commands;
using TesteVsoft.Application.Interfaces.BackgroundServices;
using TesteVsoft.Application.Interfaces.Repositories;
using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Tests.Commands;

[TestFixture]
public class CreateRandomUsersCommandHandlerTests
{
    private Mock<IBackgroundTaskQueue> _taskQueueMock = null!;
    private Mock<IUserRepository> _userRepoMock = null!;
    private CreateRandomUsersCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _taskQueueMock = new Mock<IBackgroundTaskQueue>();
        _userRepoMock = new Mock<IUserRepository>();
        _handler = new CreateRandomUsersCommandHandler(_taskQueueMock.Object, _userRepoMock.Object);
    }

    [Test]
    public async Task Handle_Should_Enqueue_Correct_Number_Of_Tasks()
    {
        // Arrange
        var command = new CreateRandomUsersCommand
        {
            Amount = 3,
            UserNameMask = "user_{{random}}"
        };

        var enqueued = new List<Func<IUserRepository, CancellationToken, Task>>();
        _taskQueueMock.Setup(q => q.EnqueueAsync(It.IsAny<Func<IUserRepository, CancellationToken, Task>>()))
                      .Callback<Func<IUserRepository, CancellationToken, Task>>(f => enqueued.Add(f))
                      .Returns(ValueTask.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        enqueued.Should().HaveCount(3);
        _taskQueueMock.Verify(q => q.EnqueueAsync(It.IsAny<Func<IUserRepository, CancellationToken, Task>>()), Times.Exactly(3));
    }

    [Test]
    public async Task Handle_Should_Throw_If_Amount_Is_Zero()
    {
        // Arrange
        var command = new CreateRandomUsersCommand
        {
            Amount = 0,
            UserNameMask = "user_{{random}}"
        };

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*maior que zero*");
    }

    [Test]
    public async Task Handle_Should_Throw_If_UserNameMask_Is_Empty()
    {
        // Arrange
        var command = new CreateRandomUsersCommand
        {
            Amount = 5,
            UserNameMask = ""
        };

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*obrigatória*");
    }
}
