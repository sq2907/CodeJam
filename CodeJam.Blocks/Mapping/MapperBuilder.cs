﻿#if NET40_OR_GREATER || TARGETS_NETSTANDARD || TARGETS_NETCOREAPP // PUBLIC_API_CHANGES. TODO: update after fixes in Theraot.Core
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

using JetBrains.Annotations;

namespace CodeJam.Mapping
{
	using Expressions;
	using Reflection;

	/// <summary>
	/// Builds a mapper that maps an object of <i>TFrom</i> type to an object of <i>TTo</i> type.
	/// </summary>
	/// <typeparam name="TFrom">Type to map from.</typeparam>
	/// <typeparam name="TTo">Type to map to.</typeparam>
	[PublicAPI]
	public class MapperBuilder<TFrom, TTo> : IMapperBuilder
	{
		[JetBrains.Annotations.NotNull]
		private MappingSchema _mappingSchema = MappingSchema.Default;

		/// <summary>
		/// Mapping schema.
		/// </summary>
		public MappingSchema MappingSchema
		{
			get => _mappingSchema;
			set =>
				// ReSharper disable once ConstantNullCoalescingCondition
				_mappingSchema = value ?? throw new ArgumentNullException(nameof(value), "MappingSchema cannot be null.");
		}

		/// <summary>
		/// Returns a mapper expression to map an object of <i>TFrom</i> type to an object of <i>TTo</i> type.
		/// Returned expression is compatible to IQueriable.
		/// </summary>
		/// <returns>Mapping expression.</returns>
		[Pure]
		public Expression<Func<TFrom, TTo>> GetMapperExpressionEx()
			=> (Expression<Func<TFrom, TTo>>)GetExpressionMapper().GetExpressionEx()!;

		LambdaExpression IMapperBuilder.GetMapperLambdaExpressionEx()
			=> GetExpressionMapper().GetExpressionEx()!;

		/*
		/// <summary>
		/// Returns a mapper to map an object of <i>TFrom</i> type to an object of <i>TTo</i> type.
		/// </summary>
		/// <returns>Mapping expression.</returns>
		[Pure]
		public Func<TFrom,TTo> GetMapperEx()
			=> GetMapperExpressionEx().Compile();
		*/

		/// <summary>
		/// Returns a mapper expression to map an object of <i>TFrom</i> type to an object of <i>TTo</i> type.
		/// </summary>
		/// <returns>Mapping expression.</returns>
		[Pure]
		[JetBrains.Annotations.NotNull]
		public Expression<Func<TFrom, TTo, IDictionary<object, object>?, TTo>> GetMapperExpression()
			=> (Expression<Func<TFrom, TTo, IDictionary<object, object>?, TTo>>)GetExpressionMapper().GetExpression();

		[JetBrains.Annotations.NotNull]
		LambdaExpression IMapperBuilder.GetMapperLambdaExpression()
			=> GetExpressionMapper().GetExpression();

		/// <summary>
		/// Returns a mapper to map an object of <i>TFrom</i> type to an object of <i>TTo</i> type.
		/// </summary>
		/// <returns>Mapping expression.</returns>
		[Pure]
		[JetBrains.Annotations.NotNull]
		public Mapper<TFrom, TTo> GetMapper()
			=> new(this);

		/// <summary>
		/// Sets mapping schema.
		/// </summary>
		/// <param name="schema">Mapping schema to set.</param>
		/// <returns>Returns this mapper.</returns>
		[JetBrains.Annotations.NotNull]
		public MapperBuilder<TFrom, TTo> SetMappingSchema([JetBrains.Annotations.NotNull] MappingSchema schema)
		{
			Code.NotNull(schema, nameof(schema));

			_mappingSchema = schema;
			return this;
		}

		/// <summary>
		/// Filters target members to map.
		/// </summary>
		public Func<MemberAccessor, bool> MemberFilter { get; set; } = _ => true;

		/// <summary>
		/// Adds a predicate to filter target members to map.
		/// </summary>
		/// <param name="predicate">Predicate to filter members to map.</param>
		/// <returns>Returns this mapper.</returns>
		[JetBrains.Annotations.NotNull]
		public MapperBuilder<TFrom, TTo> SetMemberFilter([JetBrains.Annotations.NotNull] Func<MemberAccessor, bool> predicate)
		{
			Code.NotNull(predicate, nameof(predicate));

			MemberFilter = predicate;
			return this;
		}

		/// <summary>
		/// Defines member name mapping for source types.
		/// </summary>
		public Dictionary<Type, Dictionary<string, string>>? FromMappingDictionary { get; set; }

		/// <summary>
		/// Defines member name mapping for source types.
		/// </summary>
		/// <param name="type">Type to map.</param>
		/// <param name="memberName">Type member name.</param>
		/// <param name="mapName">Mapping name.</param>
		/// <returns>Returns this mapper.</returns>
		[JetBrains.Annotations.NotNull]
		public MapperBuilder<TFrom, TTo> FromMapping([JetBrains.Annotations.NotNull] Type type, [JetBrains.Annotations.NotNull] string memberName, [JetBrains.Annotations.NotNull] string mapName)
		{
			Code.NotNull(type, nameof(type));
			Code.NotNull(memberName, nameof(memberName));
			Code.NotNull(mapName, nameof(mapName));

			if (FromMappingDictionary == null)
				FromMappingDictionary = new Dictionary<Type, Dictionary<string, string>>();

			if (!FromMappingDictionary.TryGetValue(type, out var dic))
				FromMappingDictionary[type] = dic = new Dictionary<string, string>();

			dic[memberName] = mapName;

			return this;
		}

		/// <summary>
		/// Defines member name mapping for source types.
		/// </summary>
		/// <typeparam name="T">Type to map.</typeparam>
		/// <param name="memberName">Type member name.</param>
		/// <param name="mapName">Mapping name.</param>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> FromMapping<T>([JetBrains.Annotations.NotNull] string memberName, [JetBrains.Annotations.NotNull] string mapName)
			=> FromMapping(typeof(T), memberName, mapName);

		/// <summary>
		/// Defines member name mapping for source types.
		/// </summary>
		/// <param name="memberName">Type member name.</param>
		/// <param name="mapName">Mapping name.</param>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> FromMapping([JetBrains.Annotations.NotNull] string memberName, [JetBrains.Annotations.NotNull] string mapName)
			=> FromMapping(typeof(TFrom), memberName, mapName);

		/// <summary>
		/// Defines member name mapping for source types.
		/// </summary>
		/// <param name="type">Type to map.</param>
		/// <param name="mapping">Mapping parameters.</param>
		/// <returns>Returns this mapper.</returns>
		[JetBrains.Annotations.NotNull]
		public MapperBuilder<TFrom, TTo> FromMapping([JetBrains.Annotations.NotNull] Type type, [JetBrains.Annotations.NotNull] IReadOnlyDictionary<string, string> mapping)
		{
			Code.NotNull(type, nameof(type));
			Code.NotNull(mapping, nameof(mapping));

			foreach (var item in mapping)
				FromMapping(type, item.Key, item.Value);

			return this;
		}

		/// <summary>
		/// Defines member name mapping for source types.
		/// </summary>
		/// <param name="mapping">Mapping parameters.</param>
		/// <typeparam name="T">Type to map.</typeparam>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> FromMapping<T>([JetBrains.Annotations.NotNull] IReadOnlyDictionary<string, string> mapping)
			=> FromMapping(typeof(T), mapping);

		/// <summary>
		/// Defines member name mapping for source types.
		/// </summary>
		/// <param name="mapping">Mapping parameters.</param>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> FromMapping([JetBrains.Annotations.NotNull] IReadOnlyDictionary<string, string> mapping)
			=> FromMapping(typeof(TFrom), mapping);

		/// <summary>
		/// Defines member name mapping for destination types.
		/// </summary>
		public Dictionary<Type, Dictionary<string, string>>? ToMappingDictionary { get; set; }

		/// <summary>
		/// Defines member name mapping for destination types.
		/// </summary>
		/// <param name="type">Type to map.</param>
		/// <param name="memberName">Type member name.</param>
		/// <param name="mapName">Mapping name.</param>
		/// <returns>Returns this mapper.</returns>
		[JetBrains.Annotations.NotNull]
		public MapperBuilder<TFrom, TTo> ToMapping([JetBrains.Annotations.NotNull] Type type, [JetBrains.Annotations.NotNull] string memberName, string mapName)
		{
			if (ToMappingDictionary == null)
				ToMappingDictionary = new Dictionary<Type, Dictionary<string, string>>();

			if (!ToMappingDictionary.TryGetValue(type, out var dic))
				ToMappingDictionary[type] = dic = new Dictionary<string, string>();

			dic[memberName] = mapName;

			return this;
		}

		/// <summary>
		/// Defines member name mapping for destination types.
		/// </summary>
		/// <typeparam name="T">Type to map.</typeparam>
		/// <param name="memberName">Type member name.</param>
		/// <param name="mapName">Mapping name.</param>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> ToMapping<T>(string memberName, string mapName)
			=> ToMapping(typeof(T), memberName, mapName);

		/// <summary>
		/// Defines member name mapping for destination types.
		/// </summary>
		/// <param name="memberName">Type member name.</param>
		/// <param name="mapName">Mapping name.</param>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> ToMapping(string memberName, string mapName)
			=> ToMapping(typeof(TTo), memberName, mapName);

		/// <summary>
		/// Defines member name mapping for destination types.
		/// </summary>
		/// <param name="type">Type to map.</param>
		/// <param name="mapping">Mapping parameters.</param>
		/// <returns>Returns this mapper.</returns>
		[JetBrains.Annotations.NotNull]
		public MapperBuilder<TFrom, TTo> ToMapping([JetBrains.Annotations.NotNull] Type type, [JetBrains.Annotations.NotNull] IReadOnlyDictionary<string, string> mapping)
		{
			Code.NotNull(type, nameof(type));
			Code.NotNull(mapping, nameof(mapping));

			foreach (var item in mapping)
				ToMapping(type, item.Key, item.Value);

			return this;
		}

		/// <summary>
		/// Defines member name mapping for destination types.
		/// </summary>
		/// <param name="mapping">Mapping parameters.</param>
		/// <typeparam name="T">Type to map.</typeparam>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> ToMapping<T>([JetBrains.Annotations.NotNull] IReadOnlyDictionary<string, string> mapping)
			=> ToMapping(typeof(T), mapping);

		/// <summary>
		/// Defines member name mapping for destination types.
		/// </summary>
		/// <param name="mapping">Mapping parameters.</param>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> ToMapping([JetBrains.Annotations.NotNull] IReadOnlyDictionary<string, string> mapping)
			=> ToMapping(typeof(TTo), mapping);

		/// <summary>
		/// Defines member name mapping for source and destination types.
		/// </summary>
		/// <param name="type">Type to map.</param>
		/// <param name="memberName">Type member name.</param>
		/// <param name="mapName">Mapping name.</param>
		/// <returns>Returns this mapper.</returns>
		[JetBrains.Annotations.NotNull]
		public MapperBuilder<TFrom, TTo> Mapping([JetBrains.Annotations.NotNull] Type type, [JetBrains.Annotations.NotNull] string memberName, [JetBrains.Annotations.NotNull] string mapName)
			=> FromMapping(type, memberName, mapName).ToMapping(type, memberName, mapName);

		/// <summary>
		/// Defines member name mapping for source and destination types.
		/// </summary>
		/// <typeparam name="T">Type to map.</typeparam>
		/// <param name="memberName">Type member name.</param>
		/// <param name="mapName">Mapping name.</param>
		/// <returns>Returns this mapper.</returns>
		[JetBrains.Annotations.NotNull]
		public MapperBuilder<TFrom, TTo> Mapping<T>([JetBrains.Annotations.NotNull] string memberName, [JetBrains.Annotations.NotNull] string mapName)
			=> Mapping(typeof(T), memberName, mapName);

		/// <summary>
		/// Defines member name mapping for source and destination types.
		/// </summary>
		/// <param name="memberName">Type member name.</param>
		/// <param name="mapName">Mapping name.</param>
		/// <returns>Returns this mapper.</returns>
		[JetBrains.Annotations.NotNull]
		public MapperBuilder<TFrom, TTo> Mapping(string memberName, string mapName)
			=> Mapping(typeof(TFrom), memberName, mapName).Mapping(typeof(TTo), memberName, mapName);

		/// <summary>
		/// Defines member name mapping for source and destination types.
		/// </summary>
		/// <param name="type">Type to map.</param>
		/// <param name="mapping">Mapping parameters.</param>
		/// <returns>Returns this mapper.</returns>
		[JetBrains.Annotations.NotNull]
		public MapperBuilder<TFrom, TTo> Mapping([JetBrains.Annotations.NotNull] Type type, [JetBrains.Annotations.NotNull] IReadOnlyDictionary<string, string> mapping)
		{
			Code.NotNull(type, nameof(type));
			Code.NotNull(mapping, nameof(mapping));

			foreach (var item in mapping)
				Mapping(type, item.Key, item.Value);

			return this;
		}

		/// <summary>
		/// Defines member name mapping for source and destination types.
		/// </summary>
		/// <param name="mapping">Mapping parameters.</param>
		/// <typeparam name="T">Type to map.</typeparam>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> Mapping<T>([JetBrains.Annotations.NotNull] IReadOnlyDictionary<string, string> mapping)
			=> Mapping(typeof(T), mapping);

		/// <summary>
		/// Defines member name mapping for source and destination types.
		/// </summary>
		/// <param name="mapping">Mapping parameters.</param>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> Mapping([JetBrains.Annotations.NotNull] IReadOnlyDictionary<string, string> mapping)
			=> Mapping(typeof(TFrom), mapping).Mapping(typeof(TFrom), mapping);

		/// <summary>
		/// Member mappers.
		/// </summary>
		public List<MemberMapper>? MemberMappers { get; set; }

		/// <summary>
		/// Adds member mapper.
		/// </summary>
		/// <typeparam name="T">Type of the member to map.</typeparam>
		/// <param name="toMember">Expression that returns a member to map.</param>
		/// <param name="setter">Expression to set the member.</param>
		/// <returns>Returns this mapper.</returns>
		/// <example>
		/// This example shows how to explicitly convert one value to another.
		/// <code source="CodeJam.Blocks.Tests\Mapping\Examples\MapMemberTests.cs" region="Example" lang="C#"/>
		/// </example>
		[JetBrains.Annotations.NotNull]
		[MemberNotNull("MemberMappers")]
		public MapperBuilder<TFrom, TTo> MapMember<T>(Expression<Func<TTo, T>> toMember, Expression<Func<TFrom, T>> setter)
		{
			if (MemberMappers == null)
				MemberMappers = new List<MemberMapper>();

			MemberMappers.Add(new MemberMapper(toMember, setter));

			return this;
		}

		/// <summary>
		/// If true, processes object cross references.
		/// if default (null), the <see cref="GetMapperExpressionEx"/> method does not process cross references,
		/// however the <see cref="GetMapperExpression"/> method does.
		/// </summary>
		public bool? ProcessCrossReferences { get; set; }

		/// <summary>
		/// If true, processes object cross references.
		/// </summary>
		/// <param name="doProcess">If true, processes object cross references.</param>
		/// <returns>Returns this mapper.</returns>
		[JetBrains.Annotations.NotNull]
		public MapperBuilder<TFrom, TTo> SetProcessCrossReferences(bool? doProcess)
		{
			ProcessCrossReferences = doProcess;
			return this;
		}

		/// <summary>
		/// If true, performs deep copy.
		/// if default (null), the <see cref="GetMapperExpressionEx"/> method does not do deep copy,
		/// however the <see cref="GetMapperExpression"/> method does.
		/// </summary>
		public bool? DeepCopy { get; set; }

		/// <summary>
		/// Type to map from.
		/// </summary>
		public Type FromType => typeof(TFrom);

		/// <summary>
		/// Type to map to.
		/// </summary>
		public Type ToType => typeof(TTo);

		/// <summary>
		/// If true, performs deep copy.
		/// </summary>
		/// <param name="deepCopy">If true, performs deep copy.</param>
		/// <returns>Returns this mapper.</returns>
		[JetBrains.Annotations.NotNull]
		public MapperBuilder<TFrom, TTo> SetDeepCopy(bool? deepCopy)
		{
			DeepCopy = deepCopy;
			return this;
		}

		/// <summary>
		/// Gets an instance of <see cref="ExpressionBuilder"/> class.
		/// </summary>
		/// <returns><see cref="ExpressionBuilder"/>.</returns>
		[JetBrains.Annotations.NotNull]
		internal ExpressionBuilder GetExpressionMapper()
			=> new(this, MemberMappers?.Select(mm => ValueTuple.Create(mm.From.GetMembersInfo(), mm.To)).ToArray()!);
	}
}
#endif