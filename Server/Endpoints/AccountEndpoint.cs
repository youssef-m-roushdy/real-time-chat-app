using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Common;
using Server.DTOs;
using Server.Extensions;
using Server.Models;
using Server.Services;

namespace Server.Endpoints
{
    public static class AccountEndpoint
    {
        public static RouteGroupBuilder MapAccountEndpoint(this WebApplication app)
        {
            var group = app.MapGroup("/api/account").WithTags("account");

            group.MapPost("/register", async (HttpContext context, UserManager<AppUser> userManager, 
                                                                [FromForm] string fullName, 
                                                                [FromForm] string userName, 
                                                                [FromForm] string email, 
                                                                [FromForm] string password, 
                                                                [FromForm] IFormFile? profileImage) =>
            {
                try
                {
                    var userFromDb = await userManager.FindByEmailAsync(email);
                    if (userFromDb != null)
                    {
                        return Results.BadRequest(Response<String>.Failure("User with this email already exists."));
                    }

                    if (profileImage is null)
                    {
                        return Results.BadRequest(Response<string>.Failure("Profile Image Is Required"));
                    }

                    var picture = await FileUploadService.Upload(profileImage);

                    picture = $"{context.Request.Scheme}://{context.Request.Host}/uploads/{picture}";

                    // Create a new user
                    var user = new AppUser
                    {
                        FullName = fullName,
                        UserName = userName,
                        Email = email,
                        ProfileImage = picture
                    };


                    // Attempt to create the user
                    var result = await userManager.CreateAsync(user, password);
                    if (!result.Succeeded)
                    {
                        // Return validation errors
                        return Results.BadRequest(Response<String>.Failure(result.Errors.Select(x => x.Description).FirstOrDefault()!));
                    }

                    return Results.Ok(Response<String>.Success("", "User registered successfully."));
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(Response<string>.Failure(ex.Message));
                }
                catch (System.Exception)
                {
                    return Results.StatusCode(StatusCodes.Status500InternalServerError, 
                        Response<string>.Failure("An error occurred while registering the user."));
                }
                
            }).DisableAntiforgery();

            group.MapPost("/login", async (UserManager<AppUser> userManager, TokenService tokenService, [FromBody]LoginDto loginDto) => 
            {
                if(loginDto is null)
                {
                    return Results.BadRequest(Response<string>.Failure("Invalid Login Details"));
                }

                var user = await userManager.FindByEmailAsync(loginDto.Email);
                if (user is null)
                {
                    return Results.BadRequest(Response<String>.Failure("User Not Found"));
                }

                var result = await userManager.CheckPasswordAsync(user!, loginDto.Password);

                if(!result)
                {
                    return Results.BadRequest(Response<String>.Failure("Invalid Password"));
                }

                var token = tokenService.GenerateToken(user.Id, user.UserName!);

                return Results.Ok(Response<string>.Success(token, "Login Success"));

            });

            group.MapGet("/me", async (HttpContext context, UserManager<AppUser> userManager) => {
                var currentLoggedInUserId = context.User.GetUserId();

                var currentLoggedInUser = await userManager.Users.SingleOrDefaultAsync(x => x.Id == currentLoggedInUserId.ToString());

                return Results.Ok(Response<AppUser>.Success(currentLoggedInUser!, "User Fetched Successfully."));
            }).RequireAuthorization();

            return group;
        }

        
    }
}
