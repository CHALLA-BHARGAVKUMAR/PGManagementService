using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PGManagementService.BusinessLogic;
using PGManagementService.Data;
using PGManagementService.Interfaces;
using PGManagementService.Models;
using RoleBasedAuthExample.Data;
using Serilog;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using FluentValidation;
using System.Reflection;
using FluentValidation.AspNetCore;
using PGManagementService.Validators;
using Microsoft.AspNetCore.Mvc;
using PGManagementService.Data.DTO;
using PGManagementService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext and configure SQL Server connection
builder.Services.AddDbContext<PGManagementServiceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<PGManagementServiceDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<EmailService>();
builder.Services.AddScoped<IAdminBL, AdminBL>();

// Configure Serilog using appsettings.json
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Add services to the container
builder.Services.AddControllersWithViews()
    .AddFluentValidation(fv =>
    {
        // Automatically register all validators from the current assembly
        fv.RegisterValidatorsFromAssemblyContaining<Program>();
    })
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.ReferenceHandler = null;
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(ms => ms.Value.Errors.Count > 0)
            .Select(ms => new
            {
                Field = ms.Key,
                Messages = ms.Value.Errors.Select(e => e.ErrorMessage)
            });

        var response = new ApiResponse
        {
            Result = "false",
            Error = errors
        };

return new BadRequestObjectResult(response);
    };
});

builder.Services.AddRazorPages();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PG Management API", Version = "v1" });
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

// Apply database initialization at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.Initialize(services, userManager, roleManager);
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


// Enable middleware to serve generated Swagger as a JSON endpoint
app.UseSwagger();

// Enable middleware to serve Swagger UI (HTML, JS, CSS, etc.)
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PG Management API v1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

// Map controller routes for MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Map routes for API controllers
app.MapControllers();

app.Run();
