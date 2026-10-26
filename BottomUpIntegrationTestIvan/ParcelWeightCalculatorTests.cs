
using BottomUpIntegrationExercise;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BottomUpIntegrationTestIvan;

[TestClass]
public sealed class ParcelWeightCalculatorTests
{
    [TestMethod]
    public void Calculate_WithMultipleItems_ReturnsTotalWeight()
    {
        // Arrange
        var calculator = new ParcelWeightCalculator(
            new ItemWeightCalculator()); // Dependency Injection über den Konstruktor

        ParcelItem[] items =
        [
            new(0.4m, 2),
            new(0.2m, 1)
        ];

        // Act
        var result = calculator.Calculate(items);

        // Assert
        Assert.AreEqual(1m, result);
    }


    [TestMethod]
    public void Calculate_WithEmptyParcel_Throws()
    {
        // Arrange
        var calculator = new ParcelWeightCalculator(
            new ItemWeightCalculator());

        // Act
        var exception = Assert.Throws<ArgumentException>(
            () => calculator.Calculate([]));

        // Assert
        Assert.AreEqual("items", exception.ParamName);
    }


    [TestMethod]
    public void Calculate_WithInvalidSecondItem_Throws()
    {
        // Arrange
        var calculator = new ParcelWeightCalculator(
            new ItemWeightCalculator());

        ParcelItem[] items =
        [
            new(0.4m, 2),
            new(0.2m, 0) // invalid quantity
        ];

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => calculator.Calculate(items));

        // Assert
        Assert.AreEqual("item", exception.ParamName);
    }

}
