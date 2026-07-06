using DataAccess.Data;
using DataAccess.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using ScriptureMemory.Server.Tools;
using System.Security.Claims;

namespace ScriptureMemory.Server.Services;

public class AdminService
{
    private readonly IAdminData _adminData;
    private readonly TokenProvider _tokenProvider;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IAdminData adminData,
        TokenProvider tokenProvider,
        ILogger<AdminService> logger)
    {
        _adminData = adminData;
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    public async Task<IResult> Login(string username, string password)
    {
        var admin = await _adminData.GetAdminByUsername(username);
        if (admin is null)
        {
            _logger.LogWarning("Failed admin login, username attempted: {Username}", username);
            
            return Results.Unauthorized();
        }

        var passwordHasher = new PasswordHasher<Admin>();
        if (string.IsNullOrEmpty(admin?.HashedPassword))
        {
            await _adminData.UpdatePassword(admin!.UserId, passwordHasher.HashPassword(admin, password));
            
            _logger.LogInformation("Initial admin login with new password successful: {Admin}", admin);

            return Results.Ok(_tokenProvider.Create(admin));
        }
        else
        {
            PasswordVerificationResult result 
                = passwordHasher.VerifyHashedPassword(admin, admin.HashedPassword, password);

            if (result == PasswordVerificationResult.Success
                || result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                _logger.LogInformation("Login successful: {Admin}", admin);
                
                return Results.Ok(_tokenProvider.Create(admin));
            }
            else
            {
                _logger.LogWarning("Failed invalid password login for: {AdminEmail}.", admin.AdminEmail);
                
                return Results.Unauthorized();
            }
        }
    }
}
