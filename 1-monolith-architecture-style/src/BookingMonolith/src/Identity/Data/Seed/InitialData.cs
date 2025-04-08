using BookingMonolith.Identity.Identities.Models;
using MassTransit;

namespace BookingMonolith.Identity.Data.Seed;

public static class InitialData
{
    public static List<User> Users { get; }

    static InitialData()
    {
        Users = new List<User>
        {
            new User
            {
                Id = NewId.NextGuid(),
                FirstName = "Sam",
                LastName = "H",
                UserName = "samh",
                PassPortNumber = "12345678",
                Email = "sam@test.com",
                SecurityStamp = Guid.NewGuid().ToString()
            },
            new User
            {
                Id = NewId.NextGuid(),
                FirstName = "Sam2",
                LastName = "H2",
                UserName = "samh2",
                PassPortNumber = "87654321",
                Email = "sam2@test.com",
                SecurityStamp = Guid.NewGuid().ToString()
            }
        };
    }
}
