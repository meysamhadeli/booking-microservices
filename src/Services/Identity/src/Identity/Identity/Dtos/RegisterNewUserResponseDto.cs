using System;

namespace Identity.Identity.Dtos;

public record RegisterNewUserResponseDto
{
    public long Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Username { get; init; }

    public string PassportNumber { get; set; }
}
