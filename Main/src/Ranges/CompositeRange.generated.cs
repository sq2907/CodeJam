﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;

using JetBrains.Annotations;

namespace CodeJam.Ranges
{
	/// <summary>Helper methods for the <seealso cref="CompositeRange{T}"/>.</summary>
	[PublicAPI]
	public static partial class CompositeRange
	{
		/// <summary>Creates the composite range.</summary>
		/// <typeparam name="T">The type of the range values.</typeparam>
		/// <typeparam name="TKey">The type of the range key</typeparam>
		/// <param name="range">The range.</param>
		/// <returns>A new composite range.</returns>
		[Pure]
		public static CompositeRange<T, TKey> Create<T, TKey>(Range<T, TKey> range) =>
			new CompositeRange<T, TKey>(range);

		/// <summary>Creates the composite range.</summary>
		/// <typeparam name="T">The type of the range values.</typeparam>
		/// <typeparam name="TKey">The type of the range key</typeparam>
		/// <param name="ranges">The ranges.</param>
		/// <returns>A new composite range.</returns>
		[Pure]
		public static CompositeRange<T, TKey> Create<T, TKey>([NotNull] params Range<T, TKey>[] ranges) =>
			new CompositeRange<T, TKey>(ranges);
	}
}