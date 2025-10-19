var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Optional: Swagger only for development
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// Serve static files from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

// Optional: Swagger UI for development only
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// Remove HTTPS redirection for Render (Render automatically handles HTTPS)
// app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();
