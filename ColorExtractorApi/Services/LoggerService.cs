using Serilog;
using ColorExtractorApi.Services.Interfaces;

namespace ColorExtractorApi.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly Serilog.ILogger _logger = Log.ForContext<LoggerService>();

        public void LogInfo(string message) => _logger.Information(message);
        public void LogWarn(string message) => _logger.Warning(message);
        public void LogDebug(string message) => _logger.Debug(message);
        public void LogError(string message) => _logger.Error(message);
    }
}