using System.Diagnostics;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.OpenTelemetryCollector.DiagnosticsProvider;

namespace BuildingBlocks.OpenTelemetryCollector.CoreDiagnostics.Commands;

public class CommandHandlerActivity(IDiagnosticsProvider diagnosticsProvider)
{
    public async Task Execute<TCommand>(
        Func<Activity?, CancellationToken, Task> action,
        CancellationToken cancellationToken
    )
    {
        var commandName = typeof(TCommand).Name;
        var handlerType = typeof(TCommand)
            .Assembly.GetTypes()
            .FirstOrDefault(t =>
                t.GetInterfaces()
                    .Any(i =>
                        i.IsGenericType
                        && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
                        && i.GetGenericArguments()[0] == typeof(TCommand)
                    )
            );
        var commandHandlerName = handlerType?.Name;

        // usually we use class/methodName
        var activityName = $"{ObservabilityConstant.Components.CommandHandler}.{commandHandlerName}/{commandName}";

        await diagnosticsProvider.ExecuteActivityAsync(
            new CreateActivityInfo
            {
                Name = activityName,
                ActivityKind = ActivityKind.Consumer,
                Tags = new Dictionary<string, object?>
                {
                    { TelemetryTags.Tracing.Application.Commands.Command, commandName },
                    { TelemetryTags.Tracing.Application.Commands.CommandType, typeof(TCommand).FullName },
                    { TelemetryTags.Tracing.Application.Commands.CommandHandler, commandHandlerName },
                    { TelemetryTags.Tracing.Application.Commands.CommandHandlerType, handlerType?.FullName },
                },
            },
            action,
            cancellationToken
        );
    }

    public async Task<TResult> Execute<TCommand, TResult>(
        Func<Activity?, CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken
    )
    {
        var commandName = typeof(TCommand).Name;
        var handlerType = typeof(TCommand)
            .Assembly.GetTypes()
            .FirstOrDefault(t =>
                t.GetInterfaces()
                    .Any(i =>
                        i.IsGenericType
                        && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
                        && i.GetGenericArguments()[0] == typeof(TCommand)
                    )
            );
        var commandHandlerName = handlerType?.Name;

        // usually we use class/methodName
        var activityName = $"{ObservabilityConstant.Components.CommandHandler}.{commandHandlerName}/{commandName}";

        return await diagnosticsProvider.ExecuteActivityAsync(
            new CreateActivityInfo
            {
                Name = activityName,
                ActivityKind = ActivityKind.Consumer,
                Tags = new Dictionary<string, object?>
                {
                    { TelemetryTags.Tracing.Application.Commands.Command, commandName },
                    { TelemetryTags.Tracing.Application.Commands.CommandType, typeof(TCommand).FullName },
                    { TelemetryTags.Tracing.Application.Commands.CommandHandler, commandHandlerName },
                    { TelemetryTags.Tracing.Application.Commands.CommandHandlerType, handlerType?.FullName },
                },
            },
            action,
            cancellationToken
        );
    }
}
