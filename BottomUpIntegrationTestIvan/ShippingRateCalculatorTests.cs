using BottomUpIntegrationExercise;

namespace BottomUpIntegrationTestIvan;

[TestClass]
public class ShippingRateCalculatorTests
{
    [TestMethod]
    public void Calculate_WithOneKg_ReturnsFiveFrancs()
    {
        // Arrange
        var calculator = new ShippingRateCalculator();
        decimal weight = 1m;

        // Act
        var result = calculator.Calculate(weight);

        // Assert
        Assert.AreEqual(5m, result);
    }


    [DataTestMethod]
    [DataRow("1", "5")]
    [DataRow("1.01", "9")]
    [DataRow("5", "9")]
    [DataRow("5.01", "15")]
    public void Calculate_AtTariffBoundaries_ReturnsExpectedCost(
        string weightText, string expectedCostText)
    {
        // Arrange
        var calculator = new ShippingRateCalculator();
        decimal weight = decimal.Parse(
            weightText, System.Globalization.CultureInfo.InvariantCulture);
        decimal expectedCost = decimal.Parse(
            expectedCostText, System.Globalization.CultureInfo.InvariantCulture);

        // Act
        var result = calculator.Calculate(weight);

        // Assert
        Assert.AreEqual(expectedCost, result);
    }


    [TestMethod]
    public void Calculate_WithZeroWeight_Throws()
    {
        // Arrange
        var calculator = new ShippingRateCalculator();

        // Act & Assert | paramName prüft ob Parameter totalWeightKg korrekt ist
        Assert.AreEqual("totalWeightKg",
            Assert.Throws<ArgumentOutOfRangeException>(
                () => calculator.Calculate(0m)).ParamName);
    }

}
