// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Core.Logging
{
#if !SILVERLIGHT
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
#if DOTNET40
    using System.Security;
#endif

    /// <summary>
    ///   The TraceLogger sends all logging to the System.Diagnostics.TraceSource
    ///   built into the .net framework.
    /// </summary>
    /// <remarks>
    ///   Logging can be configured in the system.diagnostics configuration 
    ///   section. 
    /// 
    ///   If logger doesn't find a source name with a full match it will
    ///   use source names which match the namespace partially. For example you can
    ///   configure from all castle components by adding a source name with the
    ///   name "Castle". 
    /// 
    ///   If no portion of the namespace matches the source named "Default" will
    ///   be used.
    /// </remarks>
    public class TraceLogger : ILogger
    {
        private static readonly Dictionary<string, TraceSource> cache = new Dictionary<string, TraceSource>();

        private TraceSource traceSource;

        /// <summary>
        /// Build a new trace logger based on the named TraceSource
        /// </summary>
        /// <param name="name">The name used to locate the best TraceSource. In most cases comes from the using type's fullname.</param>
#if DOTNET40
        [SecuritySafeCritical]
#endif
        public TraceLogger(string name)
        {
            Name = name;
            Initialize();
            Level = MapLoggerLevel(traceSource.Switch.Level);
        }

        /// <summary>
        /// Build a new trace logger based on the named TraceSource
        /// </summary>
        /// <param name="name">The name used to locate the best TraceSource. In most cases comes from the using type's fullname.</param>
        /// <param name="level">The default logging level at which this source should write messages. In almost all cases this
        /// default value will be overridden in the config file. </param>
#if DOTNET40
        [SecuritySafeCritical]
#endif
        public TraceLogger(string name, LoggerLevel level)
        {
            Name = name;
            Initialize();
            Level = MapLoggerLevel(traceSource.Switch.Level);
        }

        /// <summary>
        /// Create a new child logger.
        /// The name of the child logger is [current-loggers-name].[passed-in-name]
        /// </summary>
        /// <param name="loggerName">The Subname of this logger.</param>
        /// <returns>The New ILogger instance.</returns> 
#if DOTNET40
        [SecuritySafeCritical]
#endif
        public ILogger CreateChildLogger(string loggerName)
        {
            return InternalCreateChildLogger(loggerName);
        }

#if DOTNET40
        [SecurityCritical]
#endif
        private ILogger InternalCreateChildLogger(string loggerName)
        {
            return new TraceLogger(string.Concat(Name, ".", loggerName), Level);
        }

        protected void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception)
        {
            if (exception == null)
            {
                traceSource.TraceEvent(MapTraceEventType(loggerLevel), 0, message);
            }
            else
            {
                traceSource.TraceData(MapTraceEventType(loggerLevel), 0, message, exception);
            }
        }

#if DOTNET40
        [SecurityCritical]
#endif
        private void Initialize()
        {
            lock (cache)
            {
                // because TraceSource is meant to be used as a static member, and because
                // building up the configuraion inheritance is non-trivial, the instances
                // themselves are cached for so multiple TraceLogger instances will reuse
                // the named TraceSources which have been created

                if (cache.TryGetValue(Name, out traceSource))
                {
                    return;
                }

                var defaultLevel = MapSourceLevels(Level);
                traceSource = new TraceSource(Name, defaultLevel);

                // no further action necessary when the named source is configured
                if (IsSourceConfigured(traceSource))
                {
                    cache.Add(Name, traceSource);
                    return;
                }

                // otherwise hunt for a shorter source that been configured            
                var foundSource = new TraceSource("Default", defaultLevel);

                var searchName = ShortenName(Name);
                while (!string.IsNullOrEmpty(searchName))
                {
                    var searchSource = new TraceSource(searchName, defaultLevel);
                    if (IsSourceConfigured(searchSource))
                    {
                        foundSource = searchSource;
                        break;
                    }

                    searchName = ShortenName(searchName);
                }

                // reconfigure the created source to act like the found source
                traceSource.Switch = foundSource.Switch;
                traceSource.Listeners.Clear();
                foreach (TraceListener listener in foundSource.Listeners)
                {
                    traceSource.Listeners.Add(listener);
                }

                cache.Add(Name, traceSource);
            }
        }

        private static string ShortenName(string name)
        {
            var lastDot = name.LastIndexOf('.');
            if (lastDot != -1)
            {
                return name.Substring(0, lastDot);
            }
            return null;
        }

#if DOTNET40
        [SecuritySafeCritical]
#endif
        private static bool IsSourceConfigured(TraceSource source)
        {
            if (source.Listeners.Count == 1 &&
                source.Listeners[0] is DefaultTraceListener &&
                source.Listeners[0].Name == "Default")
            {
                return false;
            }
            return true;
        }

        private static LoggerLevel MapLoggerLevel(SourceLevels level)
        {
            switch (level)
            {
                case SourceLevels.All:
                    return LoggerLevel.Debug;
                case SourceLevels.Verbose:
                    return LoggerLevel.Debug;
                case SourceLevels.Information:
                    return LoggerLevel.Info;
                case SourceLevels.Warning:
                    return LoggerLevel.Warn;
                case SourceLevels.Error:
                    return LoggerLevel.Error;
                case SourceLevels.Critical:
                    return LoggerLevel.Fatal;
            }
            return LoggerLevel.Off;
        }

        private static SourceLevels MapSourceLevels(LoggerLevel level)
        {
            switch (level)
            {
                case LoggerLevel.Debug:
                    return SourceLevels.Verbose;
                case LoggerLevel.Info:
                    return SourceLevels.Information;
                case LoggerLevel.Warn:
                    return SourceLevels.Warning;
                case LoggerLevel.Error:
                    return SourceLevels.Error;
                case LoggerLevel.Fatal:
                    return SourceLevels.Critical;
            }
            return SourceLevels.Off;
        }

        private static TraceEventType MapTraceEventType(LoggerLevel level)
        {
            switch (level)
            {
                case LoggerLevel.Debug:
                    return TraceEventType.Verbose;
                case LoggerLevel.Info:
                    return TraceEventType.Information;
                case LoggerLevel.Warn:
                    return TraceEventType.Warning;
                case LoggerLevel.Error:
                    return TraceEventType.Error;
                case LoggerLevel.Fatal:
                    return TraceEventType.Critical;
            }
            return TraceEventType.Verbose;
        }

        public bool IsDebugEnabled
        {
            get { return true; }
        }

        public bool IsErrorEnabled
        {
            get { return true; }
        }

        public bool IsFatalEnabled
        {
            get { return true; }
        }

        public bool IsInfoEnabled
        {
            get { return true; }
        }

        public bool IsWarnEnabled
        {
            get { return true; }
        }

        public void Debug(string message)
        {
            Log(LoggerLevel.Debug, Name, message, null);
        }

        public void Debug(Func<string> messageFactory)
        {
            Log(LoggerLevel.Debug, Name, messageFactory(), null);
        }

        public void Debug(string message, Exception exception)
        {
            Log(LoggerLevel.Debug, Name, message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            Log(LoggerLevel.Debug, Name, string.Format(format, args), null);
        }

        public void DebugFormat(Exception exception, string format, params object[] args)
        {
            Log(LoggerLevel.Debug, Name, string.Format(format, args), exception);
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LoggerLevel.Debug, Name, string.Format(formatProvider, format, args), null);
        }

        public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LoggerLevel.Debug, Name, string.Format(formatProvider, format, args), exception);
        }

        public void Error(string message)
        {
            Log(LoggerLevel.Error, Name, message, null);
        }

        public void Error(Func<string> messageFactory)
        {
            Log(LoggerLevel.Error, Name, messageFactory(), null);
        }

        public void Error(string message, Exception exception)
        {
            Log(LoggerLevel.Error, Name, message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            Log(LoggerLevel.Error, Name, string.Format(format, args), null);
        }

        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            Log(LoggerLevel.Error, Name, string.Format(format, args), exception);
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LoggerLevel.Error, Name, string.Format(formatProvider, format, args), null);
        }

        public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LoggerLevel.Error, Name, string.Format(formatProvider, format, args), exception);
        }

        public void Fatal(string message)
        {
            Log(LoggerLevel.Fatal, Name, message, null);
        }

        public void Fatal(Func<string> messageFactory)
        {
            Log(LoggerLevel.Fatal, Name, messageFactory(), null);
        }

        public void Fatal(string message, Exception exception)
        {
            Log(LoggerLevel.Fatal, Name, message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            Log(LoggerLevel.Fatal, Name, string.Format(format, args), null);
        }

        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            Log(LoggerLevel.Fatal, Name, string.Format(format, args), exception);
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LoggerLevel.Fatal, Name, string.Format(formatProvider, format, args), null);
        }

        public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LoggerLevel.Fatal, Name, string.Format(formatProvider, format, args), exception);
        }

        public void Info(string message)
        {
            Log(LoggerLevel.Info, Name, message, null);
        }

        public void Info(Func<string> messageFactory)
        {
            Log(LoggerLevel.Info, Name, messageFactory(), null);
        }

        public void Info(string message, Exception exception)
        {
            Log(LoggerLevel.Info, Name, message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            Log(LoggerLevel.Info, Name, string.Format(format, args), null);
        }

        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            Log(LoggerLevel.Info, Name, string.Format(format, args), exception);
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LoggerLevel.Info, Name, string.Format(formatProvider, format, args), null);
        }

        public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LoggerLevel.Info, Name, string.Format(formatProvider, format, args), exception);
        }

        public void Warn(string message)
        {
            Log(LoggerLevel.Warn, Name, message, null);
        }

        public void Warn(Func<string> messageFactory)
        {
            Log(LoggerLevel.Warn, Name, messageFactory(), null);
        }

        public void Warn(string message, Exception exception)
        {
            Log(LoggerLevel.Warn, Name, message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            Log(LoggerLevel.Warn, Name, string.Format(format, args), null);
        }

        public void WarnFormat(Exception exception, string format, params object[] args)
        {
            Log(LoggerLevel.Warn, Name, string.Format(format, args), exception);
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LoggerLevel.Warn, Name, string.Format(formatProvider, format, args), null);
        }

        public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LoggerLevel.Warn, Name, string.Format(formatProvider, format, args), exception);
        }

        public LoggerLevel Level { get; set; }

        public string Name { get; set; }
    }

#endif
}
