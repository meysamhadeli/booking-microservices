using System.Diagnostics;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.OpenTelemetryCollector.DiagnosticsProvider;

namespace BuildingBlocks.OpenTelemetryCollector.CoreDiagnostics.Query;

public class QueryHandlerActivity(IDiagnosticsProvider diagnosticsProvider)
{
    public async Task<TResult> Execute<TQuery, TResult>(
        Func<Activity?, CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken
    )
    {
        var queryName = typeof(TQuery).Name;
        var handlerType = typeof(TQuery)
            .Assembly.GetTypes()
            .FirstOrDefault(t =>
                t.GetInterfaces()
                    .Any(i =>
                        i.IsGenericType
                        && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)
                        && i.GetGenericArguments()[0] == typeof(TQuery)
                    )
            );
        var queryHandlerName = handlerType?.Name;

        // usually we use class/methodName
        var activityName = $"{ObservabilityConstant.Components.QueryHandler}.{queryHandlerName}/{queryName}";

        return await diagnosticsProvider.ExecuteActivityAsync(
            new CreateActivityInfo
            {
                Name = activityName,
                ActivityKind = ActivityKind.Consumer,
                Tags = new Dictionary<string, object?>
                {
                    { TelemetryTags.Tracing.Application.Queries.Query, queryName },
                    { TelemetryTags.Tracing.Application.Queries.QueryType, typeof(TQuery).FullName },
                    { TelemetryTags.Tracing.Application.Queries.QueryHandler, queryHandlerName },
                    { TelemetryTags.Tracing.Application.Queries.QueryHandlerType, handlerType?.FullName },
                },
            },
            action,
            cancellationToken
        );
    }
}