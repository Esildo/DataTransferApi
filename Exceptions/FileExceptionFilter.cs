using DataTransferApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

public class ExceptionFilter : ExceptionFilterAttribute
{
    private readonly ILogger<ExceptionFilter> _logger;
    public ExceptionFilter(ILogger<ExceptionFilter> logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {

        base.OnException(context);
        var statusCode = HttpStatusCode.BadRequest;
        Error? error = null;
        switch (context.Exception)
        {
            case GroupNotFoundException ex:
                _logger.LogError(ex.Message + ex.FileName);
                error = new Error(ErrorCode.NotFound, ex.Message + " " + ex.FileName);
                break;
            case CustFileNotFoundException ex:
                _logger.LogError(ex.Message + ex.FileName);
                error = new Error(ErrorCode.NotFound, ex.Message +" " + ex.FileName);
                break;
            case NotFoundException ex:
                _logger.LogError(ex.Message);
                error = new Error(ErrorCode.NotFound, ex.Message);
                break;
            case System.IO.FileNotFoundException ex:
                _logger.LogError(ex.Message);
                error = new(ErrorCode.NotFound, ex.Message);
                break;
            case TokenTimeOutException ex:
                _logger.LogError(ex.Message);
                error = new(ErrorCode.NotFound, ex.Message);
                break;
            case TokenException ex:
                _logger.LogError(ex.Message);
                error = new(ErrorCode.NotFound, ex.Message);
                break;
            case NotFullLoad ex:
                _logger.LogError(ex.Message);
                error = new(ErrorCode.NotFound, ex.Message);
                break;
            case UnauthorizedAccessException ex:
                _logger.LogError(ex.Message);
                error = new(ErrorCode.Unauthorized, ex.Message);
                break;
            case Exception ex:
                _logger.LogError(ex.Message);
                error = new(ErrorCode.ServerError, ex.Message);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(context.Exception));
        }

        if (error == null) return;
        var result = new ObjectResult(error)
        {
            StatusCode = (int)error.ErrorCode
        };

        context.Result = result;
    }
}

public class Error
{
    public ErrorCode ErrorCode { get; }
    public string ErrorMessage { get; }
    public Error(ErrorCode code, string message)
    {
        ErrorCode = code;
        ErrorMessage = message;
    }
}

public enum ErrorCode
{
    NotFound = 400,
    ServerError = 500,
    Unauthorized = 401
}