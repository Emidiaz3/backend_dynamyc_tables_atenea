using ApiRestCuestionario.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using ApiRestCuestionario.Utils;
using Microsoft.OpenApi.Models;

namespace ApiRestCuestionario
{
    public class StaticFolder
    {
        public string Path { get; }
        public StaticFolder(string path)
        {
            Path = path;
        }
    }
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            string pathCombination = string.IsNullOrWhiteSpace(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) ? Environment.CurrentDirectory : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            staticFolder = new StaticFolder(Path.Combine(pathCombination, "MyStaticFiles"));

        }
        public IConfiguration Configuration { get; }
        public StaticFolder staticFolder;
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(staticFolder);
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.AddTransient<IGmailSender, GmailSender>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cuestionario", Version = "v1" });
            });
            services.AddAuthentication(x =>
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetValue<string>("SecretKey"))),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });


            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Database")));
            services.AddCors(options =>
            {
               //options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
                string[] origins =
                new string[] {
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
                    "http://localhost:8092",
                    "http://developer:8080",
                    "https://da3d-161-132-237-29.sa.ngrok.io",
                    "https://af97-161-132-237-29.sa.ngrok.io",
                    "https://dda1a3ece486.sa.ngrok.io",
                    "https://encuestas1.ddigital.pe",
                    "https://encuestas.atenealatam.com",
                    "https://encuestas.atenealatam.com/",
                    "https://encuestas.atenealatam.com:8090",
                    "https://encuestasatenea.netlify.app",
                    "https://127.0.0.1:4200",
                    "https://127.0.0.1:4200/"
                };
                options.AddDefaultPolicy(builder => builder.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod());
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    //options.RoutePrefix = string.Empty;
                });
            } else
            {
                app.UseDefaultFiles();
                app.UseStaticFiles();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

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


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("/index.html");

            });

        }
    }
}
