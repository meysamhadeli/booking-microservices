namespace Identity.Extensions.Infrastructure;

using BuildingBlocks.Exception;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class ProblemDetailsExtensions
{
    public static WebApplication UseCustomProblemDetails(this WebApplication app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.ContentType = "application/problem+json";

                if (context.RequestServices.GetService<IProblemDetailsService>() is { } problemDetailsService)
                {
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exceptionType = exceptionHandlerFeature?.Error;

                    if (exceptionType is not null)
                    {
                        (string Detail, string Type, string Title, int StatusCode) details = exceptionType switch
                        {
                            ConflictException =>
                            (
                                exceptionType.Message,
                                exceptionType.GetType().ToString(),
                                exceptionType.GetType().Name,
                                context.Response.StatusCode = StatusCodes.Status409Conflict
                            ),
                            ValidationException validationException =>
                            (
                                exceptionType.Message,
                                exceptionType.GetType().ToString(),
                                exceptionType.GetType().Name,
                                context.Response.StatusCode = (int)validationException.StatusCode
                            ),
                            BadRequestException =>
                            (
                                exceptionType.Message,
                                exceptionType.GetType().ToString(),
                                exceptionType.GetType().Name,
                                context.Response.StatusCode = StatusCodes.Status400BadRequest
                            ),
                            NotFoundException =>
                            (
                                exceptionType.Message,
                                exceptionType.GetType().ToString(),
                                exceptionType.GetType().Name,
                                context.Response.StatusCode = StatusCodes.Status404NotFound
                            ),
                            AppException =>
                            (
                                exceptionType.Message,
                                exceptionType.GetType().ToString(),
                                exceptionType.GetType().Name,
                                context.Response.StatusCode = StatusCodes.Status400BadRequest
                            ),
                            DbUpdateConcurrencyException =>
                            (
                                exceptionType.Message,
                                exceptionType.GetType().ToString(),
                                exceptionType.GetType().Name,
                                context.Response.StatusCode = StatusCodes.Status409Conflict
                            ),
                            RpcException =>
                            (
                                exceptionType.Message,
                                exceptionType.GetType().ToString(),
                                exceptionType.GetType().Name,
                                context.Response.StatusCode = StatusCodes.Status400BadRequest
                            ),
                            _ =>
                            (
                                exceptionType.Message,
                                exceptionType.GetType().ToString(),
                                exceptionType.GetType().Name,
                                context.Response.StatusCode = StatusCodes.Status500InternalServerError
                            )
                        };

                        await problemDetailsService.WriteAsync(new ProblemDetailsContext
                        {
                            HttpContext = context,
                            ProblemDetails =
                            {
                                Title = details.Title,
                                Detail = details.Detail,
                                Type = details.Type,
                                Status = details.StatusCode
                            }
                        });
                    }
                }
            });
        });

        return app;
    }
}
