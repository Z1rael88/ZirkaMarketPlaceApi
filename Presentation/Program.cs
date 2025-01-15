using System.Reflection;
using Application.Initializers;
using Application.Interfaces;
using Application.Mappers;
using Application.Services;
using Domain.Models;
using Elastic.Clients.Elasticsearch;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Infrastructure.Repositories;
using Mailjet.Client;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Presentation.Extensions;
using Presentation.Middlewares;
using Presentation.Services;
using Stripe;
using ProductService = Application.Services.ProductService;

namespace Presentation;

static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(config =>
        {
            config.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });

            config.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "My API",
                Version = "v1",
            });

            config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Please enter the JWT token",
            });

            config.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    Array.Empty<string>()
                },
            });
        });
        builder.Services.AddAuthenticationWithJwtTokenSettings(builder.Configuration);
        builder.Services.AddIdentityCore<User>(
                options =>
                {
                    var passwordSettings = builder.Configuration.GetSection("PasswordValidation");

                    options.Password.RequiredUniqueChars = passwordSettings.GetValue<int>("RequiredUniqueChars");
                    options.Password.RequireUppercase = passwordSettings.GetValue<bool>("RequireUppercase");
                    options.Password.RequiredLength = passwordSettings.GetValue<int>("RequiredLength");
                })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        builder.Services.AddValidatorsFromAssembly(Assembly.Load("Application"));
        builder.Services.Configure<CategoryValidationOptions>(builder.Configuration.GetSection("CategoryValidation"));
        builder.Services.Configure<ProductValidationOptions>(builder.Configuration.GetSection("ProductValidation"));
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IApplicationUser, CurrentApplicationUser>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IFileStorageService, FileStorageService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        builder.Services.AddScoped<GlobalExceptionHandler>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IPurchaseService, PurchaseService>();
        builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();//
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            options.EnableSensitiveDataLogging();
        });
        builder.Services.AddMapster();
        MapsterConfig.ProductMappings();
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
        builder.Services.Configure<GoogleOptions>(builder.Configuration.GetSection("Google"));
        builder.Services.Configure<MailJetOptions>(builder.Configuration.GetSection("MailJet"));
        var stripeSection = builder.Configuration.GetSection("Stripe");
        StripeConfiguration.ApiKey = stripeSection["SecretKey"];
        
        builder.Services.Configure<ElasricsearchOptions>(builder.Configuration.GetSection("Elasticsearch"));
        builder.Services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ElasricsearchOptions>>().Value;
            var settings = new ElasticsearchClientSettings(new Uri(options.Uri))
                .DefaultIndex(options.DefaultIndex);
            return new ElasticsearchClient(settings);
        });
        builder.Services.AddTransient<IMailjetClient>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var apiKey = configuration["Mailjet:ApiKey"];
            var apiSecret = configuration["Mailjet:ApiSecret"];
            return new MailjetClient(apiKey, apiSecret);
        });


        builder.Services.AddControllers();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp", builder =>
            {
                builder.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync(); 

            await RolesInitializer.InitializeRolesAsync(services);

            await SystemAdministratorInitializer.InitializeSystemAdministratorAsync(app.Services, builder.Configuration);
        }
        app.UseMiddleware<GlobalExceptionHandler>();
        app.UseCors("AllowReactApp");
        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.None
        });
        app.MapControllers();
        app.UseHttpsRedirection();
        await app.RunAsync();
    }
}