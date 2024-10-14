using System.Security.Claims;
using Domain.Constants;
using Infrastructure.Interfaces;

namespace Presentation.Services
{
    public class CurrentApplicationUser(IHttpContextAccessor httpContextAccessor)
        : IApplicationUser
    {
        public Guid Id
        {
            get
            {
                var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Sid)?.Value;
                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    return userId;
                }

                return DefaultUser.DefaultUserId;
            }
        }
    }
}