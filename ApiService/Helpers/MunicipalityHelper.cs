using System.Security.Claims;

namespace ApiService.Helpers;

public static class MunicipalityHelper
{
    // Mapping tussen AD groepsnamen en MunicipalityId
    private static readonly Dictionary<string, int> GroupToMunicipalityId = new()
    {
        { "GG_Gebruiker_Hattem", 1 },
        { "GG_Gebruiker_Oldebroek", 2 },
        { "GG_Gebruiker_Heerde", 3 }
    };

    /// <summary>
    /// Haalt de MunicipalityId op uit de user claims op basis van de AD groep rol
    /// </summary>
    /// <param name="user">De ClaimsPrincipal van de gebruiker</param>
    /// <returns>De MunicipalityId (1, 2, of 3) of null als geen geldige groep rol gevonden wordt</returns>
    public static int? GetMunicipalityIdFromClaims(ClaimsPrincipal user)
    {
        var roleClaims = user.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        // Zoek naar gemeente rollen: GG_Gebruiker_Hattem, GG_Gebruiker_Oldebroek, GG_Gebruiker_Heerde
        // Case-sensitive matching (exacte match vereist)
        foreach (var role in roleClaims)
        {
            if (GroupToMunicipalityId.TryGetValue(role, out var municipalityId))
            {
                return municipalityId;
            }
        }

        return null;
    }

    /// <summary>
    /// Controleert of de gebruiker een geldige gemeente rol heeft
    /// </summary>
    public static bool HasMunicipalityRole(ClaimsPrincipal user)
    {
        return GetMunicipalityIdFromClaims(user) != null;
    }

    /// <summary>
    /// Legacy method - haalt de gemeentenaam op (voor backwards compatibility)
    /// </summary>
    [Obsolete("Use GetMunicipalityIdFromClaims instead")]
    public static string? GetMunicipalityFromClaims(ClaimsPrincipal user)
    {
        var municipalityId = GetMunicipalityIdFromClaims(user);
        return municipalityId switch
        {
            1 => "HATTEM",
            2 => "OLDEBROEK",
            3 => "HEERDE",
            _ => null
        };
    }
}

