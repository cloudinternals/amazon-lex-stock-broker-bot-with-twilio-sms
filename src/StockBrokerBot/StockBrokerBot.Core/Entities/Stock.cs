using Amazon.DynamoDBv2.DataModel;

namespace StockBrokerBot.Core.Entities;

[DynamoDBTable("stock-prices")]
public class Stock
{
    [DynamoDBHashKey]
    public string Name { get; set; }
    public decimal Price { get; set; }
}