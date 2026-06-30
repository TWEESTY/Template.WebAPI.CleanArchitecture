using FluentResults;
using ProjectName.Application.Common.Errors;

namespace ProjectName.Web.Api.Common.Problems;

/// <summary>
/// Provides extension methods for converting FluentResults errors into a format suitable for problem details in the web API. 
/// This class includes methods to transform validation errors into a dictionary structure that can be used in HTTP responses, facilitating consistent error handling and reporting across the application.
/// </summary>
public static class ErrorExtension
{
    extension(IEnumerable<IError> errors)
    {

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
