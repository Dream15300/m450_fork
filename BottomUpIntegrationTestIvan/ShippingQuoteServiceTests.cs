
using BottomUpIntegrationExercise;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BottomUpIntegrationTestIvan;

[TestClass]
public sealed class ShippingQuoteServiceTests
{
    [TestMethod]
    public void CreateQuote_WithRatePlaceholder_ReturnsCorrectQuote()
    {
        // Arrange
        var rate = new RecordingRatePlaceholder();

        var service = new ShippingQuoteService(
            new ParcelWeightCalculator(new ItemWeightCalculator()),
            rate);

        ParcelItem[] items =
        [
            new(0.4m, 2),
            new(0.2m, 1)
        ];

        // Act
        var quote = service.CreateQuote(items);

        // Assert
        Assert.AreEqual(1m, quote.TotalWeightKg);
        Assert.AreEqual(42m, quote.ShippingCostChf);
        Assert.AreEqual(1m, rate.ReceivedWeightKg);
    }


    [TestMethod]
    public void CreateQuote_WithRealComponents_ReturnsCorrectQuote()
    {
        // Arrange
        var service = new ShippingQuoteService(
            new ParcelWeightCalculator(new ItemWeightCalculator()),
            new ShippingRateCalculator());

        ParcelItem[] items =
        [
            new(0.4m, 2),
            new(0.2m, 1)
        ];

        // Act
        var quote = service.CreateQuote(items);

        // Assert
        Assert.AreEqual(1m, quote.TotalWeightKg);
        Assert.AreEqual(5m, quote.ShippingCostChf);
    }


    [TestMethod]
    public void CreateQuote_AboveOneKg_ReturnsNineFrancs()
    {
        // Arrange
        var service = new ShippingQuoteService(
            new ParcelWeightCalculator(new ItemWeightCalculator()),
            new ShippingRateCalculator());

        ParcelItem[] items =
        [
            new(0.4m, 2),
            new(0.3m, 1)
        ];

        // Act
        var quote = service.CreateQuote(items);

        // Assert
        Assert.AreEqual(1.1m, quote.TotalWeightKg);
        Assert.AreEqual(9m, quote.ShippingCostChf);
    }


    [TestMethod]
    public void CreateQuote_AtFiveKg_ReturnsNineFrancs()
    {
        // Arrange
        var service = new ShippingQuoteService(
            new ParcelWeightCalculator(new ItemWeightCalculator()),
            new ShippingRateCalculator());

        ParcelItem[] items =
        [
            new(2m, 2), // 2x 2kg = 4kg
            new(1m, 1)  // 1x 1kg = 1kg
        ];

        // Act
        var quote = service.CreateQuote(items);

        // Assert
        Assert.AreEqual(5m, quote.TotalWeightKg); // 4kg + 1kg = 5kg
        Assert.AreEqual(9m, quote.ShippingCostChf); // 5kg ist über 1kg, daher 9 CHF
    }


    [TestMethod]
    public void CreateQuote_AboveFiveKg_ReturnsFifteenFrancs()
    {
        // Arrange
        var service = new ShippingQuoteService(
            new ParcelWeightCalculator(new ItemWeightCalculator()),
            new ShippingRateCalculator());

        ParcelItem[] items =
        [
            new(2m, 2),
        new(1m, 2)
        ];

        // Act
        var quote = service.CreateQuote(items);

        // Assert
        Assert.AreEqual(6m, quote.TotalWeightKg);
        Assert.AreEqual(15m, quote.ShippingCostChf);
    }


    [TestMethod]
    public void CreateQuote_WithEmptyParcel_Throws()
    {
        // Arrange
        var service = new ShippingQuoteService(
            new ParcelWeightCalculator(new ItemWeightCalculator()),
            new ShippingRateCalculator());

        // Act
        var exception = Assert.Throws<ArgumentException>(
            () => service.CreateQuote([]));

        // Assert
        Assert.AreEqual("items", exception.ParamName);
    }




    private sealed class RecordingRatePlaceholder : IShippingRateCalculator
    {
        public decimal? ReceivedWeightKg { get; private set; }

        public decimal Calculate(decimal totalWeightKg)
        {
            ReceivedWeightKg = totalWeightKg;
            return 42m;
        }
    }
}
