using Diet.Api.Infrastructure.ExceptionHandling;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Diet.Api.Controllers
{
    [Route("Error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpDelete]
        public IActionResult Get([FromServices]IWebHostEnvironment environment, [FromServices] IHttpContextAccessor httpContextAccessor)
        {
            var exceptionHandlerFeature = httpContextAccessor.HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (exceptionHandlerFeature == null)
            {
                return new ForbidResult();
            }

            return exceptionHandlerFeature.Error switch
            {
                RestException rest => Problem(rest.Detail, rest.Instance, (int) rest.HttpStatus, rest.Title, rest.Type),
                { } ex => (environment.IsDevelopment() ? Problem(title: ex.Message, detail: ex.StackTrace) : Problem()),
                _ => Problem()
            };
        }
    }
}