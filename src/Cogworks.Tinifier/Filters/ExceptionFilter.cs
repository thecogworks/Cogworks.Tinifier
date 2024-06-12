namespace Cogworks.Tinifier.Filters;

public class ExceptionFilter : ExceptionFilterAttribute
{
  /// <summary>
  /// Custom exception filter
  /// </summary>
  /// <param name="context">HttpActionExecutedContext</param>
  public override void OnException(ExceptionContext context)
  {
    var httpContext = context.HttpContext;

    var ex = context.Exception;

    if (ex is EntityNotFoundException || ex is NotSupportedExtensionException ||
        ex is ConcurrentOptimizingException || ex is NotSuccessfullRequestException ||
        ex is HttpRequestException)
    {
      context.Result = new ObjectResult(new TNotification("Tinifier Oops", ex.Message, EventMessageType.Error)
      {
        Sticky = true,
      })
      {
        StatusCode = (int)HttpStatusCode.BadRequest
      };
    }
    else
    {
      context.Result = new ObjectResult(new TNotification("Tinifier unknown error", GetUnknownErrorMessage(ex), EventMessageType.Error)
      {
        Sticky = true,
        Url = "https://our.umbraco.org/projects/backoffice-extensions/tinifier/bugs/"
      })
      {
        StatusCode = (int)HttpStatusCode.InternalServerError
      };
    }
  }

  private string GetUnknownErrorMessage(Exception ex)
  {
    return $"{ex.Message}. We logged the error, if you are a hero, please take it and post in the forum (just click on this message)";
  }
}