﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable

using System.Collections.Generic;

using JetBrains.Annotations;

using SuppressMessageAttribute = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;

namespace CodeJam.Ranges
{
	/// <summary>Extension methods for <seealso cref="CompositeRange{T}"/>.</summary>
	[SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
	[SuppressMessage("ReSharper", "ArrangeBraces_while")]
	public static partial class CompositeRangeExtensions
	{
		#region ToCompositeRange
		/// <summary>Converts range to the composite range.</summary>
		/// <typeparam name="T">The type of the range values.</typeparam>
		/// <typeparam name="TKey">The type of the range key</typeparam>
		/// <param name="range">The range.</param>
		/// <returns>A new composite range.</returns>
		[Pure]
		public static CompositeRange<T, TKey> ToCompositeRange<T, TKey>(this Range<T, TKey> range) =>
			new CompositeRange<T, TKey>(range);

		/// <summary>Converts sequence of elements to the composite range.</summary>
		/// <typeparam name="T">The type of the range values.</typeparam>
		/// <typeparam name="TKey">The type of the range key</typeparam>
		/// <param name="ranges">The ranges.</param>
		/// <returns>A new composite range.</returns>
		[Pure]
		public static CompositeRange<T, TKey> ToCompositeRange<T, TKey>([NotNull] this IEnumerable<Range<T, TKey>> ranges) =>
			new CompositeRange<T, TKey>(ranges);
		#endregion
	}
}