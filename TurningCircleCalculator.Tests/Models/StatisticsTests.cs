using FluentAssertions;
using TurningCircleCalculator.Models;

namespace TurningCircleCalculator.Tests.Models;

public class StatisticsTests
{
    [Fact]
    public void Mean_ThreeValues_ReturnsAverage()
    {
        Statistics.Mean(new[] { 1.0, 2.0, 3.0 }).Should().BeApproximately(2.0, 1e-9);
    }

    [Fact]
    public void Mean_Empty_Throws()
    {
        var act = () => Statistics.Mean(Array.Empty<double>());
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SampleStandardDeviation_KnownDataset()
    {
        // For [1,2,3,4,5]: mean=3, sum of squared deviations = 10, s = sqrt(10/4) ≈ 1.5811
        Statistics.SampleStandardDeviation(new[] { 1.0, 2.0, 3.0, 4.0, 5.0 })
            .Should().BeApproximately(1.5811388, 1e-6);
    }

    [Fact]
    public void SampleStandardDeviation_SingleValue_Throws()
    {
        var act = () => Statistics.SampleStandardDeviation(new[] { 42.0 });
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Compute_PopulatesAllFields()
    {
        var s = Statistics.Compute(new[] { 2.0, 4.0, 4.0, 4.0, 5.0, 5.0, 7.0, 9.0 });
        s.Count.Should().Be(8);
        s.Mean.Should().BeApproximately(5.0, 1e-9);
        s.Min.Should().BeApproximately(2.0, 1e-9);
        s.Max.Should().BeApproximately(9.0, 1e-9);
        s.StdDev.Should().BeApproximately(2.13809, 1e-4);
    }

    [Fact]
    public void Compute_SingleValue_StdDevIsZero()
    {
        var s = Statistics.Compute(new[] { 5.0 });
        s.Count.Should().Be(1);
        s.Mean.Should().Be(5.0);
        s.StdDev.Should().Be(0);
        s.Min.Should().Be(5.0);
        s.Max.Should().Be(5.0);
    }
}
