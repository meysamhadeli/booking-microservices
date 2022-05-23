using BuildingBlocks.Validation;

namespace BuildingBlocks.Exception
{
    public class ValidationException : CustomException
    {
        public ValidationException(ValidationResultModel validationResultModel)
        {
            ValidationResultModel = validationResultModel;
        }

        public ValidationResultModel ValidationResultModel { get; }
    }
}
