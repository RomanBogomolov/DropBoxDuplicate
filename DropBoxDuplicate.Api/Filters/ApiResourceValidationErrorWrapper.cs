using System.Collections.Generic;
using System.Linq;
using System.Web.Http.ModelBinding;

namespace DropBoxDuplicate.Api.Filters
{
    public class ApiResourceValidationErrorWrapper
    {
        private const string ErrorMessage = "Недопустимый запрос.";
        private const string MissingPropertyError = "Неизвестная ошибка.";

        public ApiResourceValidationErrorWrapper(ModelStateDictionary modelState)
        {
            Message = ErrorMessage;
            SerializeModelState(modelState);
        }

        public ApiResourceValidationErrorWrapper(string message, ModelStateDictionary modelState)
        {
            Message = message;
            SerializeModelState(modelState);
        }

        public string Message { get; private set; }
        public IDictionary<string, IEnumerable<string>> Errors { get; private set; }

        public void SerializeModelState(ModelStateDictionary modelState)
        {
            Errors = new Dictionary<string, IEnumerable<string>>();

            foreach (var keyModelStatePair in modelState)
            {
                var key = keyModelStatePair.Key;

                var errors = keyModelStatePair.Value.Errors;

                if (errors != null && errors.Count > 0)
                {
                    IEnumerable<string> errorMessage =
                        errors.Select(
                                error =>
                                    string.IsNullOrEmpty(error.ErrorMessage) ? MissingPropertyError : error.ErrorMessage)
                            .ToArray();

                    Errors.Add(key, errorMessage);
                }
            }
        }
    }
}