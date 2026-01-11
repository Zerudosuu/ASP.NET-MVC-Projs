using System.IdentityModel.Tokens.Jwt;
using System.Text;
using LibrarySystemServer.CompiledModels;
using LibrarySystemServer.Data;
using LibrarySystemServer.Model;
using LibrarySystemServer.Repositories.Implementations;
using LibrarySystemServer.Repositories.Interfaces;
using LibrarySystemServer.Services.Auth;
using LibrarySystemServer.Services.GoogleBooks;
using LibrarySystemServer.Services.Implementations;
using LibrarySystemServer.Services.Interfaces;
using LibrarySystemServer.Services.Seeder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.IdentityModel.Tokens;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddDbContextPool<LibrarySystemContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found."
            ),
        sql => sql.Equals(LibrarySystemContextModel.Instance)
    )
);

builder
    .Services.AddIdentity<Member, IdentityRole>(options =>
    {
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<LibrarySystemContext>() // <— connects Identity to EF Core
    .AddDefaultTokenProviders();

builder.Services.AddHostedService<SeedHostedService>();
builder.Services.AddHostedService<LibrarySystemServer.Services.Hosted.CleanupExpiredTokensHostedService>();

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async ctx =>
            {
                var jti = ctx.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(jti))
                    return;

                var db = ctx.HttpContext.RequestServices.GetRequiredService<LibrarySystemContext>();
                var isRevoked = await db.RevokedTokens.AnyAsync(
                    rt => rt.Jti == jti && rt.ExpiresAt > DateTime.UtcNow,
                    ctx.HttpContext.RequestAborted
                );
                if (isRevoked)
                    ctx.Fail("Token revoked.");
            },
        };
    });

builder.Services.AddAuthorization();

builder
    .Services.AddHttpClient<IGoogleBooksClient, GoogleBooksClient>(client =>
    {
        client.BaseAddress = new Uri("https://www.googleapis.com/books/v1/");
    })
    .AddResilienceHandler(
        "google-books",
        pipeline =>
        {
            // Automatic retry
            pipeline.AddRetry(
                new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(2),
                    UseJitter = true,
                }
            );

            // Timeout per request
            pipeline.AddTimeout(TimeSpan.FromSeconds(10));

            // Circuit breaker for repeated failures
            pipeline.AddCircuitBreaker(
                new HttpCircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.1,
                    MinimumThroughput = 10,
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    BreakDuration = TimeSpan.FromSeconds(15),
                }
            );
        }
    );

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddScoped<ILibrarianRepository, LibrarianRepository>();
builder.Services.AddScoped<ILibrarianService, LibrarianService>();

builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IMemberService, MemberService>();

builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<RefreshTokenService>();

var app = builder.Build();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()
    );
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
