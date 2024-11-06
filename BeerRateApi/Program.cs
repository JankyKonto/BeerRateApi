using BeerRateApi;
using BeerRateApi.Interfaces;
using BeerRateApi.Profiles;
using BeerRateApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
        .AllowAnyMethod()
        .AllowCredentials()
        .AllowAnyHeader();
    });
});


builder.Services.AddControllers();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBeerService, BeerService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped(typeof(ILogger), typeof(Logger<Program>));

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(
    option => option
    .UseLazyLoadingProxies()
    .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var key = Encoding.UTF8.GetBytes(builder.Configuration.GetSection("TokenOptions:Key").Value!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("TokenOptions:Issuer").Value,
        ValidAudience = builder.Configuration.GetSection("TokenOptions:Audience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(key),
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var claimsPrincipal = context.Principal;
            var username = claimsPrincipal?.FindFirstValue(ClaimTypes.Name);
            

            if (username == null)
            {
                context.Fail("Bad request");
            }

            var userId = claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                context.Fail("Bad request");
            }

            if(!int.TryParse(userId, out var id))
            {
                context.Fail("Bad request");
            }

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

            var isUserInDb = await dbContext.Users.AnyAsync(user => user.Id == id);

            if (!isUserInDb)
            {
                context.Fail("Unauthorized");
            }

        },

        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("jwtToken"))
            {
                context.Token = context.Request.Cookies["jwtToken"];
            }

            return Task.CompletedTask;
        }
    };

    options.SaveToken = true;

}).AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
});

var app = builder.Build();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
