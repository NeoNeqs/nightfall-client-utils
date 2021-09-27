using System;
using Godot;
using Nightfall.SharedUtils.InfoCodes;
using Nightfall.SharedUtils.Logging;
using Nightfall.SharedUtils.Net.Messaging;

namespace Nightfall.ClientUtils.Net
{
    public sealed partial class StreamPeerTCPMessage : StreamPeerTCP
    {
        [Signal]
        public delegate void MessageReceived(Message message, long error);

        [Signal]
        public delegate void ConnectedToHost();

        [Signal]
        public delegate void Reconnected();

        [Signal]
        public delegate void ConnectionFailed();

        [Signal]
        public delegate void Disconnected();

        public static readonly Logger NetLogger = NFInstance.GetNetLogger();
        private readonly string _host;
        private readonly int _port;

        // ReSharper disable once UnusedMember.Global
        // Godot needs an empty constructor for creation of dummy objects in the Editor.
        // TODO: Check if export build complains about missing empty constructor.
        [Obsolete]
        public StreamPeerTCPMessage()
        {
        }

        public StreamPeerTCPMessage(string host, int port)
        {
            _host = host;
            _port = port;
        }

        private readonly Thread _thread = new();
        private readonly Mutex _mutex = new();
        private static bool _once = true;
        private static bool _lostConnection;
        private static bool _stop;

        public NFError ConnectToHost()
        {
            if (GetStatus() != Status.None)
            {
                return NFError.AlreadyExists;
            }

            _lostConnection = false;
            _once = true;
            _stop = false;

            var error = base.ConnectToHost(_host, _port);
            if (!_thread.IsActive())
            {
                _thread.Start(this, nameof(ThreadFunction), (byte) 0);
            }

            return error;
        }

        private void ThreadFunction(byte obj)
        {
            while (true)
            {
                OS.DelayMsec(50); // (1000 / 50ms) = 20 ticks
                _mutex.Lock();

                if (_stop)
                {
                    NetLogger.Info("Stopping the thread function");
                    _mutex.Unlock();
                    return;
                }

                switch (GetStatus())
                {
                    // Connected for the first time.
                    case Status.Connected when _once:
                    {
                        NetLogger.Info($"Connected to {_host}:{_port.ToString()}.");
                        EmitSignal(nameof(ConnectedToHost));
                        _once = false;

                        break;
                    }
                    // Reconnected
                    case Status.Connected when _lostConnection:
                    {
                        NetLogger.Info($"Reconnected to {_host}:{_port.ToString()}.");
                        EmitSignal(nameof(Reconnected));
                        _lostConnection = false;

                        break;
                    }
                    case Status.Connected:
                        break;
                    case Status.Connecting:
                    {
                        _mutex.Unlock();
                        continue;
                    }
                    // Connection timeout (connection failed) or sudden shutdown when sending data?
                    case Status.Error:
                    {
                        NetLogger.Error(
                            $"Could not establish connection with {_host}:{_port.ToString()}.");
                        base.DisconnectFromHost();
                        ConnectToHost();

                        _mutex.Unlock();
                        continue;
                    }
                    // DO NOT RECONNECT! Server closed itself or we were disconnected by the server or locally.
                    case Status.None: 
                    {
                        _stop = true;
                        _mutex.Unlock();
                        continue;
                    }
                    default:
                    {
                        NetLogger.Critical("This is a bug. This should never happened.");

                        _mutex.Unlock();
                        continue;
                    }
                }

                if (!IsConnectedToHost())
                {
                    _mutex.Unlock();
                    continue;
                }

                if (GetAvailableBytes() == 0)
                {
                    _mutex.Unlock();
                    continue;
                }

                var (message, error) = Message.Create(this);
                EmitSignal(nameof(MessageReceived), message, error);

                _mutex.Unlock();
            }
        }

        public NFError SendMessage(Message message)
        {
            var (data, error) = message.Serialize();

            return error == NFError.Ok ? SendRawMessage(data) : error;
        }

        private NFError SendRawMessage(byte[] buffer)
        {
            return PutData(buffer);
        }

        public new void DisconnectFromHost()
        {
            _mutex.Lock();
            _stop = true;
            _mutex.Unlock();

            _thread.WaitToFinish();
            base.DisconnectFromHost();
        }

        public event MessageReceived MessageReceivedEvent
        {
            add => Connect(nameof(MessageReceived), new Callable(value));
            remove => Disconnect(nameof(MessageReceived), new Callable(value));
        }

        public event ConnectedToHost ConnectedToHostEvent
        {
            add => Connect(nameof(ConnectedToHost), new Callable(value));
            remove => Disconnect(nameof(ConnectedToHost), new Callable(value));
        }

        public event Reconnected ReconnectedEvent
        {
            add => Connect(nameof(Reconnected), new Callable(value));
            remove => Disconnect(nameof(Reconnected), new Callable(value));
        }

        public event ConnectionFailed ConnectionFailedEvent
        {
            add => Connect(nameof(ConnectionFailed), new Callable(value));
            remove => Disconnect(nameof(ConnectionFailed), new Callable(value));
        }

        public event Disconnected DisconnectedEvent
        {
            add => Connect(nameof(Disconnected), new Callable(value));
            remove => Disconnect(nameof(Disconnected), new Callable(value));
        }
    }
}