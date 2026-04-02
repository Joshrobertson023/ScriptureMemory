using DataAccess.Data;
using DataAccess.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using ScriptureMemory.Server.Tools;
using System.Security.Claims;

namespace ScriptureMemory.Server.Services;

public class AdminService
{
    private readonly AdminData _adminData;
    private readonly TokenProvider _tokenProvider;

    public AdminService(
        AdminData adminData,
        TokenProvider tokenProvider)
    {
        _adminData = adminData;
        _tokenProvider = tokenProvider;
    }

    public async Task<IResult> Login(string username, string password)
    {
        var admin = await _adminData.GetAdminByUsername(username);
        if (admin is null)
            return Results.NotFound("Admin not found");

        var passwordHasher = new PasswordHasher<Admin>();
        if (string.IsNullOrEmpty(admin?.HashedPassword))
        {
            await _adminData.UpdatePassword(admin!.Id, 
                passwordHasher.HashPassword(admin, password!));

            return Results.Ok(_tokenProvider.Create(admin));
        }
        else
        {
            PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(admin, admin.HashedPassword, password);

            if (result == PasswordVerificationResult.Success
                || result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                return Results.Ok(_tokenProvider.Create(admin));
            }
            else
            {
                return Results.Unauthorized();
            }
        }
    }
}
