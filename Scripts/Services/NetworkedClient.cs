using Godot;

using SharedUtils.Common;
using SharedUtils.Exceptions;
using SharedUtils.Services;
using SharedUtils.Loaders;

namespace ClientsUtils.Services
{
    public abstract class NetworkedClient : NetworkedPeer
    {
        protected NetworkedClient() : base()
        {
        }

        public override void _EnterTree()
        {
            base._EnterTree();
            SetupDTLS();
        }

        public override void _Ready()
        {
            ConnectSignals();
        }
        
        public ErrorCode CreateClient(string ipAddress, int port)
        {
            var error = _peer.CreateClient(ipAddress, port);
            base.Create();
            return (ErrorCode)((int)error);
        }

        protected void SetupDTLS()
        {
            string path = "user://DTLS/";
            base.SetupDTLS(path);

            ErrorCode error;
            _peer.SetDtlsCertificate(X509CertificateLoader.Load(path, GetCertificateName(), out error));
            if (error != ErrorCode.Ok)
            {
                throw new X509CertificateNotFoundException($"Failed to load x509 certificate from '{path.PlusFile(GetCertificateName())}'");
            }
        }

        protected override void ConnectSignals()
        {
            CustomMultiplayer.Connect("connection_failed", this, nameof(ConnetionFailed));
            CustomMultiplayer.Connect("connected_to_server", this, nameof(ConnectionSuccessful));
            CustomMultiplayer.Connect("server_disconnected", this, nameof(ServerDisconnected));
        }
        
        protected void Send(params object[] args)
        {
            RpcId(1, "OnPeerPacket", args);
        }
        protected abstract void ConnetionFailed();
        protected abstract void ConnectionSuccessful();
        protected abstract void ServerDisconnected();
    }
}