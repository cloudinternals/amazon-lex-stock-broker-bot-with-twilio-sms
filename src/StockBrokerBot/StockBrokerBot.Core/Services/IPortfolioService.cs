using StockBrokerBot.Core.Entities;

namespace StockBrokerBot.Core.Services;

public interface IPortfolioService
{
    Task<UserPortfolio> GetUserPortfolio(string userId);
    Task<UserPortfolio> BuyStocks(string userId, string stockName, decimal numberOfShares);
    Task<UserPortfolio> SellStocks(string userId, string stockName, decimal numberOfShares);
}