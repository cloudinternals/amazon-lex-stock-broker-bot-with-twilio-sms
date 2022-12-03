using Newtonsoft.Json;
using StockBrokerBot.Core.Entities;
using StockBrokerBot.Core.Persistence;

namespace StockBrokerBot.Demo.Persistence;

public class PortfolioJsonDataProvider : IPortfolioDataProvider
{
    private string DB_PATH = "./Data/portfolio-db.json";
    private List<UserPortfolio> _userPortfolios;
    
    public async Task<UserPortfolio> GetUserPortfolio(string userId)
    {
        LoadDatabase();
        
        var rawJson = File.ReadAllText(DB_PATH);
        _userPortfolios = JsonConvert.DeserializeObject<List<UserPortfolio>>(rawJson);
        
        var userPortfolio = _userPortfolios.Single(s =>
            string.Compare(s.UserId, userId, StringComparison.InvariantCultureIgnoreCase) == 0);
        return userPortfolio;
    }

    public async Task SaveUserPortfolio(UserPortfolio userPortfolio)
    {
        LoadDatabase();
        
        var currentPortfolio = _userPortfolios.Single(s =>
            string.Compare(s.UserId, userPortfolio.UserId, StringComparison.InvariantCultureIgnoreCase) == 0);
        
        _userPortfolios.Remove(currentPortfolio);
        _userPortfolios.Add(userPortfolio);

        var newData = JsonConvert.SerializeObject(_userPortfolios);
        
        await File.WriteAllTextAsync(DB_PATH, newData);
    }

    private void LoadDatabase()
    {
        var rawJson = File.ReadAllText(DB_PATH);
        _userPortfolios = JsonConvert.DeserializeObject<List<UserPortfolio>>(rawJson);
    }
    
}