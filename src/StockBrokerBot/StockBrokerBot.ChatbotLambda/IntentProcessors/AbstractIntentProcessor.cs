using Amazon.Lambda.Core;
using Amazon.Lambda.LexV2Events;
 
namespace StockBrokerBot.ChatbotLambda.IntentProcessors;
 
public abstract class AbstractIntentProcessor
{
    internal const string MessageContentType = "PlainText";
    internal const string IntentStateFulfilled = "Fulfilled";
    internal const string IntentStateFailed = "Failed";
    internal const string DialogActionClose = "Close";
    
    public abstract Task<LexV2Response> Process(LexV2Event lexEvent, ILambdaContext context);
    
    protected LexV2Response Close(string intentName, Dictionary<string, string> sessionAttributes, string fulfillmentState, string responseMessage)
    {
        return new LexV2Response
        {
            SessionState = new LexV2SessionState
            {
                Intent = new LexV2Intent { Name = intentName, State = fulfillmentState },
                SessionAttributes = sessionAttributes,
                DialogAction = new LexV2DialogAction  { Type = DialogActionClose }
            },
            Messages = new List<LexV2Message>
            {
                new()
                {
                    ContentType = MessageContentType,
                    Content = responseMessage
                }
            }
        };
    }
}