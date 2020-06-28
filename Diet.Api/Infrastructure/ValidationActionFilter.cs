using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Diet.Api.Infrastructure
{
    public class ValidationActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid) return;

            var result = new ContentResult();
            var errors = context.ModelState.ToDictionary(
                valuePair => valuePair.Key,
                valuePair => valuePair.Value.Errors.Select(x => x.ErrorMessage).ToArray());
            var content = JsonConvert.SerializeObject(new { errors });
            result.Content = content;
            result.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int) HttpStatusCode.UnprocessableEntity;
            context.Result = result;
        }
    }
}