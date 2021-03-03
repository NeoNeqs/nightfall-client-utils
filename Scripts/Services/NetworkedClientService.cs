using Godot;

using SharedUtils.Scripts.Services;
using SharedUtils.Scripts.Loaders;

namespace ClientsUtils.Scripts.Services
{
    public abstract class NetworkedClientService : NetworkedPeerService
    {
        protected NetworkedClientService() : base()
        {
        }

        public Error CreateClient(string ipAddress, int port)
        {
            var creationError = _peer.CreateClient(ipAddress, port);
            base.Create();
            return creationError;
        }

        protected override Error SetupDTLS(string path)
        {
            Error error = base.SetupDTLS(path);
            if (error != Error.Ok)
            {
                return error;
            }

            _peer.SetDtlsCertificate(X509CertificateLoader.Load(path, GetCertificateName(), out error));
            if (error != Error.Ok)
            {
                return error;
            }
            return Error.Ok;
        }
    }
}