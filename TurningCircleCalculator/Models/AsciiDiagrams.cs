using System;
using System.Text;

namespace TurningCircleCalculator.Models;

/// <summary>
/// Renders small ASCII diagrams used inside Terminal.Gui labels.
/// All output is rectangular: each line is padded to the same width.
/// </summary>
public static class AsciiDiagrams
{
    /// <summary>
    /// Renders a top-down turn diagram showing the turning-circle outline,
    /// the actual arc traversed, the chord between entry and exit, and the
    /// labelled entry/exit points. The diagram is rendered in the ship's
    /// initial-course frame (initial heading is "up" on the page).
    /// </summary>
    /// <param name="turnAngleDeg">Turn angle traversed, in degrees (<c>[0, 360)</c>).</param>
    /// <param name="direction">Direction of the turn.</param>
    /// <param name="width">Diagram width in characters; minimum 12.</param>
    /// <param name="height">Diagram height in characters; minimum 7.</param>
    /// <returns>The diagram as a newline-separated string.</returns>
    public static string TurnDiagram(double turnAngleDeg, TurnDirection direction, int width = 24, int height = 12)
    {
        if (width < 12) width = 12;
        if (height < 7) height = 7;

        var grid = new char[height, width];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                grid[y, x] = ' ';

        int cx = width / 2;
        int cy = height / 2;
        int r = Math.Min(cx, cy) - 2;
        if (r < 2) r = 2;

        // Outline of the full circle as faint dots.
        for (int deg = 0; deg < 360; deg += 6)
        {
            var (px, py) = PolarToGrid(cx, cy, r, deg);
            Place(grid, px, py, '.');
        }

        // Centre marker.
        Place(grid, cx, cy, '+');

        // For a starboard (right) turn the centre lies to the ship's right;
        // looking from the centre back to the entry point, the bearing is 270°
        // (i.e. the entry sits on the left of the grid). The arc then sweeps
        // clockwise as the ship turns. The opposite holds for a port turn.
        int startDeg = direction == TurnDirection.Right ? 270 : 90;
        int sweepSign = direction == TurnDirection.Right ? 1 : -1;
        int span = (int)Math.Round(turnAngleDeg);
        if (span < 0) span = 0;
        if (span > 360) span = 360;

        // Trace the arc.
        for (int s = 0; s <= span; s++)
        {
            int deg = Mod360(startDeg + (sweepSign * s));
            var (ax, ay) = PolarToGrid(cx, cy, r, deg);
            Place(grid, ax, ay, '*');
        }

        // Entry and exit points.
        var (ex, ey) = PolarToGrid(cx, cy, r, startDeg);
        var (xx, xy) = PolarToGrid(cx, cy, r, Mod360(startDeg + (sweepSign * span)));

        // Chord between entry and exit (Bresenham).
        DrawLine(grid, ex, ey, xx, xy, ':');

        // Re-stamp entry and exit labels on top.
        if (ex == xx && ey == xy)
        {
            // Degenerate (zero-angle) turn: collapse both labels into one glyph.
            Place(grid, ex, ey, '@');
        }
        else
        {
            Place(grid, ex, ey, 'E');
            Place(grid, xx, xy, 'X');
        }

        // Direction tag at the top corners.
        var tag = direction == TurnDirection.Right ? "R>" : "<L";
        if (width >= 4)
        {
            int tagX = direction == TurnDirection.Right ? width - 2 : 0;
            Place(grid, tagX, 0, tag[0]);
            Place(grid, tagX + 1, 0, tag[1]);
        }

        var sb = new StringBuilder(height * (width + 1));
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++) sb.Append(grid[y, x]);
            if (y < height - 1) sb.Append('\n');
        }
        return sb.ToString();
    }

    private static (int X, int Y) PolarToGrid(int cx, int cy, int r, int bearingDeg)
    {
        var rad = bearingDeg * Math.PI / 180.0;
        // Nav bearing 0° points "up" on screen (y decreasing).
        int x = cx + (int)Math.Round(r * Math.Sin(rad));
        int y = cy - (int)Math.Round(r * Math.Cos(rad));
        return (x, y);
    }

    private static int Mod360(int v) => ((v % 360) + 360) % 360;

    private static void Place(char[,] grid, int x, int y, char c)
    {
        int h = grid.GetLength(0);
        int w = grid.GetLength(1);
        if (x >= 0 && x < w && y >= 0 && y < h) grid[y, x] = c;
    }

    private static void DrawLine(char[,] grid, int x0, int y0, int x1, int y1, char c)
    {
        int dx = Math.Abs(x1 - x0);
        int dy = -Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;
        int x = x0, y = y0;
        while (true)
        {
            // Don't overwrite existing arc/marker glyphs with the lighter chord.
            int h = grid.GetLength(0);
            int w = grid.GetLength(1);
            if (x >= 0 && x < w && y >= 0 && y < h && grid[y, x] == ' ')
                grid[y, x] = c;
            if (x == x1 && y == y1) break;
            int e2 = 2 * err;
            if (e2 >= dy) { err += dy; x += sx; }
            if (e2 <= dx) { err += dx; y += sy; }
        }
    }
}
