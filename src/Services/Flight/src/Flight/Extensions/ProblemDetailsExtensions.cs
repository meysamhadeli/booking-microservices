using System;
using BuildingBlocks.Exception;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Flight.Extensions;

public static class ProblemDetailsExtensions
{
    public static IServiceCollection AddCustomProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(x =>
        {
            // Control when an exception is included
            x.IncludeExceptionDetails = (ctx, _) =>
            {
                // Fetch services from HttpContext.RequestServices
                var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                return env.IsDevelopment() || env.IsStaging();
            };
            x.Map<ConflictException>(ex => new ProblemDetailsWithCode
            {
                Title = "Application rule broken",
                Status = StatusCodes.Status409Conflict,
                Detail = ex.Message,
                Type = "https://somedomain/application-rule-validation-error",
                Code = ex.Code
            });

            // Exception will produce and returns from our FluentValidation RequestValidationBehavior
            x.Map<ValidationException>(ex => new ProblemDetailsWithCode
            {
                Title = "input validation rules broken",
                Status = StatusCodes.Status400BadRequest,
                Detail = JsonConvert.SerializeObject(ex.ValidationResultModel.Errors),
                Type = "https://somedomain/input-validation-rules-error",
                Code = ex.Code
            });
            x.Map<BadRequestException>(ex => new ProblemDetailsWithCode
            {
                Title = "bad request exception",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message,
                Type = "https://somedomain/bad-request-error",
                Code = ex.Code
            });
            x.Map<NotFoundException>(ex => new ProblemDetailsWithCode
            {
                Title = "not found exception",
                Status = StatusCodes.Status404NotFound,
                Detail = ex.Message,
                Type = "https://somedomain/not-found-error",
                Code = ex.Code
            });
            x.Map<InternalServerException>(ex => new ProblemDetailsWithCode
            {
                Title = "api server exception",
                Status = StatusCodes.Status500InternalServerError,
                Detail = ex.Message,
                Type = "https://somedomain/api-server-error",
                Code = ex.Code
            });
            x.Map<AppException>(ex => new ProblemDetailsWithCode
            {
                Title = "application exception",
                Status = StatusCodes.Status500InternalServerError,
                Detail = ex.Message,
                Type = "https://somedomain/application-error",
                Code = ex.Code
            });
            x.Map<IdentityException>(ex =>
            {
                var pd = new ProblemDetailsWithCode
                {
                    Status = (int)ex.StatusCode,
                    Title = "identity exception",
                    Detail = ex.Message,
                    Type = "https://somedomain/identity-error",
                    Code = ex.Code
                };
                return pd;
            });
            x.MapToStatusCode<ArgumentNullException>(StatusCodes.Status400BadRequest);
        });
        return services;
    }
}
