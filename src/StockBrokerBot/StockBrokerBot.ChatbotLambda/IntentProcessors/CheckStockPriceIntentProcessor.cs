// using Amazon.Lambda.Core;
// using Amazon.Lambda.LexV2Events;
//
// namespace StockBrokerBot.ChatbotLambda.IntentProcessors;
//
// public class CheckStockPriceIntentProcessor : AbstractIntentProcessor
// {
//     public override async Task<LexV2Response> Process(LexV2Event lexEvent, ILambdaContext context)
//     {
//         var slots = lexEvent.SessionState.Intent.Slots;
//         var requestedStockName = slots["stockName"].Value.InterpretedValue;
//         
//         var stockMarketService = new JsonFileStockMarketService();
//
//         try
//         {
//             var price = await stockMarketService.GetStockPrice(requestedStockName);
//             var responseMessage = $"Current price of {requestedStockName} is ${price.ToString("N2")}";
//             return Close(
//                 lexEvent.SessionState.Intent.Name,
//                 lexEvent.SessionState.SessionAttributes,
//                 INTENT_STATE_FULFILLED,
//                 responseMessage
//             );
//         }
//         catch (Exception e)
//         {
//             var responseMessage = $"Error while getting stock price: {requestedStockName}. Please check the stock name you're requesting. Call us at +0800 555-555 if the problem persists.";
//             return Close(
//                 lexEvent.SessionState.Intent.Name,
//                 lexEvent.SessionState.SessionAttributes,
//                 INTENT_STATE_FAILED,
//                 responseMessage
//             );
//         }
//     }
// }