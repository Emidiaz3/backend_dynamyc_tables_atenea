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

namespace ApiRestCuestionario
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(options => {
                //options.DocumentFilter<PathPrefixInsertDocumentFilter>();
            });

            var key = Encoding.ASCII.GetBytes(Configuration.GetValue<string>("SecretKey"));
            
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
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            var db = Configuration.GetConnectionString("Database");

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(db));

            services.AddCors(options =>
            {
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
                    "http://developer:8080",
                    "https://da3d-161-132-237-29.sa.ngrok.io",
                    "https://af97-161-132-237-29.sa.ngrok.io",
                    "https://dda1a3ece486.sa.ngrok.io",
                    "https://encuestas.atenealatam.com",
                    "https://encuestas.atenealatam.com/",
                    "https://encuestas.atenealatam.com:8090"
                };
                options.AddDefaultPolicy(builder => builder.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod());
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
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

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });

        }
    }
}
