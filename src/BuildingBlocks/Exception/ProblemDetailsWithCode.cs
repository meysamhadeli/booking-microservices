using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Exception;

public class ProblemDetailsWithCode : ProblemDetails
{
    [JsonPropertyName("code")]
    public int? Code { get; set; }
}
