using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ServerRPC_LowCPU : Server
{
    public void DestroyDisconnectedPlayerObjects(ushort playerid)
    {
        try
        {
            #region Remove spawned objects
            List<ushort> keys_simpleobj = new List<ushort>(clientsSimpleObjects.Keys);
            foreach (var element in keys_simpleobj)
            {
                if (clientsSimpleObjects[element].OwnerID == playerid)
                {
                    foreach (NetConnection conn in _server.Connections)
                    {
                        NetOutgoingMessage msg = _server.CreateMessage();
                        msg.Write((ushort)RPC.DestroySimpleObject);
                        msg.Write(element);
                        _server.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);
                    }

                    bool check = false;
                    while (!check)
                        check = clientsSimpleObjects.TryRemove(element, out _);
                } 
            }
            #endregion

            #region Remove spawned player
            List<ushort> keys_plinst = new List<ushort>(clientsInstanses.Keys);
            foreach (var element in keys_plinst)
            {
                if (clientsInstanses[element].OwnerID == playerid)
                {
                    foreach (NetConnection conn in _server.Connections)
                    {
                        NetOutgoingMessage msg = _server.CreateMessage();
                        msg.Write((ushort)RPC.DestroyPlayer);
                        msg.Write(element);
                        _server.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);
                    }

                    bool suc = false;
                    while (!suc)
                        suc = clientsInstanses.TryRemove(element, out _);
                }
            }
            #endregion
        }
        catch { }
    }

    [OFRPC(OFRPC.RPCType.Serverside, (ushort)RPC.ConnectionPlayerData)]
    public void SendConnectionFirstData(NetIncomingMessage message)
    {
        print("catched");
        string Nickname = message.ReadString();
        string Guid = message.ReadString();
        print("readed");
       /* try
        {*/
            #region Create to connected ID
            byte key = 0;
            for (byte i = 0; i <= 255; i++)
            {
                if (!clients.ContainsKey(i))
                {
                    key = i;
                    break;
                }
                else if (i == 255)
                {
                    message.SenderConnection.Disconnect("Out of players!");
                    break;
                }
            }
            #endregion

            var cl = new PlayerLocalServer()
            {
                Nickname = Nickname,
                Local_Kills = 0,
                Local_Deaths = 0,
                Local_ID = key,
                Current_Team = 0,
                Selected_Class = 0,
                Guid = Guid
            };

            print("before conq");
            bool check = false;
            while (!check)
                check = clients.TryAdd(key, cl);
            print("after concurrent");

            NetOutgoingMessage msg = _server.CreateMessage();
            msg.Write((ushort)RPC.ConnectionPlayerData);
            msg.Write(key);
            msg.Write(server_config.Tickrate);
            msg.Write(SelectedGamemode);
            msg.Write(cl.Current_Team);
            msg.Write(cl.Selected_Class);
            _server.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
            _server.FlushSendQueue();
            print("sended");
       /* }
        catch(Exception e)
        {
            print(e);
            message.SenderConnection.Disconnect("Unexpected error");
        }*/
    }

    [OFRPC(OFRPC.RPCType.Serverside, (ushort)RPC.ConnectionEndSync)]
    public void SyncConnectedPlayer(NetIncomingMessage message)
    {
        try
        {
            foreach (var dict in clientsSimpleObjects)
            {
                NetOutgoingMessage msg = _server.CreateMessage();
                msg.Write((ushort)RPC.InstintiateSimpleObject);
                msg.Write(dict.Key);
                msg.Write(dict.Value.PrefabID);
                msg.Write(dict.Value.OwnerID);
                msg.Write(dict.Value.Position);
                msg.Write(dict.Value.Rotation);
                _server.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
            }

            foreach (var obj in clientsInstanses)
            {
                NetOutgoingMessage msg = _server.CreateMessage();
                msg.Write((ushort)RPC.InstintiatePlayer);
                msg.Write(obj.Value.ID);
                msg.Write(obj.Value.OwnerID);
                msg.Write(obj.Value.SkinID);
                msg.Write(obj.Value.WeaponID_First);
                msg.Write(obj.Value.WeaponFisrt_Modifications);
                msg.Write(obj.Value.WeaponID_Secondary);
                msg.Write(obj.Value.WeaponSecondary_Modifications);
                msg.Write(obj.Value.WeaponID_Third);
                msg.Write(obj.Value.WeaponThird_Modifications);
                msg.Write(obj.Value.WeaponID_Grenage);
                msg.Write(obj.Value.WeaponID_Knife);
                msg.Write(obj.Value.SpecializationID);
                msg.Write(obj.Value.Position);
                msg.Write(obj.Value.Rotation);
                msg.Write(obj.Value.Current_Health);
                msg.Write(obj.Value.Class_Speed);
                msg.Write(obj.Value.Selected_Class);
                msg.Write(obj.Value.Team);
                _server.SendMessage(msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
            }

            NetOutgoingMessage mesg = _server.CreateMessage();
            mesg.Write((ushort)RPC.ConnectionEndSync);
            _server.SendMessage(mesg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
        }
        catch
        {
            message.SenderConnection.Disconnect("Unexpected error");
        }
    }

    [OFRPC(OFRPC.RPCType.Serverside, (ushort)RPC.InstintiateSimpleObject)]
    public void InstintiateObject(NetIncomingMessage message)
    {
        ushort prefabId = message.ReadUInt16();
        byte ownerId = message.ReadByte();
        Vector3 position = message.ReadVector3();
        Quaternion rotation = message.ReadQuaternion();
        try
        {
            ushort okey = 0;
            for (ushort i = 0; i < (ushort)65535; i++)
            {
                if (!clientsSimpleObjects.ContainsKey(i))
                {
                    okey = i;
                    break;
                }
                else if (i == 65535)
                {
                    break;
                }
            }
            var obj = new PlayerObject
            {
                PrefabID = prefabId,
                OwnerID = ownerId,
                ID = okey,
                Position = position,
                Rotation = rotation
            };

            bool status = false;
            while (!status)
                status = clientsSimpleObjects.TryAdd(okey, obj);

            List<NetConnection> all = _server.Connections;
            if (all.Count > 0)
            {
                foreach (NetConnection conn in all)
                {
                    NetOutgoingMessage msg = _server.CreateMessage();
                    msg.Write((ushort)RPC.InstintiateSimpleObject);
                    msg.Write(okey);
                    msg.Write(obj.PrefabID);
                    msg.Write(obj.OwnerID);
                    msg.Write(obj.Position);
                    msg.Write(obj.Rotation);
                    _server.SendMessage(msg, conn, NetDeliveryMethod.ReliableUnordered);
                }
            }
        }
        catch { }
    }

    [OFRPC(OFRPC.RPCType.Serverside, (ushort)RPC.InstintiatePlayer)]
    public void InstintiatePlayer(NetIncomingMessage message)
    {
        byte Owner_ID = message.ReadByte();
        try
        {
            ushort okey = 0;
            for (ushort i = 0; i < (ushort)65535; i++)
            {
                if (!clientsInstanses.ContainsKey(i))
                {
                    okey = i;
                    break;
                }
                else if (i == 65535)
                {
                    return;
                }
            }

            var obj = new PlayerInstance
            {
                ID = okey,
                OwnerID = Owner_ID,
                SkinID = message.ReadUInt16(),
                WeaponID_First = message.ReadUInt16(),
                WeaponFisrt_Modifications = message.ReadString(),
                WeaponID_Secondary = message.ReadUInt16(),
                WeaponSecondary_Modifications = message.ReadString(),
                WeaponID_Third = message.ReadUInt16(),
                WeaponThird_Modifications = message.ReadString(),
                WeaponID_Grenage = message.ReadByte(),
                WeaponID_Knife = message.ReadByte(),
                SpecializationID = message.ReadByte(),
                Position = message.ReadVector3(),
                Rotation = message.ReadQuaternion(),
                Current_Health = 100,                 //REMADE THAT
                Class_Speed = 255,                    //AS FAST, AS POSSIBLE!
                Selected_Class = clients[Owner_ID].Selected_Class,
                Team = clients[Owner_ID].Current_Team
            };

            bool success = false;
            while (!success)
                success = clientsInstanses.TryAdd(okey, obj);


            List<NetConnection> all = _server.Connections;
            foreach (NetConnection conn in all)
            {
                NetOutgoingMessage msg = _server.CreateMessage();
                msg.Write((byte)RPC.InstintiatePlayer);
                msg.Write(obj.ID);
                msg.Write(obj.OwnerID);
                msg.Write(obj.SkinID);
                msg.Write(obj.WeaponID_First);
                msg.Write(obj.WeaponFisrt_Modifications);
                msg.Write(obj.WeaponID_Secondary);
                msg.Write(obj.WeaponSecondary_Modifications);
                msg.Write(obj.WeaponID_Third);
                msg.Write(obj.WeaponThird_Modifications);
                msg.Write(obj.WeaponID_Grenage);
                msg.Write(obj.WeaponID_Knife);
                msg.Write(obj.SpecializationID);
                msg.Write(obj.Position);
                msg.Write(obj.Rotation);
                msg.Write(obj.Current_Health);
                msg.Write(obj.Class_Speed);
                msg.Write(obj.Selected_Class);
                msg.Write(obj.Team);
                _server.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);
            }
        }
        catch
        {

        }
    }

    /*
    public static void UpdatePlayerTransform(ushort id, Vector3 position, Vector3 rotation, NetConnection sender)
    {
        try
        {
            clientsInstanses[id].Position = position;
            clientsInstanses[id].Rotation = rotation;
        }
        catch
        {
        }
    }

    public static void DestroyObject(ushort id)
    {
        try
        {
            List<NetConnection> all = _server.Connections;
            clientsSimpleObjects.Remove(id);
            foreach (NetConnection conn in all)
            {
                NetOutgoingMessage msg = _server.CreateMessage();
                msg.Write((byte)DataTransmissionEnum.DestroySimpleObject);
                msg.Write(id);
                _server.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);
            }
        }
        catch
        {
        }
    }

    public static void PlayerGetRagdolled(ushort id)
    {
        try
        {
            Killed?.Invoke(clients[clientsInstanses[id].ownerID].Team);
        }
        catch
        {
        }
    }

    public static void DestroyPlayer(ushort id)
    {
        try
        {
            List<NetConnection> all = _server.Connections;
            clientsInstanses.Remove(id);
            foreach (NetConnection conn in all)
            {
                NetOutgoingMessage msg = _server.CreateMessage();
                msg.Write((byte)DataTransmissionEnum.DestroyPlayer);
                msg.Write(id);
                _server.SendMessage(msg, conn, NetDeliveryMethod.ReliableOrdered);
            }
        }
        catch
        {
        }
    }

    public static void DeliverDamage(ushort toWho, ushort withWhat, byte playerWho, byte value, byte slot)
    {
        try
        {
            clientsInstanses[toWho].Health -= value;
            clientsInstanses[toWho].LastDamagedPlayerID = playerWho;
            clientsInstanses[toWho].LastDamagedWeaponSlot = slot;
            clientsInstanses[toWho].LastDamagedWeaponID = withWhat;
            var nick = clientsInstanses[toWho].LastDamagedPlayerNickname = clients[playerWho].Nickname;
            foreach (var conn in _server.Connections)
            {
                NetOutgoingMessage mesg = _server.CreateMessage();
                mesg.Write((byte)DataTransmissionEnum.DeliverDamage);
                mesg.Write(toWho);
                mesg.Write(playerWho);
                mesg.Write(withWhat);
                mesg.Write(nick);
                mesg.Write(value);
                mesg.Write(slot);
                _server.SendMessage(mesg, conn, NetDeliveryMethod.ReliableUnordered);
            }
        }
        catch
        {
        }
    }

    public static void ChangeTeam(byte who, NetConnection sender)
    {
        try
        {
            if (clients.ContainsKey(who))
            {
                if (clients[who].Team == 0)
                {
                    clients[who].Team = 1;
                }
                else
                {
                    clients[who].Team = 0;
                }
                NetOutgoingMessage msg = _server.CreateMessage();
                msg.Write((byte)DataTransmissionEnum.ChangeTeam);
                msg.Write(clients[who].Team);
                _server.SendMessage(msg, sender, NetDeliveryMethod.ReliableUnordered);
            }
        }
        catch
        {
        }
    }

    public static void ChangeClass(byte who, byte newClass, NetConnection sender)
    {
        try
        {
            lock (clients)
            {
                if (clients.ContainsKey(who))
                {
                    clients[who].Class = newClass;

                    NetOutgoingMessage msg = _server.CreateMessage();
                    msg.Write((byte)DataTransmissionEnum.ChangeClass);
                    msg.Write(clients[who].Class);
                    _server.SendMessage(msg, sender, NetDeliveryMethod.ReliableUnordered);
                }
            }
        }
        catch
        {
        }
    }*/
}


