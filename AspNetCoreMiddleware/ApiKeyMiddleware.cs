using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AspNetCoreMiddleware
{
    /// <summary>
    /// Middleware to check if api-key presents in header 
    /// </summary>
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// If request do not have api-key - block moving down in pipeline
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // Check if header contains api-key value
            if (!context.Request.Headers.ContainsKey("api-key"))
            {
                context.Response.StatusCode = 401; //UnAuthorized
               
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    message = "Invalid API Key"
                }));

                return;
            }

            // Call the next delegate/middleware in the pipeline
            await next(context);
        }
    }
}