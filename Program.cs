using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters.RoleClaimType = ClaimConstants.Roles;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(builder =>
   builder.WithOrigins("http://localhost")
           .AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod());

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, httpRequest) =>
    {
        var basePath = "/";
        var host = httpRequest.Host.Value;
        var scheme = (httpRequest.IsHttps || httpRequest.Headers["x-forwarded-proto"].ToString() == "https") ? "https" : "http";

        if (httpRequest.Headers["x-forwarded-host"].ToString() != "")
        {
            host = httpRequest.Headers["x-forwarded-host"].ToString() + ":" + httpRequest.Headers["x-forwarded-port"].ToString();
        }

        swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{scheme}://{host}{basePath}" } };

    });
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mark Harrison IdentityAPI V1");
    c.RoutePrefix = string.Empty;
});


app.UseAuthorization();

app.MapControllers();

app.Run();
