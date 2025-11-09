using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UserManagement.Data;
using UserManagement.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "User Management API",
        Version = "v1",
        Description = "Operations for managing user accounts and related metadata."
    });
});

var connectionString = DatabaseConnectionStringResolver.Resolve(builder.Configuration);

builder.Services.AddDbContext<UserManagementDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1");
    });
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<UserManagementDbContext>();
    await dbContext.Database.MigrateAsync();

    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var seederLogger = loggerFactory.CreateLogger("DatabaseSeeder");
    await DatabaseSeeder.SeedAsync(dbContext, seederLogger);
}

app.MapGet("/", () => Results.Ok("User Management API is running."));
app.MapControllers();

app.Run();

public partial class Program;
