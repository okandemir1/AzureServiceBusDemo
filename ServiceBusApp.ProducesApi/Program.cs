using Microsoft.Azure.ServiceBus.Management;
using ServiceBusApp.Models;
using ServiceBusApp.ProducesApi.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<AzureService>();
//Dependency Injection ile ekleyelim s�rekli s�rekli client olu�turmaya gerek yok
//Ne zaman managementClient istersem sen git ConstantsInfo'dan ConnectionString'i al demi� olduk
builder.Services.AddSingleton<ManagementClient>(x => new ManagementClient(ConstInfo.ConnectionString));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
