using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using StockBrokerBot.Core.Entities;
using StockBrokerBot.Core.Persistence;

namespace StockBrokerBot.ChatbotLambda.Persistence;

public class StockMarketDynamoDBDataProvider : IStockMarketDataProvider
{
    private AmazonDynamoDBClient _dynamoDbClient;
    private DynamoDBContext _dynamoDbContext;
    
    public StockMarketDynamoDBDataProvider()
    {
        _dynamoDbClient = new AmazonDynamoDBClient();
        _dynamoDbContext = new DynamoDBContext(_dynamoDbClient);
    }
    
    public async Task<decimal> GetStockPrice(string name)
    {
        var stock = await _dynamoDbContext.LoadAsync<Stock>(name);
        return stock.Price;
    }
}