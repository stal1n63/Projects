using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace OF_GameServer
{
    public class LoopMessages : MonoBehaviour
    {
        internal static NetServer _server;
        internal static NetClient _client;

        public bool EnabledLooping { get; private set; }
        Thread looperThread;

        public LoopMessages(NetServer server)
        {
            _server = server;
            EnabledLooping = true;
            looperThread = new Thread(new ThreadStart(ProcessServer)) { IsBackground = true, Name = "Loop messages (ServerOF)", Priority = System.Threading.ThreadPriority.Highest };
            looperThread.Start();
        }
        public LoopMessages(NetClient client)
        {
            _client = client;
            EnabledLooping = true;
            looperThread = new Thread(new ThreadStart(ProcessClient)) { IsBackground = true, Name = "Loop messages (Client)", Priority = System.Threading.ThreadPriority.Highest };
            looperThread.Start();
        }


        static void ProcessServer()
        {
            while (true)
            {
                _server.MessageReceivedEvent.WaitOne();
                NetIncomingMessage message;
                while ((message = _server.ReadMessage()) != null)
                {
                    switch (message.MessageType)
                    { 
                        case NetIncomingMessageType.Data:
                            ushort id = message.ReadUInt16();
                            RPC_LoadMethods.Serverside[id](message);
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            if((NetConnectionStatus)message.ReadByte() == NetConnectionStatus.Connected)
                            {
                                print("Connected client " + message.SenderEndPoint);
                            }
                            break;
                        default: //Drop out unuserful messages
                            print(message.ReadString());
                            break;
                    }
                    _server.Recycle(message);
                }
            }
        }
        static void ProcessClient()
        {
            while (true)
            {
                _client.MessageReceivedEvent.WaitOne();
                NetIncomingMessage message;
                while ((message = _client.ReadMessage()) != null)
                {
                    switch (message.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            ushort id = message.ReadUInt16();
                            RPC_LoadMethods.Clientside[id](message);
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            if ((NetConnectionStatus)message.ReadByte() == NetConnectionStatus.Connected)
                            { 
                                print("Connected to server " + message.SenderEndPoint);

                                NetOutgoingMessage msg = _client.CreateMessage();
                                msg.Write((ushort)RPC.ConnectionPlayerData);
                                msg.Write("Offline");
                                msg.Write(Guid.NewGuid().ToString());
                                _client.SendMessage(msg, _client.ServerConnection, NetDeliveryMethod.Unreliable);
                                _client.FlushSendQueue();
                            }
                            break;
                        default: //Drop out unuserful messages
                            break;
                    }
                    _server.Recycle(message);
                }
            }
        }

        //Really, that's a dispose
        //Call on STOP SERVER command
        public void CloseLooping()
        {
            EnabledLooping = false;
            if (looperThread != null)
                looperThread.Abort();
        }
    }
}
