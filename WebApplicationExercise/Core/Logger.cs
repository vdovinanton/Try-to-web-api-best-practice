using Serilog;
using System;
using WebApplicationExercise.Models;

namespace WebApplicationExercise.Core
{
    public class Logger
    {
        #region Singleton
        private static readonly Lazy<Logger> _instance = new Lazy<Logger>(() => new Logger());
        public static Logger Instance => _instance.Value;
        #endregion

        private readonly ILogger _logger;

        private Logger()
        {
            //todo: move to settings
            var startupPath = AppDomain.CurrentDomain.BaseDirectory;
            var logDirectory = "logs";

            var logFilesPath = $"{startupPath}{logDirectory}\\";

            _logger = new LoggerConfiguration()
                .WriteTo.File(logFilesPath + "log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public void Information(string messageTemplate)
        {
            _logger.Information(messageTemplate);
        }

        public void Error(ExceptionLog exception, bool isCritical = false)
        {
            var callStack = isCritical ? Environment.NewLine + exception.CallStack : string.Empty;
            var messageTemplate = $"{ exception.RequestInfo.Methood } { exception.RequestInfo.Uri } - { exception.Type } - { exception.Message.Replace('\r', ' ').Replace('\n', ' ') } { callStack }";

            _logger.Error(messageTemplate);
        }
    }
}