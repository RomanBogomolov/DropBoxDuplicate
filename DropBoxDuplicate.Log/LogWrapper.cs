using System;
using System.Diagnostics;
using System.Reflection;
using NLog;

namespace DropBoxDuplicate.Log
{
    public class LogWrapper : IDisposable
    {

        NLog.Logger _logger;
        MethodBase methodBase;

        public LogWrapper()
        {
            _logger = LogManager.GetCurrentClassLogger();
            methodBase = new StackTrace().GetFrame(1).GetMethod();
            if (methodBase.ReflectedType != null)
                _logger.Info($"Start of method {methodBase.ReflectedType.Name}.{methodBase.Name}");
        }

        public void Dispose()
        {
            if (methodBase.ReflectedType != null)
                _logger.Info($"Finish of method {methodBase.ReflectedType.Name}.{methodBase.Name}");
        }

        public void Trace(string message)
        {
            _logger.Trace(message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Error(string exeptionMessage)
        {
            if (methodBase.ReflectedType != null)
                _logger.Error(
                    $"Method {methodBase.ReflectedType.Name}.{methodBase.Name}|Error message: {exeptionMessage}");
        }

        public void Fatal(string exeptionMessage)
        {
            if (methodBase.ReflectedType != null)
                _logger.Fatal(
                    $"Method {methodBase.ReflectedType.Name}.{methodBase.Name}|Error message: {exeptionMessage}");
        }
    }
}
