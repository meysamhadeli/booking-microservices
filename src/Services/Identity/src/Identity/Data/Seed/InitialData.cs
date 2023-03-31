namespace Identity.Data.Seed;

using System;
using System.Collections.Generic;
using Identity.Models;
using MassTransit;

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
                PassPortNumber = "123456789",
                Email = "sam@test.com",
                SecurityStamp = Guid.NewGuid().ToString()
            },
            new User
            {
                Id = NewId.NextGuid(),
                FirstName = "Sam2",
                LastName = "H2",
                UserName = "samh2",
                PassPortNumber = "987654321",
                Email = "sam2@test.com",
                SecurityStamp = Guid.NewGuid().ToString()
            }
        };
    }
}
