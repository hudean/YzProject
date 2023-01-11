using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace YzProject.WebMVC.Middlewares
{
    public class MyMiddleware : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            throw new System.NotImplementedException();
        }
    }


    public class TestMyMiddleware
    {

        private RequestDelegate _next;

        public TestMyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            throw new System.NotImplementedException();
        }

    }
}
