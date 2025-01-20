using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Application.Core
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var result = new ObjectResult(new { error = exception.Message })
            {
                StatusCode = exception is InvalidOperationException ? 400 : 500
            };

            context.Result = result;
            context.ExceptionHandled = true;
        }
    }
}
