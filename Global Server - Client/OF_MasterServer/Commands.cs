using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Commands
{
    [OFRPC((ushort)ReceivedMessageID.RequestBalance)]
    public static void BroadcastBalanceData(NetIncomingMessage message)
    {
        NetOutgoingMessage msg = Program.peer.CreateMessage();
        msg.Write((ushort)ReceivedMessageID.RequestBalance);
        msg.Write(Newtonsoft.Json.JsonConvert.SerializeObject(MemoryOperations.BalanceData));
        Program.peer.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
    }

    public static void BroadcastBalanceData()
    {
        foreach (NetConnection conn in Program.peer.Connections)
        {
            NetOutgoingMessage msg = Program.peer.CreateMessage();
            msg.Write((ushort)ReceivedMessageID.RequestBalance);
            msg.Write(Newtonsoft.Json.JsonConvert.SerializeObject(MemoryOperations.BalanceData));
            Program.peer.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);
        }
    }

    [OFRPC((ushort)ReceivedMessageID.RequestServers)]
    public static void SendServers(NetIncomingMessage message)
    {
        try
        {
            foreach (var server in ServerHandler.servers)
            {
                NetOutgoingMessage msg = Program.peer.CreateMessage();
                msg.Write(server.Value.InternalIP);
                msg.Write(server.Value.Server_Name);
                msg.Write(server.Value.Map);
                msg.Write(server.Value.Gamemode);
                msg.Write(server.Value.Server_OnLoadMessage);
                msg.Write(server.Value.TeamLeft);
                msg.Write(server.Value.TeamLestScore);
                msg.Write(server.Value.TeamRight);
                msg.Write(server.Value.TeamRightScore);
                msg.Write(server.Value.Players_Max);
                msg.Write(server.Value.Players_Connected);
                Program.peer.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.Unreliable);
            }
        }
        catch
        {

        }
    }

    [OFRPC((ushort)ReceivedMessageID.Introduce)]
    public static void Inroduce(NetIncomingMessage message)
    {
        System.Net.IPEndPoint whereConnect = message.ReadIPEndPoint();
        System.Net.IPEndPoint whoInternal = message.ReadIPEndPoint();

        ServerInfo whereWantConnect = ServerHandler.servers.First(x => x.Value.InternalIP == whereConnect).Value;
        if(whereConnect != null)
        {
            Program.peer.Introduce(whereWantConnect.InternalIP, whereWantConnect.ExternalIP, whoInternal, message.SenderEndPoint, "OF Global Intoduce");
        }
    }
}

