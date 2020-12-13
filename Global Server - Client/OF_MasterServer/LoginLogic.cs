using Lidgren.Network;
using System;
using System.Linq;
using Newtonsoft.Json;

public class PlayerLogic
{
    [OFRPC((ushort)ReceivedMessageID.VerifyGame)]
    public static void VerifyGame(NetIncomingMessage message)
    {
        string version = message.ReadString();

        if (MemoryOperations.BalanceData.ActualGameVersion == version)
        {
            NetOutgoingMessage msg = Program.peer.CreateMessage();
            msg.Write((ushort)ReceivedMessageID.VerifyGame);
            msg.Write(true);
            Program.peer.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
        }
        else
        {
            NetOutgoingMessage msg = Program.peer.CreateMessage();
            msg.Write((ushort)ReceivedMessageID.VerifyGame);
            msg.Write(false);
            Program.peer.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
        }
    }

    [OFRPC((ushort)ReceivedMessageID.Login)]
    public static void Login(NetIncomingMessage message)
    {
        string login = message.ReadString();
        string password = message.ReadString();

        try
        {
            PlayerInfo request = MemoryOperations.PlayersData.Keys.FirstOrDefault(x => x.Nickname == login);
            if(request.Password == password)
            {
                NetOutgoingMessage msg = Program.peer.CreateMessage();
                msg.Write((ushort)ReceivedMessageID.Login);
                msg.Write(true);
                Program.peer.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);

                SendCheckedPlayerData(message, request);
            }
            else
            {
                NetOutgoingMessage msg = Program.peer.CreateMessage();
                msg.Write((ushort)ReceivedMessageID.Login);
                msg.Write(false);
                Program.peer.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
            }
        }
        catch
        {
            NetOutgoingMessage msg = Program.peer.CreateMessage();
            msg.Write((ushort)ReceivedMessageID.Login);
            msg.Write(false);
            Program.peer.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
        }
    }

    [OFRPC((ushort)ReceivedMessageID.LoginWithAccess)]
    public static void Login4Plus(NetIncomingMessage message)
    {
        string login = message.ReadString();
        string password = message.ReadString();

        try
        {
            PlayerInfo request = MemoryOperations.PlayersData.Keys.FirstOrDefault(x => x.Nickname == login);
            if (request.Password == password && request.AccessLevel >= 4)
            {
                NetOutgoingMessage msg = Program.peer.CreateMessage();
                msg.Write((ushort)ReceivedMessageID.LoginWithAccess);
                msg.Write(true);
                Program.peer.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);

                SendCheckedPlayerData(message, request);
            }
            else
            {
                NetOutgoingMessage msg = Program.peer.CreateMessage();
                msg.Write((ushort)ReceivedMessageID.LoginWithAccess);
                msg.Write(false);
                Program.peer.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
            }
        }
        catch
        {
            NetOutgoingMessage msg = Program.peer.CreateMessage();
            msg.Write((ushort)ReceivedMessageID.LoginWithAccess);
            msg.Write(false);
            Program.peer.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
        }
    }

    [OFRPC((ushort)ReceivedMessageID.UpdatePlayer)]
    public static void SendPlayerData(NetIncomingMessage message)
    {
        string GUID = message.ReadString();
        string password = message.ReadString();
        try
        {
            PlayerInfo request = MemoryOperations.PlayersData.Keys.FirstOrDefault(x => x.Guid == GUID);
            if(request.Password == password)
            {
                NetOutgoingMessage msg = Program.peer.CreateMessage();
                msg.Write((ushort)ReceivedMessageID.UpdatePlayer);
                msg.Write(JsonConvert.SerializeObject(request));
                Program.peer.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
            }
            else
            {
                NetOutgoingMessage msg = Program.peer.CreateMessage();
                msg.Write((ushort)ReceivedMessageID.UpdatePlayer);
                msg.Write("get crashed game)0)0))00");
                Program.peer.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
            }
        }
        catch
        {
            NetOutgoingMessage msg = Program.peer.CreateMessage();
            msg.Write((ushort)ReceivedMessageID.UpdatePlayer);
            msg.Write("get crashed game)0)0))00");
            Program.peer.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
        }
    }

    public static void SendCheckedPlayerData(NetIncomingMessage message, PlayerInfo player)
    {
        NetOutgoingMessage msg = Program.peer.CreateMessage();
        msg.Write((ushort)ReceivedMessageID.UpdatePlayer);
        msg.Write(JsonConvert.SerializeObject(player));
        Program.peer.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
    }
}

