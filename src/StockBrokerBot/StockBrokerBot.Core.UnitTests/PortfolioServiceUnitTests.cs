using FluentAssertions;
using Moq;
using StockBrokerBot.Core.Entities;
using StockBrokerBot.Core.Exceptions;
using StockBrokerBot.Core.Persistence;
using StockBrokerBot.Core.Services;

namespace StockBrokerBot.Core.UnitTests;

public class UnitTest1
{
    [Fact]
    public async Task GetUserPortfolio_Should_Return_Portfolio()
    {
        //Arrange
        var userId = "TestUser1234";
        var expectedUserPortfolio = new UserPortfolio()
        {
            UserId = userId,
            AvailableCash = 1000.00m
        };
        var mockStockMarketService = new Mock<IStockMarketService>();
        var mockPortfolioDataProvider = new Mock<IPortfolioDataProvider>();
        mockPortfolioDataProvider.Setup(m => m.GetUserPortfolio(userId)).ReturnsAsync(expectedUserPortfolio);
        
        var portfolioService = new PortfolioService(mockStockMarketService.Object, mockPortfolioDataProvider.Object);
        
        // Act
        var userPortfolio = await portfolioService.GetUserPortfolio(userId);
        
        // Assert
        userPortfolio.UserId.Should().Be(userId);
        userPortfolio.AvailableCash.Should().Be(1000.00m);
        mockStockMarketService.Verify(m => m.GetStockPrice(It.IsAny<string>()), Times.Never);
        userPortfolio.StockPortfolio.Count.Should().Be(0);
    }

    [Fact]
    public async Task BuyStocks_Should_Throw_If_No_Available_Cash()
    {
        //Arrange
        var userId = "TestUser1234";
        var stockName = "DemoStock";
        var expectedSharePrice = 50.00m;
        var expectedUserPortfolio = new UserPortfolio()
        {
            UserId = userId,
            AvailableCash = 500.00m,
            StockPortfolio = new List<StockPortfolioItem>
            {
                new() { StockName = stockName, NumberOfShares = 5.00m, AveragePricePerShare = 100.00m },
            }
        };
        var mockStockMarketService = new Mock<IStockMarketService>();
        mockStockMarketService.Setup(m => m.GetStockPrice(stockName)).ReturnsAsync(expectedSharePrice);
        var mockPortfolioDataProvider = new Mock<IPortfolioDataProvider>();
        mockPortfolioDataProvider.Setup(m => m.GetUserPortfolio(userId)).ReturnsAsync(expectedUserPortfolio);
        mockPortfolioDataProvider.Setup(m => m.SaveUserPortfolio(It.IsAny<UserPortfolio>()));
        
        var portfolioService = new PortfolioService(mockStockMarketService.Object, mockPortfolioDataProvider.Object);
        
        // Act
        Func<Task> act = () => portfolioService.BuyStocks(userId, stockName, 15.00m);
        
        // Assert
        await act.Should().ThrowAsync<InsufficientFundsException>();
    }
    
    [Fact]
    public async Task BuyStocks_Existing_Stock_Purchase_Should_Update_Portfolio()
    {
        //Arrange
        var userId = "TestUser1234";
        var stockName = "DemoStock";
        var expectedSharePrice = 50.00m;
        var expectedUserPortfolio = new UserPortfolio()
        {
            UserId = userId,
            AvailableCash = 500.00m,
            StockPortfolio = new List<StockPortfolioItem>
            {
                new() { StockName = stockName, NumberOfShares = 5.00m, AveragePricePerShare = 100.00m },
            }
        };
        var mockStockMarketService = new Mock<IStockMarketService>();
        mockStockMarketService.Setup(m => m.GetStockPrice(stockName)).ReturnsAsync(expectedSharePrice);
        var mockPortfolioDataProvider = new Mock<IPortfolioDataProvider>();
        mockPortfolioDataProvider.Setup(m => m.GetUserPortfolio(userId)).ReturnsAsync(expectedUserPortfolio);
        mockPortfolioDataProvider.Setup(m => m.SaveUserPortfolio(It.IsAny<UserPortfolio>()));
        
        var portfolioService = new PortfolioService(mockStockMarketService.Object, mockPortfolioDataProvider.Object);
        
        // Act
        var userPortfolio = await portfolioService.BuyStocks(userId, stockName, 5.00m);

        // Assert
        userPortfolio.UserId.Should().Be(userId);
        userPortfolio.AvailableCash.Should().Be(250.00m);
        mockPortfolioDataProvider.Verify(m => m.GetUserPortfolio(It.IsAny<string>()), Times.Once);
        mockStockMarketService.Verify(m => m.GetStockPrice(It.IsAny<string>()), Times.Once);
        userPortfolio.StockPortfolio.Count.Should().Be(1);
        var stock = userPortfolio.StockPortfolio.Single();
        stock.NumberOfShares.Should().Be(10.00m);
        stock.AveragePricePerShare.Should().Be(75.00m);
    }
    
    [Fact]
    public async Task BuyStocks_New_Stock_Purchase_Should_Update_Portfolio()
    {
        //Arrange
        var userId = "TestUser1234";
        var existingStockName = "DemoStockExisting";
        var newStockName = "DemoStockNew";
        var expectedSharePriceForExistingStock = 50.00m;
        var expectedSharePriceForNewStock = 10.00m;
        var expectedUserPortfolio = new UserPortfolio()
        {
            UserId = userId,
            AvailableCash = 500.00m,
            StockPortfolio = new List<StockPortfolioItem>
            {
                new() { StockName = existingStockName, NumberOfShares = 5.00m, AveragePricePerShare = 100.00m },
            }
        };
        var mockStockMarketService = new Mock<IStockMarketService>();
        mockStockMarketService.Setup(m => m.GetStockPrice(existingStockName)).ReturnsAsync(expectedSharePriceForExistingStock);
        mockStockMarketService.Setup(m => m.GetStockPrice(newStockName)).ReturnsAsync(expectedSharePriceForNewStock);
        var mockPortfolioDataProvider = new Mock<IPortfolioDataProvider>();
        mockPortfolioDataProvider.Setup(m => m.GetUserPortfolio(userId)).ReturnsAsync(expectedUserPortfolio);
        mockPortfolioDataProvider.Setup(m => m.SaveUserPortfolio(It.IsAny<UserPortfolio>()));
        
        var portfolioService = new PortfolioService(mockStockMarketService.Object, mockPortfolioDataProvider.Object);
        
        // Act
        var userPortfolio = await portfolioService.BuyStocks(userId, newStockName, 2.50m);

        // Assert
        userPortfolio.UserId.Should().Be(userId);
        userPortfolio.AvailableCash.Should().Be(475.00m);
        mockPortfolioDataProvider.Verify(m => m.GetUserPortfolio(It.IsAny<string>()), Times.Once);
        mockStockMarketService.Verify(m => m.GetStockPrice(It.IsAny<string>()), Times.Once);
        userPortfolio.StockPortfolio.Count.Should().Be(2);
        var existingStock = userPortfolio.StockPortfolio[0];
        existingStock.NumberOfShares.Should().Be(5.00m);
        existingStock.AveragePricePerShare.Should().Be(100.00m);
        var newStock = userPortfolio.StockPortfolio[1];
        newStock.NumberOfShares.Should().Be(2.50m);
        newStock.AveragePricePerShare.Should().Be(expectedSharePriceForNewStock);
    }
    
    [Fact]
    public async Task SellStocks_Should_Throw_If_Selling_NonExisting_Stocks()
    {
        //Arrange
        var userId = "TestUser1234";
        var existingStockName = "DemoStockExisting";
        var stockToSellName = "DemoStockNew";
        var expectedSharePriceForExistingStock = 50.00m;
        var expectedUserPortfolio = new UserPortfolio()
        {
            UserId = userId,
            AvailableCash = 500.00m,
            StockPortfolio = new List<StockPortfolioItem>
            {
                new() { StockName = existingStockName, NumberOfShares = 5.00m, AveragePricePerShare = 100.00m },
            }
        };
        var mockStockMarketService = new Mock<IStockMarketService>();
        mockStockMarketService.Setup(m => m.GetStockPrice(existingStockName)).ReturnsAsync(expectedSharePriceForExistingStock);
        var mockPortfolioDataProvider = new Mock<IPortfolioDataProvider>();
        mockPortfolioDataProvider.Setup(m => m.GetUserPortfolio(userId)).ReturnsAsync(expectedUserPortfolio);
        mockPortfolioDataProvider.Setup(m => m.SaveUserPortfolio(It.IsAny<UserPortfolio>()));
        
        var portfolioService = new PortfolioService(mockStockMarketService.Object, mockPortfolioDataProvider.Object);
        
        // Act
        Func<Task> act = () => portfolioService.SellStocks(userId, stockToSellName, 2.50m);
        
        // Assert
        await act.Should().ThrowAsync<InvalidStockOperationException>();
    }
    
    [Fact]
    public async Task SellStocks_Should_Throw_If_Selling_More_Stocks_Than_Owned()
    {
        //Arrange
        var userId = "TestUser1234";
        var existingStockName = "DemoStockExisting";
        var expectedSharePriceForExistingStock = 50.00m;
        var expectedUserPortfolio = new UserPortfolio()
        {
            UserId = userId,
            AvailableCash = 500.00m,
            StockPortfolio = new List<StockPortfolioItem>
            {
                new() { StockName = existingStockName, NumberOfShares = 5.00m, AveragePricePerShare = 100.00m },
            }
        };
        var mockStockMarketService = new Mock<IStockMarketService>();
        mockStockMarketService.Setup(m => m.GetStockPrice(existingStockName)).ReturnsAsync(expectedSharePriceForExistingStock);
        var mockPortfolioDataProvider = new Mock<IPortfolioDataProvider>();
        mockPortfolioDataProvider.Setup(m => m.GetUserPortfolio(userId)).ReturnsAsync(expectedUserPortfolio);
        mockPortfolioDataProvider.Setup(m => m.SaveUserPortfolio(It.IsAny<UserPortfolio>()));
        
        var portfolioService = new PortfolioService(mockStockMarketService.Object, mockPortfolioDataProvider.Object);
        
        // Act
        Func<Task> act = () => portfolioService.SellStocks(userId, existingStockName, 5.01m);
        
        // Assert
        await act.Should().ThrowAsync<InvalidStockOperationException>();
    }
    
    [Fact]
    public async Task BuyStocks_Selling_Existing_Stock_Should_Update_Portfolio()
    {
        //Arrange
        var userId = "TestUser1234";
        var stockName = "DemoStock";
        var expectedSharePrice = 200.00m;
        var expectedUserPortfolio = new UserPortfolio()
        {
            UserId = userId,
            AvailableCash = 500.00m,
            StockPortfolio = new List<StockPortfolioItem>
            {
                new() { StockName = stockName, NumberOfShares = 5.00m, AveragePricePerShare = 100.00m },
            }
        };
        var mockStockMarketService = new Mock<IStockMarketService>();
        mockStockMarketService.Setup(m => m.GetStockPrice(stockName)).ReturnsAsync(expectedSharePrice);
        var mockPortfolioDataProvider = new Mock<IPortfolioDataProvider>();
        mockPortfolioDataProvider.Setup(m => m.GetUserPortfolio(userId)).ReturnsAsync(expectedUserPortfolio);
        mockPortfolioDataProvider.Setup(m => m.SaveUserPortfolio(It.IsAny<UserPortfolio>()));
        
        var portfolioService = new PortfolioService(mockStockMarketService.Object, mockPortfolioDataProvider.Object);
        
        // Act
        var userPortfolio = await portfolioService.SellStocks(userId, stockName, 2.50m);

        // Assert
        userPortfolio.UserId.Should().Be(userId);
        userPortfolio.AvailableCash.Should().Be(1000.00m);
        mockPortfolioDataProvider.Verify(m => m.GetUserPortfolio(It.IsAny<string>()), Times.Once);
        mockStockMarketService.Verify(m => m.GetStockPrice(It.IsAny<string>()), Times.Once);
        userPortfolio.StockPortfolio.Count.Should().Be(1);
        var stock = userPortfolio.StockPortfolio.Single();
        stock.NumberOfShares.Should().Be(2.50m);
        stock.AveragePricePerShare.Should().Be(0.00m);
    }
    
    [Fact]
    public async Task BuyStocks_Selling_Existing_Stock_Should_Update_Portfolio_With_Loss()
    {
        //Arrange
        var userId = "TestUser1234";
        var stockName = "DemoStock";
        var expectedSharePrice = 10.00m;
        var expectedUserPortfolio = new UserPortfolio()
        {
            UserId = userId,
            AvailableCash = 500.00m,
            StockPortfolio = new List<StockPortfolioItem>
            {
                new() { StockName = stockName, NumberOfShares = 5.00m, AveragePricePerShare = 100.00m },
            }
        };
        var mockStockMarketService = new Mock<IStockMarketService>();
        mockStockMarketService.Setup(m => m.GetStockPrice(stockName)).ReturnsAsync(expectedSharePrice);
        var mockPortfolioDataProvider = new Mock<IPortfolioDataProvider>();
        mockPortfolioDataProvider.Setup(m => m.GetUserPortfolio(userId)).ReturnsAsync(expectedUserPortfolio);
        mockPortfolioDataProvider.Setup(m => m.SaveUserPortfolio(It.IsAny<UserPortfolio>()));
        
        var portfolioService = new PortfolioService(mockStockMarketService.Object, mockPortfolioDataProvider.Object);
        
        // Act
        var userPortfolio = await portfolioService.SellStocks(userId, stockName, 2.00m);

        // Assert
        userPortfolio.UserId.Should().Be(userId);
        userPortfolio.AvailableCash.Should().Be(520.00m);
        mockPortfolioDataProvider.Verify(m => m.GetUserPortfolio(It.IsAny<string>()), Times.Once);
        mockStockMarketService.Verify(m => m.GetStockPrice(It.IsAny<string>()), Times.Once);
        userPortfolio.StockPortfolio.Count.Should().Be(1);
        var stock = userPortfolio.StockPortfolio.Single();
        stock.NumberOfShares.Should().Be(3.00m);
        stock.AveragePricePerShare.Should().Be(160.00m);
    }
    
    [Fact]
    public async Task BuyStocks_Selling_Existing_Stock_Should_Update_Portfolio_With_Profit()
    {
        //Arrange
        var userId = "TestUser1234";
        var stockName = "DemoStock";
        var expectedSharePrice = 200.00m;
        var expectedUserPortfolio = new UserPortfolio()
        {
            UserId = userId,
            AvailableCash = 500.00m,
            StockPortfolio = new List<StockPortfolioItem>
            {
                new() { StockName = stockName, NumberOfShares = 5.00m, AveragePricePerShare = 100.00m },
            }
        };
        var mockStockMarketService = new Mock<IStockMarketService>();
        mockStockMarketService.Setup(m => m.GetStockPrice(stockName)).ReturnsAsync(expectedSharePrice);
        var mockPortfolioDataProvider = new Mock<IPortfolioDataProvider>();
        mockPortfolioDataProvider.Setup(m => m.GetUserPortfolio(userId)).ReturnsAsync(expectedUserPortfolio);
        mockPortfolioDataProvider.Setup(m => m.SaveUserPortfolio(It.IsAny<UserPortfolio>()));
        
        var portfolioService = new PortfolioService(mockStockMarketService.Object, mockPortfolioDataProvider.Object);
        
        // Act
        var userPortfolio = await portfolioService.SellStocks(userId, stockName, 4.50m);

        // Assert
        userPortfolio.UserId.Should().Be(userId);
        userPortfolio.AvailableCash.Should().Be(1400.00m);
        mockPortfolioDataProvider.Verify(m => m.GetUserPortfolio(It.IsAny<string>()), Times.Once);
        mockStockMarketService.Verify(m => m.GetStockPrice(It.IsAny<string>()), Times.Once);
        userPortfolio.StockPortfolio.Count.Should().Be(1);
        var stock = userPortfolio.StockPortfolio.Single();
        stock.NumberOfShares.Should().Be(0.50m);
        stock.AveragePricePerShare.Should().Be(-800.00m); // Not sure if it actually works this way!
    }
}