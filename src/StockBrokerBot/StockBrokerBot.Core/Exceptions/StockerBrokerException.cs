namespace StockBrokerBot.Core.Exceptions;

public class StockerBrokerException : Exception
{
    public StockerBrokerException(string message): base(message)
    {
    }   
}