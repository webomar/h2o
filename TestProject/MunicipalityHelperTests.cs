using ApiService.Helpers;
using System.Security.Claims;
using Xunit;

namespace TestProject;

public class MunicipalityHelperTests
{
    [Fact]
    public void GetMunicipalityFromClaims_WithHattemRole_ReturnsHattem()
    {
        // Arrange
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "GG_Gebruiker_Hattem"));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = MunicipalityHelper.GetMunicipalityFromClaims(principal);

        // Assert
        Assert.Equal("HATTEM", result);
    }

    [Fact]
    public void GetMunicipalityFromClaims_WithOldebroekRole_ReturnsOldebroek()
    {
        // Arrange
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "GG_Gebruiker_Oldebroek"));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = MunicipalityHelper.GetMunicipalityFromClaims(principal);

        // Assert
        Assert.Equal("OLDEBROEK", result);
    }

    [Fact]
    public void GetMunicipalityFromClaims_WithHeerdeRole_ReturnsHeerde()
    {
        // Arrange
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "GG_Gebruiker_Heerde"));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = MunicipalityHelper.GetMunicipalityFromClaims(principal);

        // Assert
        Assert.Equal("HEERDE", result);
    }

    [Fact]
    public void GetMunicipalityFromClaims_WithNoMunicipalityRole_ReturnsNull()
    {
        // Arrange
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "SomeOtherRole"));
        identity.AddClaim(new Claim(ClaimTypes.Name, "TestUser"));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = MunicipalityHelper.GetMunicipalityFromClaims(principal);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetMunicipalityFromClaims_WithNoClaims_ReturnsNull()
    {
        // Arrange
        var identity = new ClaimsIdentity("Test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = MunicipalityHelper.GetMunicipalityFromClaims(principal);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetMunicipalityFromClaims_WithMultipleRoles_ReturnsFirstMunicipality()
    {
        // Arrange
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "GG_Gebruiker_Hattem"));
        identity.AddClaim(new Claim(ClaimTypes.Role, "GG_Gebruiker_Oldebroek"));
        identity.AddClaim(new Claim(ClaimTypes.Role, "SomeOtherRole"));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = MunicipalityHelper.GetMunicipalityFromClaims(principal);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("HATTEM", result);
    }

    [Fact]
    public void GetMunicipalityFromClaims_CaseInsensitive_ReturnsNull()
    {
        // Arrange
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "gg_gebruiker_hattem"));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = MunicipalityHelper.GetMunicipalityFromClaims(principal);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetMunicipalityFromClaims_WithMixedCase_ReturnsNull()
    {
        // Arrange
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "Gg_GeBrUiKeR_HeErDe"));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = MunicipalityHelper.GetMunicipalityFromClaims(principal);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetMunicipalityFromClaims_WithLowercaseMunicipality_ReturnsNull()
    {
        // Arrange
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "GG_Gebruiker_hattem"));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = MunicipalityHelper.GetMunicipalityFromClaims(principal);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetMunicipalityFromClaims_WithUppercaseMunicipality_ReturnsNull()
    {
        // Arrange
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "GG_Gebruiker_HATTEM"));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = MunicipalityHelper.GetMunicipalityFromClaims(principal);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetMunicipalityFromClaims_WithInvalidMunicipalityName_ReturnsNull()
    {
        // Arrange
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "GG_Gebruiker_Amsterdam"));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = MunicipalityHelper.GetMunicipalityFromClaims(principal);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void HasMunicipalityRole_WithMunicipalityRole_ReturnsTrue()
    {
        // Arrange
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "GG_Gebruiker_Hattem"));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = MunicipalityHelper.HasMunicipalityRole(principal);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasMunicipalityRole_WithNoMunicipalityRole_ReturnsFalse()
    {
        // Arrange
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "SomeOtherRole"));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = MunicipalityHelper.HasMunicipalityRole(principal);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasMunicipalityRole_WithNoClaims_ReturnsFalse()
    {
        // Arrange
        var identity = new ClaimsIdentity("Test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = MunicipalityHelper.HasMunicipalityRole(principal);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasMunicipalityRole_WithMultipleMunicipalityRoles_ReturnsTrue()
    {
        // Arrange
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.Role, "GG_Gebruiker_Hattem"));
        identity.AddClaim(new Claim(ClaimTypes.Role, "GG_Gebruiker_Oldebroek"));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = MunicipalityHelper.HasMunicipalityRole(principal);

        // Assert
        Assert.True(result);
    }
}

