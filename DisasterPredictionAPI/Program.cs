using DisasterPredictionAPI.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using System;
using StackExchange.Redis;
using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Hosting.Server;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using DisasterPredictionAPI.Models;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var keyVaultUrl2 = "https://disasterpredictionsecret.vault.azure.net/";




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsProduction())
{
    var keyVaultURL = builder.Configuration.GetSection("KeyVault:KeyVaultURL");
    var keyVaultClientID = builder.Configuration.GetSection("KeyVault:ClientId");
    var keyVaultClientSecret = builder.Configuration.GetSection("KeyVault:ClientSecret");
    var keyVaultDiretoryID = builder.Configuration.GetSection("KeyVault:DiretoryID");
    var credential = new ClientSecretCredential(keyVaultDiretoryID.Value!.ToString(), keyVaultClientID.Value!.ToString(), keyVaultClientSecret.Value!.ToString());
    var client = new SecretClient(new Uri(keyVaultUrl2), credential);


    KeyVaultSecret secretDbAzure = client.GetSecret("DbAzure");
    string dbAzure = secretDbAzure.Value.Trim();

    KeyVaultSecret secretRedis = client.GetSecret("RedisAzure");
    string Redis = secretRedis.Value;



    KeyVaultSecret secretLine= client.GetSecret("LineSecretKey");
    string line = secretLine.Value;

    KeyVaultSecret secretLineChanel = client.GetSecret("LineChanelSecretKey");
    string lineChanel = secretLineChanel.Value;



    KeyVaultSecret secretWeather = client.GetSecret("WeatherSecretKey");
    string weather = secretWeather.Value;


    KeyAPIs key = new KeyAPIs(weather, line , lineChanel);


    builder.Services.AddSingleton(key);

    builder.Services.AddDbContext<DBDEV>(options => options.UseSqlServer(dbAzure));
    builder.Services.AddStackExchangeRedisCache(options =>
    {


        options.Configuration = Redis;
        options.InstanceName = "newphoto-cache";
    });




    // ***** get data from appsettings.json *****
    //builder.Services.AddDbContext<DBDEV>(options => options.UseSqlServer(builder.Configuration.GetValue<string>(dbAzure)); 
    //builder.Services.AddStackExchangeRedisCache(options =>
    //{


    //    options.Configuration = builder.Configuration.GetConnectionString(builder.Configuration.GetConnectionString(dbAzure));
    //    options.InstanceName = "newphoto-cache";
    //});


}


//if (builder.Environment.IsDevelopment())
//{
//    builder.Services.AddDbContext<DBDEV>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("LocalConnection")));

//}





builder.Services.AddHttpClient();






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

