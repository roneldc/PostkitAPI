
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Poskit.Posts.Interfaces;
using Poskit.Posts.Repository;
using Poskit.Posts.Services;
using Postkit.API.Middleware;
using Postkit.Comments.Interfaces;
using Postkit.Comments.Repository;
using Postkit.Comments.Services;
using Postkit.Identity.Interfaces;
using Postkit.Identity.Services;
using Postkit.Infrastructure.Data;
using Postkit.Notifications.Hubs;
using Postkit.Notifications.Interfaces;
using Postkit.Notifications.Repositories;
using Postkit.Notifications.Services;
using Postkit.Reactions.Interfaces;
using Postkit.Reactions.Repositories;
using Postkit.Reactions.Services;
using Postkit.Shared.Constants;
using Postkit.Shared.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PostkitDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<PostkitDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSignalR()
    .AddHubOptions<NotificationHub>(options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    }); 

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IReactionRepository, ReactionRepository>();
builder.Services.AddScoped<IReactionService, ReactionService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notifications"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOrUser", policy =>
        policy.RequireRole(UserRoles.Admin, UserRoles.User));
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddLogging();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ConfiguredCors", policy =>
    {
        policy.WithOrigins(allowedOrigins ?? Array.Empty<string>())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PostkitDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await DbSeeder.SeedAsync(dbContext, userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(_ => { });
app.UseHttpsRedirection();

app.UseCors("ConfiguredCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/hubs/notifications");

app.MapControllers();

app.Run();