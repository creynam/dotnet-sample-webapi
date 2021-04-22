using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using WebApi.Filters;

namespace WebApi
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

            services.AddWebApi();
            services.AddSwagger();

            // <PackageReference Include="AutoMapper" Version="10.1.1" />
            // <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="8.1.0" />
            // services.AddAutoMapper(typeof(MyHandler), typeof(Startup));

            // <PackageReference Include="MediatR" Version="9.0.0" />
            // <PackageReference Include="MediatR.Extensions.Microsoft.DependencyInjection" Version="9.0.0" />
            // services.AddMediatR(typeof(MyHandler));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample Dotnet WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("CorsPolicy");
            
            // app.UseAuthentication();
            // app.UseAuthorization();

            app.UseSerilogRequestLogging();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public static class StartupExtensions
    {
        public static void AddWebApi(this IServiceCollection services)
        {
            services.AddControllers(opts =>
            {
                opts.Filters.Add(new HttpResponseExceptionFilter());
                // options.Filters.Add(new ValidationFilter()); //FluentValidation
            });
            
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            // <PackageReference Include="FluentValidation.AspNetCore" Version="10.0.4" />
            // FluentValidation
            // .AddFluentValidation(fv =>
            // {
            //     fv.RegisterValidatorsFromAssemblyContaining<MyValidator>();
            // });
            // services.Configure<ApiBehaviorOptions>(options =>
            // {
            //     options.SuppressModelStateInvalidFilter = true;
            // });
        }
        public static void AddDatabase(this IServiceCollection services)
        {
            // services.AddAuthorization(options => 
            // {
            //     options.AddPolicy("IsRole1", pol => 
            //         pol.RequireClaim("role", "Role1", "AdminRole"));
            //     options.AddPolicy("IsAdmin", pol => 
            //         pol.RequireClaim("role", "AdminRole"));
            // });
        }

        public static void AddIdentity(this IServiceCollection services)
        {
            // services.AddIdentityCore<MyUser>(opts =>
            // {
            //     opts.Password.RequireDigit = false;
            //     opts.Password.RequireUppercase = false;
            //     opts.Password.RequireDigit = false;
            //     opts.Password.RequireNonAlphanumeric = false;
            // })
            // .AddSignInManager()
            // .AddEntityFrameworkStores<IdentityContext>();
        }

        public static void AddApplicationInsights(this IServiceCollection services)
        {
            // <PackageReference Include="Serilog.Sinks.ApplicationInsights" Version="3.1.0" />
            // if (bool.TryParse(Configuration["ApplicationInsights:EnableTelemetry"], out bool enabled) && enabled)
            // {
            //     services.AddApplicationInsightsTelemetry();
            //     services.AddApplicationInsightsTelemetryProcessor<NotFoundTelemetryProcessor>();
            // }
        }

        public static void AddJwtAuthentication(this IServiceCollection services)
        {
            // services.AddAuthentication(options =>
            // {
            //     options.DefaultAuthenticateScheme = JwtAuthenticationOptions.DefaultScheme;
            //     options.DefaultChallengeScheme = JwtAuthenticationOptions.DefaultScheme;
            // })
            // .AddJwtSupport(options => { });
        }

        public static void AddRoleBaseAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            
        }
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });

                //swagger apikey
                // c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                // {
                //     In = ParameterLocation.Header,
                //     Description = "Please insert JWT with Bearer into field",
                //     Name = "Authorization",
                //     Type = SecuritySchemeType.ApiKey
                // });
                // c.AddSecurityRequirement(new OpenApiSecurityRequirement
                // {
                //     {
                //         new OpenApiSecurityScheme
                //         {
                //            Reference = new OpenApiReference
                //            {
                //              Type = ReferenceType.SecurityScheme,
                //              Id = "Bearer"
                //            }
                //         },
                //         new string[] {}
                //     }
                // });
            });
        }
    }
}
