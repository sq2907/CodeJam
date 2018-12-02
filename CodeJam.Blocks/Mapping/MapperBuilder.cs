﻿#if !LESSTHAN_NET40
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
		[NotNull]
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
			=> (Expression<Func<TFrom, TTo>>)GetExpressionMapper().GetExpressionEx();

		LambdaExpression IMapperBuilder.GetMapperLambdaExpressionEx()
			=> GetExpressionMapper().GetExpressionEx();

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
		public Expression<Func<TFrom, TTo, IDictionary<object, object>, TTo>> GetMapperExpression()
			=> (Expression<Func<TFrom, TTo, IDictionary<object, object>, TTo>>)GetExpressionMapper().GetExpression();

		LambdaExpression IMapperBuilder.GetMapperLambdaExpression()
			=> GetExpressionMapper().GetExpression();

		/// <summary>
		/// Returns a mapper to map an object of <i>TFrom</i> type to an object of <i>TTo</i> type.
		/// </summary>
		/// <returns>Mapping expression.</returns>
		[Pure]
		public Mapper<TFrom, TTo> GetMapper()
			=> new Mapper<TFrom, TTo>(this);

		/// <summary>
		/// Sets mapping schema.
		/// </summary>
		/// <param name="schema">Mapping schema to set.</param>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> SetMappingSchema([NotNull] MappingSchema schema)
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
		public MapperBuilder<TFrom, TTo> SetMemberFilter([NotNull] Func<MemberAccessor, bool> predicate)
		{
			Code.NotNull(predicate, nameof(predicate));

			MemberFilter = predicate;
			return this;
		}

		/// <summary>
		/// Defines member name mapping for source types.
		/// </summary>
		public Dictionary<Type, Dictionary<string, string>> FromMappingDictionary { get; set; }

		/// <summary>
		/// Defines member name mapping for source types.
		/// </summary>
		/// <param name="type">Type to map.</param>
		/// <param name="memberName">Type member name.</param>
		/// <param name="mapName">Mapping name.</param>
		/// <returns>Returns this mapper.</returns>
		[NotNull]
		public MapperBuilder<TFrom, TTo> FromMapping([NotNull] Type type, [NotNull] string memberName, [NotNull] string mapName)
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
		public MapperBuilder<TFrom, TTo> FromMapping<T>([NotNull] string memberName, [NotNull] string mapName)
			=> FromMapping(typeof(T), memberName, mapName);

		/// <summary>
		/// Defines member name mapping for source types.
		/// </summary>
		/// <param name="memberName">Type member name.</param>
		/// <param name="mapName">Mapping name.</param>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> FromMapping([NotNull] string memberName, [NotNull] string mapName)
			=> FromMapping(typeof(TFrom), memberName, mapName);

		/// <summary>
		/// Defines member name mapping for source types.
		/// </summary>
		/// <param name="type">Type to map.</param>
		/// <param name="mapping">Mapping parameters.</param>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> FromMapping([NotNull] Type type, [NotNull] IReadOnlyDictionary<string, string> mapping)
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
		public MapperBuilder<TFrom, TTo> FromMapping<T>(IReadOnlyDictionary<string, string> mapping)
			=> FromMapping(typeof(T), mapping);

		/// <summary>
		/// Defines member name mapping for source types.
		/// </summary>
		/// <param name="mapping">Mapping parameters.</param>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> FromMapping(IReadOnlyDictionary<string, string> mapping)
			=> FromMapping(typeof(TFrom), mapping);

		/// <summary>
		/// Defines member name mapping for destination types.
		/// </summary>
		public Dictionary<Type, Dictionary<string, string>> ToMappingDictionary { get; set; }

		/// <summary>
		/// Defines member name mapping for destination types.
		/// </summary>
		/// <param name="type">Type to map.</param>
		/// <param name="memberName">Type member name.</param>
		/// <param name="mapName">Mapping name.</param>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> ToMapping(Type type, string memberName, string mapName)
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
		public MapperBuilder<TFrom, TTo> ToMapping([NotNull] Type type, [NotNull] IReadOnlyDictionary<string, string> mapping)
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
		public MapperBuilder<TFrom, TTo> ToMapping<T>(IReadOnlyDictionary<string, string> mapping)
			=> ToMapping(typeof(T), mapping);

		/// <summary>
		/// Defines member name mapping for destination types.
		/// </summary>
		/// <param name="mapping">Mapping parameters.</param>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> ToMapping(IReadOnlyDictionary<string, string> mapping)
			=> ToMapping(typeof(TTo), mapping);

		/// <summary>
		/// Defines member name mapping for source and destination types.
		/// </summary>
		/// <param name="type">Type to map.</param>
		/// <param name="memberName">Type member name.</param>
		/// <param name="mapName">Mapping name.</param>
		/// <returns>Returns this mapper.</returns>
		[NotNull]
		public MapperBuilder<TFrom, TTo> Mapping(Type type, string memberName, string mapName)
			=> FromMapping(type, memberName, mapName).ToMapping(type, memberName, mapName);

		/// <summary>
		/// Defines member name mapping for source and destination types.
		/// </summary>
		/// <typeparam name="T">Type to map.</typeparam>
		/// <param name="memberName">Type member name.</param>
		/// <param name="mapName">Mapping name.</param>
		/// <returns>Returns this mapper.</returns>
		[NotNull]
		public MapperBuilder<TFrom, TTo> Mapping<T>(string memberName, string mapName)
			=> Mapping(typeof(T), memberName, mapName);

		/// <summary>
		/// Defines member name mapping for source and destination types.
		/// </summary>
		/// <param name="memberName">Type member name.</param>
		/// <param name="mapName">Mapping name.</param>
		/// <returns>Returns this mapper.</returns>
		[NotNull]
		public MapperBuilder<TFrom, TTo> Mapping(string memberName, string mapName)
			=> Mapping(typeof(TFrom), memberName, mapName).Mapping(typeof(TTo), memberName, mapName);

		/// <summary>
		/// Defines member name mapping for source and destination types.
		/// </summary>
		/// <param name="type">Type to map.</param>
		/// <param name="mapping">Mapping parameters.</param>
		/// <returns>Returns this mapper.</returns>
		[NotNull]
		public MapperBuilder<TFrom, TTo> Mapping([NotNull] Type type, [NotNull] IReadOnlyDictionary<string, string> mapping)
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
		public MapperBuilder<TFrom, TTo> Mapping<T>(IReadOnlyDictionary<string, string> mapping)
			=> Mapping(typeof(T), mapping);

		/// <summary>
		/// Defines member name mapping for source and destination types.
		/// </summary>
		/// <param name="mapping">Mapping parameters.</param>
		/// <returns>Returns this mapper.</returns>
		public MapperBuilder<TFrom, TTo> Mapping(IReadOnlyDictionary<string, string> mapping)
			=> Mapping(typeof(TFrom), mapping).Mapping(typeof(TFrom), mapping);

		/// <summary>
		/// Member mappers.
		/// </summary>
		public List<ValueTuple<LambdaExpression, LambdaExpression>> MemberMappers { get; set; }

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
		public MapperBuilder<TFrom, TTo> MapMember<T>(Expression<Func<TTo, T>> toMember, Expression<Func<TFrom, T>> setter)
		{
			if (MemberMappers == null)
				MemberMappers = new List<ValueTuple<LambdaExpression, LambdaExpression>>();

			MemberMappers.Add(ValueTuple.Create((LambdaExpression)toMember, (LambdaExpression)setter));

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
		public MapperBuilder<TFrom, TTo> SetDeepCopy(bool? deepCopy)
		{
			DeepCopy = deepCopy;
			return this;
		}

		/// <summary>
		/// Gets an instance of <see cref="ExpressionBuilder"/> class.
		/// </summary>
		/// <returns><see cref="ExpressionBuilder"/>.</returns>
		[NotNull]
		internal ExpressionBuilder GetExpressionMapper()
			=> new ExpressionBuilder(this, MemberMappers?.Select(mm => ValueTuple.Create(mm.Item1.GetMembersInfo(), mm.Item2)).ToArray());
	}
}
#endif