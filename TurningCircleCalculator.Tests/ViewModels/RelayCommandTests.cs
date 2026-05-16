using FluentAssertions;
using TurningCircleCalculator.ViewModels;

namespace TurningCircleCalculator.Tests.ViewModels;

public class RelayCommandTests
{
    [Fact]
    public void Execute_InvokesAction()
    {
        var executed = false;
        var cmd = new RelayCommand(() => executed = true);
        cmd.Execute(null);
        executed.Should().BeTrue();
    }

    [Fact]
    public void CanExecute_ReturnsTrueByDefault()
    {
        var cmd = new RelayCommand(() => { });
        cmd.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public void CanExecute_DelegatesToGuard()
    {
        var allowed = false;
        var cmd = new RelayCommand(() => { }, () => allowed);
        cmd.CanExecute(null).Should().BeFalse();

        allowed = true;
        cmd.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public void RaiseCanExecuteChanged_FiresEvent()
    {
        var cmd = new RelayCommand(() => { });
        var fired = false;
        cmd.CanExecuteChanged += (_, _) => fired = true;

        cmd.RaiseCanExecuteChanged();

        fired.Should().BeTrue();
    }
}
