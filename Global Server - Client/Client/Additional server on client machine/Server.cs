using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;
using Lidgren.Network;
using OF_GameServer;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class Server : MonoBehaviour
{
    public bool isOffline = false;

    public delegate void KillChecker(byte team);
    public static event KillChecker Killed;

    internal static NetServer _server;
    internal static ServerConfig server_config;

    public static ConcurrentDictionary<byte, PlayerLocalServer> clients = new ConcurrentDictionary<byte, PlayerLocalServer>();
    public static ConcurrentDictionary<ushort, PlayerObject> clientsSimpleObjects = new ConcurrentDictionary<ushort, PlayerObject>();
    public static ConcurrentDictionary<ushort, PlayerInstance> clientsInstanses = new ConcurrentDictionary<ushort, PlayerInstance>();

    public static IPEndPoint GlobalServerIP = new IPEndPoint(IPAddress.Parse("79.174.12.222"), 57125);
    public string serverName = "";

    internal static TDM gamemodeTDM = null;
    public static byte SelectedGamemode = 0;
    public static bool SessionStatus = false;

    static ServerRPC_LowCPU rpc;

    public void StartServer()
    {
        clients.Clear();
        clientsSimpleObjects.Clear();
        clientsInstanses.Clear();

        if (!isOffline)
        {
            if (File.Exists("server_config.json"))
            {
                server_config = JsonConvert.DeserializeObject<ServerConfig>(File.ReadAllText("server_config.json"));
                serverName = server_config.ServerName;
            }
            else
            {
                server_config = new ServerConfig
                {
                    ServerName = "Unnamed " + DateTime.Now,
                    OnLoadMessage = "Thanks for testing OSTFRONT \nJoin official discord: discord.gg/V7UEFxp",
                    Map = "Polygon",
                    Gamemode = "TDM",
                    Players = 24,
                    Port = 27015,
                    ScoreTeamNATO = 150,
                    ScoreTeamWP = 150,
                    GameTime = 600,
                    AwaitNewSession = 30,
                    Tickrate = 8,
                    UPnP = true
                };
                File.WriteAllText("server_config.json", JsonConvert.SerializeObject(server_config, Formatting.Indented));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("> Server started without config - server will be closed by 4 seconds");
                Console.ForegroundColor = ConsoleColor.White;
                Thread.Sleep(4000);
                Environment.Exit(0);
            }

            NetPeerConfiguration config = new NetPeerConfiguration("Ostfront")
            {
                Port = server_config.Port,
                MaximumConnections = server_config.Players,
                AcceptIncomingConnections = true,

                EnableUPnP = true,
                AutoFlushSendQueue = true,
                DefaultOutgoingMessageCapacity = 1000000,
                ReceiveBufferSize = 1000000
            };
            config.SetMessageTypeEnabled(NetIncomingMessageType.NatIntroductionSuccess, true);
            config.SetMessageTypeEnabled(NetIncomingMessageType.UnconnectedData, true);

            _server = new NetServer(config);
            _server.Start();

            if (server_config.UPnP)
                _server.UPnP.ForwardPort(server_config.Port, "OSTFRONT Game Server UPnP");

            NewSession();

            NetOutgoingMessage toGlobal = _server.CreateMessage();
            toGlobal.Write((byte)GlobalServerTransmissionEnum.AddServerInList);
            toGlobal.Write(new IPEndPoint(GetInternal(), server_config.Port));
            toGlobal.Write(server_config.Port);
            toGlobal.Write(server_config.ServerName);
            toGlobal.Write(server_config.OnLoadMessage);
            toGlobal.Write(server_config.Gamemode);
            toGlobal.Write(server_config.Map);
            toGlobal.Write(server_config.Players);
            toGlobal.Write((byte)_server.ConnectionsCount);
            _server.SendUnconnectedMessage(toGlobal, GlobalServerIP);

            #region Startup MS sender data
            new Thread(new ThreadStart(UdpateStatistics)) { IsBackground = true, Name = "MS Update Time" }.Start();
            #endregion

            #region Startup server loop
            new LoopMessages(_server);
            #endregion
        }
        else
        {
            server_config = new ServerConfig
            {
                ServerName = "Offline",
                OnLoadMessage = "",
                Port = 27015,
                ScoreTeamNATO = 1000,
                ScoreTeamWP = 1000,
                AwaitNewSession = 0,
                Gamemode = "Offline",
                GameTime = 2 ^ 32 - 1,
                Tickrate = (byte)60,
                UPnP = false,
                Players = 1,
                Map = "Offline"
            };

            clients = new ConcurrentDictionary<byte, PlayerLocalServer>();
            clientsSimpleObjects = new ConcurrentDictionary<ushort, PlayerObject>();
            clientsInstanses = new ConcurrentDictionary<ushort, PlayerInstance>();

            NetPeerConfiguration config = new NetPeerConfiguration("Ostfront")
            {
                Port = server_config.Port,
                MaximumConnections = server_config.Players,
                AcceptIncomingConnections = true,

                EnableUPnP = false,
               // AutoFlushSendQueue = true,
            };
            config.SetMessageTypeEnabled(NetIncomingMessageType.NatIntroductionSuccess, true);
            config.SetMessageTypeEnabled(NetIncomingMessageType.UnconnectedData, true);
            config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionApproval, false);

            _server = new NetServer(config);
            _server.Start();

            new LoopMessages(_server);
        }

        rpc = new ServerRPC_LowCPU();
    }

    async void UdpateStatistics()
    {
        NetOutgoingMessage _toGlobal = _server.CreateMessage();
        _toGlobal.Write((byte)GlobalServerTransmissionEnum.UpdateServerInList);
        _toGlobal.Write(serverName);
        _toGlobal.Write((byte)_server.ConnectionsCount);
        _toGlobal.Write(server_config.Map);
        _toGlobal.Write(server_config.Gamemode);
        _server.SendUnconnectedMessage(_toGlobal, GlobalServerIP);
        await Task.Delay(2000);
        UdpateStatistics();
    }

    public void Shutdown()
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("> Send delete forwarting rule");
        Console.ForegroundColor = ConsoleColor.White;
        _server.UPnP.DeleteForwardingRule(server_config.Port);
    }

    static IPAddress GetInternal()
    {
        IPAddress localIP;
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint.Address;
        }
        return localIP;
    }

    public static async Task Execute(Action action, int timeoutInMilliseconds)
    {
        await Task.Delay(timeoutInMilliseconds);
        action();
    }

    #region Gamemode control
    public void EndSession()
    {
        SelectedGamemode = 1;

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("> Session ended, await new session:" + server_config.AwaitNewSession + " seconds");
        Console.ForegroundColor = ConsoleColor.White;

        #region Delete players object

        List<ushort> keys = new List<ushort>(clientsSimpleObjects.Keys);

        foreach (var element in keys)
        {
            foreach (NetConnection conn in _server.Connections)
            {
                NetOutgoingMessage msg = _server.CreateMessage();
             //   msg.Write((byte)DataTransmissionEnum.DestroySimpleObject);
                msg.Write(element);
                _server.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);
            }

            bool suc = false;
            while (!suc)
                suc = clientsSimpleObjects.TryRemove(element, out _);
        }
        #endregion

        #region Delete Players
        List<ushort> keys2 = new List<ushort>(clientsInstanses.Keys);
        foreach (var element in keys2)
        {
            foreach (NetConnection conn in _server.Connections)
            {
                NetOutgoingMessage msg = _server.CreateMessage();
               // msg.Write((byte)DataTransmissionEnum.DestroyPlayer);
                msg.Write(element);
                _server.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);
            }

            bool suc = false;
            while (!suc)
                suc = clientsInstanses.TryRemove(element, out _);
        }
        #endregion

        gamemodeTDM.SessionFinished -= EndSession;
        gamemodeTDM = null;

        foreach (var conn in _server.Connections)
        {
            NetOutgoingMessage msg = _server.CreateMessage();
           // msg.Write((byte)DataTransmissionEnum.SessionEnded);
            _server.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);
        }

        SessionStatus = false;
        Execute(() => NewSession(), server_config.AwaitNewSession * 1000);
    }

    public void NewSession()
    {
        if (server_config.Gamemode == "TDM")
        {
            gamemodeTDM = new TDM();
            SelectedGamemode = 0;
            SendNewGamemode(0);
            gamemodeTDM.SessionFinished += EndSession;
        }
        SessionStatus = true;
    }

    public void SendNewGamemode(byte gamemode)
    {
        foreach (var conn in _server.Connections)
        {
            NetOutgoingMessage msge = _server.CreateMessage();                                          //TODO
            msge.Write((ushort)RPC.NewSession_TDM);
            _server.SendMessage(msge, conn, NetDeliveryMethod.ReliableOrdered);
        }
    }
    #endregion
}
