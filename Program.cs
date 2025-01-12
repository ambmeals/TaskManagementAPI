using Microsoft.AspNetCore.Diagnostics;
using TaskManagementAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITaskRepository, TaskRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "https://your-vercel-app-url.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var exceptionDetails = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        var response = new
        {
            message = "An unexpected error occurred.",
            details = exceptionDetails?.Message
        };

        await context.Response.WriteAsJsonAsync(response).ConfigureAwait(false);
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 403)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { message = "Access forbidden. Please check your permissions or CORS configuration." });
    }
});

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();