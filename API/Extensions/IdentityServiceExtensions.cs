using System;
using System.Text;
using System.Security.Claims;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddIdentityCore<User>(options =>
       {
           options.Password.RequireNonAlphanumeric = false;
       })
          .AddRoles<AppRole>()
          .AddRoleManager<RoleManager<AppRole>>()
          .AddEntityFrameworkStores<DataContext>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var tokenKey = config["TokenKey"] ?? throw new Exception("TokenKey not found");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();

                        var userIdValue = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                        if (!int.TryParse(userIdValue, out var userId))
                        {
                            context.Fail("Invalid token");
                            return;
                        }

                        var user = await userManager.FindByIdAsync(userId.ToString());
                        if (user is null)
                        {
                            context.Fail("User not found");
                            return;
                        }

                        if (user.IsDeleted)
                        {
                            context.Fail("User deleted");
                            return;
                        }

                        var stampClaim = context.Principal?.FindFirst("sstamp")?.Value;
                        if (string.IsNullOrEmpty(stampClaim) || user.SecurityStamp != stampClaim)
                        {
                            context.Fail("Token revoked");
                        }
                    }
                };
            });

        services.AddAuthorizationBuilder()
           .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
           .AddPolicy("StaffRole", policy => policy.RequireRole("Admin", "Staff"))
           .AddPolicy("RequireStaffOrAdminRole", policy => policy.RequireRole("Admin", "Staff"));


        return services;

    }
}
