using FluentAssertions;
using TurningCircleCalculator.Modules.Statistics;

namespace TurningCircleCalculator.Tests.ViewModels;

public class StatisticsViewModelTests
{
    [Fact]
    public void RawInput_FiveNumbers_PopulatesStats()
    {
        var vm = new StatisticsViewModel
        {
            RawInput = "1\n2\n3\n4\n5"
        };
        vm.Stats.Should().NotBeNull();
        vm.Stats!.Value.Count.Should().Be(5);
        vm.Stats.Value.Mean.Should().BeApproximately(3.0, 1e-9);
        vm.Stats.Value.StdDev.Should().BeApproximately(1.5811388, 1e-6);
        vm.SkippedCount.Should().Be(0);
    }

    [Fact]
    public void RawInput_MixedDelimiters_ParsesAll()
    {
        var vm = new StatisticsViewModel
        {
            RawInput = "1.5, 2.5\t3.5;4.5 5.5"
        };
        vm.Stats!.Value.Count.Should().Be(5);
        vm.Stats.Value.Mean.Should().BeApproximately(3.5, 1e-9);
    }

    [Fact]
    public void RawInput_InvalidTokens_CountedAsSkipped()
    {
        var vm = new StatisticsViewModel
        {
            RawInput = "1\nfoo\n2\nbar\n3"
        };
        vm.Stats!.Value.Count.Should().Be(3);
        vm.SkippedCount.Should().Be(2);
    }

    [Fact]
    public void RawInput_Empty_NoStats()
    {
        var vm = new StatisticsViewModel { RawInput = "" };
        vm.Stats.Should().BeNull();
        vm.SkippedCount.Should().Be(0);
    }
}
