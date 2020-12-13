using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class ServerLoop
{
    internal static NetServer _server;
    public static bool EnabledLooping { get; private set; }

    public static void StartServerLoop(NetServer server)
    {
        _server = server;
        EnabledLooping = true;
        new Thread(new ThreadStart(ProcessMain)) { IsBackground = true, Name = "Loop messages", Priority = ThreadPriority.Highest }.Start();
    }

    static void ProcessMain()
    {
        while (EnabledLooping)
        {
            _server.MessageReceivedEvent.WaitOne();
            NetIncomingMessage message;
            while ((message = _server.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        ushort id = message.ReadUInt16();
                        RPCAbleMethods.RpcMethods[id](message);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        if ((NetConnectionStatus)message.ReadByte() == NetConnectionStatus.Connected)
                        {
                            Console.WriteLine("Connected " + message.SenderEndPoint);
                        }
                        break;
                    default: //Drop out unuserful messages
                        break;
                }
            }
            Program.peer.Recycle(message);
        }
    }

    //Call on STOP SERVER command
    public static void CloseLooping()
    {
        EnabledLooping = false;
    }
}    

