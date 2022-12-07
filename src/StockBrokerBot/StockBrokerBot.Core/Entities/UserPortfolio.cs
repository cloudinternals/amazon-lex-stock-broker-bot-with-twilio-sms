using System.Text;
using Amazon.DynamoDBv2.DataModel;

namespace StockBrokerBot.Core.Entities;

[DynamoDBTable("user-portfolio")]
public class UserPortfolio
{
    [DynamoDBHashKey]
    public string UserId { get; set; }
    public decimal AvailableCash { get; set; }
    public List<StockPortfolioItem> StockPortfolio { get; set; } = new List<StockPortfolioItem>();

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Available cash: ${AvailableCash.ToString("N2")}");

        decimal totalStockValue = 0.00m;

        if (StockPortfolio?.Count > 0)
        {
            sb.AppendLine("Your Portfolio:");
            foreach (var portfolioItem in StockPortfolio)
            {
                sb.AppendLine($"Stock: {portfolioItem.StockName}, Shares: {portfolioItem.NumberOfShares.ToString("N2")}, Average Price: ${portfolioItem.AveragePricePerShare.ToString("N2")}");
                totalStockValue += portfolioItem.NumberOfShares * portfolioItem.AveragePricePerShare;
            }
        }

        sb.AppendLine($"Total Stock Value: ${totalStockValue.ToString("N2")}");
        return sb.ToString();
    }
}