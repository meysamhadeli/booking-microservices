using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Exception;

using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

public class ProblemDetailsWithCode : ProblemDetails
{
    [JsonPropertyName("code")]
    public int? Code { get; set; }
}
