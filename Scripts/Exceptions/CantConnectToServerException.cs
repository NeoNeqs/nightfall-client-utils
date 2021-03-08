using SharedUtils.Exceptions;

namespace ClientsUtils.Exceptions
{
    public class CantConnectToServerException : NightFallException
    {
        public CantConnectToServerException(string message) : base(message)
        {
        }
    }
}