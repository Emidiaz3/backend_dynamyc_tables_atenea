using ApiRestCuestionario.Context;
using System.Text;
using Microsoft.Extensions.FileProviders;
using ApiRestCuestionario.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using angular.Server.Model;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.SqlServer;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

// Configurar Firebase
//FirebaseApp.Create(new AppOptions()
//{
//    Credential = GoogleCredential.FromFile("C:\\Users\\Administrador\\Documents\\Intslla\\listenerapp-6660a-firebase-adminsdk-yjv88-9fc1c6c2c0.json"),
//});

try
{
    string firebaseConfigPath = builder.Configuration.GetValue<string>("FirebaseConfigPath");
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile(firebaseConfigPath),
    });
    Console.WriteLine("Firebase initialized successfully.");

}
catch (Exception ex)
{
    // Log the error
    Console.WriteLine($"Error initializing Firebase: {ex.Message}");
    // Optionally, you might want to throw the exception if Firebase is critical for your application
    // throw;
}

//string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "atenea-notification-push-firebase-adminsdk.json");

//FirebaseApp.Create(new AppOptions()
//{
//    Credential = GoogleCredential.FromFile(path),
//});


string pathCombination = string.IsNullOrWhiteSpace(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) ? Environment.CurrentDirectory : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
StaticFolder staticFolder = new(Path.Combine(pathCombination, "MyStaticFiles"));

builder.Services.AddSingleton(staticFolder);

// Configuración de EmailSettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddTransient<IGmailSender, GmailSender>();

// Registrar NotificationService
builder.Services.AddScoped<NotificationService>();

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

// Registro de HttpClient
builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    string[] origins = ["https://*.atenealatam.com", "https://*.ddigital.pe", "https://localhost:4200", "http://localhost:4200", "https://*.netlify.app", "https://127.0.0.1:4200", "https://127.0.0.1:62309"];
    options.AddDefaultPolicy(builder => builder.SetIsOriginAllowedToAllowWildcardSubdomains().WithOrigins(origins).AllowAnyHeader().AllowAnyMethod());
});

// Configurar Hangfire
builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseDefaultTypeSerializer()
          .UseSqlServerStorage(builder.Configuration.GetConnectionString("Database"), new SqlServerStorageOptions
          {
              CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
              SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
              QueuePollInterval = TimeSpan.Zero,
              UseRecommendedIsolationLevel = true,
              DisableGlobalLocks = true
          });
});

builder.Services.AddHangfireServer();

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

app.UseHangfireDashboard(); // Hangfire Dashboard

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
