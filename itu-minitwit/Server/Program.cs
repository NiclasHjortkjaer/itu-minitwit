using AutoMapper;
using itu_minitwit.Server.Database;
using itu_minitwit.Server.Repositories;
using ituminitwit.Server.Interfaces.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDbContext<MinitwitContext>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add identity for authentication
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<MinitwitContext>();

Console.WriteLine("__________");
Console.WriteLine("Running in development: " + builder.Environment.IsDevelopment());
Console.WriteLine("__________");

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddIdentityServer()
        .AddApiAuthorization<User, MinitwitContext>().AddSigningCredentials();
}
else
{
    builder.Services.AddIdentityServer()
        .AddApiAuthorization<User, MinitwitContext>();
}

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Mini Twitter API",
        Description = "An ASP.NET Core Web API for managing Mini Twitter",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
});

// Auto Mapper Configurations
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new ituminitwit.Server.Database.MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Logging
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.WebHost.UseStaticWebAssets();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    using (var context = scope.ServiceProvider.GetService<MinitwitContext>())
    {
        context.Database.Migrate();
        context.SeedDatabase();
    }

    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });

    app.UseSwagger(options =>
    {
        options.SerializeAsV2 = true;
    });
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();


// Order of these is very important!
app.UseIdentityServer();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

Console.WriteLine("Configuration complete");

app.Run();

