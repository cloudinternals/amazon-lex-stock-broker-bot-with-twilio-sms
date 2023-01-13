using Amazon.Lambda.Core;
using Amazon.Lambda.LexV2Events;
using StockBrokerBot.ChatbotLambda.IntentProcessors;
 
// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
 
namespace StockBrokerBot.ChatbotLambda;
 
public class Function
{
    public async Task<LexV2Response> FunctionHandler(LexV2Event lexEvent, ILambdaContext context)
    {
        AbstractIntentProcessor process = lexEvent.SessionState.Intent.Name switch
        {
            "CheckStockPrice" => new CheckStockPriceIntentProcessor(),
            "GetPortfolio" => new GetPortfolioIntentProcessor(),
            "BuyStocks" => new BuyStocksIntentProcessor(),
            "SellStocks" => new SellStocksIntentProcessor(),
            _ => throw new Exception($"Intent with name {lexEvent.SessionState.Intent.Name} is not supported")
        };
        
        return await process.Process(lexEvent, context);
    }
}