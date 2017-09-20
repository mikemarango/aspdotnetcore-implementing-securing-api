using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MyCodeCamp
{
    public class Startup
    {
        private readonly IHostingEnvironment env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            this.env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CampContext>(ServiceLifetime.Scoped); // I think the Scoped is the default anyway
            services.AddScoped<ICampRepository, CampRepository>();
            services.AddTransient<CampDbInitializer>();
            services.AddTransient<CampIdentityInitializer>();

            services.AddAutoMapper();

            services.AddIdentity<CampUser, IdentityRole>()
                .AddEntityFrameworkStores<CampContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                            ctx.Response.StatusCode = 401;

                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                            ctx.Response.StatusCode = 403;

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthentication();

            services.AddCors(cfg =>
            {
                cfg.AddPolicy("Wildermuth", bldr =>
                {
                    bldr.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("http://wildermuth.com");
                });

                cfg.AddPolicy("AnyGET", bldr =>
                {
                    bldr.AllowAnyHeader()
                        .WithMethods("GET")
                        .AllowAnyOrigin();
                });
            });

            services.AddMvc(opt =>
                {
                    opt.Filters.Add(new RequireHttpsAttribute());

                    if (!env.IsProduction())
                        opt.SslPort = 44388;
                })
                .AddJsonOptions(opt =>
                {
                    // Avoid looping the entity framework properties: parent > child
                    opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, CampDbInitializer seeder, CampIdentityInitializer identitySeeder)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            // app.UseIdentity() is obsolete.
            // Also, this must be added before UseMvc()
            app.UseAuthentication();

            app.UseMvc();

            seeder.Seed().Wait();
            identitySeeder.Seed().Wait();
        }
    }
}
