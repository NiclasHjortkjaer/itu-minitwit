using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniTwit.Database;
using MiniTwit.Other_Services;
using MiniTwit.Repositories;
using Prometheus;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

//Add logger
builder.Host.UseSerilog((ctx, lc) =>
{
    if (builder.Environment.IsDevelopment()) lc.MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .WriteTo.Console();
    else lc.MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri($"http://{Environment.GetEnvironmentVariable("ELASTICSEARCH_HOST")}:9200"))
    {
        AutoRegisterTemplate = true,
        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
        IndexFormat = "minitwit"
    });
});

// Add services to the container.
builder.Services.AddDbContext<MiniTwitContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    if ((Environment.GetEnvironmentVariable("IGNORE_ANTIFORGERY_TOKEN") ?? "False") == "True")
        options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
});
builder.Services.AddControllers();
builder.Services.AddSignalR();


builder.Services.AddHostedService<MetricWorker>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.LoginPath = "/login";
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAllForDevelopment",
        policyBuilder =>
        {
            policyBuilder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

using (var scope = app.Services.CreateScope())
using (var context = scope.ServiceProvider.GetService<MiniTwitContext>())
{
    context.Database.Migrate();
    if (app.Environment.IsDevelopment()) context.SeedDatabase();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAllForDevelopment");
}

app.UseHttpsRedirection();

app.UseHttpMetrics();
app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UseStaticFiles();

app.MapMetrics();
app.MapRazorPages();
app.MapControllers();
app.MapHub<TwitHub>("/twithub");

app.Run();