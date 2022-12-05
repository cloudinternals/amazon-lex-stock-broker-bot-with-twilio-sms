using Newtonsoft.Json;
using StockBrokerBot.Core.Entities;
using StockBrokerBot.Core.Persistence;

namespace StockBrokerBot.Demo.Persistence;

public class StockMarketJsonDataProvider : IStockMarketDataProvider
{
    private string DB_PATH = "./Data/stock-db.json";
    private List<Stock> _stocks;
    
    public StockMarketJsonDataProvider()
    {
        var rawJson = File.ReadAllText(DB_PATH);
        _stocks = JsonConvert.DeserializeObject<List<Stock>>(rawJson);
    }

    public async Task<decimal> GetStockPrice(string name)
    {
        var stock = _stocks.Single(s =>
            string.Compare(s.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0);
        return stock.Price;
    }
}