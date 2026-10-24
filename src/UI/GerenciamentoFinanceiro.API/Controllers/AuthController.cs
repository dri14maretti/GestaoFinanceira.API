using System;
using System.Threading.Tasks;
using GerenciamentoFinanceiro.API.Security;
using GerenciamentoFinanceiro.Application.DTOs.Auth;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoFinanceiro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ICurrentUserService _currentUserService;
    private readonly RefreshTokenCookie _cookieHelper;

    public AuthController(
        AuthService authService,
        ICurrentUserService currentUserService,
        RefreshTokenCookie cookieHelper)
    {
        _authService = authService;
        _currentUserService = currentUserService;
        _cookieHelper = cookieHelper;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        _cookieHelper.Write(Response, result.RefreshToken, result.RefreshTokenExpiresAt);
        return CreatedAtAction(nameof(Me), null, result.Response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        _cookieHelper.Write(Response, result.RefreshToken, result.RefreshTokenExpiresAt);
        return Ok(result.Response);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [RequireCsrfHeader]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Refresh()
    {
        var rawRefreshToken = _cookieHelper.Read(Request);
        try
        {
            var result = await _authService.RefreshAsync(rawRefreshToken);
            _cookieHelper.Write(Response, result.RefreshToken, result.RefreshTokenExpiresAt);
            return Ok(result.Response);
        }
        catch (UnauthorizedAccessException)
        {
            _cookieHelper.Clear(Response);
            throw;
        }
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    [RequireCsrfHeader]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        var rawRefreshToken = _cookieHelper.Read(Request);
        await _authService.LogoutAsync(rawRefreshToken);
        _cookieHelper.Clear(Response);
        return NoContent();
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(CurrentUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CurrentUserDto>> Me()
    {
        var userId = _currentUserService.GetRequiredUserId();
        var user = await _authService.GetCurrentUserAsync(userId);
        return Ok(user);
    }
}
