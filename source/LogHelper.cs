using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MicrosoftOnline.Ads.LogAssistant
{
    public class LogHelper
    {
        private static object lockObj = new object();
        private static EventHandler<LogEventArgs> LogHandler = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public static void Init(EventHandler<LogEventArgs> handler)
        {
            LogHandler = handler;
        }

        public static void Info(LogCategoryType categoryType,
            string wayPoint,
            string message,
            object parameters = null,
            Exception ex = null,
            string trackingId = null,
            string[] sensitiveProperties = null,
            string sensitiveReplacer = "******")
        {
            Log(LogLevel.Info, categoryType, wayPoint, message, parameters, ex, trackingId, sensitiveProperties, sensitiveReplacer);
        }

        public static void Warn(LogCategoryType categoryType,
            string wayPoint,
            string message,
            object parameters = null,
            Exception ex = null,
            string trackingId = null,
            string[] sensitiveProperties = null,
            string sensitiveReplacer = "******")
        {
            Log(LogLevel.Warn, categoryType, wayPoint, message, parameters, ex, trackingId, sensitiveProperties, sensitiveReplacer);
        }

        public static void Error(LogCategoryType categoryType,
            string wayPoint,
            string message,
            object parameters = null,
            Exception ex = null,
            string trackingId = null,
            string[] sensitiveProperties = null,
            string sensitiveReplacer = "******")
        {
            Log(LogLevel.Error, categoryType, wayPoint, message, parameters, ex, trackingId, sensitiveProperties, sensitiveReplacer);
        }

        public static void Log(LogLevel level,
            LogCategoryType categoryType,
            string wayPoint,
            string message,
            object parameters = null,
            Exception ex = null,
            string trackingId = null,
            string[] sensitiveProperties = null,
            string sensitiveReplacer = "******")
        {
            var logArgs = new LogEventArgs(DateTime.UtcNow.AddHours(8), level, categoryType, wayPoint, message, parameters, ex, trackingId, sensitiveProperties)
            {
                SensitiveString = sensitiveReplacer ?? "******"
            };

            lock (lockObj)
            {
                if (LogHandler != null)
                {
                    LogHandler(null, logArgs);
                }
                else
                {
                    try
                    {
                        var _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

                        if (!Directory.Exists(_path))
                            Directory.CreateDirectory(_path);
                        var _log = Path.Combine(_path, string.Format("Log_{0:yyyy-MM-dd}.txt", DateTime.UtcNow.AddHours(8)));

                        File.AppendAllText(_log, "{\r\n" + logArgs.ToString() + "\r\n}\r\n");
                    }
                    catch (Exception) { }
                }
            }
        }

        public static void Log(LogEventArgs e)
        {
            lock (lockObj)
            {
                if (LogHandler != null)
                {
                    LogHandler(null, e);
                }
                else
                {
                    try
                    {
                        var _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

                        if (!Directory.Exists(_path))
                            Directory.CreateDirectory(_path);
                        var _log = Path.Combine(_path, string.Format("Log_{0:yyyy-MM-dd}.txt", DateTime.UtcNow.AddHours(8)));

                        File.AppendAllText(_log, "{\r\n" + e.ToString() + "\r\n}\r\n");
                    }
                    catch (Exception) { }
                }
            }
        }
    }
}