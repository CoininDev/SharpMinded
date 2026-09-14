using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;

namespace SharpMinded.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly Supabase.Client _supabase;
    public AuthController(Supabase.Client supa)
    {
        _supabase = supa;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest req)
    {
        if ( string.IsNullOrWhiteSpace(req.Email) 
        || string.IsNullOrWhiteSpace(req.Password))
        {
            return BadRequest(new { message = "Name, E-mail and password are required." });
        }

        try
        {
            var session = await _supabase.Auth.SignUp(req.Email, req.Password);

            if(session?.User != null)
            {
                return Ok(new
                {
                    message = "Login succeded",
                    accessToken = session.AccessToken,
                    refreshToken = session.RefreshToken,
                    user = new
                    {
                        id = session.User.Id,
                        email = session.User.Email
                    }
                });
            }

            return Unauthorized(new {message = "Invalid credentials"});
        }
        // Errors from supabase
        catch (GotrueException ex)
        {
            return Unauthorized(new {message = ex.Message});
        }
        catch (System.Exception ex)
        {
            return StatusCode(500);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
        {
            return BadRequest(new { message = "E-mail and password are required." });
        }

        try
        {
            var session = await _supabase.Auth.SignIn(req.Email, req.Password);

            if(session?.User != null)
            {
                return Ok(new
                {
                    message = "Login succeded",
                    accessToken = session.AccessToken,
                    refreshToken = session.RefreshToken,
                    user = new
                    {
                        id = session.User.Id,
                        email = session.User.Email
                    }
                });
            }

            return Unauthorized(new {message = "Invalid credentials"});
        }
        // Errors from supabase
        catch (GotrueException ex)
        {
            return Unauthorized(new {message = ex.Message});
        }
        catch (System.Exception ex)
        {
            return StatusCode(500);
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _supabase.Auth.SignOut();
        return Ok(new { message = "Logout succeded." });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetUser()
    {
        var currentUser = _supabase.Auth.CurrentUser;
        
        if (currentUser == null)
        {
            return Unauthorized(new { message = "Not logged yet." });
        }

        return Ok(new User(currentUser.Id, currentUser.Email));
    }
}