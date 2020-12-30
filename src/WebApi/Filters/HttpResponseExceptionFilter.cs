using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Filters
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        //TODO: Inject logger

        public int Order { get; set; } = int.MaxValue - 10;


        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            switch (context.Exception)
            {
                case ArgumentNullException ex:
                    break;
                case ArgumentException ex:
                    context.Result = InvalidArguments(ex);
                    context.ExceptionHandled = true;
                    return;
                case KeyNotFoundException ex:
                    context.Result = KeyNotFound(ex);
                    context.ExceptionHandled = true;
                    return;
                default:
                    break;
            }
        }

        private IActionResult KeyNotFound(Exception ex)
        {
            var msj = "Not found.";
            return new NotFoundObjectResult(new
            {
                Title = msj,
                Detail = new List<string> { ex.Message }
            });
        }

        private IActionResult InvalidArguments(Exception ex)
        {
            var msj = "Invalid request arguments.";
            var errores = new List<string> { ex.Message };
            return new BadRequestObjectResult(new
            {
                Title = msj,
                Detail = errores
            });
        }
    }
}