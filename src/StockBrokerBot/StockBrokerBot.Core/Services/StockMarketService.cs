using StockBrokerBot.Core.Persistence;

namespace StockBrokerBot.Core.Services;

public class StockMarketService : IStockMarketService
{
    private readonly IStockMarketDataProvider _stockMarketDataProvider;

    public StockMarketService(IStockMarketDataProvider stockMarketDataProvider)
    {
        _stockMarketDataProvider = stockMarketDataProvider;
    }
    
    public async Task<decimal> GetStockPrice(string stockName)
    {
        return await _stockMarketDataProvider.GetStockPrice(stockName);
    }
}