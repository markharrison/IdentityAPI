using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Identity.Web;
using IdentityAPI.Models;

namespace IdentityAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
//[Authorize(Roles = "black")]
//[RequiredScope("Black.Read")]
public class BlackController : ControllerBase
{
    private static readonly string[] BlackItems = new[]
    {
        "charcoal", "liquorice", "black sapphire", "black pepper", "black olive", "black ink", "black orchid", "black widow spider", "panther", "black hole"
    };

    private readonly ILogger<BlackController> _logger;

    public BlackController(ILogger<BlackController> logger)
    {
        _logger = logger;
    }

    bool checkrole(string acceptedroles)
    {
        // HttpContext.ValidateAppRole();

        if (HttpContext.User.Claims is null)
        {
            return false;
        }

        IEnumerable<string> rolesClaim = HttpContext.User.Claims.Where(
            c => c.Type == ClaimConstants.Roles || c.Type == ClaimConstants.Role)
            .SelectMany(c => c.Value.Split(' '));

        if (rolesClaim is null)
        {
            return false;
        }

        if (!rolesClaim.Intersect(acceptedroles.Split(' ')).Any())
        {
            return false;
        }

        return true;
    }

    bool checkscope(string acceptedscopes)
    {
        // HttpContext.VerifyUserHasAnyAcceptedScope();

        if (HttpContext.User.Claims is null)
        {
            return false;
        }

        Claim? scopeClaim = HttpContext.User.FindFirst(ClaimConstants.Scp) ?? HttpContext.User.FindFirst(ClaimConstants.Scope);
        if (scopeClaim is null)
        {
            return false;
        }

        if (!scopeClaim.Value.Split(' ').Intersect(acceptedscopes.Split(' ')).Any())
        {
            return false;
        }

        return true;
    }

    [HttpPost(Name = "postblack")]
    [ProducesResponseType(typeof(BlackData), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PostAll()
    {

        if (!(checkrole("black") && checkscope("Black.ReadWrite")))
        {
            return Forbid();
        }

        await Task.Run(() => { });

        var rsp = "{\"status\" : \"black-ok\" }";

        return Ok(rsp);
    }

    [HttpGet(Name = "getblack")]
    [ProducesResponseType(typeof(BlackData),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {

        if (!(checkrole("black") && (checkscope("Black.Read") || checkscope("Black.ReadWrite"))))
        {
            return Forbid();
        }

        var _BlackList = new List<BlackData>();
        for (var i = 0; i < 5; i++)
        {
            _BlackList.Add(new BlackData
            {
                idx = Guid.NewGuid().ToString(),
                payload = BlackItems[Random.Shared.Next(BlackItems.Length)]
            });
        };

        await Task.Run(() => { });

        return Ok(_BlackList);
    }


}
