using System;

namespace ClientsUtils.Scripts.Exceptions
{
    public class CantConnectToServerException : Exception
    {
        public CantConnectToServerException(string ipAddress, int port) : base($"Failed to connect to {ipAddress}:{port}")
        {
        }
    }
}