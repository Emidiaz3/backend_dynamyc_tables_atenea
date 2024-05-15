using ApiRestCuestionario.Context;
using System.Text;
using Microsoft.Extensions.FileProviders;
using ApiRestCuestionario.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using angular.Server.Model;

var builder = WebApplication.CreateBuilder(args);

string pathCombination = string.IsNullOrWhiteSpace(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) ? Environment.CurrentDirectory : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
StaticFolder staticFolder = new(Path.Combine(pathCombination, "MyStaticFiles"));

builder.Services.AddSingleton(staticFolder);
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IGmailSender, GmailSender>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("SecretKey")!)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddCors(options =>
{
    //options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    string[] origins = ["https://*.atenealatam.com", "https://*.ddigital.pe", "https://localhost:4200", "https://*.netlify.app"];
    options.AddDefaultPolicy(builder => builder.SetIsOriginAllowedToAllowWildcardSubdomains().WithOrigins(origins).AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors();

if (!Directory.Exists(staticFolder.Path))
{
    Directory.CreateDirectory(staticFolder.Path);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(staticFolder.Path),
    RequestPath = new PathString("/StaticFiles")
});

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();