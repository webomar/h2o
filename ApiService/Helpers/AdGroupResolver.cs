using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace ApiService.Helpers;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class AdGroupResolver : IAdGroupResolver
{
    public string? Resolve(string sid)
    {
        var sidObj = new SecurityIdentifier(sid);

        using var context = new PrincipalContext(ContextType.Domain);
        var group = GroupPrincipal.FindByIdentity(
            context,
            IdentityType.Sid,
            sidObj.Value
        );

        return group?.Name;
    }
}