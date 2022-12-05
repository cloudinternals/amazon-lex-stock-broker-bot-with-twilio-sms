using StockBrokerBot.Core.Entities;

namespace StockBrokerBot.Core.Persistence;

public interface IPortfolioDataProvider
{
    Task<UserPortfolio> GetUserPortfolio(string userId);
    Task SaveUserPortfolio(UserPortfolio userPortfolio);
}