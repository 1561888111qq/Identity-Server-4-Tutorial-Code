using System.Linq;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Restful.Api.Authorization
{
    public class MustHandler : AuthorizationHandler<MustRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            MustRequirement requirement)
        {
            var filterContext = context.Resource as AuthorizationFilterContext;
            if (filterContext == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var gender = context.User.Claims.FirstOrDefault(c => c.Type == "gender").Value;
            var nationality = context.User.Claims.FirstOrDefault(c => c.Type == "nationality").Value;

            if (gender == "female" && nationality == "China")
            {
                context.Succeed(requirement);
            }
            if (gender == "male" && nationality == "USA")
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}