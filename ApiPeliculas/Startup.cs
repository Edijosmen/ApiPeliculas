using ApiPeliculas.Data;
using ApiPeliculas.Herpers;
using ApiPeliculas.Profiles;
using ApiPeliculas.Repository;
using ApiPeliculas.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiPeliculas
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AplicationsDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IPeliculaRepository, PeliculaRespository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddAutoMapper(typeof(PeliculasMapper));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option =>
                {
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("AppSettings:Token").Value))
                    };
                });
            //Damos soporte para CORS
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                  builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
                                  });
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("ApiPeliuclaCategoria", new OpenApiInfo { Title = "Categoria", Version = "v1" });
                c.SwaggerDoc("ApiPelicula", new OpenApiInfo { Title = "ApiPeliculas", Version = "v1" });
                c.SwaggerDoc("ApiPeliculaUsuarios", new OpenApiInfo { Title = "Usuarios", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Atenticación JWT (Bearer)",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, new List<string>()
                    }
                  });

                var archivoXmlComentarios = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var rutaApiComentarios = Path.Combine(AppContext.BaseDirectory, archivoXmlComentarios);
                c.IncludeXmlComments(rutaApiComentarios);

            });
            
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if(error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
             
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/ApiPeliuclaCategoria/swagger.json", "Categoria");
                c.SwaggerEndpoint("/swagger/ApiPelicula/swagger.json", "Pelicula");
                c.SwaggerEndpoint("/swagger/ApiPeliculaUsuarios/swagger.json", "Usuarios");

            });
            app.UseHttpsRedirection();

            app.UseRouting();
            //damos soporte a CORS
            app.UseCors(MyAllowSpecificOrigins);
            //autorización y autenticación
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

           
        }
    }
}
