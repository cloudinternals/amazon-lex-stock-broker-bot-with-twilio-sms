namespace StockBrokerBot.Core.Persistence;

public interface IStockMarketDataProvider
{
    Task<decimal> GetStockPrice(string name);
}