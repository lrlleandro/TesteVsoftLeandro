using TesteVsoft.Application.Interfaces.CQRS;

namespace TesteVsoft.Infrastructure.Common.EventDispatchers;

public class HandlerResolver : IHandlerResolver
{
    private readonly Dictionary<Type, HashSet<Type>> _handlersMap = [];

    public void Register(Type requestType, Type handlerType)
    {
        var handlers = GetHandlers(requestType);

        if (IsNotificationType(requestType) || handlers.Count == 0)
        {
            handlers.Add(handlerType);
        }
    }

    public Type? GetHandlerTypesForType(Type requestType)
    {
        var handlers = GetHandlers(requestType);
        return handlers.FirstOrDefault();
    }

    public IEnumerable<Type>? GetHandlerTypesForNotificationType(Type requestType)
    {
        var handlers = GetHandlers(requestType);
        return handlers;
    }

    public IEnumerable<Type>? GetHandlerTypesByNotificationTypeName(string requestName)
    {
        var requestType = _handlersMap.Keys
            .FirstOrDefault(type => type.Name.Equals(requestName, StringComparison.OrdinalIgnoreCase));

        if (requestType == default)
        {
            return null;
        }

        var handlers = GetHandlers(requestType);
        return handlers;
    }

    public Type? GetHandlerTypesByTypeName(string requestName)
    {
        var requestType = _handlersMap.Keys
            .FirstOrDefault(type => type.Name.Equals(requestName, StringComparison.OrdinalIgnoreCase));

        if (requestType == default)
        {
            return null;
        }

        var handlers = GetHandlers(requestType);
        return handlers.FirstOrDefault();
    }

    public Type? GetTypeByName(string typeName)
    {
        return _handlersMap.Keys
            .FirstOrDefault(type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
    }

    private HashSet<Type> GetHandlers(Type requestType)
    {
        if (!_handlersMap.TryGetValue(requestType, out var handlers))
        {
            handlers = [];
            _handlersMap[requestType] = handlers;
        }

        return handlers;
    }

    private static bool IsNotificationType(Type requestType)
    {
        return typeof(INotification).IsAssignableFrom(requestType);
    }
}