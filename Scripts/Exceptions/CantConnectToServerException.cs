using SharedUtils.Exceptions;

namespace ClientsUtils.Exceptions
{
    public class CantConnectToServerException : NightFallException
    {
        public CantConnectToServerException(string ip, int port) : base($"Could not create connection to server {ip}:{port}.") { }
    }
}