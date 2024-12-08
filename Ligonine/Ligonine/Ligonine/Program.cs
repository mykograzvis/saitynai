using Ligonine.Auth;
using Ligonine.Auth.Model;
using Ligonine.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<HospitalDbContext>();

builder.Services.AddIdentity<ForumRestUser, IdentityRole>()
    .AddEntityFrameworkStores<HospitalDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(option =>
    {
        option.MapInboundClaims = false;
        option.TokenValidationParameters.ValidAudience = builder.Configuration["Jwt:ValidAudience"];
        option.TokenValidationParameters.ValidIssuer = builder.Configuration["Jwt:ValidIssuer"];
        option.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]));
    });

builder.Services.AddAuthorization();
builder.Services.AddTransient<JwtTokenServices>();
builder.Services.AddTransient<SessionService>();
builder.Services.AddScoped<AuthSeeder>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Allow any origin (adjust for security in production)
              .AllowAnyMethod()  // Allow any HTTP method (GET, POST, etc.)
              .AllowAnyHeader(); // Allow any headers
    });
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
//var dbContext = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
var dbSeeder = scope.ServiceProvider.GetRequiredService<AuthSeeder>();
await dbSeeder.SeedAsync();

app.UseCors("AllowAll");
app.AddAuthApi();
app.UseAuthorization();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
