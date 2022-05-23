using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Exception;

public class ProblemDetailsWithCode : ProblemDetails
{
    public string Code { get; set; }
}
