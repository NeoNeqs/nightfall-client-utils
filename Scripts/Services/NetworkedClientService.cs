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

        public void CreateClient(string ipAddress, int port)
        {
            _peer.CreateClient(ipAddress, port);
            base.Create();
        }

        protected override void SetupDTLS(string path)
        {
            base.SetupDTLS(path);
            Error error;

            _peer.SetDtlsCertificate(X509CertificateLoader.Load(path, "ag.crt", out error));
            QuitIfError((int) error);
        }
    }
}