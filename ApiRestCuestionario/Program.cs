using ApiRestCuestionario.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options => {
    //options.DocumentFilter<PathPrefixInsertDocumentFilter>();
});

var key = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("SecretKey"));
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
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var db = builder.Configuration.GetConnectionString("Database");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(db));

builder.Services.AddCors(options =>
{
    string[] origins =
    [
        "http://localhost:4200", 
        "http://localhost:8084", 
        "http://cjp.ddigital.pe", 
        "https://cjp.ddigital.pe", 
        "https://127.0.0.1", 
        "http://localhost:8200", 
        "http://localhost:9095", 
        "http://localhost:9096", 
        "http://localhost:80", 
        "http://localhost:81", 
        "http://localhost", 
        "http://localhost:8080", 
        "http://localhost:8090", 
        "http://developer:8080", 
        "https://da3d-161-132-237-29.sa.ngrok.io", 
        "https://af97-161-132-237-29.sa.ngrok.io", 
        "https://dda1a3ece486.sa.ngrok.io", 
        "https://encuestas.atenealatam.com", 
        "https://encuestas.atenealatam.com/", 
        "https://encuestas.atenealatam.com:8090"
    ];
    options.AddDefaultPolicy(builder => builder.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UsePathBase(new PathString("/api/v1"));

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();