﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using static CodeJam.Ranges.CompositeRangeInternal;

namespace CodeJam.Ranges
{
	/// <summary>Extension methods for <seealso cref="CompositeRange{T}"/>.</summary>
	public static partial class CompositeRangeExtensions
	{
		#region ToCompositeRange
		/// <summary>Converts range to the composite range.</summary>
		/// <typeparam name="T">The type of the range values.</typeparam>
		/// <typeparam name="TKey">The type of the range key</typeparam>
		/// <param name="range">The range.</param>
		/// <returns>A new composite range.</returns>
		public static CompositeRange<T, TKey> ToCompositeRange<T, TKey>(this Range<T, TKey> range)
			=> new CompositeRange<T, TKey>(range);

		/// <summary>Converts sequence of elements to the composite range.</summary>
		/// <typeparam name="T">The type of the range values.</typeparam>
		/// <typeparam name="TKey">The type of the range key</typeparam>
		/// <param name="ranges">The ranges.</param>
		/// <returns>A new composite range.</returns>
		public static CompositeRange<T, TKey> ToCompositeRange<T, TKey>([NotNull] this IEnumerable<Range<T, TKey>> ranges)
			=> new CompositeRange<T, TKey>(ranges);
		#endregion

		#region Updating a range
		/// <summary>
		/// Replaces exclusive boundaries with inclusive ones with the values from the selector callbacks
		/// </summary>
		/// <typeparam name="T">The type of the range values.</typeparam>
		/// <typeparam name="TKey">The type of the range key</typeparam>
		/// <param name="compositeRange">The source range.</param>
		/// <param name="fromValueSelector">Callback to obtain a new value for the From boundary. Used if the boundary is exclusive.</param>
		/// <param name="toValueSelector">Callback to obtain a new value for the To boundary. Used if the boundary is exclusive.</param>
		/// <returns>A range with inclusive boundaries.</returns>
		public static CompositeRange<T, TKey> MakeInclusive<T, TKey>(
			this CompositeRange<T, TKey> compositeRange,
			[NotNull] Func<T, T> fromValueSelector,
			[NotNull] Func<T, T> toValueSelector)
		{
			if (compositeRange.IsEmpty)
				return compositeRange;

			return compositeRange.SubRanges
				.Select(r => r.MakeInclusive(fromValueSelector, toValueSelector))
				.ToCompositeRange();
		}

		/// <summary>
		/// Replaces inclusive boundaries with exclusive ones with the values from the selector callbacks
		/// </summary>
		/// <typeparam name="T">The type of the range values.</typeparam>
		/// <typeparam name="TKey">The type of the range key</typeparam>
		/// <param name="compositeRange">The source range.</param>
		/// <param name="fromValueSelector">Callback to obtain a new value for the From boundary. Used if the boundary is inclusive.</param>
		/// <param name="toValueSelector">Callback to obtain a new value for the To boundary. Used if the boundary is inclusive.</param>
		/// <returns>A range with exclusive boundaries.</returns>
		public static CompositeRange<T, TKey> MakeExclusive<T, TKey>(
			this CompositeRange<T, TKey> compositeRange,
			[NotNull] Func<T, T> fromValueSelector,
			[NotNull] Func<T, T> toValueSelector)
		{
			if (compositeRange.IsEmpty)
				return compositeRange;

			return compositeRange.SubRanges
				.Select(r => r.MakeExclusive(fromValueSelector, toValueSelector))
				.Where(r => r.IsNotEmpty)
				.ToCompositeRange();
		}
		#endregion

		#region Get intersections
		private static RangeIntersection<T, TKey> GetGrouping<T, TKey>(
			RangeBoundaryFrom<T> groupFrom, RangeBoundaryTo<T> groupTo, [NotNull] IEnumerable<Range<T, TKey>> groupingRanges) =>
				new RangeIntersection<T, TKey>(
					Range.Create(groupFrom, groupTo),
					groupingRanges.ToArray());

		[NotNull]
		public static IEnumerable<RangeIntersection<T, TKey>> GetIntersections<T, TKey>(
			this CompositeRange<T, TKey> compositeRange)
		{
			if (compositeRange.IsEmpty)
			{
				yield break;
			}

			var toBoundaries = new List<RangeBoundaryTo<T>>(); // Sorted by descending.
			var rangesToYield = new List<Range<T, TKey>>();

			var fromBoundary = RangeBoundaryFrom<T>.NegativeInfinity;
			foreach (var range in compositeRange.SubRanges)
			{
				// return all ranges that has no intersection with current range.
				while (toBoundaries.Count > 0 && toBoundaries.Last() < range.From)
				{
					var toBoundary = toBoundaries.Last();
					yield return GetGrouping(fromBoundary, toBoundary, rangesToYield);

					rangesToYield.RemoveAll(r => r.To == toBoundary);
					toBoundaries.RemoveAt(toBoundaries.Count - 1);
					fromBoundary = toBoundary.GetComplementation();
				}

				// return rangesToYield as they starts before current range.
				if (fromBoundary < range.From)
				{
					var to = range.From.GetComplementation();
					yield return GetGrouping(fromBoundary, to, rangesToYield);
				}

				// updating the state
				rangesToYield.Add(range);
				InsertInSortedList(
					toBoundaries, range.To,
					RangeBoundaryToDescendingComparer<T>.Instance,
					// ReSharper disable once ArgumentsStyleLiteral
					skipDuplicates: true);

				fromBoundary = range.From;
			}

			// flush all ranges.
			while (toBoundaries.Count > 0 && toBoundaries.Last() < RangeBoundaryTo<T>.PositiveInfinity)
			{
				var toBoundary = toBoundaries.Last();
				yield return GetGrouping(fromBoundary, toBoundary, rangesToYield);

				rangesToYield.RemoveAll(r => r.To == toBoundary);
				toBoundaries.RemoveAt(toBoundaries.Count - 1);
				fromBoundary = toBoundary.GetComplementation();
			}

			yield return GetGrouping(fromBoundary, RangeBoundaryTo<T>.PositiveInfinity, rangesToYield);
		}

		#endregion
	}
}