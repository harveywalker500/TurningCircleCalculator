using FluentAssertions;
using TurningCircleCalculator.Models;

namespace TurningCircleCalculator.Tests.Models;

public class AsciiDiagramsTests
{
    [Fact]
    public void TurnDiagram_DefaultDimensions_AreCorrect()
    {
        var s = AsciiDiagrams.TurnDiagram(90, TurnDirection.Right);
        var lines = s.Split('\n');
        lines.Length.Should().Be(12);
        lines.Should().OnlyContain(l => l.Length == 24);
    }

    [Fact]
    public void TurnDiagram_RightTurn_ContainsRightTagAndEntryExitMarkers()
    {
        var s = AsciiDiagrams.TurnDiagram(90, TurnDirection.Right);
        s.Should().Contain("R>");
        s.Should().Contain("E");
        s.Should().Contain("X");
        s.Should().Contain("+"); // centre
    }

    [Fact]
    public void TurnDiagram_LeftTurn_ContainsLeftTag()
    {
        var s = AsciiDiagrams.TurnDiagram(90, TurnDirection.Left);
        s.Should().Contain("<L");
    }

    [Fact]
    public void TurnDiagram_BelowMinimumSize_ClampsUp()
    {
        var s = AsciiDiagrams.TurnDiagram(45, TurnDirection.Right, width: 4, height: 3);
        var lines = s.Split('\n');
        lines.Length.Should().BeGreaterThanOrEqualTo(7);
        lines[0].Length.Should().BeGreaterThanOrEqualTo(12);
    }

    [Fact]
    public void TurnDiagram_ZeroAngle_RendersDegenerateMarker()
    {
        var s = AsciiDiagrams.TurnDiagram(0, TurnDirection.Right);
        s.Should().Contain("@");
    }
}
