using DataTransferApi.Exceptions;
using System.Net;
using System.Web.Http.Filters;

public class FileExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(HttpActionExecutedContext context)
    {
        if (context.Exception is NotFoundException)
        {
            HandleNotFoundException(context);
        }
        else if (context.Exception is NotFullLoad)
        {
            HandleNotFullLoadException(context);
        }
        else if (context.Exception is TokenException)
        {
            HandleTokenException(context);
        }
        else
        {
            HandleOtherExceptions(context);
        }
    }

    private void HandleNotFoundException(HttpActionExecutedContext context)
    {
        var ex = context.Exception as NotFoundException;
        context.Response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent(ex.Message),
            ReasonPhrase = "Not Found"
        };
    }

    private void HandleNotFullLoadException(HttpActionExecutedContext context)
    {
        var ex = context.Exception as NotFullLoad;
        if (ex is NotFileFullLoad)
        {
            var fileEx = ex as NotFileFullLoad;
            context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(ex.Message),
                ReasonPhrase = "Bad Request"
            };
            
        }
        else
        {
            context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(ex.Message),
                ReasonPhrase = "Internal Server Error"
            };
        }
    }

    private void HandleTokenException(HttpActionExecutedContext context)
    {
        var ex = context.Exception as TokenException;
        if (ex is TokenTimeOutException)
        {
            context.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent(ex.Message),
                ReasonPhrase = "Token Error"
            };
        }
        else
        {
            context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(ex.Message),
                ReasonPhrase = "Internal Server Error"
            };
        }
    }

    private void HandleOtherExceptions(HttpActionExecutedContext context)
    {
        context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("An error occurred"),
            ReasonPhrase = "Internal Server Error"
        };
    }
}
