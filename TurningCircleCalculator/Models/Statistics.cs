using System;
using System.Collections.Generic;

namespace TurningCircleCalculator.Models;

/// <summary>
/// Aggregate statistics for a sample.
/// </summary>
/// <param name="Count">Number of values in the sample.</param>
/// <param name="Mean">Arithmetic mean.</param>
/// <param name="StdDev">Sample standard deviation (n-1 form).</param>
/// <param name="Min">Smallest value in the sample.</param>
/// <param name="Max">Largest value in the sample.</param>
public readonly record struct Stats(int Count, double Mean, double StdDev, double Min, double Max);

/// <summary>
/// Sample statistics used for error analysis (Krigsmarine Navigation
/// Guide, Chapter 6).
/// </summary>
public static class Statistics
{
    /// <summary>
    /// Computes the arithmetic mean of the samples.
    /// </summary>
    /// <param name="samples">A non-empty collection of values.</param>
    /// <returns>The arithmetic mean.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="samples"/> is empty.</exception>
    public static double Mean(IReadOnlyList<double> samples)
    {
        if (samples is null) throw new ArgumentNullException(nameof(samples));
        if (samples.Count == 0) throw new ArgumentException("At least one sample is required.", nameof(samples));
        double sum = 0;
        for (int i = 0; i < samples.Count; i++) sum += samples[i];
        return sum / samples.Count;
    }

    /// <summary>
    /// Computes the sample standard deviation using the (n-1) denominator,
    /// as defined in the navigation guide.
    /// </summary>
    /// <param name="samples">At least two values are required.</param>
    /// <returns>The sample standard deviation.</returns>
    /// <exception cref="ArgumentException">Thrown when fewer than two samples are supplied.</exception>
    public static double SampleStandardDeviation(IReadOnlyList<double> samples)
    {
        if (samples is null) throw new ArgumentNullException(nameof(samples));
        if (samples.Count < 2) throw new ArgumentException("At least two samples are required.", nameof(samples));
        var mean = Mean(samples);
        double sumSq = 0;
        for (int i = 0; i < samples.Count; i++)
        {
            var d = samples[i] - mean;
            sumSq += d * d;
        }
        return Math.Sqrt(sumSq / (samples.Count - 1));
    }

    /// <summary>
    /// Computes <see cref="Stats"/> for the sample.
    /// </summary>
    /// <param name="samples">At least one sample is required; <see cref="Stats.StdDev"/> is zero when only one value is present.</param>
    /// <returns>An aggregate <see cref="Stats"/> record.</returns>
    public static Stats Compute(IReadOnlyList<double> samples)
    {
        if (samples is null) throw new ArgumentNullException(nameof(samples));
        if (samples.Count == 0) throw new ArgumentException("At least one sample is required.", nameof(samples));

        var mean = Mean(samples);
        double min = samples[0];
        double max = samples[0];
        for (int i = 1; i < samples.Count; i++)
        {
            if (samples[i] < min) min = samples[i];
            if (samples[i] > max) max = samples[i];
        }
        var stdDev = samples.Count < 2 ? 0.0 : SampleStandardDeviation(samples);
        return new Stats(samples.Count, mean, stdDev, min, max);
    }
}
