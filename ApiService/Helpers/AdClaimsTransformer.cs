using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace ApiService.Helpers;

public sealed class AdClaimsTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;

        if (identity == null)
        {
            return Task.FromResult(principal);
        }

        foreach (var sidClaim in identity.FindAll(ClaimTypes.GroupSid))
        {
            var groupName = AdGroupResolver.Resolve(sidClaim.Value);
            
            if (groupName != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, groupName));
            }
        }
        
        return Task.FromResult(principal);
    }
}