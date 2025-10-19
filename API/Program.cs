using Npgsql.EntityFrameworkCore.PostgreSQL;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ✅ Serve static files and default index.html from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();



// ✅ Redirect root (/) to /ctrl only once
app.MapGet("/", async context =>
{
    // Redirect to /ctrl/ only if not already there
    if (!context.Request.Path.StartsWithSegments("/ctrl"))
    {
        context.Response.Redirect("/ctrl/");
    }
    else
    {
        await context.Response.SendFileAsync("wwwroot/ctrl/index.html");
    }
});

// ✅ Map API controllers
app.MapControllers();

// ❌ Remove fallback — you don’t need it since you’re manually redirecting
// app.MapFallbackToFile("index.html");

// Run the app
app.Run();
