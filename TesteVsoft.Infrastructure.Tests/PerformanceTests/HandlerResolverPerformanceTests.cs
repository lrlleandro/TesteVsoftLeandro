using FluentAssertions;
using System.Diagnostics;
using TesteVsoft.Infrastructure.Common.EventDispatchers;
using TesteVsoft.Infrastructure.Tests.Fakes;

namespace TesteVsoft.Infrastructure.Tests.PerformanceTestes;

[TestFixture]
public class HandlerResolverPerformanceTests
{
    [Test]
    public void Register_MillionTypes_PerformsUnder150ms()
    {
        var resolver = new HandlerResolver();
        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < 1000000; i++)
        {
            var requestType = typeof(FakeCommand);
            var handlerType = typeof(FakeCommandHandler);
            resolver.Register(requestType, handlerType);
        }

        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(150);
    }

    [Test]
    public void Register_And_Resolve_Many_Should_Maintain_Integrity()
    {
        var resolver = new HandlerResolver();

        for (int i = 0; i < 1000000; i++)
        {
            var requestType = typeof(FakeQuery);
            var handlerType = typeof(FakeQueryHandler);
            resolver.Register(requestType, handlerType);

            var resolved = resolver.GetHandlerTypesForNotificationType(requestType);
            resolved.Should().Contain(handlerType);
        }
    }
}
