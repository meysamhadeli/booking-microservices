using System.Diagnostics;
using System.Diagnostics.Metrics;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.OpenTelemetryCollector;
using BuildingBlocks.OpenTelemetryCollector.DiagnosticsProvider;

namespace BuildingBlocks.OpenTelemetryCollector.CoreDiagnostics.Commands;

public class CommandHandlerMetrics
{
    private readonly UpDownCounter<long> _activeCommandsCounter;
    private readonly Counter<long> _totalCommandsNumber;
    private readonly Counter<long> _successCommandsNumber;
    private readonly Counter<long> _failedCommandsNumber;
    private readonly Histogram<double> _handlerDuration;

    private Stopwatch _timer;

    public CommandHandlerMetrics(IDiagnosticsProvider diagnosticsProvider)
    {
        _activeCommandsCounter = diagnosticsProvider.Meter.CreateUpDownCounter<long>(
            TelemetryTags.Metrics.Application.Commands.ActiveCount,
            unit: "{active_commands}",
            description: "Number of commands currently being handled"
        );

        _totalCommandsNumber = diagnosticsProvider.Meter.CreateCounter<long>(
            TelemetryTags.Metrics.Application.Commands.TotalExecutedCount,
            unit: "{total_commands}",
            description: "Total number of executed command that sent to command handlers"
        );

        _successCommandsNumber = diagnosticsProvider.Meter.CreateCounter<long>(
            TelemetryTags.Metrics.Application.Commands.SuccessCount,
            unit: "{success_commands}",
            description: "Number commands that handled successfully"
        );

        _failedCommandsNumber = diagnosticsProvider.Meter.CreateCounter<long>(
            TelemetryTags.Metrics.Application.Commands.FaildCount,
            unit: "{failed_commands}",
            description: "Number commands that handled with errors"
        );

        _handlerDuration = diagnosticsProvider.Meter.CreateHistogram<double>(
            TelemetryTags.Metrics.Application.Commands.HandlerDuration,
            unit: "s",
            description: "Measures the duration of command handler"
        );
    }

    public void StartExecuting<TCommand>()
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

        var tags = new TagList
        {
            { TelemetryTags.Tracing.Application.Commands.Command, commandName },
            { TelemetryTags.Tracing.Application.Commands.CommandType, typeof(TCommand).FullName },
            { TelemetryTags.Tracing.Application.Commands.CommandHandler, commandHandlerName },
            { TelemetryTags.Tracing.Application.Commands.CommandHandlerType, handlerType?.FullName },
        };

        if (_activeCommandsCounter.Enabled)
        {
            _activeCommandsCounter.Add(1, tags);
        }

        if (_totalCommandsNumber.Enabled)
        {
            _totalCommandsNumber.Add(1, tags);
        }

        _timer = Stopwatch.StartNew();
    }

    public void FinishExecuting<TCommand>()
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

        var tags = new TagList
        {
            { TelemetryTags.Tracing.Application.Commands.Command, commandName },
            { TelemetryTags.Tracing.Application.Commands.CommandType, typeof(TCommand).FullName },
            { TelemetryTags.Tracing.Application.Commands.CommandHandler, commandHandlerName },
            { TelemetryTags.Tracing.Application.Commands.CommandHandlerType, handlerType?.FullName },
        };

        if (_activeCommandsCounter.Enabled)
        {
            _activeCommandsCounter.Add(-1, tags);
        }

        if (!_handlerDuration.Enabled)
            return;

        var elapsedTimeSeconds = _timer.Elapsed.Seconds;

        _handlerDuration.Record(elapsedTimeSeconds, tags);

        if (_successCommandsNumber.Enabled)
        {
            _successCommandsNumber.Add(1, tags);
        }
    }

    public void FailedCommand<TCommand>()
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

        var tags = new TagList
        {
            { TelemetryTags.Tracing.Application.Commands.Command, commandName },
            { TelemetryTags.Tracing.Application.Commands.CommandType, typeof(TCommand).FullName },
            { TelemetryTags.Tracing.Application.Commands.CommandHandler, commandHandlerName },
            { TelemetryTags.Tracing.Application.Commands.CommandHandlerType, handlerType?.FullName },
        };

        if (_failedCommandsNumber.Enabled)
        {
            _failedCommandsNumber.Add(1, tags);
        }
    }
}
