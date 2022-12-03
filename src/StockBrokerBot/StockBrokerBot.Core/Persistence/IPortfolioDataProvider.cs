using StockBrokerBot.Core.Entities;

namespace StockBrokerBot.Core.Persistence;

public interface IPortfolioDataProvider
{
    Task<UserPortfolio> GetUserPortfolio(string userId);
    Task SaveUserPortfolio(UserPortfolio userPortfolio);
    
    // Task<UserPortfolio> BuyStocks(string userId, string stockName, decimal numberOfShares, decimal pricePerShare);
    // Task<UserPortfolio> RemoveStocks(string userId, string stockName, decimal numberOfShares);
}