using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiService.Controllers;

[Authorize]
[ApiController]
[Route("api/identity-test")]
public class IdentityTestController : ControllerBase
{
    [HttpGet("console")]
    public IActionResult Get()
    {
        var user = HttpContext.User;
        var identity = user.Identity;

        Console.WriteLine("================ IDENTITY TEST ================");

        Console.WriteLine("IsAuthenticated: " + identity?.IsAuthenticated);
        Console.WriteLine("AuthenticationType: " + identity?.AuthenticationType);
        Console.WriteLine("Name: " + identity?.Name);

        Console.WriteLine("---------------- ROLES ----------------");

        foreach (var role in user.Claims.Where(c => c.Type == ClaimTypes.Role))
        {
            Console.WriteLine("Role: " + role.Value);
        }

        Console.WriteLine("==============================================");

        return Ok("Identity info logged to console");
    }
    
    [HttpGet("json")]
    public IActionResult GetJson()
    {
        var user = HttpContext.User;
        var identity = user.Identity;

        Console.WriteLine("================ IDENTITY TEST ================");
        Console.WriteLine("IsAuthenticated: " + identity?.IsAuthenticated);
        Console.WriteLine("AuthenticationType: " + identity?.AuthenticationType);
        Console.WriteLine("Name: " + identity?.Name);

        foreach (var role in user.Claims.Where(c => c.Type == ClaimTypes.Role))
        {
            Console.WriteLine("Role: " + role.Value);
        }
        Console.WriteLine("==============================================");

        // JSON response
        var result = new
        {
            IsAuthenticated = identity?.IsAuthenticated ?? false,
            AuthenticationType = identity?.AuthenticationType,
            Name = identity?.Name,
            Roles = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList(),
            Groups = user.Claims
                .Where(c => c.Type == ClaimTypes.GroupSid)
                .Select(c => new { Sid = c.Value })
                .ToList()
        };

        return Ok(result);
    }
}