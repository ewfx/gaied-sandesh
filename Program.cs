using GenAIED_Sandesh.Interfaces;
using GenAIED_Sandesh.Models;
using GenAIED_Sandesh.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<List<InputData>>(builder.Configuration.GetSection("BankingQueries"));
builder.Services.AddSingleton<IModelTrainer, ModelTrainer>();
builder.Services.AddSingleton<IEmailExtractorService,EmailExtractorService>();
builder.Services.AddSingleton<ILabelMapper, LabelMapper>();
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
