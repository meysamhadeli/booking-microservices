using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.OpenTelemetryCollector.DiagnosticsProvider;

public class CustomeDiagnosticsProvider(IMeterFactory meterFactory, IOptions<ObservabilityOptions> options)
    : IDiagnosticsProvider
{
    private readonly Version? _version = Assembly.GetCallingAssembly().GetName().Version;
    private ActivitySource? _activitySource;
    private ActivityListener? _listener;
    private Meter? _meter;

    public string InstrumentationName { get; } = options.Value.InstrumentationName ?? throw new ArgumentException("InstrumentationName cannot be null or empty.");

    // https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-instrumentation-walkthroughs
    public ActivitySource ActivitySource
    {
        get
        {
            if (_activitySource != null)
                return _activitySource;

            _activitySource = new(InstrumentationName, _version?.ToString());

            _listener = new ActivityListener
            {
                ShouldListenTo = x => true,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            };
            ActivitySource.AddActivityListener(_listener);

            return _activitySource;
        }
    }

    // https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics-instrumentation
    public Meter Meter
    {
        get
        {
            if (_meter != null)
                return _meter;

            _meter = meterFactory.Create(InstrumentationName, _version?.ToString());

            return _meter;
        }
    }

    public async Task ExecuteActivityAsync(
        CreateActivityInfo createActivityInfo,
        Func<Activity?, CancellationToken, Task> action,
        CancellationToken cancellationToken = default
    )
    {
        if (!options.Value.TracingEnabled)
        {
            await action(null, cancellationToken);

            return;
        }

        using var activity =
            ActivitySource
                .CreateActivity(
                    name: $"{InstrumentationName}.{createActivityInfo.Name}",
                    kind: createActivityInfo.ActivityKind,
                    parentContext: createActivityInfo.Parent ?? default,
                    idFormat: ActivityIdFormat.W3C,
                    tags: createActivityInfo.Tags
                )
                ?.Start() ?? Activity.Current;

        try
        {
            await action(activity!, cancellationToken);
            activity?.SetOkStatus();
        }
        catch (System.Exception ex)
        {
            activity?.SetErrorStatus(ex);
            throw;
        }
    }

    public async Task<TResult?> ExecuteActivityAsync<TResult>(
        CreateActivityInfo createActivityInfo,
        Func<Activity?, CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken = default
    )
    {
        if (!options.Value.TracingEnabled)
        {
            return await action(null, cancellationToken);
        }

        using var activity =
            ActivitySource
                .CreateActivity(
                    name: $"{InstrumentationName}.{createActivityInfo.Name}",
                    kind: createActivityInfo.ActivityKind,
                    parentContext: createActivityInfo.Parent ?? default,
                    idFormat: ActivityIdFormat.W3C,
                    tags: createActivityInfo.Tags
                )
                ?.Start() ?? Activity.Current;

        try
        {
            var result = await action(activity!, cancellationToken);

            activity?.SetOkStatus();

            return result;
        }
        catch (System.Exception ex)
        {
            activity?.SetErrorStatus(ex);
            throw;
        }
    }

    public void Dispose()
    {
        _listener?.Dispose();
        _meter?.Dispose();
        _activitySource?.Dispose();
    }
}
