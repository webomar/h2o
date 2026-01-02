using ApiService.Helpers;
using System.Security.Claims;
using Moq;
using Xunit;

namespace TestProject;

public class AdClaimsTransformerTests
{
    [Fact]
    public async Task TransformAsync_WithNullIdentity_ReturnsPrincipalUnchanged()
    {
        // Arrange
        var mockResolver = new Mock<IAdGroupResolver>();
        var transformer = new AdClaimsTransformer(mockResolver.Object);
        var principal = new ClaimsPrincipal();

        // Act
        var result = await transformer.TransformAsync(principal);

        // Assert
        Assert.Same(principal, result);
        mockResolver.Verify(x => x.Resolve(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task TransformAsync_WithNoGroupSidClaims_DoesNotAddRoleClaims()
    {
        // Arrange
        var mockResolver = new Mock<IAdGroupResolver>();
        var transformer = new AdClaimsTransformer(mockResolver.Object);
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.Name, "TestUser"));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = await transformer.TransformAsync(principal);

        // Assert
        Assert.Same(principal, result);
        var roleClaims = result.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        Assert.Empty(roleClaims);
        mockResolver.Verify(x => x.Resolve(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task TransformAsync_WithGroupSidThatResolvesToGroupName_AddsRoleClaim()
    {
        // Arrange
        var mockResolver = new Mock<IAdGroupResolver>();
        var groupSid = "S-1-5-21-1234567890-1234567890-1234567890-1234";
        var groupName = "TestGroup";
        
        mockResolver.Setup(x => x.Resolve(groupSid)).Returns(groupName);
        
        var transformer = new AdClaimsTransformer(mockResolver.Object);
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.GroupSid, groupSid));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = await transformer.TransformAsync(principal);

        // Assert
        Assert.Same(principal, result);
        var roleClaims = result.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        Assert.Single(roleClaims);
        Assert.Equal(groupName, roleClaims[0].Value);
        mockResolver.Verify(x => x.Resolve(groupSid), Times.Once);
    }

    [Fact]
    public async Task TransformAsync_WithGroupSidThatResolvesToNull_DoesNotAddRoleClaim()
    {
        // Arrange
        var mockResolver = new Mock<IAdGroupResolver>();
        var groupSid = "S-1-5-21-1234567890-1234567890-1234567890-1234";
        
        mockResolver.Setup(x => x.Resolve(groupSid)).Returns((string?)null);
        
        var transformer = new AdClaimsTransformer(mockResolver.Object);
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.GroupSid, groupSid));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = await transformer.TransformAsync(principal);

        // Assert
        Assert.Same(principal, result);
        var roleClaims = result.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        Assert.Empty(roleClaims);
        mockResolver.Verify(x => x.Resolve(groupSid), Times.Once);
    }

    [Fact]
    public async Task TransformAsync_WithMultipleGroupSids_AddsMultipleRoleClaims()
    {
        // Arrange
        var mockResolver = new Mock<IAdGroupResolver>();
        var groupSid1 = "S-1-5-21-1234567890-1234567890-1234567890-1234";
        var groupSid2 = "S-1-5-21-1234567890-1234567890-1234567890-1235";
        var groupName1 = "TestGroup1";
        var groupName2 = "TestGroup2";
        
        mockResolver.Setup(x => x.Resolve(groupSid1)).Returns(groupName1);
        mockResolver.Setup(x => x.Resolve(groupSid2)).Returns(groupName2);
        
        var transformer = new AdClaimsTransformer(mockResolver.Object);
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.GroupSid, groupSid1));
        identity.AddClaim(new Claim(ClaimTypes.GroupSid, groupSid2));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = await transformer.TransformAsync(principal);

        // Assert
        Assert.Same(principal, result);
        var roleClaims = result.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        Assert.Equal(2, roleClaims.Count);
        Assert.Contains(roleClaims, c => c.Value == groupName1);
        Assert.Contains(roleClaims, c => c.Value == groupName2);
        mockResolver.Verify(x => x.Resolve(groupSid1), Times.Once);
        mockResolver.Verify(x => x.Resolve(groupSid2), Times.Once);
    }

    [Fact]
    public async Task TransformAsync_WithMixedResolvedAndUnresolvedGroups_AddsOnlyResolvedRoleClaims()
    {
        // Arrange
        var mockResolver = new Mock<IAdGroupResolver>();
        var groupSid1 = "S-1-5-21-1234567890-1234567890-1234567890-1234";
        var groupSid2 = "S-1-5-21-1234567890-1234567890-1234567890-1235";
        var groupName1 = "TestGroup1";
        
        mockResolver.Setup(x => x.Resolve(groupSid1)).Returns(groupName1);
        mockResolver.Setup(x => x.Resolve(groupSid2)).Returns((string?)null);
        
        var transformer = new AdClaimsTransformer(mockResolver.Object);
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.GroupSid, groupSid1));
        identity.AddClaim(new Claim(ClaimTypes.GroupSid, groupSid2));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = await transformer.TransformAsync(principal);

        // Assert
        Assert.Same(principal, result);
        var roleClaims = result.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        Assert.Single(roleClaims);
        Assert.Equal(groupName1, roleClaims[0].Value);
        mockResolver.Verify(x => x.Resolve(groupSid1), Times.Once);
        mockResolver.Verify(x => x.Resolve(groupSid2), Times.Once);
    }

    [Fact]
    public async Task TransformAsync_WithExistingRoleClaims_PreservesAndAddsNewRoleClaims()
    {
        // Arrange
        var mockResolver = new Mock<IAdGroupResolver>();
        var groupSid = "S-1-5-21-1234567890-1234567890-1234567890-1234";
        var groupName = "TestGroup";
        var existingRole = "ExistingRole";
        
        mockResolver.Setup(x => x.Resolve(groupSid)).Returns(groupName);
        
        var transformer = new AdClaimsTransformer(mockResolver.Object);
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.Role, existingRole));
        identity.AddClaim(new Claim(ClaimTypes.GroupSid, groupSid));
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = await transformer.TransformAsync(principal);

        // Assert
        Assert.Same(principal, result);
        var roleClaims = result.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        Assert.Equal(2, roleClaims.Count);
        Assert.Contains(roleClaims, c => c.Value == existingRole);
        Assert.Contains(roleClaims, c => c.Value == groupName);
    }
}

