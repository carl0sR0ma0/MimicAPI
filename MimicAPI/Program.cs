using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.V1.Repositories;
using MimicAPI.V1.Repositories.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(cfg =>
{
    cfg.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    cfg.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "MimicAPI - V1",
        Version = "v1"
    });

});

builder.Services.AddControllers();

var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new DTOMapperProfile());
});

IMapper mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddDbContext<MimicContext>(opt =>
{
    opt.UseSqlite("Data Source=Database\\Mimic.db");
});

builder.Services.AddScoped<IWordRepository, WordRepository>();
builder.Services.AddApiVersioning(cfg =>
{
    cfg.ReportApiVersions = true;
    cfg.AssumeDefaultVersionWhenUnspecified = true;
    cfg.DefaultApiVersion = new ApiVersion(1, 0);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(cfg =>
    {
        cfg.SwaggerEndpoint("/swagger/v1/swagger.json", "MimicAPI");
    });
}

app.MapControllers();

app.Run();
