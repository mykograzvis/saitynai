using Azure.Core;
using Ligonine.Auth.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Ligonine.Auth
{
    public static class AuthEndpoints
    {
        public static void AddAuthApi(this WebApplication app)
        {
            //register
            app.MapPost("api/accounts", async (UserManager<ForumRestUser> userManager, RegisterUserDto dto) =>
            {
                //check user exists
                var user = await userManager.FindByNameAsync(dto.UserName);
                if (user != null)
                    return Results.UnprocessableEntity("UserName already taken");

                var newUser =  new ForumRestUser()
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                };

                var CreateUserResults = await userManager.CreateAsync(newUser, dto.Password);
                if (!CreateUserResults.Succeeded)
                    return Results.UnprocessableEntity();

                await userManager.AddToRoleAsync(newUser, ForumRoles.HospitalUser);

                return Results.Created();
            });

            //login
            app.MapPost("api/login", async (UserManager<ForumRestUser> userManager, JwtTokenServices jwtTokenService, SessionService sessionService, HttpContext httpContext, LoginDto dto) =>
            {
                //check user exists
                var user = await userManager.FindByNameAsync(dto.UserName);
                if (user == null)
                    return Results.UnprocessableEntity("UserName does not exist");

                var isPasswordValid = await userManager.CheckPasswordAsync(user, dto.Password);
                if(!isPasswordValid)
                    return Results.UnprocessableEntity("Username or password was incorrect.");

                var roles = await userManager.GetRolesAsync(user);

                var sessionId = Guid.NewGuid();
                var expiresAt = DateTime.UtcNow.AddDays(2);
                var accessToken = jwtTokenService.CreateAccesToken(user.UserName, user.Id, roles);
                var refreshToken = jwtTokenService.CreateRefreshToken(sessionId, user.Id, expiresAt);

                await sessionService.CreateSessionAsync(sessionId, user.Id, refreshToken, expiresAt);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = expiresAt,
                };

                httpContext.Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);

                return Results.Ok(new SuccessfullLoginDto(accessToken));
            });

            //add doctor role
            app.MapPost("api/addDoctorRole", [Authorize(Roles = ForumRoles.Admin)] async (UserManager<ForumRestUser> userManager, AddDoctorRole dto) =>
            {
                // check user exists
                var user = await userManager.FindByNameAsync(dto.UserName);

                if (user == null)
                    return Results.UnprocessableEntity("User does not exist");

                await userManager.AddToRoleAsync(user, ForumRoles.Doctor);

                return Results.Ok();
            });

            app.MapPost("api/accessToken", async (UserManager<ForumRestUser> userManager, JwtTokenServices jwtTokenService, SessionService sessionServices, HttpContext httpContext) =>
            {
                if(!httpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
                {
                    return Results.UnprocessableEntity();
                }
                if(!jwtTokenService.TryParseRefreshToken(refreshToken, out var claims))
                {
                    return Results.UnprocessableEntity();
                }

                var sessionId = claims.FindFirstValue("SessionId");
                if(string.IsNullOrWhiteSpace(sessionId))
                {
                    return Results.UnprocessableEntity();
                }

                var sessionIdAsGuid = Guid.Parse(sessionId);
                if(!await sessionServices.IsSessionValidAsync(sessionIdAsGuid, refreshToken))
                {
                    return Results.UnprocessableEntity();
                }

                var userId = claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Results.UnprocessableEntity();
                }

                var roles = await userManager.GetRolesAsync(user);

                var expiresAt = DateTime.UtcNow.AddDays(2);
                var accessToken = jwtTokenService.CreateAccesToken(user.UserName, user.Id, roles);
                var newRefreshToken = jwtTokenService.CreateRefreshToken(sessionIdAsGuid, user.Id, expiresAt);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = expiresAt,
                };

                httpContext.Response.Cookies.Append("RefreshToken", newRefreshToken, cookieOptions);

                await sessionServices.ExtendSessionAsync(sessionIdAsGuid, newRefreshToken, expiresAt);

                return Results.Ok(new SuccessfullLoginDto(accessToken));
            });

            app.MapPost("api/logout", async (UserManager<ForumRestUser> userManager, JwtTokenServices jwtTokenService, SessionService sessionServices, HttpContext httpContext) =>
            {
                if (!httpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
                {
                    return Results.UnprocessableEntity();
                }
                if (!jwtTokenService.TryParseRefreshToken(refreshToken, out var claims))
                {
                    return Results.UnprocessableEntity();
                }

                var sessionId = claims.FindFirstValue("SessionId");
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    return Results.UnprocessableEntity();
                }

                await sessionServices.InvalidateSessionAsync(Guid.Parse(sessionId));
                httpContext.Response.Cookies.Delete("RefreshToken");

                return Results.Ok();
            });

        }

        public record RegisterUserDto(string UserName, string Email, string Password);
        public record LoginDto(string UserName, string Password);
        public record SuccessfullLoginDto(string AccessToken);
        public record AddDoctorRole(string UserName);
    }
}
