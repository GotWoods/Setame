// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Setame.ConfigurationProvider;


Console.WriteLine("Hello, Waiting on server to start up");

Thread.Sleep(5000);

var builder = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddSetame("Demo", "Dev", new Uri("https://localhost:7219"), "bfmfmy2eg69");
    //.AddJsonFile("appsettings.json");

var configuration = builder.Build();


var mySetting = configuration["ES.Logging"];
Console.WriteLine(mySetting);

Console.WriteLine("Press any key to fetch again");
Console.ReadKey();

mySetting = configuration["ES.Logging"];
Console.WriteLine(mySetting);

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
