using Common.Lib.Exceptions.ErrorHandler;

namespace Common.Lib.ResponseHandler;

public static class ResponseGenerator
{
    public static ErrorList ErrorResponseMessage(string errorType, string message)
    {
        var apiResponse = new ErrorList {Errors = new List<ErrorDetails>()};
        apiResponse.Errors.Add(
            new ErrorDetails
            {
                Type = errorType,
                Description = message
            });
        return apiResponse;
    }
}