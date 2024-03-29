using Amazon.Lambda.Core;
using Amazon.Lambda.LexV2Events;
using StockBrokerBot.ChatbotLambda.Persistence;
using StockBrokerBot.Core.Services;
 
namespace StockBrokerBot.ChatbotLambda.IntentProcessors;
 
public class SellStocksIntentProcessor : AbstractIntentProcessor
{
    public override async Task<LexV2Response> Process(LexV2Event lexEvent, ILambdaContext context)
    {
        var slots = lexEvent.SessionState.Intent.Slots;
        var requestedStockName = slots["stockName"].Value.InterpretedValue;
        var numberOfShares =  decimal.Parse(slots["numberOfShares"].Value.InterpretedValue);
        
        var userId = lexEvent.SessionId;
        var userPortfolioService = new PortfolioService(new FluctuatingStockMarketService(new StockMarketDynamoDBDataProvider()), new PortfolioDynamoDBDataProvider());
 
        try
        {
            var updatedPortfolio = await userPortfolioService.SellStocks(userId, requestedStockName, numberOfShares);
            var responseMessage = $"Your request has been fulfilled. {updatedPortfolio}";
            return Close(
                lexEvent.SessionState.Intent.Name,
                lexEvent.SessionState.SessionAttributes,
                IntentStateFulfilled,
                responseMessage
            );
        }
        catch (Exception e)
        {
            var responseMessage = $"Error while selling stock: {requestedStockName}. {e.Message}. Call us at +0800 555-555 if the problem persists.";
            return Close(
                lexEvent.SessionState.Intent.Name,
                lexEvent.SessionState.SessionAttributes,
                IntentStateFailed,
                responseMessage
            );
        }
    }
}