using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockBrokerBot.Demo.Persistence;
using StockBrokerBot.Core.Persistence;
using StockBrokerBot.Core.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<IPortfolioService, PortfolioService>();
        services.AddTransient<IStockMarketService, FluctuatingStockMarketService>();
        services.AddTransient<IPortfolioDataProvider, PortfolioJsonDataProvider>();
        services.AddTransient<IStockMarketDataProvider, StockMarketJsonDataProvider>();
    })
    .Build();

var userId = "DemoUser";

var portfolioService = host.Services.GetRequiredService<IPortfolioService>();
var stockMarketService = host.Services.GetRequiredService<IStockMarketService>();

var userPortfolio = await portfolioService.GetUserPortfolio(userId);

Console.WriteLine("--------------------------------------");
Console.WriteLine($"Getting portfolio for user {userId}");
Console.WriteLine(userPortfolio);

Console.WriteLine("--------------------------------------");
Console.WriteLine($"Checking price for Apple");
var applePrice = await stockMarketService.GetStockPrice("apple");
Console.WriteLine($"${applePrice.ToString("N2")}");

Console.WriteLine("--------------------------------------");
Console.WriteLine($"Buy 1.2 Apple stocks");
userPortfolio = await portfolioService.BuyStocks(userId, "apple", 1.2m);
Console.WriteLine($"Portfolio after the purchase:");
Console.WriteLine(userPortfolio);

Console.WriteLine("--------------------------------------");
Console.WriteLine($"Sell 0.5 Apple stocks");
userPortfolio = await portfolioService.SellStocks(userId, "apple", 0.5m);
Console.WriteLine($"Portfolio after the sell:");
Console.WriteLine(userPortfolio);

