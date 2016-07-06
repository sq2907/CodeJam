﻿using System.IO;
using System.Reflection;
using System.Threading;

using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Toolchains.InProcess;

using CodeJam.PerfTests.Configs;
using CodeJam.PerfTests.Loggers;

using JetBrains.Annotations;

namespace CodeJam.PerfTests
{
	// TODO: xml docs
	/// <summary>
	/// Class with helper metods be used for perf tests
	/// </summary>
	[PublicAPI]
	public static class CompetitionHelpers
	{
		#region Constants
		/// <summary>
		/// Please, mark all benchmark classes with [TestFixture(Category = BenchmarkConstants.BenchmarkCategory)].
		/// That way it's easier to sort out them in a Test Explorer window
		/// </summary>
		public const string PerfTestCategory = "Performance";

		/// <summary>
		/// Please, mark all benchmark classes with [Explicit(BenchmarkConstants.ExplicitExcludeReason)]
		/// Explanation: read the constant' value;)
		/// </summary>
		public const string ExplicitExcludeReason = @"Autorun disabled as it takes too long to run.
Also, running this on debug builds may produce inaccurate results.
Please, run it manually from the Test Explorer window. Remember to use release builds. Thanks and have a nice day:)";
		#endregion

		#region Benchmark-related
		/// <summary>The default count for perf test loops.</summary>
		public const int DefaultCount = 10 * 1000;

		/// <summary>Performs delay for specified number of cycles.</summary>
		/// <param name="cycles">The number of cycles to delay.</param>
		public static void Delay(int cycles) => Thread.SpinWait(cycles);
		#endregion

		#region Configs core
		public static ILogger CreateDetailedLogger() =>
			new FlushableStreamLogger(
				GetLogWriter(Assembly.GetCallingAssembly().GetName().Name + ".AllPerfTests.log"));

		public static ILogger CreateImportantInfoLogger() =>
			new HostLogger(
				new FlushableStreamLogger(
					GetLogWriter(Assembly.GetCallingAssembly().GetName().Name + ".Short.AllPerfTests.log")),
				HostLogMode.PrefixedOnly);

		private static StreamWriter GetLogWriter(string fileName)
		{
			var path = Path.GetFullPath(fileName);

			return new StreamWriter(
				new FileStream(
					path,
					FileMode.Create, FileAccess.Write, FileShare.Read));
		}
		#endregion

		#region Configs
		public static ManualCompetitionConfig CreateRunConfig(Platform platform = Platform.Host)
		{
			var result = new ManualCompetitionConfig(DefaultCompetitionConfig.Instance)
			{
				RerunIfLimitsFailed = true
			};
			result.Add(
				new Job
				{
					LaunchCount = 1,
					Mode = Mode.SingleRun,
					WarmupCount = 200,
					TargetCount = 500,
					Platform = platform,
					Jit = platform == Platform.X64 ? Jit.RyuJit : Jit.Host,
					Toolchain = InProcessToolchain.DontLogOutput
				});

			return result;
		}

		public static ManualCompetitionConfig CreateRunConfigAnnotate(Platform platform = Platform.Host)
		{
			var result = CreateRunConfig(platform);
			result.LogCompetitionLimits = true;
			result.RerunIfLimitsFailed = true;
			result.UpdateSourceAnnotations = true;
			result.Add(Exporters.TimingsExporter.Instance);
			return result;
		}

		public static ManualCompetitionConfig CreateRunConfigReAnnotate(Platform platform = Platform.Host)
		{
			var result = CreateRunConfigAnnotate(platform);
			result.IgnoreExistingAnnotations = true;
			return result;
		}
		#endregion
	}
}