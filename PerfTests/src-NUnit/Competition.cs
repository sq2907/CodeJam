﻿using System;

using CodeJam.PerfTests.Configs;
using CodeJam.PerfTests.Running.Core;

using JetBrains.Annotations;

namespace CodeJam.PerfTests
{
	/// <summary>NUnit runner for competition performance tests.</summary>
	[PublicAPI]
	public static class Competition
	{
		private static CompetitionRunnerBase Runner => new NUnitCompetitionRunner();

		/// <summary>Runs the benchmark.</summary>
		/// <typeparam name="T">Benchmark class to run.</typeparam>
		/// <param name="thisReference">Reference used to infer type of the benchmark.</param>
		/// <param name="competitionConfig">The competition config.</param>
		/// <returns>The state of the competition.</returns>
		[NotNull]
		public static CompetitionState Run<T>([NotNull] T thisReference, [CanBeNull] ICompetitionConfig competitionConfig)
			where T : class =>
				Runner.Run(thisReference, competitionConfig);

		/// <summary>Runs the benchmark.</summary>
		/// <typeparam name="T">Benchmark class to run.</typeparam>
		/// <param name="competitionConfig">The competition config.</param>
		/// <returns>The state of the competition.</returns>
		[NotNull]
		public static CompetitionState Run<T>([CanBeNull] ICompetitionConfig competitionConfig)
			where T : class =>
				Runner.Run<T>(competitionConfig);
	}
}