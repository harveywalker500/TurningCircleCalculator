using FluentAssertions;
using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Tests.ViewModels;

public class TimeSetupViewModelTests
{
    [Fact]
    public void Confirm_ValidTime_ReturnsTrueAndSetsSelectedTime()
    {
        var vm = new TimeSetupViewModel("12:00:00");
        vm.TimeInput = "14:30:00";

        var result = vm.Confirm();

        result.Should().BeTrue();
        vm.SelectedTime.Should().Be("14:30:00");
        vm.ErrorMessage.Should().BeNull();
    }

    [Theory]
    [InlineData("not-a-time")]
    [InlineData("25:00:00")]
    [InlineData("abc")]
    public void Confirm_InvalidTime_ReturnsFalseAndSetsError(string bad)
    {
        var vm = new TimeSetupViewModel("12:00:00");
        vm.TimeInput = bad;

        var result = vm.Confirm();

        result.Should().BeFalse();
        vm.SelectedTime.Should().BeNull();
        vm.ErrorMessage.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void SettingTimeInput_ClearsErrorMessage()
    {
        var vm = new TimeSetupViewModel("12:00:00");
        vm.TimeInput = "bad";
        vm.Confirm();
        vm.ErrorMessage.Should().NotBeNull();

        vm.TimeInput = "15:00:00";

        vm.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void TimeInput_RaisesPropertyChanged()
    {
        var vm = new TimeSetupViewModel("12:00:00");
        string? raised = null;
        vm.PropertyChanged += (_, e) => raised = e.PropertyName;

        vm.TimeInput = "10:00:00";

        raised.Should().Be(nameof(TimeSetupViewModel.TimeInput));
    }
}
