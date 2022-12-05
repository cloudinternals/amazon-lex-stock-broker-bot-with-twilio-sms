namespace StockBrokerBot.Core.Exceptions;

public class InvalidStockOperationException : StockerBrokerException
{
    public InvalidStockOperationException(string message): base(message)
    {
    }
}