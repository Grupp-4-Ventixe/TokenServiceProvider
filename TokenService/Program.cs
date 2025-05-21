using Microsoft.OpenApi.Models;
using TokenService.Services;


var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddEndpointsApiExplorer();

/* Swagger med JWT-support
   Denna konfiguration genererades med hjälp av ChatGPT-4o
   Lägger till en "Authorize"-knapp i Swagger UI som möjliggör testning av endpoints med JWT.
   1. Definierar säkerhetsdefinition för Bearer-token
   2. Tillämpas globalt via AddSecurityRequirement */
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "TokenService API", Version = "v1" });
    c.EnableAnnotations();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Klistra in endast din JWT-token här. Swagger lägger automatiskt till 'Bearer'."
    });

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
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();


app.Run();
