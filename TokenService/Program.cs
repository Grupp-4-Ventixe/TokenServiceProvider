using Microsoft.OpenApi.Models;
using TokenService.Services;


var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddEndpointsApiExplorer();

/* Swagger med JWT-support
   Denna konfiguration genererades med hj�lp av ChatGPT-4o
   L�gger till en "Authorize"-knapp i Swagger UI som m�jligg�r testning av endpoints med JWT.
   1. Definierar s�kerhetsdefinition f�r Bearer-token
   2. Till�mpas globalt via AddSecurityRequirement */
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
        Description = "Klistra in endast din JWT-token h�r. Swagger l�gger automatiskt till 'Bearer'."
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
