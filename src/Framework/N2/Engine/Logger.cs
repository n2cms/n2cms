using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace N2.Engine
{
	/// <summary>
	/// Wraps logging oprations performed by N2.
	/// </summary>
	public struct Logger<T>
	{
		public void Error(string message)
		{
			Trace.TraceError(message);
		}

		public void Error(Exception ex)
		{
			Trace.TraceError(ex.ToString());
		}

		public void Error(string message, Exception ex)
		{
			Trace.TraceError(message + Environment.NewLine + ex.ToString());
		}

		public void ErrorFormat(string format, params object[] args)
		{
			Trace.TraceError(format, args);
		}



		public void Warn(string message)
		{
			Trace.TraceWarning(message);
		}

		public void Warn(Exception ex)
		{
			Trace.TraceWarning(ex.ToString());
		}

		public void Warn(string message, Exception ex)
		{
			Trace.TraceWarning(message + Environment.NewLine + ex.ToString());
		}

		public void WarnFormat(string format, params object[] args)
		{
			Trace.TraceWarning(format, args);
		}



		public void Info(string message)
		{
			Trace.TraceInformation(message);
		}

		public void Info(Exception ex)
		{
			Trace.TraceInformation(ex.ToString());
		}

		public void Info(string message, Exception ex)
		{
			Trace.TraceInformation(message);
		}

		public void InfoFormat(string format, params object[] args)
		{
			Trace.TraceInformation(format, args);
		}



		[Conditional("DEBUG")]
		public void Debug(string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}

		[Conditional("DEBUG")]
		public void Debug(Exception ex)
		{
			System.Diagnostics.Debug.WriteLine(ex.ToString());
		}

		[Conditional("DEBUG")]
		public void Debug(string message, Exception ex)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}

		[Conditional("DEBUG")]
		public void DebugFormat(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(string.Format(format, args));
		}
	}

	/// <summary>
	/// Wraps logging oprations performed by N2.
	/// </summary>
	public static class Logger
	{
		public static void Error(string message)
		{
			Trace.TraceError(message);
		}

		public static void Error(Exception ex)
		{
			Trace.TraceError(ex.ToString());
		}

		public static void Error(string message, Exception ex)
		{
			Trace.TraceError(message + Environment.NewLine + ex.ToString());
		}

		public static void ErrorFormat(string format, params object[] args)
		{
			Trace.TraceError(format, args);
		}



		public static void Warn(string message)
		{
			Trace.TraceWarning(message);
		}

		public static void Warn(Exception ex)
		{
			Trace.TraceWarning(ex.ToString());
		}

		public static void Warn(string message, Exception ex)
		{
			Trace.TraceWarning(message + Environment.NewLine + ex.ToString());
		}

		public static void WarnFormat(string format, params object[] args)
		{
			Trace.TraceWarning(format, args);
		}



		public static void Info(string message)
		{
			Trace.TraceInformation(message);
		}

		public static void Info(Exception ex)
		{
			Trace.TraceInformation(ex.ToString());
		}

		public static void Info(string message, Exception ex)
		{
			Trace.TraceInformation(message);
		}

		public static void InfoFormat(string format, params object[] args)
		{
			Trace.TraceInformation(format, args);
		}



		[Conditional("DEBUG")]
		public static void Debug(string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}

		[Conditional("DEBUG")]
		public static void Debug(Exception ex)
		{
			System.Diagnostics.Debug.WriteLine(ex.ToString());
		}

		[Conditional("DEBUG")]
		public static void Debug(string message, Exception ex)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}

		[Conditional("DEBUG")]
		public static void DebugFormat(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(string.Format(format, args));
		}
	}
}
