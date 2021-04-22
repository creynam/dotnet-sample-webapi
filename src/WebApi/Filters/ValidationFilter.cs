using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class ValidationFilter : IActionFilter, IOrderedFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public int Order { get; set; } = int.MaxValue - 20;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public void OnActionExecuting(ActionExecutingContext context) 
        {
            if (!context.ModelState.IsValid)
            {
                var errorsInModelState = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(x => x.ErrorMessage)).ToArray();

                Dictionary<string, string> errorResponse = new();

                List<string> propertyErrors = new List<string>();
                foreach (var error in errorsInModelState)
                {
                    foreach (var subError in error.Value)
                    {
                        propertyErrors.Add(subError);
                    }
                    errorResponse.Add(error.Key, string.Join(" ", propertyErrors));
                    propertyErrors.Clear();
                }

                var msj = "Validation error.";
                
                context.Result = new BadRequestObjectResult(new
                {
                    Title = msj,
                    Detail = errorResponse.Values
                });
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}