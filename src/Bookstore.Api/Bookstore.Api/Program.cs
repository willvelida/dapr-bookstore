using Bookstore.Api.Services;
using Bookstore.Api.Services.Interfaces;
using Microsoft.ApplicationInsights.Extensibility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDaprClient();
builder.Services.AddSingleton<IBookstoreService, BookstoreService>();
builder.Services.AddControllers();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.Configure<TelemetryConfiguration>((o) =>
{
    o.TelemetryInitializers.Add(new AppInsightsTelemetryInitializer());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
