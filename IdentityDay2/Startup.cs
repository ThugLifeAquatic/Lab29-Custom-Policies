﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using IdentityDay2.Models;
using IdentityDay2.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace IdentityDay2
{
    public class Startup
    {
        //Configuration setup for dependancy injection
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie("MyCookieLogin", options =>
            //    options.AccessDeniedPath = new PathString("/Account/Forbidden/"));

            //Enable Admin-Only policy
            services.AddAuthorization(options => {
                options.AddPolicy("Admin Only", policy => policy.RequireRole("Admin"));
                options.AddPolicy("Medical", policy => policy.Requirements.Add(new MedicalOfficerRequirement()));
            });
                

            services.AddMvc();

            //Regular Db context
            services.AddDbContext<IdentityDay2Context>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("IdentityDay2Context")));

            // This context is derived from IdentityDbContext. This context is responsible for the ASPNET Identity tables in the database. 
            services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("IdentityDay2Context")));

            //Enable Identity Functionality using Crewmember model
            services.AddIdentity<CrewMember, IdentityRole>()
                   .AddEntityFrameworkStores<AppDbContext>()
                   .AddDefaultTokenProviders();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //Enable user profiles & authorization via the Identity API
            app.UseAuthentication();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Login to Create Announcements.");
            });
        }
    }
}
