using Godot;

using SharedUtils.Services;
using SharedUtils.Networking;
using SharedUtils.Logging;

namespace ClientsUtils.Services
{
    public abstract class NetworkedClient<T> : NetworkedPeer<T> where T : Node
    {
        protected override void Create()
        {
            _ = _peer.CreateClient(GetIpAddress(), GetPort());
        }

        protected override void ConnectSignals()
        {
            CustomMultiplayer.Connect("connection_failed", this, nameof(ConnectionFailed));
            CustomMultiplayer.Connect("connected_to_server", this, nameof(ConnectionSuccessful));
            CustomMultiplayer.Connect("server_disconnected", this, nameof(ServerDisconnected));
        }

        private void Send(object[] args)
        {
            _ = RpcId(1, nameof(PacketReceived), args);
        }

        protected void Send(PacketType packetType, object arg1)
        {
            Send(new[] { packetType, arg1 });
        }

        protected void Send(PacketType packetType, object arg1, object arg2)
        {
            Send(new[] { packetType, arg1, arg2 });
        }

        protected void Send(PacketType packetType, object arg1, object arg2, object arg3)
        {
            Send(new[] { packetType, arg1, arg2, arg3 });
        }

        protected void Send(PacketType packetType, object arg1, object arg2, object arg3, object arg4)
        {
            Send(new[] { packetType, arg1, arg2, arg3, arg4 });
        }

        protected void Send(PacketType packetType, object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            Send(new[] { packetType, arg1, arg2, arg3, arg4, arg5 });
        }

        protected virtual void ConnectionFailed()
        {
            Logger.Error($"Connection to {GetIpAddress()}:{GetPort()} failed!");
            Reconnect();
        }

        protected virtual void ConnectionSuccessful()
        {
            Logger.Info($"Successfully conected to {GetIpAddress()}:{GetPort()}.");
        }

        protected virtual void ServerDisconnected()
        {
            Logger.Info($"Disconnected from {GetIpAddress()}:{GetPort()}.");
            Reconnect();
        }

        protected abstract string GetIpAddress();
    }
}