using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace ApiService.Helpers;

public sealed class AdClaimsTransformer : IClaimsTransformation
{
    private readonly IAdGroupResolver _groupResolver;

    public AdClaimsTransformer(IAdGroupResolver groupResolver)
    {
        _groupResolver = groupResolver;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;

        if (identity == null)
        {
            return Task.FromResult(principal);
        }

        // Collect all SIDs first to avoid modifying collection during enumeration
        var sidClaims = identity.FindAll(ClaimTypes.GroupSid).ToList();
        
        foreach (var sidClaim in sidClaims)
        {
            var groupName = _groupResolver.Resolve(sidClaim.Value);
            
            if (groupName != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, groupName));
            }
        }
        
        return Task.FromResult(principal);
    }
}