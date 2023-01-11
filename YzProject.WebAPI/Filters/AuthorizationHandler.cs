using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace YzProject.WebAPI.Filters
{
    public class AuthorizationHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
