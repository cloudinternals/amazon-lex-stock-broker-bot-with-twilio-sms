using Amazon.Lambda.Core;
using Amazon.Lambda.LexV2Events;
 
namespace StockBrokerBot.ChatbotLambda.IntentProcessors;
 
public interface IIntentProcessor
{
    Task<LexV2Response> Process(LexV2Event lexEvent, ILambdaContext context);
}