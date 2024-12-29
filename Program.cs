using PGManagementService.BusinessLogic;
using PGManagementService.Data;
using PGManagementService.Interfaces;
using PGManagementService.Models;
using RoleBasedAuthExample.Data;
using PGManagementService.Data.DTO;
using PGManagementService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Serilog;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);



// Add DbContext and configure SQL Server connection
builder.Services.AddDbContext<PGManagementServiceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"))
    //.LogTo(Console.WriteLine, LogLevel.Information)
    );

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<PGManagementServiceDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<EmailService>();
builder.Services.AddScoped<IAdminBL, AdminBL>();
builder.Services.AddScoped<IRegistrationBL, RegistrationBL>();

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Add the client URL here
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddRazorPages();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PG Management API", Version = "v1" });

    // Add security definition for bearer token authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Description = "Enter 'Bearer' followed by a space and your token."
    });

    // Enforce authentication for all endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "domain",
        ValidAudience = "domain",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bhargavkumarchallaPGManagementService"))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Member", policy => policy.RequireRole("Member"));
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
app.UseCors("AllowAll");


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
