using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Diet.Api;

public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch(Exception ex)
        {
            string s = ex.Message;
        }
    }
}