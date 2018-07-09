using Microsoft.AspNetCore.Authorization;

namespace Restful.Api.Authorization
{
    public class MustRequirement: IAuthorizationRequirement
    {
        public MustRequirement()
        {
            
        }
    }
}