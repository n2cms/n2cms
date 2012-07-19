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
			Trace.TraceError(Logger.PrependInfo<T>(message));
		}

		public void Error(Exception ex)
		{
			Trace.TraceError(Logger.PrependInfo<T>(ex.ToString()));
		}

		public void Error(string message, Exception ex)
		{
			Trace.TraceError(Logger.PrependInfo<T>(message + Environment.NewLine + ex));
		}

		public void ErrorFormat(string format, params object[] args)
		{
			Trace.TraceError(Logger.PrependInfo<T>(format), args);
		}



		public void Warn(string message)
		{
			Trace.TraceWarning(Logger.PrependInfo<T>(message));
		}

		public void Warn(Exception ex)
		{
			Trace.TraceWarning(Logger.PrependInfo<T>(ex.ToString()));
		}

		public void Warn(string message, Exception ex)
		{
			Trace.TraceWarning(Logger.PrependInfo<T>(message + Environment.NewLine + ex));
		}

		public void WarnFormat(string format, params object[] args)
		{
			Trace.TraceWarning(Logger.PrependInfo<T>(format), args);
		}



		public void Info(string message)
		{
			Trace.TraceInformation(Logger.PrependInfo<T>(message));
		}

		public void Info(Exception ex)
		{
			Trace.TraceInformation(Logger.PrependInfo<T>(ex.ToString()));
		}

		public void Info(string message, Exception ex)
		{
			Trace.TraceInformation(Logger.PrependInfo<T>(message));
		}

		public void InfoFormat(string format, params object[] args)
		{
			Trace.TraceInformation(Logger.PrependInfo<T>(format), args);
		}



		[Conditional("DEBUG")]
		public void Debug(string message)
		{
			System.Diagnostics.Debug.WriteLine(Logger.PrependInfo<T>(message));
		}

		[Conditional("DEBUG")]
		public void Debug(Exception ex)
		{
			System.Diagnostics.Debug.WriteLine(Logger.PrependInfo<T>(ex.ToString()));
		}

		[Conditional("DEBUG")]
		public void Debug(string message, Exception ex)
		{
			System.Diagnostics.Debug.WriteLine(Logger.PrependInfo<T>(message));
		}

		[Conditional("DEBUG")]
		public void DebugFormat(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(Logger.PrependInfo<T>(string.Format(format, args)));
		}
	}

	/// <summary>
	/// Wraps logging oprations performed by N2.
	/// </summary>
	public static class Logger
	{
		public static void Error(string message)
		{
			Trace.TraceError(Logger.PrependInfo(message));
		}

		public static void Error(Exception ex)
		{
			Trace.TraceError(Logger.PrependInfo(ex.ToString()));
		}

		public static void Error(string message, Exception ex)
		{
			Trace.TraceError(Logger.PrependInfo(message + Environment.NewLine + ex));
		}

		public static void ErrorFormat(string format, params object[] args)
		{
			Trace.TraceError(Logger.PrependInfo(format), args);
		}



		public static void Warn(string message)
		{
			Trace.TraceWarning(Logger.PrependInfo(message));
		}

		public static void Warn(Exception ex)
		{
			Trace.TraceWarning(Logger.PrependInfo(ex.ToString()));
		}

		public static void Warn(string message, Exception ex)
		{
			Trace.TraceWarning(Logger.PrependInfo(message + Environment.NewLine + ex));
		}

		public static void WarnFormat(string format, params object[] args)
		{
			Trace.TraceWarning(Logger.PrependInfo(format), args);
		}



		public static void Info(string message)
		{
			Trace.TraceInformation(Logger.PrependInfo(message));
		}

		public static void Info(Exception ex)
		{
			Trace.TraceInformation(Logger.PrependInfo(ex.ToString()));
		}

		public static void Info(string message, Exception ex)
		{
			Trace.TraceInformation(Logger.PrependInfo(message + Environment.NewLine + ex));
		}

		public static void InfoFormat(string format, params object[] args)
		{
			Trace.TraceInformation(Logger.PrependInfo(format), args);
		}



		[Conditional("DEBUG")]
		public static void Debug(string message)
		{
			System.Diagnostics.Debug.WriteLine(Logger.PrependInfo(message));
		}

		[Conditional("DEBUG")]
		public static void Debug(Exception ex)
		{
			System.Diagnostics.Debug.WriteLine(Logger.PrependInfo(ex.ToString()));
		}

		[Conditional("DEBUG")]
		public static void Debug(string message, Exception ex)
		{
			System.Diagnostics.Debug.WriteLine(Logger.PrependInfo(message));
		}

		[Conditional("DEBUG")]
		public static void DebugFormat(string format, params object[] args)
		{
			System.Diagnostics.Debug.WriteLine(string.Format(Logger.PrependInfo(format), args));
		}

		internal static string PrependInfo(string message)
		{
			return DateTime.UtcNow.ToString("yyy-MM-dd HH:mm:ss.fff")
				+ ": "
				+ message;
		}

		internal static string PrependInfo(string category, string message)
		{
			return DateTime.UtcNow.ToString("yyy-MM-dd HH:mm:ss.fff ")
				+ category
				+ ": "
				+ message;
		}

		internal static string PrependInfo<T>(string message)
		{
			return PrependInfo("<" + typeof(T).Name + ">", message);
		}
	}
}
