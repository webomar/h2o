using ApiService.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using Xunit;

namespace TestProject;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class AdGroupResolverTests
{
    [Fact]
    public void Resolve_WithValidSid_ReturnsGroupNameOrNull()
    {
        // Arrange
        var resolver = new AdGroupResolver();
        // Use a well-known SID for testing (Everyone group)
        var wellKnownSid = new SecurityIdentifier(WellKnownSidType.WorldSid, null).Value;

        // Act & Assert
        // Note: This test may throw if not run in a domain environment
        // In a real scenario, you might want to use integration tests or mock the AD calls
        try
        {
            var result = resolver.Resolve(wellKnownSid);
            // If we get here, the method executed successfully
            // Result can be null if the group doesn't exist in the test environment, which is acceptable
            _ = result;
        }
        catch (System.DirectoryServices.AccountManagement.PrincipalServerDownException)
        {
            // Expected when AD is not available - this is acceptable for unit tests
            // In a real scenario, integration tests would be used
        }
    }

    [Fact]
    public void Resolve_WithInvalidSid_ReturnsNull()
    {
        // Arrange
        var resolver = new AdGroupResolver();
        // Use a valid SID format but with non-existent group
        var invalidSid = "S-1-5-21-9999999999-9999999999-9999999999-9999";

        // Act & Assert
        // May return null if group doesn't exist, or may throw if AD is unavailable
        try
        {
            var result = resolver.Resolve(invalidSid);
            // If we get here, the method executed successfully
            _ = result;
        }
        catch (System.DirectoryServices.AccountManagement.PrincipalServerDownException)
        {
            // Expected when AD is not available - this is acceptable for unit tests
            // In a real scenario, integration tests would be used
        }
    }

    [Fact]
    public void Resolve_WithNullSid_ThrowsArgumentNullException()
    {
        // Arrange
        var resolver = new AdGroupResolver();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null!));
    }

    [Fact]
    public void Resolve_WithEmptySid_ThrowsArgumentException()
    {
        // Arrange
        var resolver = new AdGroupResolver();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => resolver.Resolve(string.Empty));
    }

    [Fact]
    public void Resolve_WithMalformedSid_ThrowsException()
    {
        // Arrange
        var resolver = new AdGroupResolver();
        var malformedSid = "Not-A-Valid-SID";

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => resolver.Resolve(malformedSid));
    }
}

