using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
using Postkit.Posts.Interfaces;
using Postkit.Posts.Services;
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
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "PostkitAPI", Version = "v1" });

    // Add JWT Authentication
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token **_only_** (without 'Bearer ' prefix)."
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    };

    options.AddSecurityRequirement(securityRequirement);
    options.EnableAnnotations();
});
builder.Services.AddDbContext<PostkitDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PostkitApiConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<PostkitDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRouting();

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
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddTransient<IMailService, MailjetMailService>();

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

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

var app = builder.Build();

var enableSwagger = builder.Configuration.GetValue<bool>("EnableSwagger");
if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Postkit API v1");
        c.RoutePrefix = "swagger";
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<PostkitDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await DbSeeder.SeedAsync(dbContext, userManager, roleManager);
    }
}

app.UseExceptionHandler(_ => { });

app.UseCors("ConfiguredCors");

app.UseRouting();

app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/hubs/notifications");

app.MapControllers();

app.Run();