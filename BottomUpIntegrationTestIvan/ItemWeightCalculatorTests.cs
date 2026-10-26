using BottomUpIntegrationExercise;

namespace BottomUpIntegrationTestIvan;

[TestClass]
public sealed class ItemWeightCalculatorTests
{
    [TestMethod]
    public void Calculate_WithTwoItems_ReturnsCombinedWeight()
    {
        // Arrange
        var calculator = new ItemWeightCalculator();
        var item = new ParcelItem(0.4m, 2);

        // Act
        var result = calculator.Calculate(item);

        // Assert
        Assert.AreEqual(0.8m, result);
    }

    [TestMethod]
    public void Calculate_WithZeroUnitWeight_Throws()
    {
        // Arrange
        var calculator = new ItemWeightCalculator();
        var item = new ParcelItem(0m, 2);

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => calculator.Calculate(item));
    }

    [TestMethod]
    public void Calculate_WithZeroQuantity_Throws()
    {
        // Arrange
        var calculator = new ItemWeightCalculator();
        var item = new ParcelItem(0.4m, 0);

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => calculator.Calculate(item));

        Assert.AreEqual("item", exception.ParamName);
    }
}
