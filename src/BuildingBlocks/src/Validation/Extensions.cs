using FluentValidation;
using ValidationException = Woo.Core.Exception.ValidationException;

namespace Woo.Validation
{
    public static class Extensions
    {
        /// <summary>
        /// Ref https://www.jerriepelser.com/blog/validation-response-aspnet-core-webapi
        /// </summary>
        public static async Task HandleValidationAsync<TRequest>(this IValidator<TRequest> validator, TRequest request)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors?.First()?.ErrorMessage);
            }
        }
    }
}