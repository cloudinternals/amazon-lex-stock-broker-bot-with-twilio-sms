namespace StockBrokerBot.Core.Services;

public interface IStockMarketService
{
    Task<decimal> GetStockPrice(string stockName);
}