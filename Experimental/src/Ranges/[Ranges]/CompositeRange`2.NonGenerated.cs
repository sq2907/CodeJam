﻿using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

// The file contains members that should not be copied into CompositeRange<T, TKey>. DO NOT remove it

namespace CodeJam.Ranges
{
	/// <summary>Describes a range of the values.</summary>
	public partial struct CompositeRange<T, TKey> : ICompositeRange<T>
	{
		#region ICompositeRange<T>
		/// <summary>Returns a sequence of merged subranges. Should be used for operations over the ranges.</summary>
		/// <returns>A sequence of merged subranges</returns>
		IEnumerable<Range<T>> ICompositeRange<T>.GetMergedRanges() => GetMergedRanges();

		[NotNull]
		private IEnumerable<Range<T>> GetMergedRanges() => _hasRangesToMerge
			? MergeRangesCore()
			: SubRanges.Select(r => r.WithoutKey());

		[NotNull]
		private IEnumerable<Range<T>> MergeRangesCore()
		{
			var mergedRange = _emptyRangeNoKey;

			foreach (var range in SubRanges)
			{
				if (range.IsEmpty)
				{
					continue;
				}
				if (mergedRange.IsEmpty)
				{
					mergedRange = range.WithoutKey();
				}
				else if (IsContinuationFor(mergedRange.To, range))
				{
					mergedRange = mergedRange.ExtendTo(range.To);
				}
				else
				{
					yield return mergedRange;
					mergedRange = range.WithoutKey();
				}
			}

			if (mergedRange.IsNotEmpty)
			{
				yield return mergedRange;
			}
		}
		#endregion

		#region IEquatable<CompositeRange<T, TKey>>
		/// <summary>Indicates whether the current range is equal to another.</summary>
		/// <param name="other">A range to compare with this.</param>
		/// <returns>
		/// <c>True</c> if the current range is equal to the <paramref name="other"/> parameter;
		/// otherwise, false.
		/// </returns>
		public bool Equals(CompositeRange<T, TKey> other)
		{
			if (IsEmpty)
				return other.IsEmpty;
			if (other.IsEmpty)
				return false;

			DebugCode.BugIf(_ranges == null, "_ranges == null");
			DebugCode.BugIf(other._ranges == null, "other._ranges == null");

			var otherRanges = other._ranges;
			if (_containingRange != other._containingRange || _ranges.Count != otherRanges.Count)
				return false;

			var prevRange = Range<T>.Empty;
			var sameRangeKeys = new HashSet<TKey>();
			var sameRangeKeysOther = new HashSet<TKey>();

			for (int i = 0; i < _ranges.Count; i++)
			{
				var currentWithoutKey = _ranges[i].WithoutKey();
				if (!currentWithoutKey.Equals(otherRanges[i].WithoutKey()))
					return false;

				if (currentWithoutKey == prevRange)
				{
					sameRangeKeys.Add(_ranges[i].Key);
					sameRangeKeysOther.Add(otherRanges[i].Key);
				}
				else
				{
					if (!sameRangeKeys.SetEquals(sameRangeKeysOther))
						return false;

					sameRangeKeys.Clear();
					sameRangeKeysOther.Clear();
					sameRangeKeys.Add(_ranges[i].Key);
					sameRangeKeysOther.Add(otherRanges[i].Key);
				}
				prevRange = currentWithoutKey;
			}

			return sameRangeKeys.SetEquals(sameRangeKeysOther);
		}

		/// <summary>Indicates whether the current range and a specified object are equal.</summary>
		/// <param name="obj">The object to compare with this. </param>
		/// <returns>
		/// <c>True</c> if <paramref name="obj"/> and the current range are the same type
		/// and represent the same value; otherwise, false.
		/// </returns>
		public override bool Equals(object obj) =>
			obj is CompositeRange<T, TKey> && Equals((CompositeRange<T, TKey>)obj);

		/// <summary>Returns a hash code for the current range.</summary>
		/// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
		public override int GetHashCode()
		{
			int result = 0;
			foreach (var range in SubRanges)
			{
				result = HashCode.Combine(result, range.GetHashCode());
			}
			return result;
		}
		#endregion

	}
}