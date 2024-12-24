using System.Text;
using Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Presentation.Extensions
{
    public static class AuthenticationExtensions
    {
        public static void AddAuthenticationWithJwtTokenSettings(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();
            var googleAuthOptions = configuration.GetSection("Google");

            services.AddAuthentication(
                    options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                .AddGoogle(options =>
                {
                    options.ClientId = googleAuthOptions["ClientId"]!;
                    options.ClientSecret = googleAuthOptions["ClientSecret"]!;
                })
                .AddJwtBearer(
                    options =>
                    {
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var token = context.Request.Cookies["AccessToken"];
                                if (!string.IsNullOrEmpty(token))
                                {
                                    context.Token = token;
                                }

                                return Task.CompletedTask;
                            }
                        };
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtOptions!.Issuer,
                            ValidAudience = jwtOptions.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                            RequireExpirationTime = true,
                            ClockSkew = TimeSpan.Zero,
                        };
                    }
                )
                .AddCookie(options =>
                {
                    options.Cookie.Name = "AccessToken"; 
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                });
        }
    }
}