using Microsoft.EntityFrameworkCore;
using MyWebApp1.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MyWebApp1.Models;
using Microsoft.OpenApi.Models;
using MyWebApp1.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Gọi phương thức AddSwaggerAuthentication từ DependencyInjection
builder.Services.AddSwaggerAuthentication();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
}).AddGoogle(googleOptions =>
{
    // Lấy cấu hình Google từ appsettings.json
    IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");

    googleOptions.ClientId = googleAuthNSection["ClientId"];
    googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
});


//builder.Services.AddAuthentication()
//    .AddGoogle(googleOptions =>
//    {
//        IConfigurationSection googleAuthNSection = ConfigurationBinder.GetSection("Authentication:Google");

//        //setup clientID va ClientSecret de truy cap api gooogle
//        googleOptions.ClientId = googleAuthNSection["ClientId"];
//        googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];

//    })
//    .AddLocalApi("Bearer", OptionsBuilderConfigurationExtensions =>
//{
//    options.ExpectedScope = "api.WebApp";
//});


builder.Services.AddDbContext<MyDbContext>(e => e.UseSqlServer(builder.Configuration.GetConnectionString("DBCS")));

////set up
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyWebApp API", Version = "v1" });

//    // Cấu hình cho Swagger sử dụng JWT Authorization
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        Scheme = "Bearer",
//        BearerFormat = "JWT",
//        In = ParameterLocation.Header,
//        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer abc123def456\""
//    });

//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            new string[] {}
//        }
//    });
//});


var app = builder.Build();

app.UseAuthentication();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pawfund Platform v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.Run();
