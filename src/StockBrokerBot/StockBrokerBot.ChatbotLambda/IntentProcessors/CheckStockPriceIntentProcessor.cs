using Amazon.Lambda.Core;
using Amazon.Lambda.LexV2Events;
using StockBrokerBot.ChatbotLambda.Persistence;
using StockBrokerBot.Core.Services;

namespace StockBrokerBot.ChatbotLambda.IntentProcessors;

public class CheckStockPriceIntentProcessor : AbstractIntentProcessor
{
    public override async Task<LexV2Response> Process(LexV2Event lexEvent, ILambdaContext context)
    {
        var slots = lexEvent.SessionState.Intent.Slots;
        var requestedStockName = slots["stockName"].Value.InterpretedValue;

        var stockMarketService = new FluctuatingStockMarketService(new StockMarketDynamoDBDataProvider());
        
        var price = await stockMarketService.GetStockPrice(requestedStockName);
        var responseMessage = $"Current price of {requestedStockName} is ${price.ToString("N2")}";
        return Close(
            lexEvent.SessionState.Intent.Name,
            lexEvent.SessionState.SessionAttributes,
            INTENT_STATE_FULFILLED,
            responseMessage
        );
    }
}