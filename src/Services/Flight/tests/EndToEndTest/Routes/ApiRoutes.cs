namespace EndToEnd.Test.Routes;

public class ApiRoutes
{
    public const string BaseApiPath = "api/v1.0";

    public static class Flight
    {
        public const string Id = "{id}";
        public const string GetFlightById = $"{BaseApiPath}/flight/{Id}";
        public const string CreateFlight = $"{BaseApiPath}/flight";
    }
}
