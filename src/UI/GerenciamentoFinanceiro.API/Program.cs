using System.Text.Json.Serialization;
using GerenciamentoFinanceiro.Application.Interfaces;
using GerenciamentoFinanceiro.Application.Services;
using GerenciamentoFinanceiro.PostgreSQL;
using GerenciamentoFinanceiro.Application;

var builder = WebApplication.CreateBuilder(args);

// --- Controllers + JSON config ---
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// --- Swagger ---
builder.Services.AddOpenApi();

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddPostgreSQL();
builder.Services.AddApplication();

var app = builder.Build();

// --- Middleware Pipeline ---
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "GerenciamentoFinanceiro.API");
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.Run();
