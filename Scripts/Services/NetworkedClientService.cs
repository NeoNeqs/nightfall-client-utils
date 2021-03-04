using Godot;

using ClientsUtils.Scripts.Logging;

using SharedUtils.Scripts.Common;
using SharedUtils.Scripts.Exceptions;
using SharedUtils.Scripts.Services;
using SharedUtils.Scripts.Loaders;

namespace ClientsUtils.Scripts.Services
{
    public abstract class NetworkedClientService : NetworkedPeerService
    {
        protected NetworkedClientService() : base()
        {
        }

        public override void _EnterTree()
        {
            base._EnterTree();
        }

        public Error CreateClient(string ipAddress, int port)
        {
            var creationError = _peer.CreateClient(ipAddress, port);
            base.Create();
            return creationError;
        }

        protected override void SetupDTLS(string path)
        {
            base.SetupDTLS(path);

            ErrorCode error;
            _peer.SetDtlsCertificate(X509CertificateLoader.Load(path, GetCertificateName(), out error));
            if (error != ErrorCode.Ok)
            {
                var errorMessage = $"Failed to load x509 certificate from '{path.PlusFile(GetCryptoKeyName())}'";
                ClientLogger.GetSingleton().Error(errorMessage);
                throw new X509CertificateNotFoundException(errorMessage);
            }
        }

        protected override void ConnectSignals(NetworkedPeerService to)
        {
            GetTree().Connect("connection_failed", this, nameof(ConnetionFailed));
            GetTree().Connect("connected_to_server", this, nameof(ConnectionSuccessful));
            GetTree().Connect("server_disconnected", this, nameof(ServerDisconnected));
        }

        protected abstract void ConnetionFailed();
        protected abstract void ConnectionSuccessful();
        protected abstract void ServerDisconnected();
    }
}