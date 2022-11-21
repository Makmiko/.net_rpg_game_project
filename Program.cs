global using dotnet_rpg.Models;
using System.Text;
using dotnet_rpg.Data;
using dotnet_rpg.Services;
using dotnet_rpg.Services.AuthService;
using dotnet_rpg.Services.CharacterService;
using dotnet_rpg.Services.WeaponService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IFightService, FightService>();
builder.Services.AddScoped<IWeaponService, WeaponService>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
    {
        opts.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
        {
            Description = "Standard Authorization header using the Bearer scheme e.g. \"bearer {token} \"",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
        });
        opts.OperationFilter<SecurityRequirementsOperationFilter>();   
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();