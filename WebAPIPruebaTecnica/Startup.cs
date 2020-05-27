using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebAPIPruebaTecnica.Contexts;
using WebAPIPruebaTecnica.Entities;
using WebAPIPruebaTecnica.Models;

namespace WebAPIPruebaTecnica
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(configuration =>
            {
                configuration.CreateMap<ApplicationUser, UserInfo>();
                configuration.CreateMap<Cliente, ClienteDTO>();
                configuration.CreateMap<ClienteCreacionDTO, Cliente>().ReverseMap();
                configuration.CreateMap<Cuenta, CuentasClienteDTO>().ReverseMap();
                configuration.CreateMap<Cuenta, CuentaDTO>();
                configuration.CreateMap<CuentaCreacionDTO, Cuenta>().ReverseMap();
                configuration.CreateMap<Transaccion, TransaccionDTO>();
                configuration.CreateMap<TransaccionCreacionDTO, Transaccion>().ReverseMap();
                configuration.CreateMap<TransaccionCreacionDTO[], Transaccion>();
            }, typeof(Startup));

            services.AddDbContext<ApplicationDBContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"));
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = false,
                         ValidateAudience = false,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["JWT:key"])),
                         ClockSkew = TimeSpan.Zero
                     });
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders();

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "V1",
                    Title = "Mi Web API",
                    Description = "Esta es una descripción del Web API. Pruebas realizadas en PostMan",
                    TermsOfService = new Uri("https://www.google.com/"),
                    License = new OpenApiLicense()
                    {
                        Name = "MIT",
                        Url = new Uri("https://www.google.com/")
                    },
                    Contact = new OpenApiContact()
                    {
                        Name = "Nombre Creador",
                        Email = "correoCreador@dominio.com",
                        Url = new Uri("https://www.google.com/")
                    }
                });

                config.SwaggerDoc("v2", new OpenApiInfo { Title = "Web API Prueba Tecnica", Version = "V1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);

            });

            services.AddControllers().
                AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API V1");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
