using StockBrokerBot.Core.Entities;
using StockBrokerBot.Core.Exceptions;
using StockBrokerBot.Core.Persistence;

namespace StockBrokerBot.Core.Services;


public class PortfolioService : IPortfolioService
{
    private readonly IStockMarketService _stockMarketService;
    private readonly IPortfolioDataProvider _dataProvider;

    public PortfolioService(IStockMarketService stockMarketService, IPortfolioDataProvider dataProvider)
    {
        _stockMarketService = stockMarketService;
        _dataProvider = dataProvider;
    }
    
    public async Task<UserPortfolio> GetUserPortfolio(string userId)
    {
        return await _dataProvider.GetUserPortfolio(userId);
    }

    public async Task<UserPortfolio> BuyStocks(string userId, string stockName, decimal numberOfShares)
    {
        var userPortfolio = await _dataProvider.GetUserPortfolio(userId);
        var currentPricePerShare = await _stockMarketService.GetStockPrice(stockName);
        var totalCostOfRequestedShares = currentPricePerShare * numberOfShares;

        if (userPortfolio.AvailableCash < totalCostOfRequestedShares)
        {
            throw new InsufficientFundsException(
                $"Insufficient funds. Available cash: ${userPortfolio.AvailableCash.ToString("N2")}, Total cost of requested shares: ${totalCostOfRequestedShares.ToString("N2")}");
        }
        
        var existingStocks = userPortfolio.StockPortfolio.FirstOrDefault(s => string.Compare(s.StockName, stockName, StringComparison.InvariantCultureIgnoreCase) == 0);
        if (existingStocks != null)
        {
            var newNumberOfShares = existingStocks.NumberOfShares + numberOfShares;
            var newAveragePricePerShare =
                ((existingStocks.AveragePricePerShare * existingStocks.NumberOfShares) +
                 (numberOfShares * currentPricePerShare)) / newNumberOfShares;
            existingStocks.NumberOfShares = newNumberOfShares;
            existingStocks.AveragePricePerShare = newAveragePricePerShare;
        }
        else
        {
            userPortfolio.StockPortfolio.Add(new StockPortfolioItem
            {
                StockName = stockName,
                NumberOfShares = numberOfShares,
                AveragePricePerShare = currentPricePerShare
            });
        }

        userPortfolio.AvailableCash -= totalCostOfRequestedShares;
        
        await _dataProvider.SaveUserPortfolio(userPortfolio);

        return userPortfolio;
    }

    public async Task<UserPortfolio> SellStocks(string userId, string stockName, decimal numberOfShares)
    {
        var userPortfolio = await _dataProvider.GetUserPortfolio(userId);
        var existingStocks = userPortfolio.StockPortfolio.FirstOrDefault(s => string.Compare(s.StockName, stockName, StringComparison.InvariantCultureIgnoreCase) == 0);
        if (existingStocks == null)
        {
            throw new InvalidStockOperationException($"{stockName} does not exist in your portfolio.");
        }

        if (existingStocks.NumberOfShares < numberOfShares)
        {
            throw new InvalidStockOperationException($"Maximum number of shares you can sell is {existingStocks.NumberOfShares}");
        }
        
        var currentPricePerShare = await _stockMarketService.GetStockPrice(stockName);
        var totalAmountOfSharesToSell = currentPricePerShare * numberOfShares;

        var newNumberOfShares = existingStocks.NumberOfShares - numberOfShares;
        var totalSpent = existingStocks.AveragePricePerShare * existingStocks.NumberOfShares - totalAmountOfSharesToSell;
        var newAveragePricePerShare = totalSpent / newNumberOfShares;
        existingStocks.NumberOfShares = newNumberOfShares;
        existingStocks.AveragePricePerShare = newAveragePricePerShare;
        
        userPortfolio.AvailableCash += totalAmountOfSharesToSell;
            
        await _dataProvider.SaveUserPortfolio(userPortfolio);

        return userPortfolio;
    }
}