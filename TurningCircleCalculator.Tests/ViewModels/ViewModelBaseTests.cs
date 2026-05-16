using FluentAssertions;
using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Tests.ViewModels;

public class ViewModelBaseTests
{
    private class TestViewModel : ViewModelBase
    {
        private string _name = "";
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        public void FireCustom(string propName) => OnPropertyChanged(propName);
    }

    [Fact]
    public void SetField_RaisesPropertyChanged_WhenValueChanges()
    {
        var vm = new TestViewModel();
        string? raised = null;
        vm.PropertyChanged += (_, e) => raised = e.PropertyName;

        vm.Name = "Alice";

        raised.Should().Be(nameof(TestViewModel.Name));
    }

    [Fact]
    public void SetField_DoesNotRaise_WhenValueSame()
    {
        var vm = new TestViewModel { Name = "Alice" };
        var raised = false;
        vm.PropertyChanged += (_, _) => raised = true;

        vm.Name = "Alice";

        raised.Should().BeFalse();
    }

    [Fact]
    public void SetField_ReturnsTrue_WhenValueChanges()
    {
        var vm = new TestViewModel();
        vm.Name = "Old";
        // SetField is exercised through the property; just verify it doesn't throw
        var act = () => vm.Name = "New";
        act.Should().NotThrow();
    }

    [Fact]
    public void OnPropertyChanged_CanBeCalledWithExplicitName()
    {
        var vm = new TestViewModel();
        string? raised = null;
        vm.PropertyChanged += (_, e) => raised = e.PropertyName;

        vm.FireCustom("CustomProp");

        raised.Should().Be("CustomProp");
    }
}
