using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Identity.Web;
using IdentityAPI.Models;

namespace IdentityAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
//[Authorize(Roles = "yellow")]
//[RequiredScope("Yellow.Read")]
public class YellowController : ControllerBase
{
    private static readonly string[] YellowItems = new[]
    {
        "sunshine", "golden nuggets", "bananas", "hornet", "burmese python", "canary", "corn", "daffodils", "egg yolk", "lemons"
    };

    private readonly ILogger<YellowController> _logger;

    public YellowController(ILogger<YellowController> logger)
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

    [HttpPost(Name = "postyellow")]
    [ProducesResponseType(typeof(YellowData), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PostAll()
    {

        if (!(checkrole("yellow") && checkscope("Yellow.ReadWrite")))
        {
            return Forbid();
        }

        await Task.Run(() => { });

        var rsp = "{\"status\" : \"yellow-ok\" }";

        return Ok(rsp);
    }

    [HttpGet(Name = "getyellow")]
    [ProducesResponseType(typeof(YellowData),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {

        if (!(checkrole("yellow") && (checkscope("Yellow.Read") || checkscope("Yellow.ReadWrite"))))
        {
            return Forbid();
        }

        var _YellowList = new List<YellowData>();
        for (var i = 0; i < 5; i++)
        {
            _YellowList.Add(new YellowData
            {
                idx = Guid.NewGuid().ToString(),
                payload = YellowItems[Random.Shared.Next(YellowItems.Length)]
            });
        };

        await Task.Run(() => { });

        return Ok(_YellowList);
    }


}
