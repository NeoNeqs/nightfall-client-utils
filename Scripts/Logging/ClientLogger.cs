using SharedUtils.Scripts.Logging;

namespace ClientsUtils.Scripts.Logging
{
    public class ClientLogger : BasicLogger
    {
        private static ClientLogger _singleton = new ClientLogger();

        public static ClientLogger GetSingleton() => _singleton;


        public ClientLogger() : base("CLIENT")
        {

        }
    }
}