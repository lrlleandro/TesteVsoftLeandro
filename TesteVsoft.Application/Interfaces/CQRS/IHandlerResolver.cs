namespace TesteVsoft.Application.Interfaces.CQRS;

public interface IHandlerResolver
{
    IEnumerable<Type>? GetHandlerTypesByNotificationTypeName(string requestName);
    Type? GetHandlerTypesByTypeName(string requestName);
    IEnumerable<Type>? GetHandlerTypesForNotificationType(Type requestType);
    Type? GetHandlerTypesForType(Type requestType);
    Type? GetTypeByName(string typeName);
    void Register(Type requestType, Type handlerType);
}