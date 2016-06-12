﻿using System;

using BenchmarkDotNet.Reports;

using JetBrains.Annotations;

namespace CodeJam.PerfTests.Metrics
{
	/// <summary>Percentile metric provider.</summary>
	/// <seealso cref="CodeJam.PerfTests.Metrics.ILimitMetricProvider" />
	[PublicAPI]
	public class PercentileMetricProvider : MetricProviderBase
	{
		/// <summary> Metric is based on 95th percentile.</summary>
		public static readonly ILimitMetricProvider P95 = new PercentileMetricProvider(95, 95);

		/// <summary> Metric is based on 20 (lower boundary) and 80 (upper boundary) percentiles.</summary>
		public static readonly ILimitMetricProvider P20To80 = new PercentileMetricProvider(20, 80);

		private static double? TryGetPercentileMetric(
			BenchmarkReport benchmarkReport, BenchmarkReport baselineReport,
			int percentile)
		{
			var benchmarkMetric = benchmarkReport.ResultStatistics.Percentiles.Percentile(percentile);
			var baselineMetric = baselineReport.ResultStatistics.Percentiles.Percentile(percentile);

			// ReSharper disable once CompareOfFloatsByEqualityOperator
			return baselineMetric == 0 ? (double?)null : benchmarkMetric / baselineMetric;
		}

		/// <summary>Initializes a new instance of the <see cref="PercentileMetricProvider"/> class.</summary>
		/// <param name="lowerBoundaryPercentile">The lower boundary percentile.</param>
		/// <param name="upperBoundaryPercentile">The upper boundary percentile.</param>
		public PercentileMetricProvider(
			int lowerBoundaryPercentile,
			int upperBoundaryPercentile)
		{
			Code.ValidIndexPair(
				lowerBoundaryPercentile,
				nameof(lowerBoundaryPercentile),
				upperBoundaryPercentile,
				nameof(upperBoundaryPercentile),
				100);

			LowerBoundaryPercentile = lowerBoundaryPercentile;
			UpperBoundaryPercentile = upperBoundaryPercentile;
		}

		/// <summary>Short description for the provider.</summary>
		/// <value>The short description for the provider.</value>
		public override string ShortInfo =>
			LowerBoundaryPercentile == UpperBoundaryPercentile
				? $"P{LowerBoundaryPercentile}"
				: $"P{LowerBoundaryPercentile}..{UpperBoundaryPercentile}";

		/// <summary>Lower boundary percentile.</summary>
		/// <value>The lower boundary percentile.</value>
		public int LowerBoundaryPercentile { get; }
		/// <summary>Upper boundary percentile.</summary>
		/// <value>The upper boundary percentile.</value>
		public int UpperBoundaryPercentile { get; }

		/// <summary>Tries the get lower metric for the benchmark.</summary>
		/// <param name="benchmarkReport">The benchmark report.</param>
		/// <param name="baselineReport">The baseline report.</param>
		/// <returns>The lower metric for the benchmark or <c>null</c> if none.</returns>
		protected override double? TryGetMetricLower(
			BenchmarkReport benchmarkReport,
			BenchmarkReport baselineReport) =>
				TryGetPercentileMetric(benchmarkReport, baselineReport, LowerBoundaryPercentile);

		/// <summary>Tries the get upper metric for the benchmark.</summary>
		/// <param name="benchmarkReport">The benchmark report.</param>
		/// <param name="baselineReport">The baseline report.</param>
		/// <returns>The upper metric for the benchmark or <c>null</c> if none.</returns>
		protected override double? TryGetMetricUpper(
			BenchmarkReport benchmarkReport,
			BenchmarkReport baselineReport) =>
				TryGetPercentileMetric(benchmarkReport, baselineReport, UpperBoundaryPercentile);
	}
}