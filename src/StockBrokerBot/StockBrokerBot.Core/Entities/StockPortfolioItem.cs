namespace StockBrokerBot.Core.Entities;

public class StockPortfolioItem
{
    public string StockName { get; set; }
    public decimal NumberOfShares { get; set; }
    public decimal AveragePricePerShare { get; set; }
}