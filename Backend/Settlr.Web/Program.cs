using Microsoft.OpenApi.Models;
using Settlr.Web.Extension;
using Settlr.Web.Middlewares;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// --- 1. Service Registration Container ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger to handle Bearer Authentication in its UI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Settlr API",
        Version = "v1",
        Description = "Core API for the Settlr expense-sharing application"
    });

    // This block tells Swagger how to prompt the user for the JWT Token
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Use custom extension methods (found in Settlr.Web.Extension) to keep Program.cs clean
builder.Services.AddApplicationServices(builder.Configuration); // Registers Repos, Services, and DB
builder.Services.AddJwtAuthentication(builder.Configuration);  // Configures JWT Bearer validation logic
builder.Services.AddSignalR(); // Register SignalR

// Global CORS Policy: Allow the React frontend to communicate with this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

WebApplication app = builder.Build();

// --- 2. HTTP Request Pipeline (Middleware) ---
// Order matters here!

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Settlr API V1");
    });
}

// 1. Trap all unhandled exceptions and return consistent JSON responses
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

// 2. Enable CORS before Auth
app.UseCors("AllowAll");

// 3. Standard Authentication & Authorization sequence
app.UseAuthentication();
app.UseAuthorization();

// 4. Map endpoints to Controllers and Hubs
app.MapControllers();
app.MapHub<SettlrHub>("/hubs/settlr");

app.Run();

