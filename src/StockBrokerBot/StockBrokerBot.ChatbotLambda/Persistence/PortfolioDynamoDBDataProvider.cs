using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using StockBrokerBot.Core.Entities;
using StockBrokerBot.Core.Persistence;
 
namespace StockBrokerBot.ChatbotLambda.Persistence;
 
public class PortfolioDynamoDBDataProvider : IPortfolioDataProvider
{
    private AmazonDynamoDBClient _dynamoDbClient;
    private DynamoDBContext _dynamoDbContext;
 
    public PortfolioDynamoDBDataProvider()
    {
        _dynamoDbClient = new AmazonDynamoDBClient();
        _dynamoDbContext = new DynamoDBContext(_dynamoDbClient);
    }
    
    public async Task<UserPortfolio> GetUserPortfolio(string userId)
    {
        return await _dynamoDbContext.LoadAsync<UserPortfolio>(userId);
    }
 
    public async Task SaveUserPortfolio(UserPortfolio userPortfolio)
    {
        await _dynamoDbContext.SaveAsync(userPortfolio);
    }
}