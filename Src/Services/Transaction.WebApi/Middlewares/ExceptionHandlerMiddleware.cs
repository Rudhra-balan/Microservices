using Common.Lib.Extenstion;
using Newtonsoft.Json;
using Transaction.WebApi.Models;

namespace Transaction.WebApi.Middlewares
{
    public class ExceptionHandlerMiddleware : IMiddleware
    {
        private readonly Common.Lib.SerilogWrapper.Interface.ILogger _logger;

        public ExceptionHandlerMiddleware(Common.Lib.SerilogWrapper.Interface.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var message = CreateMessage(context, ex);
                _logger.Error(ex, message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception e)
        {
            var type = string.Empty;
            var result = new TransactionResultModel() { IsSuccessful = false, Message = e.Message };
            int statusCode;

            if (e is ArgumentException || e is ArgumentNullException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                type= e.GetType().Name;
            }
            else if (e is Framework.Exceptions.TransactionException transaction)
            {
                statusCode = StatusCodes.Status422UnprocessableEntity;
                type = transaction.Type;
            }
            else
            {
                statusCode = StatusCodes.Status500InternalServerError;
                var (exMessage, exType) = e.GetExceptionMessage();
                result.Message = exMessage;
                type = exType;
            }

            _logger.Error(e, e.Message);

            var errorMessage = ErrorResponseMessage(result.Message, type);
            var response = JsonConvert.SerializeObject(errorMessage, Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            context.Response.StatusCode = statusCode;
         
            await context.Response.WriteAsync(response);
        }

        private string CreateMessage(HttpContext context, Exception e)
        {
            var message = $"Exception caught in global error handler, exception message: {e.Message}, exception stack: {e.StackTrace}";

            if (e.InnerException != null)
            {
                message = $"{message}, inner exception message {e.InnerException.Message}, inner exception stack {e.InnerException.StackTrace}";
            }

            return $"{message} RequestId: {context.TraceIdentifier}";
        }

        public static ErrorList ErrorResponseMessage(string errorType, string message)
        {
            var apiResponse = new ErrorList { Errors = new List<ErrorDetails>() };
            apiResponse.Errors.Add(
                new ErrorDetails
                {
                    Type = errorType,
                    Description = message
                });
            return apiResponse;
        }
    }    
}
