using FluentAssertions;
using TesteVsoft.Infrastructure.Common.EventDispatchers;
using TesteVsoft.Infrastructure.Tests.Fakes;

namespace TesteVsoft.Infrastructure.Tests.UnitTests.EventDispatchers;

[TestFixture]
public class HandlerResolverUnitTests
{
    private HandlerResolver _resolver = null!;

    [SetUp]
    public void Setup()
    {
        _resolver = new HandlerResolver();
    }

    [Test]
    public void Register_Command_Should_Store_Single_Handler()
    {
        // Act
        _resolver.Register(typeof(FakeCommand), typeof(FakeCommandHandler));
        var handler = _resolver.GetHandlerTypesForType(typeof(FakeCommand));

        // Assert
        handler.Should().Be(typeof(FakeCommandHandler));
    }

    [Test]
    public void Register_Notification_Should_Store_Multiple_Handlers()
    {
        // Act
        _resolver.Register(typeof(FakeNotification), typeof(FakeNotificationHandler));
        _resolver.Register(typeof(FakeNotification), typeof(FakeNotificationHandler2));

        var handlers = _resolver.GetHandlerTypesForNotificationType(typeof(FakeNotification));

        // Assert
        handlers.Should().Contain([typeof(FakeNotificationHandler), typeof(FakeNotificationHandler2)]);
    }

    [Test]
    public void GetHandlerTypesByNotificationTypeName_Should_Return_Handlers_By_Type_Name()
    {
        // Arrange
        _resolver.Register(typeof(FakeNotification), typeof(FakeNotificationHandler));

        // Act
        var result = _resolver.GetHandlerTypesByNotificationTypeName("FakeNotification");

        // Assert
        result.Should().ContainSingle().Which.Should().Be(typeof(FakeNotificationHandler));
    }

    [Test]
    public void GetHandlerTypesByTypeName_Should_Return_Single_Command_Handler_By_Name()
    {
        // Arrange
        _resolver.Register(typeof(FakeCommand), typeof(FakeCommandHandler));

        // Act
        var result = _resolver.GetHandlerTypesByTypeName("FakeCommand");

        // Assert
        result.Should().Be(typeof(FakeCommandHandler));
    }

    [Test]
    public void GetTypeByName_Should_Return_Exact_Type_From_Map()
    {
        // Arrange
        _resolver.Register(typeof(FakeNotification), typeof(FakeNotificationHandler));

        // Act
        var result = _resolver.GetTypeByName("FakeNotification");

        // Assert
        result.Should().Be(typeof(FakeNotification));
    }

    [Test]
    public void Register_Command_Twice_Should_Not_Add_Multiple_Handlers()
    {
        // Arrange
        _resolver.Register(typeof(FakeCommand), typeof(FakeCommandHandler));
        _resolver.Register(typeof(FakeCommand), typeof(FakeCommandHandler));

        // Act
        var result = _resolver.GetHandlerTypesForType(typeof(FakeCommand));

        // Assert
        result.Should().Be(typeof(FakeCommandHandler));
    }

    [Test]
    public void GetHandlerTypesByNotificationTypeName_Should_Return_Null_If_Not_Exists()
    {
        var result = _resolver.GetHandlerTypesByNotificationTypeName("UnknownNotification");
        result.Should().BeNull();
    }

    [Test]
    public void GetHandlerTypesByTypeName_Should_Return_Null_If_Not_Exists()
    {
        var result = _resolver.GetHandlerTypesByTypeName("UnknownCommand");
        result.Should().BeNull();
    }

    [Test]
    public void GetTypeByName_Should_Return_Null_If_Not_Exists()
    {
        var result = _resolver.GetTypeByName("SomeTypeThatDoesNotExist");
        result.Should().BeNull();
    }

    [Test]
    public void Register_Should_Be_Thread_Safe_Under_Concurrent_Access()
    {
        // Arrange
        var resolver = new HandlerResolver();
        var tasks = new List<Task>();
        var requestType = typeof(FakeNotification);
        var handlerType = typeof(FakeNotificationHandler);

        // Act
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                resolver.Register(requestType, handlerType);
            }));
        }

        Task.WaitAll([.. tasks]);

        //Assert
        var handlers = resolver.GetHandlerTypesForNotificationType(requestType);
        handlers.Should().ContainSingle().Which.Should().Be(handlerType);
    }

    [Test]
    public void Register_With_Null_Type_Should_Not_Throw()
    {
        var resolver = new HandlerResolver();
        Action act = () => resolver.Register(null!, typeof(FakeCommandHandler));

        act.Should().Throw<ArgumentNullException>();
    }
}
