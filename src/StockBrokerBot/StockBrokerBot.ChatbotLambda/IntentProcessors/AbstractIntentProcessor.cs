using Amazon.Lambda.Core;
using Amazon.Lambda.LexV2Events;

namespace StockBrokerBot.ChatbotLambda.IntentProcessors;

public abstract class AbstractIntentProcessor : IIntentProcessor
{
    internal const string MESSAGE_CONTENT_TYPE = "PlainText";
    internal const string INTENT_STATE_FULFILLED = "Fulfilled";
    internal const string INTENT_STATE_FAILED = "Failed";
    internal const string DIALOG_ACTION_CLOSE = "Close";
    
    public abstract Task<LexV2Response> Process(LexV2Event lexEvent, ILambdaContext context);
    
    protected LexV2Response Close(string intentName, Dictionary<string, string> sessionAttributes, string fulfillmentState, string responseMessage)
    {
        return new LexV2Response
        {
            SessionState = new LexV2SessionState
            {
                Intent = new LexV2Intent { Name = intentName, State = fulfillmentState },
                SessionAttributes = sessionAttributes,
                DialogAction = new LexV2DialogAction  { Type = DIALOG_ACTION_CLOSE }
            },
            Messages = new List<LexV2Message>
            {
                new()
                {
                    ContentType = MESSAGE_CONTENT_TYPE,
                    Content = responseMessage
                }
            }
        };
    }
}