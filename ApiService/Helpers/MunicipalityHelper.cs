using System.Security.Claims;

namespace ApiService.Helpers;

public static class MunicipalityHelper
{
    private static readonly HashSet<string> ValidMunicipalities = new()
    {
        "Hattem",
        "Oldebroek",
        "Heerde"
    };

    /// <summary>
    /// Haalt de gemeentenaam op uit de user claims op basis van de rol
    /// </summary>
    /// <param name="user">De ClaimsPrincipal van de gebruiker</param>
    /// <returns>De gemeentenaam (HATTEM, OLDEBROEK, of HEERDE) of null als geen gemeente rol gevonden wordt</returns>
    public static string? GetMunicipalityFromClaims(ClaimsPrincipal user)
    {
        var roleClaims = user.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        // Zoek naar gemeente rollen: GG_Gebruiker_Hattem, GG_Gebruiker_Oldebroek, GG_Gebruiker_Heerde
        // Moet exact matchen (case-sensitive)
        foreach (var role in roleClaims)
        {
            if (role.StartsWith("GG_Gebruiker_", StringComparison.Ordinal))
            {
                var municipality = role.Substring("GG_Gebruiker_".Length);
                
                // Controleer of het een geldige gemeente is (exacte case match vereist)
                if (!string.IsNullOrEmpty(municipality) && ValidMunicipalities.Contains(municipality))
                {
                    return municipality.ToUpperInvariant();
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Controleert of de gebruiker een gemeente rol heeft
    /// </summary>
    public static bool HasMunicipalityRole(ClaimsPrincipal user)
    {
        return GetMunicipalityFromClaims(user) != null;
    }
}

