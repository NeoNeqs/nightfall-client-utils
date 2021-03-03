using SharedUtils.Scripts.Logging;

namespace ClientsUtils.Scripts.Logging
{
    public class ClientLogger : Logger
    {
        // one instance of LoggerFile for every Logger instance just in case
        private static BasicLogger _client;
        public static BasicLogger GetLogger => _client;

        public ClientLogger() : base()
        {
            // _loggerFile passed by reference to always write to the same file
            _client = new BasicLogger(ref _loggerFile, "SERVER");
        }
    }
}