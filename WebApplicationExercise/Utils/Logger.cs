using NLog;
using System;
using WebApplicationExercise.Models;
using WebApplicationExercise.Utils;

namespace WebApplicationExercise.Core
{
    public class Logger
    {
        #region Singleton
        private static readonly Lazy<Logger> _instance = new Lazy<Logger>(() => new Logger());
        public static Logger Instance => _instance.Value;
        #endregion

        private readonly NLog.Logger _logger;


        private Logger()
        {
            var config = new NLog.Config.LoggingConfiguration();

            var fileNamePrefix = Settings.Instance.LogNameFile;
            var folder = Settings.Instance.LogFolderPath;
            var logPath = $"{folder}/fileNamePrefix_{DateTime.Now.ToString("d")}"; 

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = logPath };

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            LogManager.Configuration = config;

            _logger = LogManager.GetCurrentClassLogger();
        }

        public void Information(string messageTemplate)
        {
            _logger.Info(messageTemplate);
        }

        public void Error(ExceptionLog exception, bool isCritical = false)
        {
            var callStack = isCritical ? Environment.NewLine + exception.CallStack : string.Empty;
            var messageTemplate = $"{ exception.RequestInfo.Methood } { exception.RequestInfo.Uri } - { exception.Type } - { exception.Message.Replace('\r', ' ').Replace('\n', ' ') } { callStack }";

            _logger.Error(messageTemplate);
        }
    }
}