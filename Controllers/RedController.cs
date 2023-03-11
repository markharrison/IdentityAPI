using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Identity.Web;
using IdentityAPI.Models;

namespace IdentityAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
//[Authorize(Roles = "red")]
//[RequiredScope("Red.Read")]
public class RedController : ControllerBase
{
    private static readonly string[] RedItems = new[]
    {
        "tomato", "red hot chili pepper", "fire engine", "postbox", "rose", "red car", "red panda", "red lipstick", "red ballon", "red dragon"
    };

    private readonly ILogger<RedController> _logger;

    public RedController(ILogger<RedController> logger)
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

    [HttpPost(Name = "postred")]
    [ProducesResponseType(typeof(RedData), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PostAll()
    {

        if (!(checkrole("red") && checkscope("Red.ReadWrite")))
        {
            return Forbid();
        }

        await Task.Run(() => { });

        var rsp = "{\"status\" : \"red-ok\" }";

        return Ok(rsp);
    }

    [HttpGet(Name = "getred")]
    [ProducesResponseType(typeof(RedData),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {

        if (!(checkrole("red") && (checkscope("Red.Read") || checkscope("Red.ReadWrite"))))
        {
            return Forbid();
        }

        var _RedList = new List<RedData>();
        for (var i = 0; i < 5; i++)
        {
            _RedList.Add(new RedData
            {
                idx = Guid.NewGuid().ToString(),
                payload = RedItems[Random.Shared.Next(RedItems.Length)]
            });
        };

        await Task.Run(() => { });

        return Ok(_RedList);
    }


}
