namespace Identity.Identity.Constants;

public static class Constants
{
    public static class Role
    {
        public const string Admin = "admin";
        public const string User = "user";
    }

    public static class StandardScopes
    {
        public const string Roles = "roles";
        public const string FlightApi = "flight-api";
        public const string PassengerApi = "passenger-api";
        public const string BookingApi = "booking-api";
        public const string IdentityApi = "identity-api";
    }
}
