using ProjectName.Application.Common.Errors;

namespace ProjectName.Web.Api.Common.Problems;

public static class ErrorExtension
{
    extension<IError>(IEnumerable<IError> errors){  

        /// <summary>
        /// Converts validation errors to a dictionary suitable for problem details.
        /// </summary>
        /// <param name="errors">The collection of errors to convert.</param>
        /// <returns>A dictionary mapping error identifiers to arrays of error messages.</returns>
        public Dictionary<string, string[]> ToProblemErrors()
        {
            Dictionary<string, string[]> problemErrors = [];

            foreach (IError error in errors)
            {
                if (error is ValidationError validationError)
                {
                    if (problemErrors.TryGetValue(validationError.Identifier, out string[]? value))
                    {
                        problemErrors[validationError.Identifier] = [.. value, validationError.Message];
                    }
                    else
                    {
                        problemErrors.Add(validationError.Identifier, [validationError.Message]);
                    }
                }
            }

            return problemErrors;
        }
    } 
}