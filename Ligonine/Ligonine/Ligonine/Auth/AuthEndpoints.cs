using Azure.Core;
using Ligonine.Auth.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
                if (user == null)
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


        }

        public record RegisterUserDto(string UserName, string Email, string Password);
    }
}
