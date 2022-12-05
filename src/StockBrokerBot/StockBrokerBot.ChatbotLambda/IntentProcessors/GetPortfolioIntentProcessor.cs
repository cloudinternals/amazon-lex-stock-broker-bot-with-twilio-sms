using Amazon.Lambda.Core;
using Amazon.Lambda.LexV2Events;
using StockBrokerBot.ChatbotLambda.Persistence;
using StockBrokerBot.Core.Services;

namespace StockBrokerBot.ChatbotLambda.IntentProcessors;

public class GetPortfolioIntentProcessor : AbstractIntentProcessor
{
    public override async Task<LexV2Response> Process(LexV2Event lexEvent, ILambdaContext context)
    {
        var userId = lexEvent.SessionId;
        
        var userPortfolioService = new PortfolioService(new FluctuatingStockMarketService(new StockMarketDynamoDBDataProvider()), new PortfolioDynamoDBDataProvider());
        var userPortfolio = await userPortfolioService.GetUserPortfolio(userId);

        return Close(
            lexEvent.SessionState.Intent.Name,
            lexEvent.SessionState.SessionAttributes,
            INTENT_STATE_FULFILLED,
            userPortfolio.ToString()
        );
    }
}