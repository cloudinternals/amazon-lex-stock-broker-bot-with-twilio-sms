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
        IIntentProcessor process;
        
        switch (lexEvent.SessionState.Intent.Name)
        {
            case "CheckStockPrice":
                process = new CheckStockPriceIntentProcessor();
                break;
            case "GetPortfolio":
                process = new GetPortfolioIntentProcessor();
                break;
            case "BuyStocks":
                process = new BuyStocksIntentProcessor();
                break;
            case "SellStocks":
                process = new SellStocksIntentProcessor();
                break;
            default:
                throw new Exception($"Intent with name {lexEvent.SessionState.Intent.Name} is not supported");
        }
        
        return await process.Process(lexEvent, context);
    }
}
