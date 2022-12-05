using StockBrokerBot.Core.Persistence;

namespace StockBrokerBot.Core.Services;

public class FluctuatingStockMarketService : IStockMarketService
{
    private readonly IStockMarketDataProvider _stockMarketDataProvider;
    private const decimal FLUCTUATION_RATE = 2.00m;    
    
    public FluctuatingStockMarketService(IStockMarketDataProvider stockMarketDataProvider)
    {
        _stockMarketDataProvider = stockMarketDataProvider;
    }
    
    public async Task<decimal> GetStockPrice(string stockName)
    {
       var actualPrice = await _stockMarketDataProvider.GetStockPrice(stockName);
       
       var random = new Random((int) DateTime.UtcNow.Ticks);
       
       var fluctuation = Math.Floor(actualPrice * FLUCTUATION_RATE / 100);
       var min = (int)(actualPrice - fluctuation);
       var max = (int)(actualPrice + fluctuation);
       
       var integerPart = random.Next(min, max);
       var decimalPart = random.Next(0, 100);
       
       var newPrice = integerPart + (decimal)decimalPart / 100;
       return newPrice;
    }
}