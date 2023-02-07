using itu_minitwit.Server.Database;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<MinitwitContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ToDo API",
        Description = "An ASP.NET Core Web API for managing ToDo items",
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // var User1 = new User() { Username = "user1", Email = "user1@mail.com", PwHash = "asdjl", Follows = new List<User>(), Followers = new List<User>()};
    // var User2 = new User() { Username = "user2", Email = "user2@mail.com", PwHash = "asdjl", Follows = new List<User>(), Followers = new List<User>() };
    // var User3 = new User() { Username = "user3", Email = "user3@mail.com", PwHash = "asdjl", Follows = new List<User>(),Followers = new List<User>()   };
    
    using (var scope = 
           app.Services.CreateScope())
    using (var context = scope.ServiceProvider.GetService<MinitwitContext>())
    {
        
        context.Database.Migrate();
        // context.Users.Add(User1);
        // context.Users.Add(User2);
        // context.Users.Add(User3);
        // User1.Follows.Add(User2);
        // User1.Follows.Add(User3);
        // context.SaveChanges();
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

app.UseRouting();

app.MapRazorPages();
app.MapControllers();

app.Run();

