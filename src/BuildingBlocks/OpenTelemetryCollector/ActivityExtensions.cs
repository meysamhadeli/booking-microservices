using System.Diagnostics;
using System.Globalization;

namespace BuildingBlocks.OpenTelemetryCollector;

internal static class ActivityExtensions
{
    /// <summary>
    /// Retrieves the tags from the parent of the current Activity, if available.
    /// </summary>
    /// <param name="activity">The current Activity.</param>
    /// <returns>A dictionary containing the parent tags, or an empty dictionary if no parent tags are available.</returns>
    public static Dictionary<string, object?> GetParentTags(this Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);

        var parentTags = new Dictionary<string, object?>();

        // Check if the current activity has a parent
        var parentActivity = activity.Parent;

        if (parentActivity != null)
        {
            foreach (var tag in parentActivity.Tags)
            {
                parentTags[tag.Key] = tag.Value;
            }
        }
        else
        {
            // If no parent Activity is available, check for links
            foreach (var link in activity.Links)
            {
                // Extract tags from the first link's context (assuming it's the parent-like context)
                if (link.Tags != null)
                {
                    foreach (var tag in link.Tags)
                    {
                        parentTags[tag.Key] = tag.Value;
                    }
                }

                // Break after processing the first link, as there should only be one parent context.
                break;
            }
        }

        return parentTags;
    }

    /// <summary>
    /// Extracts important information from an Activity into an ActivityInfo object.
    /// </summary>
    /// <param name="activity">The Activity from which to extract information.</param>
    /// <returns>An ActivityInfo object containing the extracted information.</returns>
    public static ActivityInfo ExtractImportantInformation(this Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);

        var activityInfo = new ActivityInfo
        {
            Name = activity.DisplayName,
            StartTime = activity.StartTimeUtc,
            Duration = activity.Duration,
            Status =
                activity.Tags.FirstOrDefault(tag => tag.Key == TelemetryTags.Tracing.Otel.StatusCode).Value
                ?? "Unknown",
            StatusDescription = activity
                .Tags.FirstOrDefault(tag => tag.Key == TelemetryTags.Tracing.Otel.StatusDescription)
                .Value,
            Tags = activity.Tags.ToDictionary(tag => tag.Key, tag => tag.Value),
            Events = activity
                .Events.Select(e => new ActivityEventInfo
                {
                    Name = e.Name,
                    Timestamp = e.Timestamp,
                    Attributes = e.Tags.ToDictionary(tag => tag.Key, tag => tag.Value),
                })
                .ToList(),
            TraceId = activity.TraceId.ToString(),
            SpanId = activity.SpanId.ToString(),
        };

        return activityInfo;
    }

    /// <summary>
    /// Sets an "OK" status on the provided Activity, indicating a successful operation.
    /// </summary>
    /// <param name="activity">The Activity to update.</param>
    /// <param name="description">An optional description of the successful operation.</param>
    /// <returns>The updated Activity with the status and tags set.</returns>
    public static Activity SetOkStatus(this Activity activity, string? description = null)
    {
        ArgumentNullException.ThrowIfNull(activity);

        // Set the status of the activity to "OK"
        activity.SetStatus(ActivityStatusCode.Ok, description);

        // Add telemetry tags for status
        activity.SetTag(
            TelemetryTags.Tracing.Otel.StatusCode,
            nameof(ActivityStatusCode.Ok).ToUpper(CultureInfo.InvariantCulture)
        );
        if (!string.IsNullOrEmpty(description))
            activity.SetTag(TelemetryTags.Tracing.Otel.StatusDescription, description);

        return activity;
    }

    /// <summary>
    /// Sets an "Unset" status on the provided Activity, indicating no explicit status was applied.
    /// </summary>
    /// <param name="activity">The Activity to update.</param>
    /// <param name="description">An optional description of the unset status.</param>
    /// <returns>The updated Activity with the status and tags set.</returns>
    public static Activity SetUnsetStatus(this Activity activity, string? description = null)
    {
        ArgumentNullException.ThrowIfNull(activity);

        // Set the status of the activity to "Unset"
        activity.SetStatus(ActivityStatusCode.Unset, description);

        // Add telemetry tags for status
        activity.SetTag(
            TelemetryTags.Tracing.Otel.StatusCode,
            nameof(ActivityStatusCode.Unset).ToUpper(CultureInfo.InvariantCulture)
        );
        if (!string.IsNullOrEmpty(description))
            activity.SetTag(TelemetryTags.Tracing.Otel.StatusDescription, description);

        return activity;
    }

    /// <summary>
    /// Sets an "Error" status on the provided Activity, indicating a failed operation.
    /// </summary>
    /// <param name="activity">The Activity to update.</param>
    /// <param name="exception">The exception associated with the error, if available.</param>
    /// <param name="description">An optional description of the error.</param>
    /// <returns>The updated Activity with the status, error details, and tags set.</returns>
    public static Activity SetErrorStatus(this Activity activity, System.Exception? exception, string? description = null)
    {
        ArgumentNullException.ThrowIfNull(activity);

        // Add telemetry tags for status
        activity.SetTag(
            TelemetryTags.Tracing.Otel.StatusCode,
            nameof(ActivityStatusCode.Error).ToUpper(CultureInfo.InvariantCulture)
        );
        if (!string.IsNullOrEmpty(description))
            activity.SetTag(TelemetryTags.Tracing.Otel.StatusDescription, description);

        // Add detailed exception tags, if an exception is provided
        return activity.SetExceptionTags(exception);
    }

    // See https://opentelemetry.io/docs/specs/otel/trace/semantic_conventions/exceptions/
    public static Activity SetExceptionTags(this Activity activity, System.Exception? ex)
    {
        if (ex is null)
        {
            return activity;
        }

        activity.SetStatus(ActivityStatusCode.Error);
        activity.AddException(ex);

        activity.AddTag(TelemetryTags.Tracing.Exception.Message, ex.Message);
        activity.AddTag(TelemetryTags.Tracing.Exception.Stacktrace, ex.ToString());
        activity.AddTag(TelemetryTags.Tracing.Exception.Type, ex.GetType().FullName);

        return activity;
    }
}
