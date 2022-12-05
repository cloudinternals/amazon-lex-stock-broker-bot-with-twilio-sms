namespace StockBrokerBot.Core.Exceptions;

public class InsufficientFundsException : StockerBrokerException
{
    public InsufficientFundsException(string message): base(message)
    {
    }
}