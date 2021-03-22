using Godot;

using SharedUtils.Common;
using SharedUtils.Services;
using SharedUtils.Networking;

namespace ClientsUtils.Services
{
    public abstract class NetworkedClient<T> : NetworkedPeer<T> where T : Node
    {
        protected NetworkedClient() : base() { }


        public override void _EnterTree()
        {
            base._EnterTree();
            _ = SetupDTLS();
        }

        public override void _Ready() => base._Ready();

        public override void _Process(float delta) => base._Process(delta);

        public ErrorCode CreateClient(string ipAddress, int port)
        {
            var error = _peer.CreateClient(ipAddress, port);
            base.Create();
            return (ErrorCode)((int)error);
        }

        protected override void ConnectSignals()
        {
            CustomMultiplayer.Connect("connection_failed", this, nameof(_ConnectionFailed));
            CustomMultiplayer.Connect("connected_to_server", this, nameof(_ConnectionSuccessful));
            CustomMultiplayer.Connect("server_disconnected", this, nameof(_ServerDisconnected));
        }

        protected void Send(params object[] args)
        {
            RpcId(1, "PacketReceived", args);
        }

        protected abstract void _ConnectionFailed();
        protected abstract void _ConnectionSuccessful();
        protected abstract void _ServerDisconnected();
    }
}